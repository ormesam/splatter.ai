using NUnit.Framework;
using Splatter.AI;

namespace Splatter.AI.Tests {
    public class SequencerTests : TestBase {
        [Test]
        public void Sequencer_Success() {
            Sequencer sequencer = new Sequencer("Sequencer", Tree, false);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Success, sequencer.Execute());
        }

        [Test]
        public void Sequencer_Failure() {
            Sequencer sequencer = new Sequencer("Sequencer", Tree, false);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());

            sequencer.Children = new[] {
                CreateFailureNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());
        }

        [Test]
        public void Sequencer_Running() {
            Sequencer sequencer = new Sequencer("Sequencer", Tree, false);
            sequencer.Children = new[] {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());

            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
        }

        [Test]
        public void Sequencer_Reset() {
            Sequencer sequencer = new Sequencer("Sequencer", Tree, true);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Tree.IncrementTick();

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Tree.IncrementTick();
            Tree.IncrementTick();

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Tree.IncrementTick();

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Tree.IncrementTick();

            Assert.AreEqual(NodeResult.Success, sequencer.Execute());
            Tree.IncrementTick();
        }

        [Test]
        public void Sequencer_Abort_Self() {
            bool condition = true;

            Sequencer sequencer = new Sequencer("Sequencer", Tree, false, AbortType.Self, () => condition);
            sequencer.Children = new[] {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            condition = false;
            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());
        }

        [Test]
        public void Sequencer_Abort_Lower() {
            bool condition = false;

            Sequencer sequencer = new Sequencer("Sequencer", Tree, false);
            Sequencer childSequencer = new Sequencer("Sequencer", Tree, false, AbortType.Lower, () => condition);

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

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());

            condition = true;

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
        }

        [Test]
        public void Sequencer_Abort_SelfAndLower() {
            bool condition = false;

            Sequencer sequencer = new Sequencer("Sequencer", Tree, false);
            Sequencer childSequencer = new Sequencer("Sequencer", Tree, false, AbortType.SelfAndLower, () => condition);

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

            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Failure, sequencer.Execute());

            condition = true;

            // Will always be running as nothing will get executed past the child sequencer
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
            Assert.AreEqual(NodeResult.Running, sequencer.Execute());
        }
    }
}