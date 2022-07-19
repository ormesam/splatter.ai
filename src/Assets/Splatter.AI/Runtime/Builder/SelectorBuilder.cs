using System;
using System.Linq;

namespace Splatter.AI {
    public class SelectorBuilder<TParent> : BuilderBase, IBuilder where TParent : IBuilder {
        private readonly Selector selector;
        private readonly TParent parent;

        public SelectorBuilder(TParent parent) : base(parent.Tree) {
            this.selector = new Selector(parent.Tree);
            this.parent = parent;
        }

        public SelectorBuilder<TParent> Abortable(AbortType abortType, Func<bool> condition) {
            selector.SetAbortType(abortType, condition);

            return this;
        }

        public void SetName(string name) {
            selector.Name = name;
        }

        public TParent End() {
            if (!selector.Children.Any()) {
                throw new InvalidOperationException("Composite node does not have any children.");
            }

            parent.AddNode(selector);

            return parent;
        }

        public void AddNode(Node node) {
            selector.Children.Add(node);
        }
    }
}
