using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class SequencerTests : TestBase {
        [Test]
        public void Sequencer_NoChildren() {
            Sequencer sequencer = new Sequencer(Tree);

            Assert.AreEqual(NodeResult.Success, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_Success() {
            Sequencer sequencer = new Sequencer(Tree);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Success, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_Failure() {
            Sequencer sequencer = new Sequencer(Tree);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());

            sequencer.Children = new[] {
                CreateFailureNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_Running() {
            Sequencer sequencer = new Sequencer(Tree);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_ResumesAtRunningChild() {
            var secondResult = NodeResult.Running;

            var first = new TrackingNode(Tree, () => NodeResult.Success);
            var second = new TrackingNode(Tree, () => secondResult);
            var third = new TrackingNode(Tree, () => NodeResult.Success);

            Sequencer sequencer = new Sequencer(Tree);
            sequencer.Children = new Node[] { first, second, third };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());

            secondResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, sequencer.OnUpdate());

            Assert.AreEqual(1, first.Updates);
            Assert.AreEqual(3, second.Updates);
            Assert.AreEqual(1, third.Updates);
        }
    }
}
