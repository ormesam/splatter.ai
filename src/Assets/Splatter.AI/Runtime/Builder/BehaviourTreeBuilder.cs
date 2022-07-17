using System;

namespace Splatter.AI {
    /// <summary>
    /// Helper for creating behaviour tree
    /// </summary>
    public class BehaviourTreeBuilder : BuilderBase {
        private Node rootNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviourTreeBuilder"/> class.
        /// </summary>
        public BehaviourTreeBuilder(BehaviourTree tree) : base(tree) {
        }

        /// <summary>
        /// Builds the behaviour tree.
        /// </summary>
        /// <returns>Built tree</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Node Build() {
            if (rootNode == null) {
                throw new InvalidOperationException("Can't create a behaviour tree with zero nodes");
            }

            return rootNode;
        }

        protected override void AddNode(Node node) {
            if (rootNode != null) {
                throw new InvalidOperationException("Root node is already set");
            }

            rootNode = node;
        }
    }
}
