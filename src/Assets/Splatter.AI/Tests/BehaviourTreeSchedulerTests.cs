using System;
using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class BehaviourTreeSchedulerTests : TestBase {
        private static BehaviourTree CreateTrackingTree(out TrackingNode node) {
            node = new TrackingNode(() => NodeResult.Running);

            return new BehaviourTree { Root = node };
        }

        private static void Pump(BehaviourTreeScheduler scheduler, int times) {
            for (int i = 0; i < times; i++) {
                scheduler.Tick();
            }
        }

        [Test]
        public void Scheduler_RegisterNullTree_Throws() {
            var scheduler = new BehaviourTreeScheduler();

            Assert.Throws<ArgumentNullException>(() => scheduler.Register(null));
        }

        [Test]
        public void Scheduler_RegisterInvalidInterval_Throws() {
            var scheduler = new BehaviourTreeScheduler();
            var tree = CreateTrackingTree(out _);

            Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Register(tree, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Register(tree, -1));
        }

        [Test]
        public void Scheduler_DefaultInterval_TicksEveryPump() {
            var scheduler = new BehaviourTreeScheduler();
            var tree = CreateTrackingTree(out var node);

            scheduler.Register(tree);
            Pump(scheduler, 3);

            Assert.AreEqual(3, node.Updates);
        }

        [Test]
        public void Scheduler_Interval_TicksEveryNthPump() {
            var scheduler = new BehaviourTreeScheduler();
            var tree = CreateTrackingTree(out var node);

            scheduler.Register(tree, 3);
            Pump(scheduler, 9);

            Assert.AreEqual(3, node.Updates);
        }

        [Test]
        public void Scheduler_RegisterAgain_UpdatesInterval() {
            var scheduler = new BehaviourTreeScheduler();
            var tree = CreateTrackingTree(out var node);

            scheduler.Register(tree, 100);
            scheduler.Tick();

            Assert.AreEqual(1, node.Updates);

            scheduler.Register(tree);

            Assert.AreEqual(1, scheduler.Count);

            Pump(scheduler, 2);

            Assert.AreEqual(3, node.Updates);
        }

        [Test]
        public void Scheduler_UnregisterRegisteredTree_StopsTicking() {
            var scheduler = new BehaviourTreeScheduler();
            var tree = CreateTrackingTree(out var node);

            scheduler.Register(tree);
            scheduler.Tick();

            Assert.AreEqual(1, node.Updates);
            Assert.IsTrue(scheduler.Unregister(tree));
            Assert.AreEqual(0, scheduler.Count);

            scheduler.Tick();

            Assert.AreEqual(1, node.Updates);
        }

        [Test]
        public void Scheduler_UnregisterUnknownTree_ReturnsFalse() {
            var scheduler = new BehaviourTreeScheduler();
            var tree = CreateTrackingTree(out _);

            Assert.IsFalse(scheduler.Unregister(tree));
            Assert.IsFalse(scheduler.Unregister(null));
        }

        [Test]
        public void Scheduler_TreeUnregisteringItselfDuringTick_IsSafe() {
            BehaviourTreeScheduler scheduler = null;
            BehaviourTree tree = null;

            var node = new TrackingNode(() => {
                scheduler.Unregister(tree);

                return NodeResult.Running;
            });

            tree = new BehaviourTree { Root = node };
            scheduler = new BehaviourTreeScheduler();
            scheduler.Register(tree);

            scheduler.Tick();

            Assert.AreEqual(1, node.Updates);
            Assert.AreEqual(0, scheduler.Count);

            scheduler.Tick();

            Assert.AreEqual(1, node.Updates);
        }

        [Test]
        public void Scheduler_TreeUnregisteringAnotherTreeDuringTick_IsSafe() {
            BehaviourTreeScheduler scheduler = null;
            BehaviourTree treeB = null;

            var nodeA = new TrackingNode(() => {
                scheduler.Unregister(treeB);

                return NodeResult.Running;
            });

            treeB = CreateTrackingTree(out var nodeB);
            scheduler = new BehaviourTreeScheduler();
            scheduler.Register(new BehaviourTree { Root = nodeA });
            scheduler.Register(treeB);

            Pump(scheduler, 2);

            Assert.AreEqual(2, nodeA.Updates);
            Assert.AreEqual(0, nodeB.Updates);
        }

        [Test]
        public void Scheduler_SameInterval_TreesAreStaggered() {
            var scheduler = new BehaviourTreeScheduler();
            int pump = 0;

            var firstTickPumps = new int?[3];

            for (int i = 0; i < 3; i++) {
                int index = i;
                var node = new TrackingNode(() => {
                    if (!firstTickPumps[index].HasValue) {
                        firstTickPumps[index] = pump;
                    }

                    return NodeResult.Running;
                });

                scheduler.Register(new BehaviourTree { Root = node }, 3);
            }

            for (pump = 1; pump <= 3; pump++) {
                scheduler.Tick();
            }

            // Each tree's first tick lands on a different pump within the first interval.
            Assert.AreEqual(1, firstTickPumps[0]);
            Assert.AreEqual(2, firstTickPumps[1]);
            Assert.AreEqual(3, firstTickPumps[2]);
        }
    }
}
