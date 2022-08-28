using System;

namespace Splatter.AI {
    /// <summary>
    /// Returns <see cref="NodeResult.Running"/> until the condition is true.
    /// </summary>
    public class WaitUntilNode : Node {
        private readonly Func<bool> condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntilNode"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="condition">Condition to evaluate</param>
        public WaitUntilNode(string name, BehaviourTree tree, Func<bool> condition) : base(name, tree) {
            this.condition = condition;
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            return condition() ? NodeResult.Success : NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}
