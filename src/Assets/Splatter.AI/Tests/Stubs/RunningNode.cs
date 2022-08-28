namespace Splatter.AI.Tests.Stubs {
    public class RunningNode : Node {
        public RunningNode(BehaviourTree tree) : base("Running", tree) {
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