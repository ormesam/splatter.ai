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

        public NodeResult Result { get; private set; } = NodeResult.Running;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        public Node(string name) {
            this.Name = name;
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

        /// <summary>
        /// Called when the node stops, either by completing or by being interrupted via <see cref="Stop"/>.
        /// <see cref="Result"/> holds the final result on completion, or <see cref="NodeResult.Running"/> when interrupted.
        /// </summary>
        protected abstract void OnStop();

        public void Stop() {
            BehaviourTree.Traverse(this, (node) => {
                if (!node.IsStarted) {
                    return;
                }

                node.IsStarted = false;
                node.OnStop();
                node.Result = NodeResult.Running;
            });
        }
    }
}
