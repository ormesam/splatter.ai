using NUnit.Framework;
using Splatter.AI.Decorators;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class RepeatUntilFailureTests : TestBase {
        [Test]
        public void RepeatUntilFailure_RepeatsWhileChildSucceeds() {
            var child = new TrackingNode(() => NodeResult.Success);

            var repeat = new RepeatUntilFailure();
            repeat.Child = child;

            Assert.AreEqual(NodeResult.Running, repeat.OnUpdate());
            Assert.AreEqual(NodeResult.Running, repeat.OnUpdate());
            Assert.AreEqual(NodeResult.Running, repeat.OnUpdate());

            Assert.AreEqual(3, child.Starts);
            Assert.AreEqual(3, child.Stops);
        }

        [Test]
        public void RepeatUntilFailure_ChildFails_ReturnsSuccess() {
            var childResult = NodeResult.Success;
            var child = new TrackingNode(() => childResult);

            var repeat = new RepeatUntilFailure();
            repeat.Child = child;

            Assert.AreEqual(NodeResult.Running, repeat.OnUpdate());

            childResult = NodeResult.Failure;

            Assert.AreEqual(NodeResult.Success, repeat.OnUpdate());
        }

        [Test]
        public void RepeatUntilFailure_RunningChild_ReturnsRunning() {
            var child = new TrackingNode(() => NodeResult.Running);

            var repeat = new RepeatUntilFailure();
            repeat.Child = child;

            Assert.AreEqual(NodeResult.Running, repeat.OnUpdate());
            Assert.AreEqual(NodeResult.Running, repeat.OnUpdate());
            Assert.AreEqual(1, child.Starts);
        }
    }
}
