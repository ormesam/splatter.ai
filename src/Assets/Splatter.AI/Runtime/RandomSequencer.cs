using System;

namespace Splatter.AI {
    /// <summary>
    /// A <see cref="Sequencer"/> that runs its children in a random order.
    /// The order is reshuffled each time this node starts.
    /// </summary>
    public class RandomSequencer : Composite {
        private readonly Random random;
        private int[] order;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSequencer"/> class.
        /// </summary>
        public RandomSequencer() : this(new Random()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSequencer"/> class.
        /// </summary>
        /// <param name="random">Source of randomness for shuffling</param>
        public RandomSequencer(Random random) : base("Random Sequence") {
            this.random = random;
        }

        protected override void OnStart() {
            CurrentNodeIdx = 0;
            Shuffle();
        }

        protected override NodeResult Update() {
            if (order.Length != Children.Count) {
                Shuffle();
            }

            while (CurrentNodeIdx < Children.Count) {
                var result = Children[order[CurrentNodeIdx]].OnUpdate();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                }

                if (result == NodeResult.Failure) {
                    return NodeResult.Failure;
                }

                CurrentNodeIdx++;
            }

            return NodeResult.Success;
        }

        protected override void OnStop() {
        }

        private void Shuffle() {
            if (order == null || order.Length != Children.Count) {
                order = new int[Children.Count];
            }

            for (int i = 0; i < order.Length; i++) {
                order[i] = i;
            }

            for (int i = order.Length - 1; i > 0; i--) {
                int j = random.Next(i + 1);

                int temp = order[i];
                order[i] = order[j];
                order[j] = temp;
            }
        }
    }
}
