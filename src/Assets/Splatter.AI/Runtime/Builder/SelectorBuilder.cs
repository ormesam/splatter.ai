using System;
using System.Linq;

namespace Splatter.AI {
    public class SelectorBuilder : BuilderBase {
        private readonly Selector selector;

        public SelectorBuilder(BehaviourTree tree) : base(tree) {
            this.selector = new Selector(tree);
        }

        /// <summary>
        /// Makes the composite node abortable
        /// </summary>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate</param>
        public void Abortable(AbortType abortType, Func<bool> condition) {
            selector.SetAbortType(abortType, condition);
        }

        public void Name(string name) {
            selector.Name = name;
        }

        /// <summary>
        /// End the composite node
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public Selector Build() {
            if (!selector.Children.Any()) {
                throw new InvalidOperationException("Composite node does not have any children.");
            }

            return selector;
        }

        protected override void AddNode(Node node) {
            selector.Children.Add(node);
        }
    }
}
