using UnityEngine;

namespace Splatter.AI {
    /// <summary>
    /// Wait for x number of seconds before returning <see cref="NodeResult.Success"/>.
    /// </summary>
    public class WaitNode : Node {
        private readonly float? waitTime;
        private float? exitTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitNode"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="waitTime">Seconds to wait</param>
        public WaitNode(string name, BehaviourTree tree, float waitTime) : base(name, tree) {
            this.waitTime = waitTime;
        }

        protected override void OnStart() {
            exitTime = Time.time + waitTime;
        }

        protected override NodeResult Update() {
            if (Time.time >= exitTime) {
                exitTime = null;

                return NodeResult.Success;
            }

            return NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}
