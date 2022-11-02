namespace Splatter.AI {
    /// <summary>
    /// Base class for all nodes on a behaviour tree.
    /// </summary>
    public abstract class Node {
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Node name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Behaviour tree this node is on.
        /// </summary>
        protected BehaviourTree Tree { get; private set; }

        public NodeResult Result { get; private set; } = NodeResult.Running;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree this node is on.</param>
        public Node(string name, BehaviourTree tree) {
            this.Name = name;

            Tree = tree;
        }

        protected abstract void OnStart();

        /// <summary>
        /// Behaviour of the node.
        /// </summary>
        /// <returns>The result of the execution.</returns>
        protected abstract NodeResult Update();

        /// <summary>
        /// Behaviour of the node.
        /// </summary>
        /// <returns>The result of the execution.</returns>
        public NodeResult OnUpdate() {
            if (!IsStarted) {
                OnStart();
                IsStarted = true;
            }

            Result = Update();

            if (Result != NodeResult.Running) {
                OnStop();
                IsStarted = false;
            }

            return Result;
        }

        protected abstract void OnStop();

        public virtual void Abort() {
            BehaviourTree.Traverse(this, (node) => {
                node.IsStarted = false;
                node.Result = NodeResult.Running;
                node.OnStop();
            });
        }
    }
}
