using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class RetryUntilSuccessTests : TestBase {
        [Test]
        public void RetryUntilSuccess_SucceedsOnLaterAttempt() {
            var childResult = NodeResult.Failure;
            var child = new TrackingNode(() => childResult);

            var retry = new RetryUntilSuccess();
            retry.Child = child;

            Assert.AreEqual(NodeResult.Running, retry.OnUpdate());
            Assert.AreEqual(NodeResult.Running, retry.OnUpdate());

            childResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, retry.OnUpdate());
            Assert.AreEqual(3, child.Starts);
        }

        [Test]
        public void RetryUntilSuccess_Capped_FailsAfterMaxAttempts() {
            var child = new TrackingNode(() => NodeResult.Failure);

            var retry = new RetryUntilSuccess(2);
            retry.Child = child;

            Assert.AreEqual(NodeResult.Running, retry.OnUpdate());
            Assert.AreEqual(NodeResult.Failure, retry.OnUpdate());
            Assert.AreEqual(2, child.Starts);
        }

        [Test]
        public void RetryUntilSuccess_RunningChild_ReturnsRunning() {
            var child = new TrackingNode(() => NodeResult.Running);

            var retry = new RetryUntilSuccess(1);
            retry.Child = child;

            Assert.AreEqual(NodeResult.Running, retry.OnUpdate());
            Assert.AreEqual(NodeResult.Running, retry.OnUpdate());
            Assert.AreEqual(1, child.Starts);
        }

        [Test]
        public void RetryUntilSuccess_AttemptsResetOnRestart() {
            var retry = new RetryUntilSuccess(2);
            retry.Child = CreateFailureNode();

            Assert.AreEqual(NodeResult.Running, retry.OnUpdate());
            Assert.AreEqual(NodeResult.Failure, retry.OnUpdate());

            // The decorator completed, so the next update starts a fresh run of attempts.
            Assert.AreEqual(NodeResult.Running, retry.OnUpdate());
            Assert.AreEqual(NodeResult.Failure, retry.OnUpdate());
        }
    }
}
