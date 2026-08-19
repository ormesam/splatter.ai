namespace Splatter.AI {
    /// <summary>
    /// Returns <see cref="NodeResult.Failure"/> when the child completes, regardless of its result.
    /// Returns <see cref="NodeResult.Running"/> while the child is running.
    /// </summary>
    public class FailureDecorator : Decorator {
        public FailureDecorator() : base("Always Fail") {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            var result = Child.OnUpdate();

            return result == NodeResult.Running ? NodeResult.Running : NodeResult.Failure;
        }

        protected override void OnStop() {
        }
    }
}