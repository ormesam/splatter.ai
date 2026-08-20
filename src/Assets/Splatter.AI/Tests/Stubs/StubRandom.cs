using System;

namespace Splatter.AI.Tests.Stubs {
    /// <summary>
    /// Deterministic <see cref="Random"/>. <see cref="Next(int)"/> always returns
    /// <see cref="Value"/> clamped to the valid range, and <see cref="NextDouble"/> always
    /// returns <see cref="DoubleValue"/>, making random-based nodes predictable regardless
    /// of the runtime's random algorithm.
    /// </summary>
    public class StubRandom : Random {
        public int Value { get; set; }

        public double DoubleValue { get; set; }

        public override int Next(int maxValue) {
            return Math.Min(Value, maxValue - 1);
        }

        public override double NextDouble() {
            return DoubleValue;
        }
    }
}
