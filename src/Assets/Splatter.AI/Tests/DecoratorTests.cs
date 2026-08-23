using NUnit.Framework;
using Splatter.AI.Decorators;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class DecoratorTests : TestBase {
        [Test]
        [TestCaseSource(nameof(GetCompletedNodes))]
        public void Decorator_Successful(Node node) {
            var decorator = new SuccessDecorator();
            decorator.Child = node;

            Assert.AreEqual(NodeResult.Success, decorator.OnUpdate());
        }

        [Test]
        public void Decorator_Successful_RunningChild() {
            var decorator = new SuccessDecorator();
            decorator.Child = CreateRunningNode();

            Assert.AreEqual(NodeResult.Running, decorator.OnUpdate());
        }

        [Test]
        public void Decorator_Successful_ChildCompletesOnLaterUpdate() {
            var childResult = NodeResult.Running;
            var child = new TrackingNode(() => childResult);

            var decorator = new SuccessDecorator();
            decorator.Child = child;

            Assert.AreEqual(NodeResult.Running, decorator.OnUpdate());

            childResult = NodeResult.Failure;

            Assert.AreEqual(NodeResult.Success, decorator.OnUpdate());
            Assert.AreEqual(1, child.Starts);
            Assert.AreEqual(1, child.Stops);
            Assert.IsFalse(child.IsStarted);
        }

        [Test]
        [TestCaseSource(nameof(GetCompletedNodes))]
        public void Decorator_Failure(Node node) {
            var decorator = new FailureDecorator();
            decorator.Child = node;

            Assert.AreEqual(NodeResult.Failure, decorator.OnUpdate());
        }

        [Test]
        public void Decorator_Failure_RunningChild() {
            var decorator = new FailureDecorator();
            decorator.Child = CreateRunningNode();

            Assert.AreEqual(NodeResult.Running, decorator.OnUpdate());
        }

        [Test]
        public void Decorator_Failure_ChildCompletesOnLaterUpdate() {
            var childResult = NodeResult.Running;
            var child = new TrackingNode(() => childResult);

            var decorator = new FailureDecorator();
            decorator.Child = child;

            Assert.AreEqual(NodeResult.Running, decorator.OnUpdate());

            childResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Failure, decorator.OnUpdate());
            Assert.AreEqual(1, child.Starts);
            Assert.AreEqual(1, child.Stops);
            Assert.IsFalse(child.IsStarted);
        }

        [Test]
        [TestCaseSource(nameof(GetNodes))]
        public void Decorator_Running(Node node) {
            var decorator = new RunningDecorator();
            decorator.Child = node;

            Assert.AreEqual(NodeResult.Running, decorator.OnUpdate());
        }

        [Test]
        public void Decorator_Running_CompletedChild_IsNotRestarted() {
            var child = new TrackingNode(() => NodeResult.Success);

            var decorator = new RunningDecorator();
            decorator.Child = child;

            Assert.AreEqual(NodeResult.Running, decorator.OnUpdate());
            Assert.AreEqual(NodeResult.Running, decorator.OnUpdate());

            Assert.AreEqual(1, child.Starts);
            Assert.AreEqual(1, child.Updates);
        }

        [Test]
        public void Decorator_Running_AfterStop_RunsChildAgain() {
            var child = new TrackingNode(() => NodeResult.Success);

            var decorator = new RunningDecorator();
            decorator.Child = child;

            decorator.OnUpdate();
            decorator.Stop();

            Assert.AreEqual(NodeResult.Running, decorator.OnUpdate());

            Assert.AreEqual(2, child.Starts);
            Assert.AreEqual(2, child.Updates);
        }

        [Test]
        public void Invert_Running() {
            var decorator = new InvertDecorator();
            decorator.Child = CreateRunningNode();

            Assert.AreEqual(NodeResult.Running, decorator.OnUpdate());
        }

        [Test]
        public void Invert_Success() {
            var decorator = new InvertDecorator();
            decorator.Child = CreateFailureNode();

            Assert.AreEqual(NodeResult.Success, decorator.OnUpdate());
        }

        [Test]
        public void Invert_Failure() {
            var decorator = new InvertDecorator();
            decorator.Child = CreateSuccessNode();

            Assert.AreEqual(NodeResult.Failure, decorator.OnUpdate());
        }
    }
}
