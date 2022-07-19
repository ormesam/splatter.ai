using System;
using System.Linq;

namespace Splatter.AI {
    public class SequenceBuilder<TParent> : BuilderBase, IBuilder where TParent : IBuilder {
        private readonly Sequencer sequencer;
        private readonly TParent parent;

        public SequenceBuilder(TParent parent) : base(parent.Tree) {
            this.sequencer = new Sequencer(parent.Tree);
            this.parent = parent;
        }

        public SequenceBuilder<TParent> Abortable(AbortType abortType, Func<bool> condition) {
            sequencer.SetAbortType(abortType, condition);

            return this;
        }

        public SequenceBuilder<TParent> ResetIfInterrupted() {
            sequencer.SetResetIfInterrupted(true);

            return this;
        }

        public void SetName(string name) {
            sequencer.Name = name;
        }

        public TParent End() {
            if (!sequencer.Children.Any()) {
                throw new InvalidOperationException("Composite node does not have any children.");
            }

            parent.AddNode(sequencer);

            return parent;
        }

        public void AddNode(Node node) {
            sequencer.Children.Add(node);
        }
    }
}
