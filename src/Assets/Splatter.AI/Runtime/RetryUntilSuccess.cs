namespace Splatter.AI {
    /// <summary>
    /// Restarts the child each time it fails, until it succeeds.
    /// Returns <see cref="NodeResult.Success"/> when the child succeeds, and
    /// <see cref="NodeResult.Failure"/> once the child has failed the maximum number of attempts.
    /// Retries forever if no attempt count is given.
    /// </summary>
    public class RetryUntilSuccess : Decorator {
        private readonly int maxAttempts;
        private int attempts;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryUntilSuccess"/> class that retries forever.
        /// </summary>
        public RetryUntilSuccess() : this(-1) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryUntilSuccess"/> class.
        /// </summary>
        /// <param name="maxAttempts">Number of failed attempts before returning
        /// <see cref="NodeResult.Failure"/>. Negative retries forever.</param>
        public RetryUntilSuccess(int maxAttempts) : base("Retry Until Success") {
            this.maxAttempts = maxAttempts;
        }

        protected override void OnStart() {
            attempts = 0;
        }

        protected override NodeResult Update() {
            var result = Child.OnUpdate();

            if (result != NodeResult.Failure) {
                return result;
            }

            attempts++;

            if (maxAttempts >= 0 && attempts >= maxAttempts) {
                return NodeResult.Failure;
            }

            // The child reset itself on completion, so ticking it next update retries it.
            return NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}
