using System;
using System.Linq;

namespace Splatter.AI {
    public class ParallelBuilder : BuilderBase {
        private readonly Parallel parallel;

        public ParallelBuilder(BehaviourTree tree, ParallelMode mode) : base(tree) {
            this.parallel = new Parallel(tree, mode);
        }

        /// <summary>
        /// Makes the composite node abortable
        /// </summary>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate</param>
        public void Abortable(AbortType abortType, Func<bool> condition) {
            parallel.SetAbortType(abortType, condition);
        }

        public void Name(string name) {
            parallel.Name = name;
        }

        /// <summary>
        /// End the composite node
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public Parallel Build() {
            if (!parallel.Children.Any()) {
                throw new InvalidOperationException("Composite node does not have any children.");
            }

            return parallel;
        }

        protected override void AddNode(Node node) {
            parallel.Children.Add(node);
        }
    }
}
