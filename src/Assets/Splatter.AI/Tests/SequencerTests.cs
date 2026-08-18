using NUnit.Framework;

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

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
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

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
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
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
        }
    }
}