using NUnit.Framework;
using Splatter.AI.Composites;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class ReactiveSequencerTests : TestBase {
        [Test]
        public void ReactiveSequencer_AllChildrenSucceed_ReturnsSuccess() {
            var sequencer = new ReactiveSequencer() {
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Success, sequencer.OnUpdate());
        }

        [Test]
        public void ReactiveSequencer_AnyChildFails_ReturnsFailure() {
            var sequencer = new ReactiveSequencer() {
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());
        }

        [Test]
        public void ReactiveSequencer_ReTicksEarlierChildrenEveryUpdate() {
            var guard = new TrackingNode(() => NodeResult.Success);
            var action = new TrackingNode(() => NodeResult.Running);

            var sequencer = new ReactiveSequencer() { guard, action };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());

            Assert.AreEqual(2, guard.Updates);
            Assert.AreEqual(2, action.Updates);
        }

        [Test]
        public void ReactiveSequencer_GuardFails_StopsRunningChild() {
            var guardResult = NodeResult.Success;
            var guard = new TrackingNode(() => guardResult);
            var action = new TrackingNode(() => NodeResult.Running);

            var sequencer = new ReactiveSequencer() { guard, action };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.IsTrue(action.IsStarted);

            guardResult = NodeResult.Failure;

            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());
            Assert.AreEqual(1, action.Stops);
            Assert.IsFalse(action.IsStarted);
        }

        [Test]
        public void ReactiveSequencer_EarlierChildRunning_StopsLaterRunningChild() {
            var guardResult = NodeResult.Success;
            var guard = new TrackingNode(() => guardResult);
            var action = new TrackingNode(() => NodeResult.Running);

            var sequencer = new ReactiveSequencer() { guard, action };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());

            guardResult = NodeResult.Running;

            // The guard now holds the sequence at index 0, so the action is stopped
            // without being ticked this update.
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(1, action.Stops);
            Assert.AreEqual(1, action.Updates);
            Assert.IsFalse(action.IsStarted);
        }
    }
}
