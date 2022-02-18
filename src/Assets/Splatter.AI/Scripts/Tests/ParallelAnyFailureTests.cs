using NUnit.Framework;
using Splatter.AI;

namespace Splatter.Tests {
    public class ParallelAnyFailureTests : TestBase {
        [Test]
        public void Parallel_Success() {
            Parallel parallel = new Parallel("Parallel", Tree, ParallelMode.ExitOnFailure);
            parallel.Children = new[] {
                CreateRunningNode(),
                CreateSuccessNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.Execute());
        }

        [Test]
        public void Parallel_Failure() {
            Parallel parallel = new Parallel("Parallel", Tree, ParallelMode.ExitOnFailure);
            parallel.Children = new[] {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Failure, parallel.Execute());
        }

        [Test]
        public void Parallel_Running() {
            Parallel parallel = new Parallel("Parallel", Tree, ParallelMode.ExitOnFailure);
            parallel.Children = new[] {
                CreateRunningNode(),
                CreateRunningNode(),
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.Execute());
        }
    }
}