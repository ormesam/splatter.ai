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
        /// <param name="name">Node name</param>
        /// <param name="onExecute">Function to run on execution</param>
        public Leaf(string name, Func<NodeResult> onExecute) : base(name) {
            this.onExecute = onExecute;
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            return onExecute();
        }

        protected override void OnStop() {
        }
    }
}
