using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class ParallelAbortTests : TestBase {
        [Test]
        public void Parallel_Abort_Self() {
            bool condition = true;

            Parallel parallel = new Parallel(Tree, ParallelMode.WaitForAllToComplete);
            parallel.SetAbortType(AbortType.Self, () => condition);
            parallel.Children = new[]{
                CreateRunningNode(),
            };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
            condition = false;
            Assert.AreEqual(NodeResult.Failure, parallel.OnUpdate());
        }

        [Test]
        public void Parallel_Abort_Self_StopsRunningChildren() {
            bool condition = true;

            var child = new TrackingNode(Tree, () => NodeResult.Running);

            Parallel parallel = new Parallel(Tree, ParallelMode.WaitForAllToComplete);
            parallel.SetAbortType(AbortType.Self, () => condition);
            parallel.Children = new Node[] { child };

            Assert.AreEqual(NodeResult.Running, parallel.OnUpdate());
            condition = false;
            Assert.AreEqual(NodeResult.Failure, parallel.OnUpdate());

            Assert.AreEqual(1, child.Stops);
            Assert.IsFalse(child.IsStarted);
        }
    }
}
