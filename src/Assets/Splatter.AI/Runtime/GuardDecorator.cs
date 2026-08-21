using System;

namespace Splatter.AI {
    /// <summary>
    /// Runs the child only while the condition is true, returning the child's result.
    /// The condition is re-evaluated every update: if it becomes false while the child is
    /// running, the child is stopped and <see cref="NodeResult.Failure"/> is returned.
    /// For an event-driven alternative that re-checks only when an observed value actually
    /// changes, see <see cref="ObservingDecorator"/>.
    /// </summary>
    public class GuardDecorator : Decorator {
        private readonly Func<bool> condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuardDecorator"/> class.
        /// </summary>
        /// <param name="condition">Condition to evaluate</param>
        public GuardDecorator(Func<bool> condition) : this("Guard", condition) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuardDecorator"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="condition">Condition to evaluate</param>
        public GuardDecorator(string name, Func<bool> condition) : base(name) {
            this.condition = condition;
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            if (!condition()) {
                if (Child.IsStarted) {
                    Child.Stop();
                }

                return NodeResult.Failure;
            }

            return Child.OnUpdate();
        }

        protected override void OnStop() {
        }
    }
}
