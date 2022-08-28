namespace Splatter.AI {
    public class FailureDecorator : Decorator {
        public FailureDecorator(BehaviourTree tree) : base("Always Fail", tree) {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            Child.OnUpdate();

            return NodeResult.Failure;
        }

        protected override void OnStop() {
        }
    }
}