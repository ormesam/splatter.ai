using System;

namespace Splatter.AI.Composites {
    /// <summary>
    /// A <see cref="Selector"/> that tries its children in a random order.
    /// The order is reshuffled each time this node starts.
    /// </summary>
    public class RandomSelector : Composite {
        private readonly Random random;
        private int[] order;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSelector"/> class.
        /// </summary>
        /// <param name="random">Source of randomness for shuffling</param>
        public RandomSelector(Random random) : this("Random Selector", random) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSelector"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="random">Source of randomness for shuffling. Defaults to a new <see cref="Random"/>.</param>
        public RandomSelector(string name = "Random Selector", Random random = null) : base(name) {
            this.random = random ?? new Random();
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

                if (result == NodeResult.Success) {
                    return NodeResult.Success;
                }

                CurrentNodeIdx++;
            }

            return NodeResult.Failure;
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
