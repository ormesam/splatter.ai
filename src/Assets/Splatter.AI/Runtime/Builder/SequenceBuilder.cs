using System;

namespace Splatter.AI {
    public class SequenceBuilder<TParent> : BuilderBase, IBuilder where TParent : IBuilder {
        private readonly Sequencer sequencer;
        private readonly TParent parent;

        public SequenceBuilder(TParent parent) : base(parent.Tree) {
            this.sequencer = new Sequencer(parent.Tree);
            this.parent = parent;
        }

        public SequenceBuilder(TParent parent, Sequencer sequencer) : base(parent.Tree) {
            this.sequencer = sequencer;
            this.parent = parent;
        }

        public SequenceBuilder<TParent> Abortable(AbortType abortType, Func<bool> condition) {
            sequencer.SetAbortType(abortType, condition);

            return this;
        }

        public void SetName(string name) {
            sequencer.Name = name;
        }

        public TParent End() {
            if (sequencer.Children.Count == 0) {
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
