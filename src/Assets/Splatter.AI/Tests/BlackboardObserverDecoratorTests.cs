using System;
using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class BlackboardObserverDecoratorTests : TestBase {
        [Test]
        public void Observer_LowerPriority_InterruptsRunningSelectorBranch() {
            var tree = new BehaviourTree();
            var combat = new TrackingNode(() => NodeResult.Running);
            var patrol = new TrackingNode(() => NodeResult.Running);

            var observer = new BlackboardObserverDecorator(tree.Blackboard, "enemy", AbortMode.LowerPriority);
            observer.Child = combat;

            tree.Root = new Selector() {
                observer,
                patrol,
            };

            Assert.AreEqual(NodeResult.Running, tree.Tick());
            Assert.IsTrue(patrol.IsStarted);
            Assert.AreEqual(0, combat.Starts);

            tree.Blackboard["enemy"] = true;

            // Deferred: nothing happens at write time.
            Assert.AreEqual(0, patrol.Stops);
            Assert.IsTrue(patrol.IsStarted);

            Assert.AreEqual(NodeResult.Running, tree.Tick());
            Assert.AreEqual(1, patrol.Stops);
            Assert.AreEqual(1, combat.Starts);
        }

        [Test]
        public void Observer_MidTickWrite_AbortsOnNextTickOnly() {
            var tree = new BehaviourTree();
            var interrupt = new TrackingNode(() => NodeResult.Running);
            var work = new TrackingNode(() => NodeResult.Running);

            var observer = new BlackboardObserverDecorator(tree.Blackboard, "alert", AbortMode.LowerPriority);
            observer.Child = interrupt;

            tree.Root = new Selector() {
                observer,
                new Sequencer() {
                    new SetBlackboardValueNode(tree.Blackboard, "alert", true),
                    work,
                },
            };

            // The write happens mid-tick, after the observer branch failed; the node
            // running behind it must not be torn down within the same tick.
            Assert.AreEqual(NodeResult.Running, tree.Tick());
            Assert.AreEqual(1, work.Starts);
            Assert.AreEqual(0, work.Stops);
            Assert.AreEqual(0, interrupt.Starts);

            Assert.AreEqual(NodeResult.Running, tree.Tick());
            Assert.AreEqual(1, work.Stops);
            Assert.AreEqual(1, interrupt.Starts);
        }

        [Test]
        public void Observer_TransientChange_DoesNotInterrupt() {
            var tree = new BehaviourTree();
            var combat = new TrackingNode(() => NodeResult.Running);
            var patrol = new TrackingNode(() => NodeResult.Running);

            var observer = new BlackboardObserverDecorator(tree.Blackboard, "enemy", AbortMode.LowerPriority);
            observer.Child = combat;

            tree.Root = new Selector() {
                observer,
                patrol,
            };

            tree.Tick();

            tree.Blackboard["enemy"] = true;
            tree.Blackboard.Remove("enemy");

            Assert.AreEqual(NodeResult.Running, tree.Tick());
            Assert.AreEqual(0, patrol.Stops);
            Assert.AreEqual(0, combat.Starts);
            Assert.AreEqual(2, patrol.Updates);
        }

        [Test]
        public void Observer_TwoPendingAborts_LowestIndexWins() {
            var tree = new BehaviourTree();
            var a = new TrackingNode(() => NodeResult.Running);
            var b = new TrackingNode(() => NodeResult.Running);
            var fallback = new TrackingNode(() => NodeResult.Running);

            var observerA = new BlackboardObserverDecorator(tree.Blackboard, "a", AbortMode.LowerPriority);
            observerA.Child = a;

            var observerB = new BlackboardObserverDecorator(tree.Blackboard, "b", AbortMode.LowerPriority);
            observerB.Child = b;

            tree.Root = new Selector() {
                observerA,
                observerB,
                fallback,
            };

            tree.Tick();

            tree.Blackboard["a"] = true;
            tree.Blackboard["b"] = true;

            Assert.AreEqual(NodeResult.Running, tree.Tick());
            Assert.AreEqual(1, a.Starts);
            Assert.AreEqual(0, b.Starts);
            Assert.AreEqual(1, fallback.Stops);
        }

        [Test]
        public void Observer_RewoundBranchFails_FallsThroughSameTick() {
            var tree = new BehaviourTree();
            var child = new TrackingNode(() => NodeResult.Failure);
            var fallback = new TrackingNode(() => NodeResult.Running);

            var observer = new BlackboardObserverDecorator(tree.Blackboard, "a", AbortMode.LowerPriority);
            observer.Child = child;

            tree.Root = new Selector() {
                observer,
                fallback,
            };

            tree.Tick();

            tree.Blackboard["a"] = true;

            // The rewound branch is tried but fails, so the previously running branch
            // restarts from scratch within the same tick.
            Assert.AreEqual(NodeResult.Running, tree.Tick());
            Assert.AreEqual(1, child.Starts);
            Assert.AreEqual(1, fallback.Stops);
            Assert.AreEqual(2, fallback.Starts);
        }

        [Test]
        public void Observer_SequencerGuardBreaks_FailsSequence() {
            var tree = new BehaviourTree();
            var gate = new TrackingNode(() => NodeResult.Success);
            var work = new TrackingNode(() => NodeResult.Running);

            var observer = new BlackboardObserverDecorator(tree.Blackboard, "hasKey", true, AbortMode.LowerPriority);
            observer.Child = gate;

            tree.Root = new Sequencer() {
                observer,
                work,
            };

            tree.Blackboard["hasKey"] = true;

            Assert.AreEqual(NodeResult.Running, tree.Tick());
            Assert.AreEqual(1, gate.Starts);

            tree.Blackboard["hasKey"] = false;

            Assert.AreEqual(NodeResult.Failure, tree.Tick());
            Assert.AreEqual(1, work.Stops);
            Assert.AreEqual(1, gate.Starts);
        }

        [Test]
        public void Observer_NeverVisitedLowerBranch_DoesNotInterrupt() {
            var tree = new BehaviourTree();
            var running = new TrackingNode(() => NodeResult.Running);
            var lower = new TrackingNode(() => NodeResult.Running);

            var observer = new BlackboardObserverDecorator(tree.Blackboard, "k", AbortMode.LowerPriority);
            observer.Child = lower;

            tree.Root = new Selector() {
                running,
                observer,
            };

            tree.Tick();

            tree.Blackboard["k"] = true;

            Assert.AreEqual(NodeResult.Running, tree.Tick());
            Assert.AreEqual(0, lower.Starts);
            Assert.AreEqual(0, running.Stops);
            Assert.AreEqual(2, running.Updates);
        }

        [Test]
        public void Observer_ExpectedValueCtor_GatesOnEquality() {
            var tree = new BehaviourTree();
            var child = new TrackingNode(() => NodeResult.Success);

            var observer = new BlackboardObserverDecorator(tree.Blackboard, "state", "attacking", AbortMode.None);
            observer.Child = child;
            tree.Root = observer;

            tree.Blackboard["state"] = "idle";
            Assert.AreEqual(NodeResult.Failure, tree.Tick());
            Assert.AreEqual(0, child.Starts);

            tree.Blackboard["state"] = "attacking";
            Assert.AreEqual(NodeResult.Success, tree.Tick());
            Assert.AreEqual(1, child.Starts);
        }

        [Test]
        public void Observer_TreeStop_StopsRunningBranch() {
            var tree = new BehaviourTree();
            var combat = new TrackingNode(() => NodeResult.Running);
            var patrol = new TrackingNode(() => NodeResult.Running);

            var observer = new BlackboardObserverDecorator(tree.Blackboard, "enemy", AbortMode.LowerPriority);
            observer.Child = combat;

            tree.Root = new Selector() {
                observer,
                patrol,
            };

            tree.Tick();
            tree.Stop();

            Assert.AreEqual(1, patrol.Stops);
            Assert.IsFalse(patrol.IsStarted);
        }

        [Test]
        public void Observer_LowerPriorityUnderParallel_ThrowsOnFirstTick() {
            var tree = new BehaviourTree();
            var observer = new BlackboardObserverDecorator(tree.Blackboard, "k", AbortMode.LowerPriority);
            observer.Child = CreateRunningNode();

            tree.Root = new Parallel(ParallelMode.WaitForAllToComplete) {
                observer,
            };

            Assert.Throws<InvalidOperationException>(() => tree.Tick());
        }

        [Test]
        public void Observer_LowerPriorityUnderReactiveSelector_ThrowsOnFirstTick() {
            var tree = new BehaviourTree();
            var observer = new BlackboardObserverDecorator(tree.Blackboard, "k", AbortMode.Both);
            observer.Child = CreateRunningNode();

            tree.Root = new ReactiveSelector() {
                observer,
                CreateRunningNode(),
            };

            Assert.Throws<InvalidOperationException>(() => tree.Tick());
        }

        [Test]
        public void Observer_LowerPriorityAsRoot_ThrowsOnFirstTick() {
            var tree = new BehaviourTree();
            var observer = new BlackboardObserverDecorator(tree.Blackboard, "k", AbortMode.LowerPriority);
            observer.Child = CreateRunningNode();

            tree.Root = observer;

            Assert.Throws<InvalidOperationException>(() => tree.Tick());
        }

        [Test]
        public void Observer_LowerPriorityBehindDecorator_ThrowsOnFirstTick() {
            var tree = new BehaviourTree();
            var observer = new BlackboardObserverDecorator(tree.Blackboard, "k", AbortMode.LowerPriority);
            observer.Child = CreateRunningNode();

            var inverter = new InvertDecorator();
            inverter.Child = observer;

            tree.Root = new Selector() {
                inverter,
                CreateRunningNode(),
            };

            Assert.Throws<InvalidOperationException>(() => tree.Tick());
        }

        [Test]
        public void Observer_SelfMode_ValidAnywhere() {
            var tree = new BehaviourTree();
            var observer = new BlackboardObserverDecorator(tree.Blackboard, "k", AbortMode.Self);
            observer.Child = CreateRunningNode();

            tree.Root = new Parallel(ParallelMode.WaitForAllToComplete) {
                observer,
            };

            tree.Blackboard["k"] = true;

            Assert.DoesNotThrow(() => tree.Tick());
        }

        [Test]
        public void Observer_BothModeUnderSelector_Valid() {
            var tree = new BehaviourTree();
            var observer = new BlackboardObserverDecorator(tree.Blackboard, "k", AbortMode.Both);
            observer.Child = CreateRunningNode();

            tree.Root = new Selector() {
                observer,
                CreateRunningNode(),
            };

            Assert.DoesNotThrow(() => tree.Tick());
        }
    }
}
