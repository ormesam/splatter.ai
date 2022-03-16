namespace Splatter.AI {
    public class RunningDecorator : Decorator {
        public RunningDecorator(string name, BehaviourTree tree) : base(name, tree) {
        }

        protected override NodeResult ExecuteNode() {
            Child.Execute();

            return NodeResult.Running;
        }
    }
}