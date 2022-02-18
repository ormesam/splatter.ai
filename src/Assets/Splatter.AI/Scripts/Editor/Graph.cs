#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using BTNode = Splatter.AI.Node;

namespace Splatter.AI.Editor {
    public class Graph : GraphView {
        private IList<NodeView> allNodes;
        private IDictionary<int, float> columnCount;
        private float column;

        public Graph() {
            allNodes = new List<NodeView>();
            columnCount = new Dictionary<int, float>();

            AddGridBackground();

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private void AddGridBackground() {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        public void Draw(BehaviourTree tree) {
            allNodes.Clear();
            columnCount.Clear();
            column = 0;
            DeleteElements(graphElements.ToList());

            CreateNodes(null, tree.GetRoot(), 0, false);

            ConnectNodes();
            DrawNodes();

            FrameAll();
        }

        private void ConnectNodes() {
            foreach (var node in allNodes) {
                if (node.Parent == null) {
                    continue;
                }

                var edge = new Edge {
                    output = node.Parent.OutputPort,
                    input = node.InputPort,
                };

                edge.AddToClassList("edge");
                AddElement(edge);
            }
        }

        private void CreateNodes(NodeView parentNode, BTNode node, int layer, bool incrementColumnCount) {
            if (incrementColumnCount) {
                column++;
            }

            var nodeView = CreateNodeView(parentNode, node, layer, column);

            if (node is Composite composite) {
                bool firstChild = true;

                foreach (var childNode in composite.Children) {
                    CreateNodes(nodeView, childNode, layer + 1, !firstChild);
                    firstChild = false;
                }

                if (composite.Children.Count > 1) {
                    PushRight(nodeView, composite.Children.Count / 2);
                }
            }

            if (node is Decorator decorator) {
                CreateNodes(nodeView, decorator.Child, layer + 1, false);
            }
        }

        private NodeView CreateNodeView(NodeView parent, BTNode node, int layer, float column) {
            var nodeView = new NodeView(parent, node, layer, column);
            parent?.ChildNodes.Add(nodeView);

            allNodes.Add(nodeView);

            return nodeView;
        }

        private void DrawNodes() {
            if (!allNodes.Any()) {
                return;
            }

            // Must match width value in ucc stylesheet
            float width = 180;
            float height = 70;
            float spacer = 40;

            foreach (var node in allNodes) {
                var rect = new Rect(CalculateXPosition(node, width, spacer), node.Layer * 100, width, height);
                node.SetPosition(rect);
                AddElement(node);
            }
        }

        private void PushRight(NodeView node, float offset) {
            if (node == null) {
                return;
            }

            PushRight(node.Parent, offset / 2);

            node.Column += offset;
        }

        private float CalculateXPosition(NodeView node, float width, float spacer) {
            return (node.Column * (width + spacer)) - spacer - (width / 2);
        }

        public void UpdateNodeStates() {
            foreach (var node in allNodes) {
                node.UpdateState();
            }
        }
    }
}
#endif