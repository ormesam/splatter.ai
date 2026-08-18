using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class NodeAbortTests : TestBase {
        [Test]
        public void Abort_StartedNode_Stops() {
            var node = new TrackingNode(Tree, () => NodeResult.Running);

            node.OnUpdate();
            node.Abort();

            Assert.AreEqual(1, node.Stops);
            Assert.IsFalse(node.IsStarted);
        }

        [Test]
        public void Abort_NotStartedNode_DoesNotStop() {
            var node = new TrackingNode(Tree, () => NodeResult.Running);

            node.Abort();

            Assert.AreEqual(0, node.Stops);
        }

        [Test]
        public void Abort_CompletedNode_DoesNotStopAgain() {
            var node = new TrackingNode(Tree, () => NodeResult.Success);

            node.OnUpdate();

            Assert.AreEqual(1, node.Stops);

            node.Abort();

            Assert.AreEqual(1, node.Stops);
        }
    }
}
