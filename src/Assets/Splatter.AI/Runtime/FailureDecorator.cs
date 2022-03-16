namespace Splatter.AI {
    public class FailureDecorator : Decorator {
        public FailureDecorator(string name, BehaviourTree tree) : base(name, tree) {
        }

        protected override NodeResult ExecuteNode() {
            Child.Execute();

            return NodeResult.Failure;
        }
    }
}