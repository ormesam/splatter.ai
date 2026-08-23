using System;
using NUnit.Framework;
using Splatter.AI.Leaves;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class SubtreeNodeTests : TestBase {
        [Test]
        public void Subtree_RootSucceeds_ReturnsSuccess() {
            var subtree = new BehaviourTree { Root = CreateSuccessNode() };
            var node = new SubtreeNode("Subtree", subtree);

            Assert.AreEqual(NodeResult.Success, node.OnUpdate());
        }

        [Test]
        public void Subtree_RootFails_ReturnsFailure() {
            var subtree = new BehaviourTree { Root = CreateFailureNode() };
            var node = new SubtreeNode("Subtree", subtree);

            Assert.AreEqual(NodeResult.Failure, node.OnUpdate());
        }

        [Test]
        public void Subtree_RootRunning_ReturnsRunning() {
            var subtree = new BehaviourTree { Root = CreateRunningNode() };
            var node = new SubtreeNode("Subtree", subtree);

            Assert.AreEqual(NodeResult.Running, node.OnUpdate());
        }

        [Test]
        public void Subtree_NoRoot_Throws() {
            var node = new SubtreeNode("Subtree", new BehaviourTree());

            Assert.Throws<InvalidOperationException>(() => node.OnUpdate());
        }

        [Test]
        public void Subtree_RunningNode_ResumesAcrossTicks() {
            var tracking = new TrackingNode(() => NodeResult.Running);
            var subtree = new BehaviourTree { Root = tracking };
            var node = new SubtreeNode("Subtree", subtree);

            node.OnUpdate();
            node.OnUpdate();

            Assert.AreEqual(1, tracking.Starts);
            Assert.AreEqual(2, tracking.Updates);
        }

        [Test]
        public void Subtree_HasOwnBlackboard() {
            var subtree = new BehaviourTree();
            subtree.Root = new SetBlackboardValueNode(subtree.Blackboard, "Target", "Player");

            var tree = new BehaviourTree { Root = new SubtreeNode("Subtree", subtree) };

            tree.Tick();

            Assert.AreEqual("Player", subtree.GetItem<string>("Target"));
            Assert.IsFalse(tree.Blackboard.ContainsKey("Target"));
        }

        [Test]
        public void Stop_RunningSubtree_StopsNodesInsideSubtree() {
            var tracking = new TrackingNode(() => NodeResult.Running);
            var subtree = new BehaviourTree { Root = tracking };
            var node = new SubtreeNode("Subtree", subtree);

            node.OnUpdate();
            node.Stop();

            Assert.AreEqual(1, tracking.Stops);
            Assert.IsFalse(tracking.IsStarted);
        }
    }
}
