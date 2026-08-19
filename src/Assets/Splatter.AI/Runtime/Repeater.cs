namespace Splatter.AI {
    /// <summary>
    /// Restarts the child each time it completes, regardless of its result.
    /// Returns <see cref="NodeResult.Running"/> while repeating, and <see cref="NodeResult.Success"/>
    /// once the child has completed the given number of times. Repeats forever if no count is given.
    /// </summary>
    public class Repeater : Decorator {
        private readonly int times;
        private int completions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repeater"/> class that repeats forever.
        /// </summary>
        public Repeater() : this(-1) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repeater"/> class.
        /// </summary>
        /// <param name="times">Number of times the child must complete before returning
        /// <see cref="NodeResult.Success"/>. Negative repeats forever.</param>
        public Repeater(int times) : base("Repeater") {
            this.times = times;
        }

        protected override void OnStart() {
            completions = 0;
        }

        protected override NodeResult Update() {
            var result = Child.OnUpdate();

            if (result == NodeResult.Running) {
                return NodeResult.Running;
            }

            completions++;

            if (times >= 0 && completions >= times) {
                return NodeResult.Success;
            }

            // The child reset itself on completion, so ticking it next update restarts it.
            return NodeResult.Running;
        }

        protected override void OnStop() {
        }
    }
}
