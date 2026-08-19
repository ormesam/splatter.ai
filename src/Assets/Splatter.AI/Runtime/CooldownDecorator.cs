using System;

namespace Splatter.AI {
    /// <summary>
    /// Passes the child's result through, then locks the child out for the given number of
    /// seconds after it completes. While cooling down, returns <see cref="NodeResult.Failure"/>
    /// without running the child. A child that is already running is never interrupted.
    /// </summary>
    public class CooldownDecorator : Decorator {
        private readonly float seconds;
        private readonly Func<float> clock;
        private float readyTime = float.NegativeInfinity;

        /// <summary>
        /// Initializes a new instance of the <see cref="CooldownDecorator"/> class.
        /// </summary>
        /// <param name="seconds">Seconds to wait after the child completes before it can run again</param>
        /// <param name="clock">Returns the current time in seconds, e.g. () => Time.time</param>
        public CooldownDecorator(float seconds, Func<float> clock) : base("Cooldown") {
            this.seconds = seconds;
            this.clock = clock;
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            if (!Child.IsStarted && clock() < readyTime) {
                return NodeResult.Failure;
            }

            var result = Child.OnUpdate();

            if (result != NodeResult.Running) {
                readyTime = clock() + seconds;
            }

            return result;
        }

        protected override void OnStop() {
        }
    }
}
