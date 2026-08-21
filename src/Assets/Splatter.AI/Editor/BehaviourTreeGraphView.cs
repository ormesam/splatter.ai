using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using BTNode = Splatter.AI.Node;

namespace Splatter.AI.Editor {
    /// <summary>
    /// Read-only graph of a live behaviour tree. Views are keyed by node instance; a node
    /// reachable through two parents (a shared node instance or a shared subtree) is drawn once
    /// with an extra incoming edge, and its status display is shared because the state genuinely
    /// is one object's.
    /// </summary>
    public class BehaviourTreeGraphView : GraphView {
        private const float NodeWidth = 180f;
        private const float NodeHeight = 100f;
        private const float ColumnPitch = 210f;
        private const float RowPitch = 130f;

        private readonly Dictionary<BTNode, NodeView> views = new Dictionary<BTNode, NodeView>();
        private BehaviourTree tree;
        private float nextLeafSlot;

        public BehaviourTreeGraphView() {
            var grid = new GridBackground();
            grid.StretchToParentSize();
            Insert(0, grid);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new RectangleSelector());
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
            LayoutSubtree(rootView, 0);

            // Geometry is zero until the first layout pass, so framing must be deferred.
            schedule.Execute(() => FrameAll());
        }

        /// <summary>
        /// Removes every node and edge. Named to avoid hiding <c>VisualElement.Clear</c>.
        /// </summary>
        public void ClearGraph() {
            tree = null;
            views.Clear();
            DeleteElements(graphElements.ToList());
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
        /// Applies live status to every node view.
        /// </summary>
        /// <param name="now">Editor wall-clock time (<c>EditorApplication.timeSinceStartup</c>)</param>
        public void UpdateNodeStates(double now) {
            foreach (var view in views.Values) {
                view.UpdateState(now);
            }
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

            var view = new NodeView(node, isRoot: parentView == null);
            views.Add(node, view);
            AddElement(view);

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
            if (parent?.OutputPort == null || child.InputPort == null) {
                return;
            }

            var edge = parent.OutputPort.ConnectTo(child.InputPort);
            AddElement(edge);
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

            view.SetPosition(new Rect(column * ColumnPitch, depth * RowPitch, NodeWidth, NodeHeight));

            return column;
        }
    }
}
