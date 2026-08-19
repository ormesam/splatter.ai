using System;

namespace Splatter.AI.Tests.Stubs {
    /// <summary>
    /// Deterministic <see cref="Random"/> whose <see cref="Next(int)"/> always returns
    /// <see cref="Value"/> clamped to the valid range, making shuffle results predictable
    /// regardless of the runtime's random algorithm.
    /// </summary>
    public class StubRandom : Random {
        public int Value { get; set; }

        public override int Next(int maxValue) {
            return Math.Min(Value, maxValue - 1);
        }
    }
}
