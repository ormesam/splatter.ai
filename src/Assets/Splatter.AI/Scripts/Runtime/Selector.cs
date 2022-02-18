using System;

namespace Splatter.AI {
    /// <summary>
    /// Returns <see cref="NodeResult.Running"/> until a child returns <see cref="NodeResult.Success"/>. 
    /// If no children succeed, <see cref="NodeResult.Failure"/> is returned.
    /// </summary>
    public class Selector : Composite {
        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="abortType">Abort type used for this compsite node</param>
        /// <param name="condition">Condition used to evaluate when aborting</param>
        public Selector(string name, BehaviourTree tree, AbortType abortType = AbortType.None, Func<bool> condition = null)
            : base(name, tree, abortType, condition) {
        }

        protected override NodeResult ExecuteNode() {
            if (CanAbortSelf && !Condition()) {
                return NodeResult.Failure;
            }

            UpdateCurrentIdxIfInterrupted();

            if (CurrentNodeIdx < Children.Count) {
                var result = Children[CurrentNodeIdx].Execute();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                } else if (result == NodeResult.Success) {
                    CurrentNodeIdx = 0;
                    return NodeResult.Success;
                } else {
                    CurrentNodeIdx++;

                    if (CurrentNodeIdx < Children.Count) {
                        return NodeResult.Running;
                    } else {
                        CurrentNodeIdx = 0;
                        return NodeResult.Failure;
                    }
                }
            }

            return NodeResult.Failure;
        }

#if UNITY_INCLUDE_TESTS
        // Useful for debugging tests
        public int CurrentIndex => CurrentNodeIdx;
#endif
    }
}
