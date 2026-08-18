using NUnit.Framework;

namespace Splatter.AI.Tests {
    public class SelectorTests : TestBase {
        [Test]
        public void Selector_NoChildren() {
            Selector selector = new Selector(Tree);

            Assert.AreEqual(NodeResult.Failure, selector.OnUpdate());
        }

        [Test]
        public void Selector_Success() {
            Selector selector = new Selector(Tree);
            selector.Children = new[] {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());

            selector.Children = new[] {
                CreateFailureNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());

            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
        }

        [Test]
        public void Selector_Failed() {
            Selector selector = new Selector(Tree);
            selector.Children = new[] {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Failure, selector.OnUpdate());
        }

        [Test]
        public void Selector_Running() {
            Selector selector = new Selector(Tree);
            selector.Children = new[] {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
        }
    }
}