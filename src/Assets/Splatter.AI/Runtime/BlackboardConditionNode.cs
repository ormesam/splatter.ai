using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// Checks a blackboard entry, returning <see cref="NodeResult.Success"/> if the key is set
    /// (and, when an expected value is given, equal to it), otherwise <see cref="NodeResult.Failure"/>.
    /// </summary>
    public class BlackboardConditionNode : Node {
        private readonly IDictionary<string, object> blackboard;
        private readonly string key;
        private readonly object expectedValue;
        private readonly bool checkValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackboardConditionNode"/> class
        /// that succeeds if the key is set.
        /// </summary>
        /// <param name="blackboard">Blackboard to check</param>
        /// <param name="key">Blackboard key</param>
        public BlackboardConditionNode(IDictionary<string, object> blackboard, string key)
            : base($"Checking blackboard {key} is set") {

            this.blackboard = blackboard;
            this.key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackboardConditionNode"/> class
        /// that succeeds if the key is set to the expected value.
        /// </summary>
        /// <param name="blackboard">Blackboard to check</param>
        /// <param name="key">Blackboard key</param>
        /// <param name="expectedValue">Value the entry must equal</param>
        public BlackboardConditionNode(IDictionary<string, object> blackboard, string key, object expectedValue)
            : base($"Checking blackboard {key} equals {expectedValue}") {

            this.blackboard = blackboard;
            this.key = key;
            this.expectedValue = expectedValue;
            this.checkValue = true;
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            if (!blackboard.TryGetValue(key, out var value)) {
                return NodeResult.Failure;
            }

            if (checkValue && !Equals(value, expectedValue)) {
                return NodeResult.Failure;
            }

            return NodeResult.Success;
        }

        protected override void OnStop() {
        }
    }
}
