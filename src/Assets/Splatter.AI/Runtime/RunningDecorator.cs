namespace Splatter.AI {
    public class RunningDecorator : Decorator {
        public RunningDecorator(BehaviourTree tree) : base("Always Running", tree) {
        }

        protected override NodeResult ExecuteNode() {
            Child.Execute();

            return NodeResult.Running;
        }
    }
}