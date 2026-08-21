using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class SequencerTests : TestBase {
        [Test]
        public void Sequencer_NoChildren() {
            Sequencer sequencer = new Sequencer();

            Assert.AreEqual(NodeResult.Success, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_Success() {
            Sequencer sequencer = new Sequencer() {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Success, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_Failure() {
            Sequencer sequencer = new Sequencer() {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());

            sequencer = new Sequencer() {
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());

            sequencer = new Sequencer() {
                CreateFailureNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_Running() {
            Sequencer sequencer = new Sequencer() {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_ResumesAtRunningChild() {
            var secondResult = NodeResult.Running;

            var first = new TrackingNode(() => NodeResult.Success);
            var second = new TrackingNode(() => secondResult);
            var third = new TrackingNode(() => NodeResult.Success);

            Sequencer sequencer = new Sequencer() { first, second, third };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());

            secondResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, sequencer.OnUpdate());

            Assert.AreEqual(1, first.Updates);
            Assert.AreEqual(3, second.Updates);
            Assert.AreEqual(1, third.Updates);
        }

        [Test]
        public void Sequencer_CurrentNodeIdx_TracksActiveChild() {
            Sequencer sequencer = new Sequencer() {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateSuccessNode(),
            };

            sequencer.OnUpdate();

            Assert.AreEqual(1, sequencer.CurrentNodeIdx);
        }
    }
}
