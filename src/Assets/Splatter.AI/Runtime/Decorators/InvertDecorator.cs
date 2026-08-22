namespace Splatter.AI.Decorators {
    public class InvertDecorator : Decorator {
        public InvertDecorator(string name = "Inverter") : base(name) {
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
