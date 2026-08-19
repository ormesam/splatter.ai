using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class RepeaterTests : TestBase {
        [Test]
        public void Repeater_Unlimited_RestartsChildEachCompletion() {
            var child = new TrackingNode(() => NodeResult.Success);

            var repeater = new Repeater();
            repeater.Child = child;

            Assert.AreEqual(NodeResult.Running, repeater.OnUpdate());
            Assert.AreEqual(NodeResult.Running, repeater.OnUpdate());
            Assert.AreEqual(NodeResult.Running, repeater.OnUpdate());

            Assert.AreEqual(3, child.Starts);
            Assert.AreEqual(3, child.Stops);
        }

        [Test]
        public void Repeater_FixedCount_SucceedsAfterCompletions() {
            var child = new TrackingNode(() => NodeResult.Success);

            var repeater = new Repeater(2);
            repeater.Child = child;

            Assert.AreEqual(NodeResult.Running, repeater.OnUpdate());
            Assert.AreEqual(NodeResult.Success, repeater.OnUpdate());
            Assert.AreEqual(2, child.Starts);
        }

        [Test]
        public void Repeater_ChildFailure_CountsAsCompletion() {
            var child = new TrackingNode(() => NodeResult.Failure);

            var repeater = new Repeater(2);
            repeater.Child = child;

            Assert.AreEqual(NodeResult.Running, repeater.OnUpdate());
            Assert.AreEqual(NodeResult.Success, repeater.OnUpdate());
        }

        [Test]
        public void Repeater_RunningChild_DoesNotCount() {
            var childResult = NodeResult.Running;
            var child = new TrackingNode(() => childResult);

            var repeater = new Repeater(1);
            repeater.Child = child;

            Assert.AreEqual(NodeResult.Running, repeater.OnUpdate());
            Assert.AreEqual(NodeResult.Running, repeater.OnUpdate());
            Assert.AreEqual(1, child.Starts);

            childResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, repeater.OnUpdate());
        }

        [Test]
        public void Repeater_Stop_ResetsCompletionCount() {
            var repeater = new Repeater(2);
            repeater.Child = CreateSuccessNode();

            Assert.AreEqual(NodeResult.Running, repeater.OnUpdate());

            repeater.Stop();

            Assert.AreEqual(NodeResult.Running, repeater.OnUpdate());
            Assert.AreEqual(NodeResult.Success, repeater.OnUpdate());
        }
    }
}
