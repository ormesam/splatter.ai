using System.Collections.Generic;
using System.Linq;

namespace Splatter.AI {
    /// <summary>
    /// Executes all children in order each update, until the <see cref="ParallelMode"/> condition is met.
    /// </summary>
    public class Parallel : Composite {
        private readonly ParallelMode mode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parallel"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="mode">Parallel mode</param>
        public Parallel(string name, BehaviourTree tree, ParallelMode mode)
            : base(name, tree) {

            this.mode = mode;
        }

        protected override NodeResult ExecuteNode() {
            IList<NodeResult> results = new List<NodeResult>();

            CurrentNodeIdx = 0;

            foreach (var child in Children) {
                var result = child.Execute();
                results.Add(result);

                if (result == NodeResult.Running) {
                    continue;
                }

                if (result == NodeResult.Success) {
                    if (mode == ParallelMode.ExitOnAnySuccess || mode == ParallelMode.ExitOnAnyCompletion) {
                        return NodeResult.Success;
                    }
                }

                if (result == NodeResult.Failure) {
                    if (mode == ParallelMode.ExitOnAnyFailure || mode == ParallelMode.ExitOnAnyCompletion) {
                        return NodeResult.Failure;
                    }
                }

                CurrentNodeIdx++;
            }

            // Wait for all children to complete
            if (mode == ParallelMode.WaitForAllToComplete) {
                return results.All(i => i != NodeResult.Running) ? NodeResult.Success : NodeResult.Running;
            }

            if (mode == ParallelMode.WaitForAllToSucceed) {
                return results.All(i => i == NodeResult.Success) ? NodeResult.Success : NodeResult.Running;
            }

            return NodeResult.Running;
        }
    }
}
