using NUnit.Framework;
using Splatter.AI.Composites;
using Splatter.AI.Leaves;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class NodeInstrumentationTests : TestBase {
        [Test]
        public void FreshNode_HasNoStopReasonAndZeroStopCount() {
            var node = new TrackingNode(() => NodeResult.Running);

            Assert.AreEqual(NodeStopReason.None, node.LastStopReason);
            Assert.AreEqual(0, node.StopCount);
        }

        [Test]
        public void Complete_Success_RecordsSuccess() {
            var node = new TrackingNode(() => NodeResult.Success);

            node.OnUpdate();

            Assert.AreEqual(NodeStopReason.Success, node.LastStopReason);
            Assert.AreEqual(1, node.StopCount);
        }

        [Test]
        public void Complete_Failure_RecordsFailure() {
            var node = new TrackingNode(() => NodeResult.Failure);

            node.OnUpdate();

            Assert.AreEqual(NodeStopReason.Failure, node.LastStopReason);
            Assert.AreEqual(1, node.StopCount);
        }

        [Test]
        public void Complete_Repeatedly_IncrementsStopCount() {
            var node = new TrackingNode(() => NodeResult.Success);

            for (int i = 0; i < 5; i++) {
                node.OnUpdate();
            }

            Assert.AreEqual(5, node.StopCount);
        }

        [Test]
        public void Stop_RunningNode_RecordsAborted() {
            var node = new TrackingNode(() => NodeResult.Running);

            node.OnUpdate();
            node.Stop();

            Assert.AreEqual(NodeStopReason.Aborted, node.LastStopReason);
            Assert.AreEqual(1, node.StopCount);
            Assert.AreEqual(NodeResult.Running, node.Result);
        }

        [Test]
        public void Stop_CompletedNode_KeepsCompletionReason() {
            var node = new TrackingNode(() => NodeResult.Success);

            node.OnUpdate();
            node.Stop();

            Assert.AreEqual(NodeStopReason.Success, node.LastStopReason);
            Assert.AreEqual(1, node.StopCount);
        }

        [Test]
        public void Stop_Sequencer_CompletedChildKeepsReasonRunningChildAborts() {
            var first = new TrackingNode(() => NodeResult.Success);
            var second = new TrackingNode(() => NodeResult.Running);

            var sequencer = new Sequencer() { first, second };

            sequencer.OnUpdate();
            sequencer.Stop();

            Assert.AreEqual(NodeStopReason.Success, first.LastStopReason);
            Assert.AreEqual(NodeStopReason.Aborted, second.LastStopReason);
            Assert.AreEqual(NodeStopReason.Aborted, sequencer.LastStopReason);
        }

        [Test]
        public void ObserverAbort_LowerPriorityBranch_RecordsAborted() {
            var conditionValue = false;
            var lower = new TrackingNode(() => NodeResult.Running);

            var observer = new TrackingObservingDecorator(() => conditionValue, AbortMode.LowerPriority);
            observer.Child = new TrackingNode(() => NodeResult.Running);

            var selector = new Selector() { observer, lower };

            selector.OnUpdate();

            Assert.IsTrue(lower.IsStarted);

            conditionValue = true;
            observer.RaiseChanged();
            selector.OnUpdate();

            Assert.AreEqual(NodeStopReason.Aborted, lower.LastStopReason);
            Assert.IsTrue(observer.IsStarted);
        }

        [Test]
        public void ObserverConditionGoesFalse_ChildAbortedDecoratorFails() {
            var conditionValue = true;
            var child = new TrackingNode(() => NodeResult.Running);

            var observer = new TrackingObservingDecorator(() => conditionValue, AbortMode.Self);
            observer.Child = child;

            observer.OnUpdate();

            conditionValue = false;
            observer.RaiseChanged();
            observer.OnUpdate();

            Assert.AreEqual(NodeStopReason.Aborted, child.LastStopReason);
            Assert.AreEqual(NodeStopReason.Failure, observer.LastStopReason);
        }

        [Test]
        public void Stop_CrossesSubtreeBoundary_InnerNodesAborted() {
            var inner = new TrackingNode(() => NodeResult.Running);
            var subtree = new BehaviourTree { Root = inner };
            var subtreeNode = new SubtreeNode("Sub", subtree);

            subtreeNode.OnUpdate();
            subtreeNode.Stop();

            Assert.AreEqual(NodeStopReason.Aborted, subtreeNode.LastStopReason);
            Assert.AreEqual(NodeStopReason.Aborted, inner.LastStopReason);
        }

        [Test]
        public void OnStop_SeesReasonAndCount() {
            var completed = new TrackingNode(() => NodeResult.Success);

            completed.OnUpdate();

            Assert.AreEqual(NodeStopReason.Success, completed.ReasonAtLastStop);
            Assert.AreEqual(1, completed.StopCountAtLastStop);

            var aborted = new TrackingNode(() => NodeResult.Running);

            aborted.OnUpdate();
            aborted.Stop();

            Assert.AreEqual(NodeStopReason.Aborted, aborted.ReasonAtLastStop);
            Assert.AreEqual(1, aborted.StopCountAtLastStop);
        }
    }
}
