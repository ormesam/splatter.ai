namespace Splatter.AI {
    /// <summary>
    /// Executes all children each update, until the <see cref="ParallelMode"/> condition is met.
    /// Children that complete are not updated again until the parallel node restarts. Any children
    /// still running when the parallel node completes are stopped.
    /// </summary>
    public class Parallel : Composite {
        private readonly ParallelMode mode;
        private NodeResult[] childResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parallel"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="mode">Parallel mode</param>
        public Parallel(BehaviourTree tree, ParallelMode mode)
            : base("Parallel", tree) {

            this.mode = mode;
        }

        protected override void OnStart() {
            ResetChildResults();
        }

        protected override NodeResult Update() {
            if (childResults.Length != Children.Count) {
                ResetChildResults();
            }

            bool allComplete = true;

            for (int i = 0; i < Children.Count; i++) {
                var result = childResults[i];

                if (result == NodeResult.Running) {
                    result = Children[i].OnUpdate();
                    childResults[i] = result;
                }

                if (result == NodeResult.Success) {
                    if (mode == ParallelMode.ExitOnAnySuccess || mode == ParallelMode.ExitOnAnyCompletion) {
                        StopChildren();

                        return NodeResult.Success;
                    }
                } else if (result == NodeResult.Failure) {
                    if (mode == ParallelMode.ExitOnAnyFailure
                        || mode == ParallelMode.ExitOnAnyCompletion
                        || mode == ParallelMode.WaitForAllToSucceed) {
                        StopChildren();

                        return NodeResult.Failure;
                    }
                } else {
                    allComplete = false;
                }
            }

            if (allComplete) {
                // Results that would exit early were returned inside the loop, so all children
                // completing means none of them met this mode's exit condition.
                return mode == ParallelMode.ExitOnAnySuccess ? NodeResult.Failure : NodeResult.Success;
            }

            return NodeResult.Running;
        }

        protected override void OnStop() {
        }

        private void ResetChildResults() {
            if (childResults == null || childResults.Length != Children.Count) {
                childResults = new NodeResult[Children.Count];
            }

            for (int i = 0; i < childResults.Length; i++) {
                childResults[i] = NodeResult.Running;
            }
        }
    }
}
