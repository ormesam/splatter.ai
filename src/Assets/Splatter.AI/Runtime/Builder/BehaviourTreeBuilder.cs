using System;

namespace Splatter.AI {
    public class BehaviourTreeBuilder : BuilderBase, IBuilder {
        private Node rootNode;

        public BehaviourTreeBuilder(BehaviourTree tree) : base(tree) {
        }

        public Node Build() {
            if (rootNode == null) {
                throw new InvalidOperationException("Can't create a behaviour tree with zero nodes");
            }

            return rootNode;
        }

        public void AddNode(Node node) {
            if (rootNode != null) {
                throw new InvalidOperationException("Root node is already set");
            }

            rootNode = node;
        }

        public void SetName(string name) {
            if (rootNode != null) {
                rootNode.Name = name;
            }
        }
    }
}
