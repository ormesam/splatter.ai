namespace Splatter.AI {
    public class InvertDecorator : Decorator {
        public InvertDecorator(BehaviourTree tree) : base("Inverter", tree) {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            var result = Child.OnUpdate();

            switch (result) {
                case NodeResult.Failure:
                    return NodeResult.Success;
                case NodeResult.Success:
                    return NodeResult.Failure;
                default:
                    return NodeResult.Running;
            }
        }

        protected override void OnStop() {
        }
    }
}
