using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class NodeStopTests : TestBase {
        [Test]
        public void Stop_StartedNode_Stops() {
            var node = new TrackingNode(Tree, () => NodeResult.Running);

            node.OnUpdate();
            node.Stop();

            Assert.AreEqual(1, node.Stops);
            Assert.IsFalse(node.IsStarted);
        }

        [Test]
        public void Stop_NotStartedNode_DoesNotStop() {
            var node = new TrackingNode(Tree, () => NodeResult.Running);

            node.Stop();

            Assert.AreEqual(0, node.Stops);
        }

        [Test]
        public void Stop_CompletedNode_DoesNotStopAgain() {
            var node = new TrackingNode(Tree, () => NodeResult.Success);

            node.OnUpdate();

            Assert.AreEqual(1, node.Stops);

            node.Stop();

            Assert.AreEqual(1, node.Stops);
        }

        [Test]
        public void Stop_CompletedNode_OnStopSeesFinalResult() {
            var node = new TrackingNode(Tree, () => NodeResult.Success);

            node.OnUpdate();

            Assert.AreEqual(NodeResult.Success, node.ResultAtLastStop);
        }

        [Test]
        public void Stop_InterruptedNode_OnStopSeesRunningResult() {
            var node = new TrackingNode(Tree, () => NodeResult.Running);

            node.OnUpdate();
            node.Stop();

            Assert.AreEqual(NodeResult.Running, node.ResultAtLastStop);
        }
    }
}
