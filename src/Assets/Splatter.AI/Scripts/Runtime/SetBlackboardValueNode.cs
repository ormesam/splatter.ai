namespace Splatter.AI {
    /// <summary>
    /// Short hand class for setting a blackboard value
    /// </summary>
    public class SetBlackboardValueNode : Node {
        private readonly string key;
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBlackboardValueNode"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        /// <param name="key">Blackboard key</param>
        /// <param name="value">Value</param>
        public SetBlackboardValueNode(BehaviourTree tree, string key, object value) : base($"Setting blackboard {key} to {value}", tree) {
            this.key = key;
            this.value = value;
        }

        protected override NodeResult ExecuteNode() {
            Tree.Blackboard[key] = value;

            return NodeResult.Success;
        }
    }

}
