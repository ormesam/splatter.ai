using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// Short hand class for setting a blackboard value
    /// </summary>
    public class SetBlackboardValueNode : Node {
        private readonly IDictionary<string, object> blackboard;
        private readonly string key;
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBlackboardValueNode"/> class.
        /// </summary>
        /// <param name="blackboard">Blackboard to set the value on</param>
        /// <param name="key">Blackboard key</param>
        /// <param name="value">Value</param>
        public SetBlackboardValueNode(IDictionary<string, object> blackboard, string key, object value) : base($"Setting blackboard {key} to {value}") {
            this.blackboard = blackboard;
            this.key = key;
            this.value = value;
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            blackboard[key] = value;

            return NodeResult.Success;
        }

        protected override void OnStop() {
        }
    }
}
