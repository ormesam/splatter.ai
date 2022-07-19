namespace Splatter.AI {
    public abstract class BuilderBase {
        public BehaviourTree Tree { get; private set; }

        protected BuilderBase(BehaviourTree tree) {
            this.Tree = tree;
        }
    }
}
