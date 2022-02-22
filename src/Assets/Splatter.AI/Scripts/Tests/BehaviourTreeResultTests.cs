using NUnit.Framework;
using Splatter.AI;

namespace Splatter.AI.Tests {
    public class BehaviourTreeResultTests : TestBase {
        [Test]
        public void BehaviourTree_SuccessNode() {
            Assert.AreEqual(NodeResult.Success, CreateSuccessNode().Execute());
        }

        [Test]
        public void BehaviourTree_FailureNode() {
            Assert.AreEqual(NodeResult.Failure, CreateFailureNode().Execute());
        }

        [Test]
        public void BehaviourTree_RunningNode() {
            Assert.AreEqual(NodeResult.Running, CreateRunningNode().Execute());
        }
    }
}