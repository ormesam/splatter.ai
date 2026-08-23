namespace Splatter.AI.Tests.Stubs {
    public class RunningNode : Node {
        public RunningNode() : base("Running") {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            return NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}