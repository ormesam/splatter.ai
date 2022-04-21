using NUnit.Framework;

namespace Splatter.AI.Tests {
    public class DecoratorTests : TestBase {
        [Test]
        [TestCaseSource(nameof(GetNodes))]
        public void Decorator_Successful(Node node) {
            var decorator = new SuccessDecorator("Decorator", Tree);
            decorator.Child = node;

            Assert.AreEqual(NodeResult.Success, decorator.Execute());
        }

        [Test]
        [TestCaseSource(nameof(GetNodes))]
        public void Decorator_Failure(Node node) {
            var decorator = new FailureDecorator("Decorator", Tree);
            decorator.Child = node;

            Assert.AreEqual(NodeResult.Failure, decorator.Execute());
        }

        [Test]
        [TestCaseSource(nameof(GetNodes))]
        public void Decorator_Running(Node node) {
            var decorator = new RunningDecorator("Decorator", Tree);
            decorator.Child = node;

            Assert.AreEqual(NodeResult.Running, decorator.Execute());
        }

        [Test]
        public void Invert_Running() {
            var decorator = new InvertDecorator(Tree);
            decorator.Child = CreateRunningNode();

            Assert.AreEqual(NodeResult.Running, decorator.Execute());
        }

        [Test]
        public void Invert_Success() {
            var decorator = new InvertDecorator(Tree);
            decorator.Child = CreateFailureNode();

            Assert.AreEqual(NodeResult.Success, decorator.Execute());
        }

        [Test]
        public void Invert_Failure() {
            var decorator = new InvertDecorator(Tree);
            decorator.Child = CreateSuccessNode();

            Assert.AreEqual(NodeResult.Failure, decorator.Execute());
        }
    }
}
