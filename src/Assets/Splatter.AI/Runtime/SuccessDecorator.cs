namespace Splatter.AI {
    public class SuccessDecorator : Decorator {
        public SuccessDecorator(BehaviourTree tree) : base("Always Succeed", tree) {
        }

        protected override NodeResult ExecuteNode() {
            Child.Execute();

            return NodeResult.Success;
        }
    }
}
