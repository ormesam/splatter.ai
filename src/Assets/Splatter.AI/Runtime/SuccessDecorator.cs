namespace Splatter.AI {
    /// <summary>
    /// Returns <see cref="NodeResult.Success"/> when the child completes, regardless of its result.
    /// Returns <see cref="NodeResult.Running"/> while the child is running.
    /// </summary>
    public class SuccessDecorator : Decorator {
        public SuccessDecorator(BehaviourTree tree) : base("Always Succeed", tree) {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            var result = Child.OnUpdate();

            return result == NodeResult.Running ? NodeResult.Running : NodeResult.Success;
        }

        protected override void OnStop() {
        }
    }
}
