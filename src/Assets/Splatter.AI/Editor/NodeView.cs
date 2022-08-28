#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using BTNode = Splatter.AI.Node;

namespace Splatter.AI.Editor {
    public class NodeView : UnityEditor.Experimental.GraphView.Node {
        public NodeView Parent { get; set; }
        public IList<NodeView> ChildNodes { get; private set; }
        public BTNode Node { get; set; }
        public int Layer { get; set; }
        public float Column { get; set; }
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }

        public NodeView(NodeView parent, BTNode node, int layer, float column) {
            Parent = parent;
            ChildNodes = new List<NodeView>();
            Node = node;
            Layer = layer;
            Column = column;

            title = Node.Name;
            expanded = true;

            InputPort = CreateInputPort();
            OutputPort = CreateOutputPort();

            AddToClassList("node");
        }

        private Port CreateOutputPort() {
            var capacity = Port.Capacity.Single;

            if (Node is Composite) {
                capacity = Port.Capacity.Multi;
            }

            var port = InstantiatePort(Orientation.Vertical, Direction.Output, capacity, typeof(bool));
            port.portName = string.Empty;

            outputContainer.Add(port);

            return port;
        }

        private Port CreateInputPort() {
            if (Parent == null) {
                return null;
            }

            var port = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            port.portName = string.Empty;

            inputContainer.Add(port);

            return port;
        }

        public void UpdateState() {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");
            SetPortColour(Color.white);

            if (Application.isPlaying) {
                switch (Node.Result) {
                    case NodeResult.Running:
                        if (Node.IsStarted) {
                            AddToClassList("running");
                            SetPortColour(Color.yellow);
                        }
                        break;
                    case NodeResult.Failure:
                        AddToClassList("failure");
                        SetPortColour(Color.red);
                        break;
                    case NodeResult.Success:
                        AddToClassList("success");
                        SetPortColour(Color.green);
                        break;
                }
            }
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
#endif