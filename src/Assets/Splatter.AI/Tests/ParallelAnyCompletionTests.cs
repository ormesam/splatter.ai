using NUnit.Framework;

namespace Splatter.AI.Tests {
    public class ParallelAnyCompletionTests : TestBase {
        [Test]
        public void Parallel_Success() {
            Parallel parallel = new Parallel(Tree, ParallelMode.ExitOnAnyCompletion);
            parallel.Children = new[]{
                CreateRunningNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Failure() {
            Parallel parallel = new Parallel(Tree, ParallelMode.ExitOnAnyCompletion);
            parallel.Children = new[]{
                CreateRunningNode(),
                CreateRunningNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Running() {
            Parallel parallel = new Parallel(Tree, ParallelMode.ExitOnAnyCompletion);
            parallel.Children = new[]{
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
        }
    }
}