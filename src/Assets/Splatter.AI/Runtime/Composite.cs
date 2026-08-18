using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// Node with multiple children.
    /// </summary>
    public abstract class Composite : Node {
        /// <summary>
        /// Children of the composite node.
        /// </summary>
        public IList<Node> Children { get; set; }

        /// <summary>
        /// Index of the node to be executed.
        /// </summary>
        protected int CurrentNodeIdx = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Composite"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        public Composite(string name, BehaviourTree tree) : base(name, tree) {
            Children = new List<Node>();
        }

        protected void StopChildren() {
            foreach (var child in Children) {
                child.Stop();
            }
        }
    }
}
