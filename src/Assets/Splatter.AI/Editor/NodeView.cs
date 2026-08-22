using System.Collections.Generic;
using Splatter.AI.Composites;
using Splatter.AI.Leaves;
using UnityEngine;
using UnityEngine.UIElements;
using BTNode = Splatter.AI.Node;

namespace Splatter.AI.Editor {
    /// <summary>
    /// Read-only card presenting a single behaviour tree node: name, type, and live status.
    /// Running nodes are highlighted solid; stopped nodes flash their
    /// <see cref="NodeStopReason"/> colour and fade out over wall-clock time.
    /// </summary>
    public class NodeView : VisualElement {
        private static readonly Color RunningColour = new Color(0.72f, 0.53f, 0.04f);
        private static readonly Color SuccessColour = new Color(0.18f, 0.55f, 0.34f);
        private static readonly Color FailureColour = new Color(0.69f, 0.20f, 0.20f);
        private static readonly Color AbortedColour = new Color(0.57f, 0.27f, 0.85f);
        private static readonly Color DefaultFlashColour = new Color(0.8f, 0.8f, 0.8f);

        private const double FadeSeconds = 2.0;

        private readonly VisualElement titleContainer;
        private readonly Label indexLabel;
        private int lastSeenStopCount;
        private NodeStopReason fadeReason;
        private double fadeStartTime = double.NegativeInfinity;

        /// <summary>
        /// The behaviour tree node this view presents.
        /// </summary>
        public BTNode Node { get; }

        /// <summary>
        /// Views of this node's children in the spanning tree of first visits. A shared node
        /// appears only under the first parent that reached it.
        /// </summary>
        public List<NodeView> ChildViews { get; } = new List<NodeView>();

        public NodeView(BTNode node) {
            Node = node;
            AddToClassList("node");

            titleContainer = new VisualElement();
            titleContainer.AddToClassList("node-title");

            var titleLabel = new Label(string.IsNullOrEmpty(node.Name) ? node.GetType().Name : node.Name);
            titleLabel.AddToClassList("node-title-label");
            titleContainer.Add(titleLabel);

            indexLabel = new Label();
            indexLabel.AddToClassList("index-label");
            indexLabel.style.display = DisplayStyle.None;
            titleContainer.Add(indexLabel);

            Add(titleContainer);

            var typeLabel = new Label(node.GetType().Name);
            typeLabel.AddToClassList("type-label");
            Add(typeLabel);

            if (node is SubtreeNode) {
                AddToClassList("subtree");
            }

            // Only stops that happen after the view is built should flash.
            lastSeenStopCount = node.StopCount;
        }

        /// <summary>
        /// Places the card; the layout pass owns positions. Width comes from USS and height is
        /// content-driven, so only the position is applied.
        /// </summary>
        public void SetPosition(Rect position) {
            style.left = position.x;
            style.top = position.y;
        }

        /// <summary>
        /// Applies the node's live status: solid highlight while running, otherwise a flash of
        /// the last stop reason fading out over <see cref="FadeSeconds"/>.
        /// </summary>
        /// <param name="now">Editor wall-clock time (<c>EditorApplication.timeSinceStartup</c>)</param>
        public void UpdateState(double now) {
            if (Node.StopCount != lastSeenStopCount) {
                lastSeenStopCount = Node.StopCount;
                fadeReason = Node.LastStopReason;
                fadeStartTime = now;
            }

            if (Node.IsStarted) {
                AddToClassList("running");
                titleContainer.style.backgroundColor = RunningColour;
                UpdateIndexLabel();

                return;
            }

            RemoveFromClassList("running");
            indexLabel.style.display = DisplayStyle.None;

            float t = double.IsNegativeInfinity(fadeStartTime)
                ? 1f
                : Mathf.Clamp01((float)((now - fadeStartTime) / FadeSeconds));

            if (t >= 1f) {
                titleContainer.style.backgroundColor = StyleKeyword.Null;

                return;
            }

            var flash = FlashColour(fadeReason);
            titleContainer.style.backgroundColor = new Color(flash.r, flash.g, flash.b, 1f - t);
        }

        private void UpdateIndexLabel() {
            if (!(Node is Composite composite) || composite.Count == 0) {
                return;
            }

            // Progress through the composite, not child identity: randomised composites index a
            // shuffled order with CurrentNodeIdx. The active child shows via its own highlight.
            int current = Mathf.Min(composite.CurrentNodeIdx + 1, composite.Count);
            indexLabel.text = $"{current}/{composite.Count}";
            indexLabel.style.display = DisplayStyle.Flex;
        }

        private Color FlashColour(NodeStopReason reason) {
            switch (reason) {
                case NodeStopReason.Success:
                    return SuccessColour;
                case NodeStopReason.Failure:
                    return FailureColour;
                case NodeStopReason.Aborted:
                    return AbortedColour;
                default:
                    return DefaultFlashColour;
            }
        }
    }
}
