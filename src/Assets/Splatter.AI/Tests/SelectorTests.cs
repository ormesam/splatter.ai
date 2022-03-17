using NUnit.Framework;
using Splatter.AI;

namespace Splatter.AI.Tests {
    public class SelectorTests : TestBase {
        [Test]
        public void Selector_Success() {
            Selector selector = new Selector("Selector", Tree);
            selector.Children = new[] {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Success, selector.Execute());

            selector.Children = new[] {
                CreateFailureNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Success, selector.Execute());

            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Success, selector.Execute());
        }

        [Test]
        public void Selector_Failed() {
            Selector selector = new Selector("Selector", Tree);
            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Failure, selector.Execute());
        }

        [Test]
        public void Selector_Running() {
            Selector selector = new Selector("Selector", Tree);
            selector.Children = new[] {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
        }

        [Test]
        public void Selector_Abort_Self() {
            bool condition = true;

            Selector selector = new Selector("Selector", Tree);
            selector.SetAbortType(AbortType.Self, () => condition);
            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            condition = false;
            Assert.AreEqual(NodeResult.Failure, selector.Execute());
        }

        [Test]
        public void Selector_Abort_Lower() {
            bool condition = false;

            Selector selector = new Selector("Selector", Tree);
            Selector childSelector = new Selector("Selector", Tree);
            childSelector.SetAbortType(AbortType.Lower, () => condition);

            childSelector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            selector.Children = new[] {
                CreateFailureNode(),
                childSelector,
                CreateFailureNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Success, selector.Execute());

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());

            condition = true;

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
        }

        [Test]
        public void Selector_Abort_SelfAndLower() {
            bool condition = false;

            Selector selector = new Selector("Selector", Tree);
            Selector childSelector = new Selector("Selector", Tree);
            childSelector.SetAbortType(AbortType.SelfAndLower, () => condition);

            childSelector.Children = new[] {
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                childSelector,
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Success, selector.Execute());

            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());
            Assert.AreEqual(NodeResult.Running, selector.Execute());

            condition = true;

            Assert.AreEqual(NodeResult.Success, selector.Execute());
        }
    }
}