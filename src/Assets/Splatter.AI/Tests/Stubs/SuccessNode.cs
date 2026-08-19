namespace Splatter.AI.Tests.Stubs {
    public class SuccessNode : Node {
        public SuccessNode() : base("Success") {
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