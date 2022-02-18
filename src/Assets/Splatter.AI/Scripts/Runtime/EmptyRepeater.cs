namespace Splatter.AI {
    /// <summary>
    /// Node with no child. Always returns <see cref="NodeResult.Running"/>.
    /// </summary>
    public class EmptyRepeater : Node {
        /// <summary>
        /// Initializes a new instance of the <see cref="Decorator"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        public EmptyRepeater(string name, BehaviourTree tree) : base(name, tree) {
        }

        protected override NodeResult ExecuteNode() {
            return NodeResult.Running;
        }
    }
}
