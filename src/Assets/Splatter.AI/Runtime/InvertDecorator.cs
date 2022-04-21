namespace Splatter.AI {
    public class InvertDecorator : Decorator {
        public InvertDecorator(BehaviourTree tree) : base("Inverter", tree) {
        }

        protected override NodeResult ExecuteNode() {
            var result = Child.Execute();

            switch (result) {
                case NodeResult.Failure:
                    return NodeResult.Success;
                case NodeResult.Success:
                    return NodeResult.Failure;
                default:
                    return NodeResult.Running;
            }
        }
    }
}
