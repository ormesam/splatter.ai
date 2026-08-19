using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class CooldownDecoratorTests : TestBase {
        [Test]
        public void Cooldown_FirstRun_PassesResultThrough() {
            float now = 0f;

            var cooldown = new CooldownDecorator(5f, () => now);

            cooldown.Child = CreateSuccessNode();
            Assert.AreEqual(NodeResult.Success, cooldown.OnUpdate());

            cooldown = new CooldownDecorator(5f, () => now);
            cooldown.Child = CreateFailureNode();
            Assert.AreEqual(NodeResult.Failure, cooldown.OnUpdate());
        }

        [Test]
        public void Cooldown_WhileCooling_FailsWithoutRunningChild() {
            float now = 0f;
            var child = new TrackingNode(() => NodeResult.Success);

            var cooldown = new CooldownDecorator(5f, () => now);
            cooldown.Child = child;

            Assert.AreEqual(NodeResult.Success, cooldown.OnUpdate());

            now = 4f;

            Assert.AreEqual(NodeResult.Failure, cooldown.OnUpdate());
            Assert.AreEqual(1, child.Updates);
        }

        [Test]
        public void Cooldown_AfterWindow_RunsChildAgain() {
            float now = 0f;
            var child = new TrackingNode(() => NodeResult.Success);

            var cooldown = new CooldownDecorator(5f, () => now);
            cooldown.Child = child;

            Assert.AreEqual(NodeResult.Success, cooldown.OnUpdate());

            now = 5f;

            Assert.AreEqual(NodeResult.Success, cooldown.OnUpdate());
            Assert.AreEqual(2, child.Starts);
        }

        [Test]
        public void Cooldown_RunningChild_IsNotInterrupted() {
            float now = 0f;
            var childResult = NodeResult.Running;
            var child = new TrackingNode(() => childResult);

            var cooldown = new CooldownDecorator(5f, () => now);
            cooldown.Child = child;

            Assert.AreEqual(NodeResult.Running, cooldown.OnUpdate());

            now = 100f;

            Assert.AreEqual(NodeResult.Running, cooldown.OnUpdate());

            childResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, cooldown.OnUpdate());

            // Completing starts a fresh cooldown window from the completion time.
            Assert.AreEqual(NodeResult.Failure, cooldown.OnUpdate());

            now = 105f;

            Assert.AreEqual(NodeResult.Success, cooldown.OnUpdate());
        }
    }
}
