using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class ParallelWaitForAllToComplete : TestBase {
        [Test]
        public void Parallel_Success() {
            Parallel parallel = new Parallel(ParallelMode.WaitForAllToComplete) {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Failure() {
            Parallel parallel = new Parallel(ParallelMode.WaitForAllToComplete) {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Running() {
            Parallel parallel = new Parallel(ParallelMode.WaitForAllToComplete) {
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_CompletedChildren_AreNotUpdatedAgain() {
            var secondResult = NodeResult.Running;

            var completed = new TrackingNode(() => NodeResult.Success);
            var running = new TrackingNode(() => secondResult);

            Parallel parallel = new Parallel(ParallelMode.WaitForAllToComplete) {
                completed,
                running,
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());

            secondResult = NodeResult.Success;

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());

            Assert.AreEqual(1, completed.Starts);
            Assert.AreEqual(1, completed.Updates);
            Assert.AreEqual(3, running.Updates);
        }

        [Test]
        public void Parallel_Restart_UpdatesChildrenAgain() {
            var child = new TrackingNode(() => NodeResult.Success);

            Parallel parallel = new Parallel(ParallelMode.WaitForAllToComplete) { child };

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());

            Assert.AreEqual(2, child.Updates);
        }
    }
}
