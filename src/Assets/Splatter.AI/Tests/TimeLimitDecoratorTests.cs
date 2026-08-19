using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class TimeLimitDecoratorTests : TestBase {
        [Test]
        public void TimeLimit_ChildCompletesInTime_PassesResultThrough() {
            float now = 0f;

            var timeLimit = new TimeLimitDecorator(5f, () => now);

            timeLimit.Child = CreateSuccessNode();
            Assert.AreEqual(NodeResult.Success, timeLimit.OnUpdate());

            timeLimit.Child = CreateFailureNode();
            Assert.AreEqual(NodeResult.Failure, timeLimit.OnUpdate());
        }

        [Test]
        public void TimeLimit_RunningChild_WithinLimit_ReturnsRunning() {
            float now = 0f;
            var child = new TrackingNode(() => NodeResult.Running);

            var timeLimit = new TimeLimitDecorator(5f, () => now);
            timeLimit.Child = child;

            Assert.AreEqual(NodeResult.Running, timeLimit.OnUpdate());

            now = 4f;

            Assert.AreEqual(NodeResult.Running, timeLimit.OnUpdate());
        }

        [Test]
        public void TimeLimit_RunningChild_PastDeadline_IsStopped() {
            float now = 0f;
            var child = new TrackingNode(() => NodeResult.Running);

            var timeLimit = new TimeLimitDecorator(5f, () => now);
            timeLimit.Child = child;

            Assert.AreEqual(NodeResult.Running, timeLimit.OnUpdate());

            now = 5f;

            Assert.AreEqual(NodeResult.Failure, timeLimit.OnUpdate());
            Assert.AreEqual(1, child.Stops);
            Assert.IsFalse(child.IsStarted);
        }

        [Test]
        public void TimeLimit_DeadlineResetsOnNextActivation() {
            float now = 0f;
            var childResult = NodeResult.Running;
            var child = new TrackingNode(() => childResult);

            var timeLimit = new TimeLimitDecorator(5f, () => now);
            timeLimit.Child = child;

            timeLimit.OnUpdate();

            now = 5f;

            Assert.AreEqual(NodeResult.Failure, timeLimit.OnUpdate());

            // The decorator completed, so the next update starts a fresh time window.
            Assert.AreEqual(NodeResult.Running, timeLimit.OnUpdate());

            now = 9f;

            Assert.AreEqual(NodeResult.Running, timeLimit.OnUpdate());

            childResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, timeLimit.OnUpdate());
        }
    }
}
