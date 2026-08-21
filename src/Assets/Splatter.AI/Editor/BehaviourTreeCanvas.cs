using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using BTNode = Splatter.AI.Node;

namespace Splatter.AI.Editor {
    /// <summary>
    /// Read-only canvas of a live behaviour tree, built on plain UI Toolkit. Views are keyed by
    /// node instance; a node reachable through two parents (a shared node instance or a shared
    /// subtree) is drawn once with an extra incoming edge, and its status display is shared
    /// because the state genuinely is one object's.
    /// </summary>
    public class BehaviourTreeCanvas : VisualElement {
        internal const float MinZoom = 0.25f;
        internal const float MaxZoom = 2f;

        private const float NodeWidth = 180f;
        private const float NodeHeight = 100f;
        private const float ColumnPitch = 210f;
        private const float RowPitch = 130f;
        private const float GridSpacing = 15f;
        private const int GridMajorEvery = 10;
        private const float FramePadding = 40f;
        private const float EdgeLayerMargin = 40f;

        private static readonly Color RunningEdgeColour = new Color(0.72f, 0.53f, 0.04f);
        private static readonly Color EdgeColour = new Color(0.55f, 0.55f, 0.55f);
        private static readonly Color GridMinorColour = new Color(1f, 1f, 1f, 0.04f);
        private static readonly Color GridMajorColour = new Color(1f, 1f, 1f, 0.09f);

        private sealed class EdgeInfo {
            public NodeView Parent;
            public NodeView Child;
            public bool Running;
        }

        private readonly Dictionary<BTNode, NodeView> views = new Dictionary<BTNode, NodeView>();
        private readonly List<EdgeInfo> edges = new List<EdgeInfo>();
        private readonly VisualElement content;
        private readonly VisualElement edgeLayer;
        private BehaviourTree tree;
        private float nextLeafSlot;
        private Rect contentBounds;
        private Vector2 pan;
        private float zoom = 1f;

        internal Vector2 Pan => pan;

        internal float Zoom => zoom;

        public BehaviourTreeCanvas() {
            AddToClassList("bt-canvas");
            focusable = true;
            generateVisualContent += PaintGrid;

            content = new VisualElement();
            content.AddToClassList("bt-canvas__content");
            content.usageHints = UsageHints.DynamicTransform;
            content.style.position = Position.Absolute;
            // The default origin is the element centre, which would silently break the
            // pan/zoom maths: they assume viewport = pan + zoom * content.
            content.style.transformOrigin = new TransformOrigin(0f, 0f);
            Add(content);

            edgeLayer = new VisualElement();
            edgeLayer.AddToClassList("bt-canvas__edges");
            edgeLayer.pickingMode = PickingMode.Ignore;
            edgeLayer.style.position = Position.Absolute;
            edgeLayer.style.left = 0f;
            edgeLayer.style.top = 0f;
            edgeLayer.generateVisualContent += PaintEdges;
            content.Add(edgeLayer);

            this.AddManipulator(new CanvasPanZoomManipulator(this));

            RegisterCallback<KeyDownEvent>(evt => {
                if (evt.keyCode == KeyCode.F) {
                    FrameAll();
                    evt.StopPropagation();
                }
            });
        }

        /// <summary>
        /// Rebuilds the graph for the given tree. Pass null to clear.
        /// </summary>
        public void SetTree(BehaviourTree tree) {
            ClearGraph();
            this.tree = tree;

            if (tree?.Root == null) {
                return;
            }

            var rootView = Build(tree.Root, null);
            nextLeafSlot = 0;
            // Layout coordinates start at (0, 0): the leftmost leaf takes column 0 and the
            // root takes row 0, so growing the bounds from Rect.zero is exact.
            contentBounds = Rect.zero;
            LayoutSubtree(rootView, 0);

            // A zero-size element culls its painted content, so the edge layer is sized to
            // cover every edge explicitly.
            edgeLayer.style.width = contentBounds.xMax + EdgeLayerMargin;
            edgeLayer.style.height = contentBounds.yMax + EdgeLayerMargin;
            edgeLayer.MarkDirtyRepaint();

            // Geometry is zero until the first layout pass, so framing must be deferred.
            schedule.Execute(() => FrameAll());
        }

        /// <summary>
        /// Removes every node and edge. Named to avoid hiding <c>VisualElement.Clear</c>.
        /// </summary>
        public void ClearGraph() {
            tree = null;
            views.Clear();
            edges.Clear();
            contentBounds = Rect.zero;
            content.Clear();
            content.Add(edgeLayer);
            edgeLayer.MarkDirtyRepaint();
        }

        /// <summary>
        /// Whether the current views still match the tree's structure. False on root
        /// reassignment, children added at runtime, or subtree root swaps — the caller rebuilds
        /// via <see cref="SetTree"/>.
        /// </summary>
        public bool MatchesStructure(BehaviourTree tree) {
            if (this.tree != tree) {
                return false;
            }

            if (tree?.Root == null) {
                return views.Count == 0;
            }

            var seen = new HashSet<BTNode>();

            return Matches(tree.Root, seen) && seen.Count == views.Count;
        }

        /// <summary>
        /// Applies live status to every node view and retints edges into running children.
        /// </summary>
        /// <param name="now">Editor wall-clock time (<c>EditorApplication.timeSinceStartup</c>)</param>
        public void UpdateNodeStates(double now) {
            foreach (var view in views.Values) {
                view.UpdateState(now);
            }

            bool edgesChanged = false;

            foreach (var edge in edges) {
                bool running = edge.Parent.Node.IsStarted && edge.Child.Node.IsStarted;

                if (edge.Running != running) {
                    edge.Running = running;
                    edgesChanged = true;
                }
            }

            if (edgesChanged) {
                edgeLayer.MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// Pans and zooms so the whole tree fits the viewport, without zooming in past 1:1.
        /// </summary>
        public void FrameAll() {
            if (views.Count == 0) {
                return;
            }

            Rect viewport = contentRect;

            if (float.IsNaN(viewport.width) || viewport.width <= 0f || viewport.height <= 0f) {
                // Fresh window: geometry is unresolved until the first layout pass.
                RegisterCallbackOnce<GeometryChangedEvent>(_ => FrameAll());

                return;
            }

            float fit = Mathf.Min(
                (viewport.width - 2f * FramePadding) / contentBounds.width,
                (viewport.height - 2f * FramePadding) / contentBounds.height);
            float targetZoom = Mathf.Clamp(Mathf.Min(fit, 1f), MinZoom, MaxZoom);

            SetViewTransform(viewport.center - targetZoom * contentBounds.center, targetZoom);
        }

        internal void SetViewTransform(Vector2 newPan, float newZoom) {
            pan = newPan;
            zoom = Mathf.Clamp(newZoom, MinZoom, MaxZoom);
            content.style.translate = new Translate(pan.x, pan.y);
            content.style.scale = new Scale(new Vector2(zoom, zoom));

            // Only the viewport-space grid needs redrawing; the edge mesh rides the transform.
            MarkDirtyRepaint();
        }

        private bool Matches(BTNode node, HashSet<BTNode> seen) {
            if (!seen.Add(node)) {
                return true;
            }

            if (!views.ContainsKey(node)) {
                return false;
            }

            foreach (var child in BehaviourTree.GetChildren(node)) {
                if (!Matches(child, seen)) {
                    return false;
                }
            }

            return true;
        }

        private NodeView Build(BTNode node, NodeView parentView) {
            if (views.TryGetValue(node, out var existing)) {
                // Shared node: draw one extra edge to the existing view and do not recurse,
                // which also guards against accidental cycles.
                AddEdge(parentView, existing);

                return existing;
            }

            var view = new NodeView(node);
            views.Add(node, view);
            content.Add(view);

            // Edge anchors read resolved node geometry, so the first layout after a rebuild
            // (and any later height change) must repaint the edge layer.
            view.RegisterCallback<GeometryChangedEvent>(_ => edgeLayer.MarkDirtyRepaint());

            if (parentView != null) {
                parentView.ChildViews.Add(view);
                AddEdge(parentView, view);
            }

            foreach (var child in BehaviourTree.GetChildren(node)) {
                Build(child, view);
            }

            return view;
        }

        private void AddEdge(NodeView parent, NodeView child) {
            if (parent == null) {
                return;
            }

            edges.Add(new EdgeInfo { Parent = parent, Child = child });
        }

        /// <summary>
        /// Tidy-tree layout over the spanning tree: leaves take consecutive column slots and
        /// each parent centres over its first and last child, so siblings can never overlap.
        /// </summary>
        /// <returns>The column assigned to this view.</returns>
        private float LayoutSubtree(NodeView view, int depth) {
            float column;

            if (view.ChildViews.Count == 0) {
                column = nextLeafSlot++;
            } else {
                float first = float.MaxValue;
                float last = float.MinValue;

                foreach (var child in view.ChildViews) {
                    float childColumn = LayoutSubtree(child, depth + 1);
                    first = Mathf.Min(first, childColumn);
                    last = Mathf.Max(last, childColumn);
                }

                column = (first + last) / 2f;
            }

            var rect = new Rect(column * ColumnPitch, depth * RowPitch, NodeWidth, NodeHeight);
            view.SetPosition(rect);
            contentBounds = Union(contentBounds, rect);

            return column;
        }

        private void PaintEdges(MeshGenerationContext ctx) {
            var painter = ctx.painter2D;
            painter.lineWidth = 2f;
            painter.lineCap = LineCap.Round;

            foreach (var edge in edges) {
                Rect parentRect = edge.Parent.layout;
                Rect childRect = edge.Child.layout;

                if (float.IsNaN(parentRect.width) || float.IsNaN(childRect.width)) {
                    // Node geometry unresolved; the nodes' GeometryChangedEvent repaints us.
                    continue;
                }

                var start = new Vector2(parentRect.center.x, parentRect.yMax);
                var end = new Vector2(childRect.center.x, childRect.yMin);
                float handle = Mathf.Max(30f, Mathf.Abs(end.y - start.y) * 0.5f);

                painter.strokeColor = edge.Running ? RunningEdgeColour : EdgeColour;
                painter.BeginPath();
                painter.MoveTo(start);
                painter.BezierCurveTo(start + new Vector2(0f, handle), end - new Vector2(0f, handle), end);
                painter.Stroke();
            }
        }

        private void PaintGrid(MeshGenerationContext ctx) {
            Rect area = contentRect;

            if (area.width <= 0f || area.height <= 0f) {
                return;
            }

            float spacing = GridSpacing * zoom;

            // Minor lines closer than this read as moiré, not a grid.
            if (spacing >= 6f) {
                PaintGridLines(ctx.painter2D, area, spacing, GridMinorColour);
            }

            PaintGridLines(ctx.painter2D, area, spacing * GridMajorEvery, GridMajorColour);
        }

        private void PaintGridLines(Painter2D painter, Rect area, float spacing, Color colour) {
            painter.strokeColor = colour;
            painter.lineWidth = 1f;
            painter.BeginPath();

            for (float x = Mathf.Repeat(pan.x, spacing); x < area.width; x += spacing) {
                painter.MoveTo(new Vector2(x, 0f));
                painter.LineTo(new Vector2(x, area.height));
            }

            for (float y = Mathf.Repeat(pan.y, spacing); y < area.height; y += spacing) {
                painter.MoveTo(new Vector2(0f, y));
                painter.LineTo(new Vector2(area.width, y));
            }

            painter.Stroke();
        }

        private static Rect Union(Rect a, Rect b) {
            float xMin = Mathf.Min(a.xMin, b.xMin);
            float yMin = Mathf.Min(a.yMin, b.yMin);
            float xMax = Mathf.Max(a.xMax, b.xMax);
            float yMax = Mathf.Max(a.yMax, b.yMax);

            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}
