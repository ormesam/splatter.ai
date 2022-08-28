namespace Splatter.AI.Tests.Stubs {
    public class SuccessNode : Node {
        public SuccessNode(BehaviourTree tree) : base("Success", tree) {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            return NodeResult.Success;
        }

        protected override void OnStop() {
        }
    }
}