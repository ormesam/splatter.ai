using NUnit.Framework;

namespace Splatter.AI.Tests {
    public class BehaviourTreeResultTests : TestBase {
        [Test]
        public void BehaviourTree_SuccessNode() {
            Assert.AreEqual(NodeResult.Success, CreateSuccessNode().OnUpdate());
        }

        [Test]
        public void BehaviourTree_FailureNode() {
            Assert.AreEqual(NodeResult.Failure, CreateFailureNode().OnUpdate());
        }

        [Test]
        public void BehaviourTree_RunningNode() {
            Assert.AreEqual(NodeResult.Running, CreateRunningNode().OnUpdate());
        }
    }
}