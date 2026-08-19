using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Splatter.AI.Tests {
    public class BehaviourTreeTests {
        [Test]
        public void Tick_ReturnsRootResult() {
            var tree = new BehaviourTree();
            tree.Root = new Leaf("Leaf", () => NodeResult.Success);

            Assert.AreEqual(NodeResult.Success, tree.Tick());
        }

        [Test]
        public void Tick_NoRoot_Throws() {
            var tree = new BehaviourTree();

            Assert.Throws<InvalidOperationException>(() => tree.Tick());
        }

        [Test]
        public void GetItem_ReturnsTypedValue() {
            var tree = new BehaviourTree();
            tree.Blackboard["health"] = 42;

            Assert.AreEqual(42, tree.GetItem<int>("health"));
        }

        [Test]
        public void GetItem_MissingKey_Throws() {
            var tree = new BehaviourTree();

            Assert.Throws<KeyNotFoundException>(() => tree.GetItem<int>("missing"));
        }
    }
}
