using System;
using NUnit.Framework;
using Splatter.AI.Composites;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class RandomSelectorTests : TestBase {
        [Test]
        public void RandomSelector_AllChildrenFail_ReturnsFailure() {
            var selector = new RandomSelector(new Random(1)) {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, selector.OnUpdate());
        }

        [Test]
        public void RandomSelector_AnyChildSucceeds_ReturnsSuccess() {
            var selector = new RandomSelector(new Random(1)) {
                CreateFailureNode(),
                CreateSuccessNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
        }

        [Test]
        public void RandomSelector_ExecutionOrder_IsShuffled() {
            // With Value = 0 the shuffle of three children produces the order [1, 2, 0].
            var first = new TrackingNode(() => NodeResult.Running);
            var second = new TrackingNode(() => NodeResult.Running);
            var third = new TrackingNode(() => NodeResult.Running);

            var selector = new RandomSelector(new StubRandom { Value = 0 }) { first, second, third };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(0, first.Starts);
            Assert.AreEqual(1, second.Starts);
            Assert.AreEqual(0, third.Starts);
        }

        [Test]
        public void RandomSelector_ResumesRunningChildWithoutReTickingEarlier() {
            // Shuffled order is [1, 2, 0]: second fails, third runs, first is never reached.
            var secondResult = NodeResult.Running;
            var first = new TrackingNode(() => NodeResult.Success);
            var second = new TrackingNode(() => NodeResult.Failure);
            var third = new TrackingNode(() => secondResult);

            var selector = new RandomSelector(new StubRandom { Value = 0 }) { first, second, third };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(1, second.Updates);
            Assert.AreEqual(2, third.Updates);

            secondResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
            Assert.AreEqual(0, first.Starts);
        }

        [Test]
        public void RandomSelector_ReshufflesOnNextActivation() {
            var stub = new StubRandom { Value = 0 };

            var first = new TrackingNode(() => NodeResult.Success);
            var second = new TrackingNode(() => NodeResult.Success);
            var third = new TrackingNode(() => NodeResult.Success);

            var selector = new RandomSelector(stub) { first, second, third };

            // Order [1, 2, 0]: second succeeds immediately.
            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
            Assert.AreEqual(1, second.Starts);

            // A large value leaves the order unshuffled, so first goes first this time.
            stub.Value = 99;

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
            Assert.AreEqual(1, first.Starts);
            Assert.AreEqual(1, second.Starts);
        }
    }
}
