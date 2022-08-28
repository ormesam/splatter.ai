namespace Splatter.AI.Tests.Stubs {
    public class FailureNode : Node {
        public FailureNode(BehaviourTree tree) : base("Failure", tree) {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            return NodeResult.Failure;
        }

        protected override void OnStop() {
        }
    }
}