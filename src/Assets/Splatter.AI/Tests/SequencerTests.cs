using NUnit.Framework;

namespace Splatter.AI.Tests {
    public class SequencerTests : TestBase {
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

        [Test]
        public void Sequencer_Abort_Self() {
            bool condition = true;

            Sequencer sequencer = new Sequencer(Tree);
            sequencer.SetAbortType(AbortType.Self, () => condition);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            condition = false;
            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_Abort_Lower() {
            bool condition = false;

            Sequencer sequencer = new Sequencer(Tree);
            Sequencer childSequencer = new Sequencer(Tree);
            childSequencer.SetAbortType(AbortType.Lower, () => condition);

            childSequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            sequencer.Children = new[] {
                CreateSuccessNode(),
                childSequencer,
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());

            condition = true;

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
        }

        [Test]
        public void Sequencer_Abort_SelfAndLower() {
            bool condition = false;

            Sequencer sequencer = new Sequencer(Tree);
            Sequencer childSequencer = new Sequencer(Tree);
            childSequencer.SetAbortType(AbortType.SelfAndLower, () => condition);

            childSequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            sequencer.Children = new[] {
                CreateSuccessNode(),
                childSequencer,
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Failure, sequencer.OnUpdate());

            condition = true;

            // Will always be running as nothing will get executed past the child sequencer
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
            Assert.AreEqual(NodeResult.Running, sequencer.OnUpdate());
        }
    }
}