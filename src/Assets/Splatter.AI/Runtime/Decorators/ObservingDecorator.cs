namespace Splatter.AI.Decorators {
    /// <summary>
    /// Runs the child only while a condition is true, like <see cref="GuardDecorator"/>, but
    /// instead of polling it observes an external source and re-evaluates the condition only
    /// when that source reports a change. Depending on <see cref="Mode"/>, a change can stop
    /// the running child (<see cref="AbortMode.Self"/>) or make the parent memory composite
    /// abort lower-priority branches and re-evaluate from this one
    /// (<see cref="AbortMode.LowerPriority"/>).
    /// </summary>
    public abstract class ObservingDecorator : Decorator {
        private bool isObserving;
        private bool conditionMet;
        private bool changed;

        /// <summary>
        /// How this decorator reacts when the observed source changes.
        /// </summary>
        public AbortMode Mode { get; }

        internal bool StopsSelf => Mode == AbortMode.Self || Mode == AbortMode.Both;

        internal bool StopsLowerPriority => Mode == AbortMode.LowerPriority || Mode == AbortMode.Both;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservingDecorator"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="mode">How to react when the observed source changes</param>
        protected ObservingDecorator(string name, AbortMode mode) : base(name) {
            this.Mode = mode;
        }

        /// <summary>
        /// Evaluates the condition. Called when the decorator starts and when the observed
        /// source has reported a change, never per tick.
        /// </summary>
        protected abstract bool IsConditionMet();

        /// <summary>
        /// Starts observing the external source. Implementations route change events to
        /// <see cref="OnObservedValueChanged"/>.
        /// </summary>
        protected abstract void StartObserving();

        /// <summary>
        /// Stops observing the external source.
        /// </summary>
        protected abstract void StopObserving();

        protected override void OnStart() {
            if (Mode != AbortMode.None && !isObserving) {
                isObserving = true;
                StartObserving();
            }

            changed = false;
            conditionMet = IsConditionMet();
        }

        protected override NodeResult Update() {
            if (changed && StopsSelf) {
                changed = false;
                conditionMet = IsConditionMet();
            }

            if (!conditionMet) {
                if (Child.IsStarted) {
                    Child.Stop();
                }

                return NodeResult.Failure;
            }

            return Child.OnUpdate();
        }

        protected override void OnStop() {
            // Lower-priority observers must keep observing after their branch completes or is
            // interrupted; the parent composite ends the observation via CancelObservation()
            // when it stops itself. AbortMode.None never subscribed.
            if (isObserving && !StopsLowerPriority) {
                isObserving = false;
                StopObserving();
            }
        }

        /// <summary>
        /// Concrete classes call this from their change notification handler. Only records
        /// that a change happened; the tree is never mutated at notification time.
        /// </summary>
        protected void OnObservedValueChanged() {
            changed = true;
        }

        /// <summary>
        /// Called by the parent composite at the start of its update. Returns true when a
        /// recorded change would flip this branch's last delivered result, meaning
        /// lower-priority siblings should be aborted and evaluation resumed from this branch.
        /// </summary>
        internal bool ShouldAbortLowerPriority() {
            if (!changed || IsStarted || !StopsLowerPriority) {
                return false;
            }

            changed = false;

            var wouldBe = IsConditionMet() ? NodeResult.Success : NodeResult.Failure;

            return wouldBe != Result;
        }

        /// <summary>
        /// Ends the observation entirely. Called by the parent composite when it stops.
        /// </summary>
        internal void CancelObservation() {
            changed = false;

            if (isObserving) {
                isObserving = false;
                StopObserving();
            }
        }
    }
}
