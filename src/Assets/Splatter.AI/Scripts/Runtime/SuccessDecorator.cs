namespace Splatter.AI {
    public class SuccessDecorator : Decorator {
        public SuccessDecorator(string name, BehaviourTree tree) : base(name, tree) {
        }

        protected override NodeResult ExecuteNode() {
            Child.Execute();

            return NodeResult.Success;
        }
    }
}
