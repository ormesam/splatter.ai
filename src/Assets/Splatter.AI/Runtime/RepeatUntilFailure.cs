namespace Splatter.AI {
    /// <summary>
    /// Restarts the child each time it succeeds, until it fails.
    /// Returns <see cref="NodeResult.Success"/> when the child fails, and
    /// <see cref="NodeResult.Running"/> while the child keeps succeeding.
    /// </summary>
    public class RepeatUntilFailure : Decorator {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatUntilFailure"/> class.
        /// </summary>
        public RepeatUntilFailure() : base("Repeat Until Failure") {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            var result = Child.OnUpdate();

            if (result == NodeResult.Failure) {
                return NodeResult.Success;
            }

            // The child reset itself on success, so ticking it next update restarts it.
            return NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}
