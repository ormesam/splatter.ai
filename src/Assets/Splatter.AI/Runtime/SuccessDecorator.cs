namespace Splatter.AI {
    public class SuccessDecorator : Decorator {
        public SuccessDecorator(BehaviourTree tree) : base("Always Succeed", tree) {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            Child.OnUpdate();

            return NodeResult.Success;
        }

        protected override void OnStop() {
        }
    }
}
