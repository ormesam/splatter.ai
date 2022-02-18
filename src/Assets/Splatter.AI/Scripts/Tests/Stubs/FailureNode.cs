using Splatter.AI;

namespace Splatter.Tests.Stubs {
    public class FailureNode : Node {
        public FailureNode(BehaviourTree tree) : base("Failure", tree) {
        }

        protected override NodeResult ExecuteNode() {
            return NodeResult.Failure;
        }
    }
}