namespace Splatter.AI.Tests.Stubs {
    public class FailureNode : Node {
        public FailureNode() : base("Failure") {
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