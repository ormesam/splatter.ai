using System;

namespace Splatter.AI {
    /// <summary>
    /// Succeeds with the given probability, otherwise fails. A new roll is made each update.
    /// </summary>
    public class ChanceNode : Node {
        private readonly double probability;
        private readonly Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceNode"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="probability">Probability of success, from 0 to 1</param>
        public ChanceNode(string name, double probability) : this(name, probability, new Random()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceNode"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="probability">Probability of success, from 0 to 1</param>
        /// <param name="random">Source of randomness for the roll</param>
        public ChanceNode(string name, double probability, Random random) : base(name) {
            if (probability < 0 || probability > 1) {
                throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1");
            }

            this.probability = probability;
            this.random = random;
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            return random.NextDouble() < probability ? NodeResult.Success : NodeResult.Failure;
        }

        protected override void OnStop() {
        }
    }
}
