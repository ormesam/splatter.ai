using System;

namespace Splatter.AI {
    public class DecoratorBuilder : BuilderBase {
        private readonly Decorator decorator;

        public DecoratorBuilder(BehaviourTree tree, Decorator decorator) : base(tree) {
            this.decorator = decorator;
        }

        public void Name(string name) {
            decorator.Name = name;
        }

        /// <summary>
        /// End the decorator node
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public Decorator Build() {
            if (decorator.Child == null) {
                throw new InvalidOperationException("Decorator node does not have a child.");
            }

            return decorator;
        }

        protected override void AddNode(Node node) {
            if (decorator.Child != null) {
                throw new InvalidOperationException("Decorator child already set");
            }

            decorator.Child = node;
        }
    }
}
