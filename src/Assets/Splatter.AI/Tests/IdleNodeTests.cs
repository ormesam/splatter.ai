using NUnit.Framework;

namespace Splatter.AI.Tests {
    public class IdleNodeTests : TestBase {
        [Test]
        public void Idle_ReturnsRunningEveryUpdate() {
            var idle = new IdleNode();

            Assert.AreEqual(NodeResult.Running, idle.OnUpdate());
            Assert.AreEqual(NodeResult.Running, idle.OnUpdate());
        }

        [Test]
        public void Idle_Stop_ResetsStartedState() {
            var idle = new IdleNode();

            idle.OnUpdate();

            Assert.IsTrue(idle.IsStarted);

            idle.Stop();

            Assert.IsFalse(idle.IsStarted);
        }
    }
}
