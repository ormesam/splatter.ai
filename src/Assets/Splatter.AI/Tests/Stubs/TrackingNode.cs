using System;

namespace Splatter.AI.Tests.Stubs {
    public class TrackingNode : Node {
        private readonly Func<NodeResult> onUpdate;

        public int Starts { get; private set; }
        public int Updates { get; private set; }
        public int Stops { get; private set; }
        public NodeResult ResultAtLastStop { get; private set; }

        public TrackingNode(BehaviourTree tree, Func<NodeResult> onUpdate) : base("Tracking", tree) {
            this.onUpdate = onUpdate;
        }

        protected override void OnStart() {
            Starts++;
        }

        protected override NodeResult Update() {
            Updates++;

            return onUpdate();
        }

        protected override void OnStop() {
            Stops++;
            ResultAtLastStop = Result;
        }
    }
}
