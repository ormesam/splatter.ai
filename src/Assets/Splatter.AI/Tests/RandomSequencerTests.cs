using System;
using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class RandomSequencerTests : TestBase {
        [Test]
        public void RandomSequencer_AllChildrenSucceed_ReturnsSuccess() {
            var first = new TrackingNode(() => NodeResult.Success);
            var second = new TrackingNode(() => NodeResult.Success);
            var third = new TrackingNode(() => NodeResult.Success);

            var sequencer = new RandomSequencer(new Random(1)) { first, second, third };

            Assert.AreEqual(NodeResult.Success, sequencer.OnUpdate());
            Assert.AreEqual(1, first.Starts);
            Assert.AreEqual(1, second.Starts);
            Assert.AreEqual(1, third.Starts);
        }

        [Test]
        public void RandomSequencer_AnyChildFails_ReturnsFailure() {
            var sequencer = new RandomSequencer(new Random(1)) {
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());
        }

        [Test]
        public void RandomSequencer_ExecutionOrder_IsShuffled() {
            // With Value = 0 the shuffle of three children produces the order [1, 2, 0].
            var first = new TrackingNode(() => NodeResult.Running);
            var second = new TrackingNode(() => NodeResult.Running);
            var third = new TrackingNode(() => NodeResult.Running);

            var sequencer = new RandomSequencer(new StubRandom { Value = 0 }) { first, second, third };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(0, first.Starts);
            Assert.AreEqual(1, second.Starts);
            Assert.AreEqual(0, third.Starts);
        }

        [Test]
        public void RandomSequencer_ResumesRunningChildWithoutReTickingEarlier() {
            // Shuffled order is [1, 2, 0]: second succeeds, third runs, first waits its turn.
            var thirdResult = NodeResult.Running;
            var first = new TrackingNode(() => NodeResult.Success);
            var second = new TrackingNode(() => NodeResult.Success);
            var third = new TrackingNode(() => thirdResult);

            var sequencer = new RandomSequencer(new StubRandom { Value = 0 }) { first, second, third };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(1, second.Updates);
            Assert.AreEqual(2, third.Updates);
            Assert.AreEqual(0, first.Starts);

            thirdResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, sequencer.OnUpdate());
            Assert.AreEqual(1, first.Starts);
        }

        [Test]
        public void RandomSequencer_ReshufflesOnNextActivation() {
            var stub = new StubRandom { Value = 0 };

            var firstResult = NodeResult.Running;
            var first = new TrackingNode(() => firstResult);
            var second = new TrackingNode(() => NodeResult.Running);
            var third = new TrackingNode(() => NodeResult.Running);

            var sequencer = new RandomSequencer(stub) { first, second, third };

            // Order [1, 2, 0]: second runs first.
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(1, second.Starts);

            sequencer.Stop();

            // A large value leaves the order unshuffled, so first goes first this time.
            stub.Value = 99;

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(1, first.Starts);
        }
    }
}
