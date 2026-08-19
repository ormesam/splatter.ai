using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class ReactiveSelectorTests : TestBase {
        [Test]
        public void ReactiveSelector_AllChildrenFail_ReturnsFailure() {
            var selector = new ReactiveSelector() {
                CreateFailureNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, selector.OnUpdate());
        }

        [Test]
        public void ReactiveSelector_AnyChildSucceeds_ReturnsSuccess() {
            var selector = new ReactiveSelector() {
                CreateFailureNode(),
                CreateSuccessNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
        }

        [Test]
        public void ReactiveSelector_ReTicksEarlierChildrenEveryUpdate() {
            var highPriority = new TrackingNode(() => NodeResult.Failure);
            var fallback = new TrackingNode(() => NodeResult.Running);

            var selector = new ReactiveSelector() { highPriority, fallback };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());

            Assert.AreEqual(2, highPriority.Updates);
            Assert.AreEqual(2, fallback.Updates);
        }

        [Test]
        public void ReactiveSelector_HigherPrioritySucceeds_StopsRunningChild() {
            var highPriorityResult = NodeResult.Failure;
            var highPriority = new TrackingNode(() => highPriorityResult);
            var fallback = new TrackingNode(() => NodeResult.Running);

            var selector = new ReactiveSelector() { highPriority, fallback };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.IsTrue(fallback.IsStarted);

            highPriorityResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, selector.OnUpdate());
            Assert.AreEqual(1, fallback.Stops);
            Assert.IsFalse(fallback.IsStarted);
        }

        [Test]
        public void ReactiveSelector_HigherPriorityStartsRunning_StopsRunningChild() {
            var highPriorityResult = NodeResult.Failure;
            var highPriority = new TrackingNode(() => highPriorityResult);
            var fallback = new TrackingNode(() => NodeResult.Running);

            var selector = new ReactiveSelector() { highPriority, fallback };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());

            highPriorityResult = NodeResult.Running;

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(1, fallback.Stops);
            Assert.IsFalse(fallback.IsStarted);
        }
    }
}
