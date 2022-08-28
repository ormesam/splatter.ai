namespace Splatter.AI {
    public class RunningDecorator : Decorator {
        public RunningDecorator(BehaviourTree tree) : base("Always Running", tree) {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            Child.OnUpdate();

            return NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}