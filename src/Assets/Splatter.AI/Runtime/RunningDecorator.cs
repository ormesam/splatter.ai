namespace Splatter.AI {
    /// <summary>
    /// Runs the child to completion once, then returns <see cref="NodeResult.Running"/> indefinitely.
    /// The completed child is not restarted until this decorator is stopped via <see cref="Node.Stop"/>.
    /// </summary>
    public class RunningDecorator : Decorator {
        private bool childCompleted;

        public RunningDecorator(BehaviourTree tree) : base("Always Running", tree) {
        }

        protected override void OnStart() {
            childCompleted = false;
        }

        protected override NodeResult Update() {
            if (!childCompleted) {
                childCompleted = Child.OnUpdate() != NodeResult.Running;
            }

            return NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}