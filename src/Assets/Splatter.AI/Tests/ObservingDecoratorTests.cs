using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class ObservingDecoratorTests : TestBase {
        [Test]
        public void Observer_ConditionFalse_FailsWithoutRunningChild() {
            var child = new TrackingNode(() => NodeResult.Success);

            var observer = new TrackingObservingDecorator(() => false, AbortMode.Self);
            observer.Child = child;

            Assert.AreEqual(NodeResult.Failure, observer.OnUpdate());
            Assert.AreEqual(0, child.Starts);
        }

        [Test]
        public void Observer_ConditionTrue_PassesResultThrough() {
            var observer = new TrackingObservingDecorator(() => true, AbortMode.Self);

            observer.Child = CreateSuccessNode();
            Assert.AreEqual(NodeResult.Success, observer.OnUpdate());

            observer.Child = CreateFailureNode();
            Assert.AreEqual(NodeResult.Failure, observer.OnUpdate());

            observer.Child = CreateRunningNode();
            Assert.AreEqual(NodeResult.Running, observer.OnUpdate());
        }

        [Test]
        public void Observer_ChildRunning_DoesNotReEvaluateCondition() {
            var observer = new TrackingObservingDecorator(() => true, AbortMode.Self);
            observer.Child = CreateRunningNode();

            for (int i = 0; i < 5; i++) {
                Assert.AreEqual(NodeResult.Running, observer.OnUpdate());
            }

            Assert.AreEqual(1, observer.ConditionEvaluations);
        }

        [Test]
        public void Observer_SelfAbort_StopsChildOnNextUpdate() {
            var conditionValue = true;
            var child = new TrackingNode(() => NodeResult.Running);

            var observer = new TrackingObservingDecorator(() => conditionValue, AbortMode.Self);
            observer.Child = child;

            Assert.AreEqual(NodeResult.Running, observer.OnUpdate());

            conditionValue = false;
            observer.RaiseChanged();

            // Deferred: nothing happens at notification time.
            Assert.AreEqual(0, child.Stops);
            Assert.IsTrue(child.IsStarted);

            Assert.AreEqual(NodeResult.Failure, observer.OnUpdate());
            Assert.AreEqual(1, child.Stops);
            Assert.IsFalse(child.IsStarted);
        }

        [Test]
        public void Observer_TransientChange_KeepsChildRunning() {
            var child = new TrackingNode(() => NodeResult.Running);

            var observer = new TrackingObservingDecorator(() => true, AbortMode.Self);
            observer.Child = child;

            observer.OnUpdate();
            observer.RaiseChanged();

            // Condition still true when re-evaluated at the next update.
            Assert.AreEqual(NodeResult.Running, observer.OnUpdate());
            Assert.AreEqual(0, child.Stops);
            Assert.AreEqual(2, observer.ConditionEvaluations);
        }

        [Test]
        public void Observer_ModeNone_IgnoresChanges() {
            var conditionValue = true;
            var child = new TrackingNode(() => NodeResult.Running);

            var observer = new TrackingObservingDecorator(() => conditionValue, AbortMode.None);
            observer.Child = child;

            observer.OnUpdate();

            conditionValue = false;
            observer.RaiseChanged();

            Assert.AreEqual(NodeResult.Running, observer.OnUpdate());
            Assert.AreEqual(0, child.Stops);
            Assert.AreEqual(1, observer.ConditionEvaluations);
        }

        [Test]
        public void Observer_ModeLowerPriority_DoesNotStopOwnChild() {
            var conditionValue = true;
            var child = new TrackingNode(() => NodeResult.Running);

            var observer = new TrackingObservingDecorator(() => conditionValue, AbortMode.LowerPriority);
            observer.Child = child;

            observer.OnUpdate();

            conditionValue = false;
            observer.RaiseChanged();

            Assert.AreEqual(NodeResult.Running, observer.OnUpdate());
            Assert.AreEqual(0, child.Stops);
        }

        [Test]
        public void Observer_ModeNone_NeverSubscribes() {
            var observer = new TrackingObservingDecorator(() => true, AbortMode.None);
            observer.Child = CreateRunningNode();

            observer.OnUpdate();

            Assert.AreEqual(0, observer.StartObservingCalls);
        }

        [Test]
        public void Observer_ModeSelf_UnsubscribesOnCompletion() {
            var observer = new TrackingObservingDecorator(() => true, AbortMode.Self);
            observer.Child = CreateSuccessNode();

            observer.OnUpdate();

            Assert.AreEqual(1, observer.StartObservingCalls);
            Assert.AreEqual(1, observer.StopObservingCalls);
        }

        [Test]
        public void Observer_ModeSelf_ResubscribesOnRestart() {
            var observer = new TrackingObservingDecorator(() => true, AbortMode.Self);
            observer.Child = CreateSuccessNode();

            observer.OnUpdate();
            observer.OnUpdate();

            Assert.AreEqual(2, observer.StartObservingCalls);
            Assert.AreEqual(2, observer.StopObservingCalls);
        }

        [Test]
        public void Observer_ModeLowerPriority_KeepsObservingAfterCompletion() {
            var observer = new TrackingObservingDecorator(() => false, AbortMode.LowerPriority);
            observer.Child = CreateRunningNode();

            Assert.AreEqual(NodeResult.Failure, observer.OnUpdate());

            Assert.AreEqual(1, observer.StartObservingCalls);
            Assert.AreEqual(0, observer.StopObservingCalls);

            // Re-activation must not double-subscribe.
            observer.OnUpdate();

            Assert.AreEqual(1, observer.StartObservingCalls);
        }

        [Test]
        public void Observer_CompositeCompletes_ObservationCancelled() {
            var observer = new TrackingObservingDecorator(() => false, AbortMode.LowerPriority);
            observer.Child = CreateRunningNode();

            var selector = new Selector() {
                observer,
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, selector.OnUpdate());
            Assert.AreEqual(1, observer.StopObservingCalls);
        }

        [Test]
        public void Observer_CompositeStopped_ObservationCancelled() {
            var observer = new TrackingObservingDecorator(() => false, AbortMode.LowerPriority);
            observer.Child = CreateRunningNode();

            var selector = new Selector() {
                observer,
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, selector.OnUpdate());
            Assert.AreEqual(0, observer.StopObservingCalls);

            selector.Stop();

            Assert.AreEqual(1, observer.StopObservingCalls);
        }

        [Test]
        public void Observer_RestartedByRepeater_BalancedSubscriptions() {
            var observer = new TrackingObservingDecorator(() => false, AbortMode.LowerPriority);
            observer.Child = CreateRunningNode();

            var repeater = new Repeater();
            repeater.Child = new Selector() {
                observer,
                CreateFailureNode(),
            };

            // Each update completes the selector (all children fail) and the repeater
            // restarts it on the next update.
            repeater.OnUpdate();
            repeater.OnUpdate();

            Assert.AreEqual(2, observer.StartObservingCalls);
            Assert.AreEqual(2, observer.StopObservingCalls);
        }
    }
}
