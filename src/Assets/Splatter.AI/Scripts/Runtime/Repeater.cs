namespace Splatter.AI {
    /// <summary>
    /// Always returns <see cref="NodeResult.Running"/> after executing the child.
    /// </summary>
    public class Repeater : Decorator {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repeater"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        public Repeater(string name, BehaviourTree tree) : base(name, tree) {
        }

        protected override NodeResult ExecuteNode() {
            Child.Execute();

            return NodeResult.Running;
        }
    }
}
