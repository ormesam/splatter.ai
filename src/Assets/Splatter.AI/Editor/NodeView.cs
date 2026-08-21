using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using BTNode = Splatter.AI.Node;

namespace Splatter.AI.Editor {
    /// <summary>
    /// Read-only graph node presenting a single behaviour tree node: name, type, and live
    /// status. Running nodes are highlighted solid; stopped nodes flash their
    /// <see cref="NodeStopReason"/> colour and fade out over wall-clock time.
    /// </summary>
    public class NodeView : UnityEditor.Experimental.GraphView.Node {
        private static readonly Color RunningColour = new Color(0.72f, 0.53f, 0.04f);
        private static readonly Color SuccessColour = new Color(0.18f, 0.55f, 0.34f);
        private static readonly Color FailureColour = new Color(0.69f, 0.20f, 0.20f);
        private static readonly Color AbortedColour = new Color(0.57f, 0.27f, 0.85f);

        private const double FadeSeconds = 2.0;

        private readonly Label indexLabel;
        private readonly Color defaultPortColour;
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

        public Port InputPort { get; }

        public Port OutputPort { get; }

        public NodeView(BTNode node, bool isRoot) {
            Node = node;
            title = string.IsNullOrEmpty(node.Name) ? node.GetType().Name : node.Name;
            expanded = true;

            // Read-only graph: the layout pass owns positions, and nothing is deletable.
            capabilities &= ~(Capabilities.Deletable | Capabilities.Movable);

            AddToClassList("node");

            var typeLabel = new Label(node.GetType().Name);
            typeLabel.AddToClassList("type-label");
            titleContainer.Add(typeLabel);

            indexLabel = new Label();
            indexLabel.AddToClassList("index-label");
            indexLabel.style.display = DisplayStyle.None;
            titleContainer.Add(indexLabel);

            if (node is SubtreeNode) {
                AddToClassList("subtree");
            }

            if (!isRoot) {
                // Multi capacity: a shared node instance can have edges from several parents.
                InputPort = CreatePort(Direction.Input, Port.Capacity.Multi);
                inputContainer.Add(InputPort);
            }

            if (node is Composite || node is Decorator || node is SubtreeNode) {
                var capacity = node is Composite ? Port.Capacity.Multi : Port.Capacity.Single;
                OutputPort = CreatePort(Direction.Output, capacity);
                outputContainer.Add(OutputPort);
            }

            defaultPortColour = InputPort?.portColor ?? OutputPort?.portColor ?? Color.white;

            // Only stops that happen after the view is built should flash.
            lastSeenStopCount = node.StopCount;

            RefreshExpandedState();
            RefreshPorts();
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
                SetPortColour(RunningColour);
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
                SetPortColour(defaultPortColour);

                return;
            }

            var flash = FlashColour(fadeReason);
            titleContainer.style.backgroundColor = new Color(flash.r, flash.g, flash.b, 1f - t);
            SetPortColour(Color.Lerp(flash, defaultPortColour, t));
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
                    return defaultPortColour;
            }
        }

        private Port CreatePort(Direction direction, Port.Capacity capacity) {
            var port = InstantiatePort(Orientation.Vertical, direction, capacity, typeof(bool));
            port.portName = string.Empty;

            return port;
        }

        private void SetPortColour(Color colour) {
            if (InputPort != null) {
                InputPort.portColor = colour;
            }

            if (OutputPort != null) {
                OutputPort.portColor = colour;
            }
        }
    }
}
