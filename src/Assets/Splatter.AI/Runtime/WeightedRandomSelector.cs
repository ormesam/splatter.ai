using System;
using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// A <see cref="Selector"/> that tries its children in a weighted random order: the higher
    /// a child's weight, the more likely it is to be tried earlier. Children added without a
    /// weight default to 1; children with zero weight are only tried once every positively
    /// weighted child has been. The order is re-rolled each time this node starts.
    /// </summary>
    public class WeightedRandomSelector : Composite {
        private readonly Random random;
        private readonly List<double> weights = new List<double>();
        private int[] order;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedRandomSelector"/> class.
        /// </summary>
        public WeightedRandomSelector() : this(new Random()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedRandomSelector"/> class.
        /// </summary>
        /// <param name="random">Source of randomness for ordering</param>
        public WeightedRandomSelector(Random random) : base("Weighted Random Selector") {
            this.random = random;
        }

        /// <summary>
        /// Adds a child with the given selection weight.
        /// </summary>
        /// <param name="node">Child node</param>
        /// <param name="weight">Selection weight, relative to the other children's weights</param>
        public void Add(Node node, double weight) {
            if (weight < 0) {
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must not be negative");
            }

            while (weights.Count < Count) {
                weights.Add(1);
            }

            Add(node);
            weights.Add(weight);
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

        private double GetWeight(int childIdx) {
            return childIdx < weights.Count ? weights[childIdx] : 1;
        }

        private void Shuffle() {
            if (order == null || order.Length != Children.Count) {
                order = new int[Children.Count];
            }

            for (int i = 0; i < order.Length; i++) {
                order[i] = i;
            }

            for (int slot = 0; slot < order.Length; slot++) {
                int pick = PickWeighted(slot);

                int temp = order[slot];
                order[slot] = order[pick];
                order[pick] = temp;
            }
        }

        private int PickWeighted(int from) {
            double total = 0;

            for (int i = from; i < order.Length; i++) {
                total += GetWeight(order[i]);
            }

            if (total <= 0) {
                return from;
            }

            double roll = random.NextDouble() * total;
            double cumulative = 0;

            for (int i = from; i < order.Length; i++) {
                cumulative += GetWeight(order[i]);

                if (roll < cumulative) {
                    return i;
                }
            }

            // Floating-point rounding can leave the roll just above the summed weights.
            return order.Length - 1;
        }
    }
}
