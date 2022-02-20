using NUnit.Framework;
using Splatter.AI;

namespace Splatter.Tests {
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
    }
}
