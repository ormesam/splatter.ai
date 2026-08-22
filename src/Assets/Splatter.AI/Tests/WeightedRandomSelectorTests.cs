using System;
using NUnit.Framework;
using Splatter.AI.Composites;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class WeightedRandomSelectorTests : TestBase {
        [Test]
        public void WeightedSelector_AllChildrenFail_ReturnsFailure() {
            var selector = new WeightedRandomSelector(new Random(1)) {
                { CreateFailureNode(), 1 },
                { CreateFailureNode(), 2 },
                { CreateFailureNode(), 3 },
            };

            Assert.AreEqual(NodeResult.Failure, selector.OnUpdate());
        }

        [Test]
        public void WeightedSelector_AnyChildSucceeds_ReturnsSuccess() {
            var selector = new WeightedRandomSelector(new Random(1)) {
                { CreateFailureNode(), 1 },
                { CreateSuccessNode(), 2 },
                { CreateFailureNode(), 3 },
            };

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
        }

        [Test]
        public void WeightedSelector_RollSelectsByCumulativeWeight() {
            // With DoubleValue = 0.5 and weights [1, 2, 1] the order is [1, 2, 0]:
            // second fails, third runs, first is never reached.
            var first = new TrackingNode(() => NodeResult.Success);
            var second = new TrackingNode(() => NodeResult.Failure);
            var third = new TrackingNode(() => NodeResult.Running);

            var selector = new WeightedRandomSelector(new StubRandom { DoubleValue = 0.5 }) {
                { first, 1 },
                { second, 2 },
                { third, 1 },
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(1, second.Updates);
            Assert.AreEqual(1, third.Starts);
            Assert.AreEqual(0, first.Starts);
        }

        [Test]
        public void WeightedSelector_ChildAddedWithoutWeight_DefaultsToOne() {
            // Mixed adds give effective weights [1, 2, 1]; with DoubleValue = 0.5 the
            // second child is picked first, as in RollSelectsByCumulativeWeight.
            var first = new TrackingNode(() => NodeResult.Running);
            var second = new TrackingNode(() => NodeResult.Running);
            var third = new TrackingNode(() => NodeResult.Running);

            var selector = new WeightedRandomSelector(new StubRandom { DoubleValue = 0.5 }) {
                first,
                { second, 2 },
                third,
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(1, second.Starts);
            Assert.AreEqual(0, first.Starts);
            Assert.AreEqual(0, third.Starts);
        }

        [Test]
        public void WeightedSelector_ZeroWeightChild_TriedAfterWeightedChildren() {
            var first = new TrackingNode(() => NodeResult.Running);
            var second = new TrackingNode(() => NodeResult.Failure);

            var selector = new WeightedRandomSelector(new StubRandom { DoubleValue = 0 }) {
                { first, 0 },
                { second, 1 },
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(1, second.Updates);
            Assert.AreEqual(1, first.Starts);
        }

        [Test]
        public void WeightedSelector_AllZeroWeights_TriedInDeclarationOrder() {
            var first = new TrackingNode(() => NodeResult.Running);
            var second = new TrackingNode(() => NodeResult.Running);

            var selector = new WeightedRandomSelector(new StubRandom { DoubleValue = 0.5 }) {
                { first, 0 },
                { second, 0 },
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(1, first.Starts);
            Assert.AreEqual(0, second.Starts);
        }

        [Test]
        public void WeightedSelector_ResumesRunningChildWithoutReTickingEarlier() {
            // Order [1, 2, 0]: second fails, third runs, first is never reached.
            var thirdResult = NodeResult.Running;
            var first = new TrackingNode(() => NodeResult.Success);
            var second = new TrackingNode(() => NodeResult.Failure);
            var third = new TrackingNode(() => thirdResult);

            var selector = new WeightedRandomSelector(new StubRandom { DoubleValue = 0.5 }) {
                { first, 1 },
                { second, 2 },
                { third, 1 },
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(1, second.Updates);
            Assert.AreEqual(2, third.Updates);

            thirdResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
            Assert.AreEqual(0, first.Starts);
        }

        [Test]
        public void WeightedSelector_ReRollsOrderOnNextActivation() {
            var stub = new StubRandom { DoubleValue = 0 };

            var first = new TrackingNode(() => NodeResult.Success);
            var second = new TrackingNode(() => NodeResult.Success);

            var selector = new WeightedRandomSelector(stub) {
                { first, 1 },
                { second, 1 },
            };

            // Roll 0 puts first at the front.
            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
            Assert.AreEqual(1, first.Starts);

            // Roll 0.9 puts second at the front this time.
            stub.DoubleValue = 0.9;

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
            Assert.AreEqual(1, second.Starts);
            Assert.AreEqual(1, first.Starts);
        }

        [Test]
        public void WeightedSelector_NegativeWeight_Throws() {
            var selector = new WeightedRandomSelector();

            Assert.Throws<ArgumentOutOfRangeException>(() => selector.Add(CreateSuccessNode(), -1));
        }
    }
}
