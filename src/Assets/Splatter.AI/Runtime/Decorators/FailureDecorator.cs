namespace Splatter.AI.Decorators {
    /// <summary>
    /// Returns <see cref="NodeResult.Failure"/> when the child completes, regardless of its result.
    /// Returns <see cref="NodeResult.Running"/> while the child is running.
    /// </summary>
    public class FailureDecorator : Decorator {
        public FailureDecorator(string name = "Always Fail") : base(name) {
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