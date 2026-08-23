using System;
using NUnit.Framework;
using Splatter.AI.Leaves;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class ChanceNodeTests : TestBase {
        [Test]
        public void Chance_RollBelowProbability_ReturnsSuccess() {
            var chance = new ChanceNode("Chance", 0.7, new StubRandom { DoubleValue = 0.5 });

            Assert.AreEqual(NodeResult.Success, chance.OnUpdate());
        }

        [Test]
        public void Chance_RollAtProbability_ReturnsFailure() {
            var chance = new ChanceNode("Chance", 0.7, new StubRandom { DoubleValue = 0.7 });

            Assert.AreEqual(NodeResult.Failure, chance.OnUpdate());
        }

        [Test]
        public void Chance_ZeroProbability_ReturnsFailure() {
            var chance = new ChanceNode("Chance", 0, new StubRandom { DoubleValue = 0 });

            Assert.AreEqual(NodeResult.Failure, chance.OnUpdate());
        }

        [Test]
        public void Chance_FullProbability_ReturnsSuccess() {
            var chance = new ChanceNode("Chance", 1, new StubRandom { DoubleValue = 0.999 });

            Assert.AreEqual(NodeResult.Success, chance.OnUpdate());
        }

        [Test]
        public void Chance_ReRolledEachUpdate() {
            var stub = new StubRandom { DoubleValue = 0.9 };
            var chance = new ChanceNode("Chance", 0.5, stub);

            Assert.AreEqual(NodeResult.Failure, chance.OnUpdate());

            stub.DoubleValue = 0.1;

            Assert.AreEqual(NodeResult.Success, chance.OnUpdate());
        }

        [Test]
        public void Chance_ProbabilityOutOfRange_Throws() {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ChanceNode("Chance", -0.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ChanceNode("Chance", 1.1));
        }
    }
}
