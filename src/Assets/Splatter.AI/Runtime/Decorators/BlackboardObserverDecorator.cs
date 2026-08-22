using Splatter.AI.Leaves;

namespace Splatter.AI.Decorators {
    /// <summary>
    /// An <see cref="ObservingDecorator"/> gated on a blackboard entry, succeeding into its
    /// child while the key is set (and, when an expected value is given, equal to it). The
    /// event-driven counterpart to <see cref="BlackboardConditionNode"/> +
    /// <see cref="GuardDecorator"/>: the entry is re-checked only when its value actually
    /// changes, and changes can abort per <see cref="AbortMode"/>.
    /// </summary>
    public class BlackboardObserverDecorator : ObservingDecorator {
        private readonly Blackboard blackboard;
        private readonly string key;
        private readonly object expectedValue;
        private readonly bool checkValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackboardObserverDecorator"/> class
        /// whose condition is that the key is set.
        /// </summary>
        /// <param name="blackboard">Blackboard to observe</param>
        /// <param name="key">Blackboard key</param>
        /// <param name="mode">How to react when the entry changes</param>
        public BlackboardObserverDecorator(Blackboard blackboard, string key, AbortMode mode)
            : base($"Observing blackboard {key} is set", mode) {

            this.blackboard = blackboard;
            this.key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackboardObserverDecorator"/> class
        /// whose condition is that the key is set to the expected value.
        /// </summary>
        /// <param name="blackboard">Blackboard to observe</param>
        /// <param name="key">Blackboard key</param>
        /// <param name="expectedValue">Value the entry must equal</param>
        /// <param name="mode">How to react when the entry changes</param>
        public BlackboardObserverDecorator(Blackboard blackboard, string key, object expectedValue, AbortMode mode)
            : base($"Observing blackboard {key} equals {expectedValue}", mode) {

            this.blackboard = blackboard;
            this.key = key;
            this.expectedValue = expectedValue;
            this.checkValue = true;
        }

        protected override bool IsConditionMet() {
            if (!blackboard.TryGetValue(key, out var value)) {
                return false;
            }

            return !checkValue || Equals(value, expectedValue);
        }

        protected override void StartObserving() {
            blackboard.Subscribe(key, OnObservedValueChanged);
        }

        protected override void StopObserving() {
            blackboard.Unsubscribe(key, OnObservedValueChanged);
        }
    }
}
