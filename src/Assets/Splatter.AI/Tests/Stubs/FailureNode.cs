namespace Splatter.AI.Tests.Stubs {
    public class FailureNode : Node {
        public FailureNode(BehaviourTree tree) : base("Failure", tree) {
        }

        protected override NodeResult ExecuteNode() {
            return NodeResult.Failure;
        }
    }
}