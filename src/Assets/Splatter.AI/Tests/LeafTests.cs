using NUnit.Framework;
using Splatter.AI.Leaves;

namespace Splatter.AI.Tests {
    public class LeafTests : TestBase {
        [Test]
        public void Leaf_Success() {
            Leaf runningLeaf = new Leaf("Leaf", () => NodeResult.Running);
            Leaf successLeaf = new Leaf("Leaf", () => NodeResult.Success);
            Leaf failureLeaf = new Leaf("Leaf", () => NodeResult.Failure);

            Assert.AreEqual(NodeResult.Running, runningLeaf.OnUpdate());
            Assert.AreEqual(NodeResult.Success, successLeaf.OnUpdate());
            Assert.AreEqual(NodeResult.Failure, failureLeaf.OnUpdate());
        }
    }
}