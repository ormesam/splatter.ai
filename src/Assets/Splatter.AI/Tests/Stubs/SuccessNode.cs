namespace Splatter.AI.Tests.Stubs {
    public class SuccessNode : Node {
        public SuccessNode(BehaviourTree tree) : base("Success", tree) {
        }

        protected override NodeResult ExecuteNode() {
            return NodeResult.Success;
        }
    }
}