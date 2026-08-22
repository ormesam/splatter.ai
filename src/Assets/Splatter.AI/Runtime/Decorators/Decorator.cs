namespace Splatter.AI.Decorators {
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
        /// <param name="name">Node name</param>
        public Decorator(string name) : base(name) {
        }
    }
}
