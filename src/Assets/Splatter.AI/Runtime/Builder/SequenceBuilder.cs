using System;
using System.Linq;

namespace Splatter.AI {
    public class SequenceBuilder : BuilderBase {
        private readonly Sequencer sequencer;

        public SequenceBuilder(BehaviourTree tree) : base(tree) {
            this.sequencer = new Sequencer(tree);
        }

        /// <summary>
        /// Makes the composite node abortable
        /// </summary>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate</param>
        public void Abortable(AbortType abortType, Func<bool> condition) {
            sequencer.SetAbortType(abortType, condition);
        }

        /// <summary>
        /// Reset the sequence if the behaviour tree is interrupted
        /// </summary>
        public void ResetIfInterrupted() {
            sequencer.ResetIfInterrupted();
        }

        public void Name(string name) {
            sequencer.Name = name;
        }

        /// <summary>
        /// End the composite node
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public Sequencer Build() {
            if (!sequencer.Children.Any()) {
                throw new InvalidOperationException("Composite node does not have any children.");
            }

            return sequencer;
        }

        protected override void AddNode(Node node) {
            sequencer.Children.Add(node);
        }
    }
}
