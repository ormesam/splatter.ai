namespace Splatter.AI {
    /// <summary>
    /// Runs forever, never completing on its own. Useful as a "do nothing" branch that holds
    /// until it is interrupted, e.g. under a reactive composite or <see cref="GuardDecorator"/>.
    /// </summary>
    public class IdleNode : Node {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdleNode"/> class.
        /// </summary>
        public IdleNode() : this("Idle") {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdleNode"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        public IdleNode(string name) : base(name) {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            return NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}
