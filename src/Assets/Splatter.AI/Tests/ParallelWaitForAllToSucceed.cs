using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class ParallelWaitForAllToSucceed : TestBase {
        [Test]
        public void Parallel_Success() {
            Parallel parallel = new Parallel(Tree, ParallelMode.WaitForAllToSucceed) {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Failure() {
            Parallel parallel = new Parallel(Tree, ParallelMode.WaitForAllToSucceed) {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_ChildrenSucceedOnDifferentUpdates() {
            var secondResult = NodeResult.Running;

            var first = new TrackingNode(Tree, () => NodeResult.Success);
            var second = new TrackingNode(Tree, () => secondResult);

            Parallel parallel = new Parallel(Tree, ParallelMode.WaitForAllToSucceed) {
                first,
                second,
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());

            secondResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
            Assert.AreEqual(1, first.Updates);
        }

        [Test]
        public void Parallel_Running() {
            Parallel parallel = new Parallel(Tree, ParallelMode.WaitForAllToSucceed) {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
        }
    }
}
