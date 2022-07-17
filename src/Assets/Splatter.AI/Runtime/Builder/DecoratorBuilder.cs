using System;

namespace Splatter.AI {
    public class DecoratorBuilder<TParent> : BuilderBase, IBuilder where TParent : IBuilder {
        private readonly TParent parent;
        private readonly Decorator decorator;

        public DecoratorBuilder(TParent parent, Decorator decorator) : base(parent.Tree) {
            this.parent = parent;
            this.decorator = decorator;
        }

        public void SetName(string name) {
            decorator.Name = name;
        }

        public TParent End() {
            if (decorator.Child == null) {
                throw new InvalidOperationException("Decorator node does not have a child.");
            }

            parent.AddNode(decorator);

            return parent;
        }

        public void AddNode(Node node) {
            if (decorator.Child != null) {
                throw new InvalidOperationException("Decorator child already set");
            }

            decorator.Child = node;
        }
    }
}
