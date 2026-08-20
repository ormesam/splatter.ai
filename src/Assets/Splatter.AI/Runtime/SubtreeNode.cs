namespace Splatter.AI {
    /// <summary>
    /// Leaf that ticks another <see cref="BehaviourTree"/>, for composing and reusing whole
    /// trees. The subtree keeps its own blackboard, scoped separately from the parent tree's.
    /// </summary>
    public class SubtreeNode : Node {
        /// <summary>
        /// The tree ticked by this node.
        /// </summary>
        public BehaviourTree Tree { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtreeNode"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="tree">Tree to tick</param>
        public SubtreeNode(string name, BehaviourTree tree) : base(name) {
            this.Tree = tree;
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            return Tree.Tick();
        }

        protected override void OnStop() {
        }
    }
}
