using Splatter.AI;

namespace Splatter.Tests.Stubs {
    public class RunningNode : Node {
        public RunningNode(BehaviourTree tree) : base("Running", tree) {
        }

        protected override NodeResult ExecuteNode() {
            return NodeResult.Running;
        }
    }
}