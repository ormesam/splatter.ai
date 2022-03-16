using NUnit.Framework;
using Splatter.AI;

namespace Splatter.AI.Tests {
    public class ParallelWaitForAllToSucceed : TestBase {
        [Test]
        public void Parallel_Success() {
            Parallel parallel = new Parallel("Parallel", Tree, ParallelMode.WaitForAllToSucceed);
            parallel.Children = new[]{
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Success, parallel.Execute());
        }

        [Test]
        public void Parallel_Failure() {
            Parallel parallel = new Parallel("Parallel", Tree, ParallelMode.WaitForAllToSucceed);
            parallel.Children = new[]{
                CreateSuccessNode(),
                CreateSuccessNode(),
                CreateFailureNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.Execute());
        }

        [Test]
        public void Parallel_Running() {
            Parallel parallel = new Parallel("Parallel", Tree, ParallelMode.WaitForAllToSucceed);
            parallel.Children = new[]{
                CreateSuccessNode(),
                CreateRunningNode(),
                CreateSuccessNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.Execute());
        }
    }
}