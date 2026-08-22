using NUnit.Framework;
using Splatter.AI.Composites;

namespace Splatter.AI.Tests {
    public class ParallelAnyCompletionTests : TestBase {
        [Test]
        public void Parallel_Success() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnyCompletion) {
                CreateRunningNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Failure() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnyCompletion) {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Running() {
            Parallel parallel = new Parallel(ParallelMode.ExitOnAnyCompletion) {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
        }
    }
}
