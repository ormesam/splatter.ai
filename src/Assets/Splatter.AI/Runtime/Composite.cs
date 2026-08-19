using System.Collections;
using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// Node with multiple children.
    /// </summary>
    public abstract class Composite : Node, IEnumerable<Node> {
        private readonly List<Node> children = new List<Node>();

        /// <summary>
        /// Children of the composite node.
        /// </summary>
        protected IReadOnlyList<Node> Children => children;

        /// <summary>
        /// Number of children.
        /// </summary>
        public int Count => children.Count;

        /// <summary>
        /// Index of the node to be executed.
        /// </summary>
        protected int CurrentNodeIdx = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Composite"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        public Composite(string name) : base(name) {
        }

        /// <summary>
        /// Adds a child to the composite.
        /// </summary>
        /// <param name="node">Child node</param>
        public void Add(Node node) {
            children.Add(node);
        }

        public IEnumerator<Node> GetEnumerator() => children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected void StopChildren() {
            foreach (var child in children) {
                child.Stop();
            }
        }
    }
}
