using System;

namespace Splatter.AI {
    /// <summary>
    /// Short hand class for passing in functionality instead of deriving from <see cref="Node"/>.
    /// </summary>
    public class Leaf : Node {
        private readonly Func<NodeResult> onExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Leaf"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="onExecute">Function to run on execution</param>
        public Leaf(string name, BehaviourTree tree, Func<NodeResult> onExecute) : base(name, tree) {
            this.onExecute = onExecute;
        }

        protected override NodeResult ExecuteNode() {
            return onExecute();
        }
    }
}
