using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class ParallelAnySuccessTests : TestBase {
        [Test]
        public void Parallel_Success() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnySuccess) {
                CreateFailureNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Success_StopsRunningChildren() {
            var running = new TrackingNode(() => NodeResult.Running);

            Parallel parallel = new Parallel(ParallelMode.ExitOnAnySuccess) {
                running,
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());

            Assert.AreEqual(1, running.Stops);
            Assert.IsFalse(running.IsStarted);
        }

        [Test]
        public void Parallel_Failure() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnySuccess) {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_AllChildrenFail_ReturnsFailure() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnySuccess) {
                CreateFailureNode(),
                CreateFailureNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_AllChildrenFail_OnDifferentUpdates_ReturnsFailure() {
            var secondResult = NodeResult.Running;
            var second = new TrackingNode(() => secondResult);

            Parallel parallel = new Parallel(ParallelMode.ExitOnAnySuccess) {
                CreateFailureNode(),
                second,
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());

            secondResult = NodeResult.Failure;

            Assert.AreEqual(NodeResult.Failure, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_NoChildren_ReturnsFailure() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnySuccess);

            Assert.AreEqual(NodeResult.Failure, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Running() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnySuccess) {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
        }
    }
}
