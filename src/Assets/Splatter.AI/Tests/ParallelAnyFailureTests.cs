using NUnit.Framework;

namespace Splatter.AI.Tests {
    public class ParallelAnyFailureTests : TestBase {
        [Test]
        public void Parallel_Success() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnyFailure) {
                CreateRunningNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Failure() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnyFailure) {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Running() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnyFailure) {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_AllChildrenSucceed_ReturnsSuccess() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnyFailure) {
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_NoChildren_ReturnsSuccess() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnyFailure);

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
        }
    }
}
