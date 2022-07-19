using System;
using System.Linq;

namespace Splatter.AI {
    public class ParallelBuilder<TParent> : BuilderBase, IBuilder where TParent : IBuilder {
        private readonly Parallel parallel;
        private readonly TParent parent;

        public ParallelBuilder(TParent parent, ParallelMode mode) : base(parent.Tree) {
            this.parallel = new Parallel(parent.Tree, mode);
            this.parent = parent;
        }

        public ParallelBuilder<TParent> Abortable(AbortType abortType, Func<bool> condition) {
            parallel.SetAbortType(abortType, condition);

            return this;
        }

        public void SetName(string name) {
            parallel.Name = name;
        }

        public TParent End() {
            if (!parallel.Children.Any()) {
                throw new InvalidOperationException("Composite node does not have any children.");
            }

            parent.AddNode(parallel);

            return parent;
        }

        public void AddNode(Node node) {
            parallel.Children.Add(node);
        }
    }
}
