using NUnit.Framework;
using Splatter.AI.Decorators;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class GuardDecoratorTests : TestBase {
        [Test]
        public void Guard_ConditionFalse_FailsWithoutRunningChild() {
            var child = new TrackingNode(() => NodeResult.Success);

            var guard = new GuardDecorator(() => false);
            guard.Child = child;

            Assert.AreEqual(NodeResult.Failure, guard.OnUpdate());
            Assert.AreEqual(0, child.Starts);
        }

        [Test]
        public void Guard_ConditionTrue_PassesResultThrough() {
            var guard = new GuardDecorator(() => true);

            guard.Child = CreateSuccessNode();
            Assert.AreEqual(NodeResult.Success, guard.OnUpdate());

            guard.Child = CreateFailureNode();
            Assert.AreEqual(NodeResult.Failure, guard.OnUpdate());

            guard.Child = CreateRunningNode();
            Assert.AreEqual(NodeResult.Running, guard.OnUpdate());
        }

        [Test]
        public void Guard_ConditionBecomesFalse_StopsRunningChild() {
            var conditionValue = true;
            var child = new TrackingNode(() => NodeResult.Running);

            var guard = new GuardDecorator(() => conditionValue);
            guard.Child = child;

            Assert.AreEqual(NodeResult.Running, guard.OnUpdate());
            Assert.IsTrue(child.IsStarted);

            conditionValue = false;

            Assert.AreEqual(NodeResult.Failure, guard.OnUpdate());
            Assert.AreEqual(1, child.Stops);
            Assert.IsFalse(child.IsStarted);
        }

        [Test]
        public void Guard_ConditionBecomesTrueAgain_RestartsChild() {
            var conditionValue = false;
            var child = new TrackingNode(() => NodeResult.Running);

            var guard = new GuardDecorator(() => conditionValue);
            guard.Child = child;

            Assert.AreEqual(NodeResult.Failure, guard.OnUpdate());

            conditionValue = true;

            Assert.AreEqual(NodeResult.Running, guard.OnUpdate());
            Assert.AreEqual(1, child.Starts);
        }
    }
}
