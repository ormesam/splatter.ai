using UnityEngine;

namespace Splatter.AI {
    /// <summary>
    /// Wait for x number of seconds before returning <see cref="NodeResult.Success"/>.
    /// </summary>
    public class WaitNode : Node {
        private float minSeconds;
        private float maxSeconds;
        private float exitTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitNode"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="minSeconds">Seconds to wait</param>
        /// <param name="maxSeconds">Seconds to wait</param>
        public WaitNode(string name, BehaviourTree tree, float minSeconds, float maxSeconds) : base(name, tree) {
            this.minSeconds = minSeconds;
            this.maxSeconds = maxSeconds;
        }

        protected override void OnStart() {
            exitTime = Time.time + Random.Range(minSeconds, maxSeconds);
        }

        protected override NodeResult Update() {
            if (Time.time >= exitTime) {
                return NodeResult.Success;
            }

            return NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}
