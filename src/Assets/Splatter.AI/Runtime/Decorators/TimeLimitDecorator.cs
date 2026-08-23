using System;

namespace Splatter.AI.Decorators {
    /// <summary>
    /// Passes the child's result through, but fails if the child runs longer than the given
    /// number of seconds. On timeout the child is stopped and <see cref="NodeResult.Failure"/>
    /// is returned. The limit is measured from each activation of this decorator.
    /// </summary>
    public class TimeLimitDecorator : Decorator {
        private readonly float seconds;
        private readonly Func<float> clock;
        private float deadline;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeLimitDecorator"/> class.
        /// </summary>
        /// <param name="seconds">Seconds the child is allowed to run for</param>
        /// <param name="clock">Returns the current time in seconds, e.g. () => Time.time</param>
        public TimeLimitDecorator(float seconds, Func<float> clock) : this("Time Limit", seconds, clock) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeLimitDecorator"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="seconds">Seconds the child is allowed to run for</param>
        /// <param name="clock">Returns the current time in seconds, e.g. () => Time.time</param>
        public TimeLimitDecorator(string name, float seconds, Func<float> clock) : base(name) {
            this.seconds = seconds;
            this.clock = clock;
        }

        protected override void OnStart() {
            deadline = clock() + seconds;
        }

        protected override NodeResult Update() {
            var result = Child.OnUpdate();

            if (result == NodeResult.Running && clock() >= deadline) {
                Child.Stop();

                return NodeResult.Failure;
            }

            return result;
        }

        protected override void OnStop() {
        }
    }
}
