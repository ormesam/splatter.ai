using NUnit.Framework;

namespace Splatter.AI.Tests {
    public class LeafTests : TestBase {
        [Test]
        public void Leaf_Success() {
            Leaf runningLeaf = new Leaf("Leaf", Tree, () => NodeResult.Running);
            Leaf successLeaf = new Leaf("Leaf", Tree, () => NodeResult.Success);
            Leaf failureLeaf = new Leaf("Leaf", Tree, () => NodeResult.Failure);

            Assert.AreEqual(NodeResult.Running, runningLeaf.Execute());
            Assert.AreEqual(NodeResult.Success, successLeaf.Execute());
            Assert.AreEqual(NodeResult.Failure, failureLeaf.Execute());
        }
    }
}