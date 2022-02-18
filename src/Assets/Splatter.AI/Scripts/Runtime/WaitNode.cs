using UnityEngine;

namespace Splatter.AI {
    /// <summary>
    /// Wait for x number of seconds before returning <see cref="NodeResult.Success"/>.
    /// </summary>
    public class WaitNode : Node {
        private readonly float? waitTime;
        private float? existTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitNode"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="waitTime">Seconds to wait</param>
        public WaitNode(string name, BehaviourTree tree, float waitTime) : base(name, tree) {
            this.waitTime = waitTime;
        }

        protected override NodeResult ExecuteNode() {
            if (existTime == null) {
                existTime = Time.time + waitTime;
            }

            if (Time.time >= existTime) {
                existTime = null;

                return NodeResult.Success;
            }

            return NodeResult.Running;
        }
    }
}
