using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class SelectorTests : TestBase {
        [Test]
        public void Selector_NoChildren() {
            Selector selector = new Selector(Tree);

            Assert.AreEqual(NodeResult.Failure, selector.OnUpdate());
        }

        [Test]
        public void Selector_Success() {
            Selector selector = new Selector(Tree) {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());

            selector = new Selector(Tree) {
                CreateFailureNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());

            selector = new Selector(Tree) {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
        }

        [Test]
        public void Selector_Failed() {
            Selector selector = new Selector(Tree) {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, selector.OnUpdate());
        }

        [Test]
        public void Selector_Running() {
            Selector selector = new Selector(Tree) {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
        }

        [Test]
        public void Selector_ResumesAtRunningChild() {
            var secondResult = NodeResult.Running;

            var first = new TrackingNode(Tree, () => NodeResult.Failure);
            var second = new TrackingNode(Tree, () => secondResult);
            var third = new TrackingNode(Tree, () => NodeResult.Success);

            Selector selector = new Selector(Tree) { first, second, third };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());

            secondResult = NodeResult.Failure;

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());

            Assert.AreEqual(1, first.Updates);
            Assert.AreEqual(3, second.Updates);
            Assert.AreEqual(1, third.Updates);
        }
    }
}
