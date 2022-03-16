namespace Splatter.AI {
    /// <summary>
    /// Node with single child. Usually used for modifying the output of the child's execution result.
    /// </summary>
    public abstract class Decorator : Node {
        /// <summary>
        /// Decorator child node
        /// </summary>
        public Node Child { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Decorator"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        public Decorator(string name, BehaviourTree tree) : base(name, tree) {
        }
    }
}
