using System;

namespace Splatter.AI {
    /// <summary>
    /// Evaluates the condition and immediately returns <see cref="NodeResult.Success"/> if it is
    /// true, otherwise <see cref="NodeResult.Failure"/>. The instant-check counterpart to
    /// <see cref="WaitUntilNode"/>, which returns <see cref="NodeResult.Running"/> instead of failing.
    /// </summary>
    public class ConditionNode : Node {
        private readonly Func<bool> condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionNode"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="condition">Condition to evaluate</param>
        public ConditionNode(string name, Func<bool> condition) : base(name) {
            this.condition = condition;
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            return condition() ? NodeResult.Success : NodeResult.Failure;
        }

        protected override void OnStop() {
        }
    }
}
