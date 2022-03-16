using System;

namespace Splatter.AI {
    /// <summary>
    /// Always returns <see cref="NodeResult.Running"/> until all children succeed. 
    /// If a child fails, <see cref="NodeResult.Failure"/> is returned.
    /// </summary>
    public class Sequencer : Composite {
        private readonly bool resetIfInterrupted;
        private int lastRanOnTick = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequencer"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="resetIfInterrupted">Reset sequence if interrupted</param>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate when aborting</param>
        public Sequencer(string name, BehaviourTree tree, bool resetIfInterrupted, AbortType abortType = AbortType.None, Func<bool> condition = null)
            : base(name, tree, abortType, condition) {

            this.resetIfInterrupted = resetIfInterrupted;
        }

        protected override NodeResult ExecuteNode() {
            if (CanAbortSelf && !Condition()) {
                return NodeResult.Failure;
            }

            UpdateCurrentIdxIfInterrupted();

            if (resetIfInterrupted && lastRanOnTick != Tree.Ticks - 1) {
                CurrentNodeIdx = 0;
            }

            lastRanOnTick = Tree.Ticks;

            if (CurrentNodeIdx < Children.Count) {
                var result = Children[CurrentNodeIdx].Execute();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                } else if (result == NodeResult.Failure) {
                    CurrentNodeIdx = 0;
                    return NodeResult.Failure;
                } else {
                    CurrentNodeIdx++;

                    if (CurrentNodeIdx < Children.Count) {
                        return NodeResult.Running;
                    } else {
                        CurrentNodeIdx = 0;
                        return NodeResult.Success;
                    }
                }
            }

            return NodeResult.Success;
        }

#if UNITY_INCLUDE_TESTS
        // Useful for debugging tests
        public int CurrentIndex => CurrentNodeIdx;
#endif
    }
}
