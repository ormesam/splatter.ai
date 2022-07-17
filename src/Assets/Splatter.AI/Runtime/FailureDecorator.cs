namespace Splatter.AI {
    public class FailureDecorator : Decorator {
        public FailureDecorator(BehaviourTree tree) : base("Always Fail", tree) {
        }

        protected override NodeResult ExecuteNode() {
            Child.Execute();

            return NodeResult.Failure;
        }
    }
}