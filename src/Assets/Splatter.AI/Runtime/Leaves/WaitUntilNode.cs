using System;

namespace Splatter.AI.Leaves {
    /// <summary>
    /// Returns <see cref="NodeResult.Running"/> until the condition is true.
    /// </summary>
    public class WaitUntilNode : Node {
        private readonly Func<bool> condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntilNode"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="condition">Condition to evaluate</param>
        public WaitUntilNode(string name, Func<bool> condition) : base(name) {
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
