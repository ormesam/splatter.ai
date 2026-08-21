namespace Splatter.AI {
    /// <summary>
    /// A sequence without memory: every update re-ticks children from the first, so an earlier
    /// child changing its result immediately interrupts a later running child (which is stopped).
    /// Returns <see cref="NodeResult.Failure"/> as soon as a child fails, and
    /// <see cref="NodeResult.Success"/> once all children succeed in the same update.
    /// Because earlier children re-run every update, they should be cheap condition checks;
    /// use <see cref="Sequencer"/> for step-by-step action lists.
    /// </summary>
    public class ReactiveSequencer : Composite {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveSequencer"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        public ReactiveSequencer(string name = "Reactive Sequence") : base(name) {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            for (int i = 0; i < Children.Count; i++) {
                var result = Children[i].OnUpdate();

                if (result != NodeResult.Success) {
                    StopChildren(i + 1);

                    return result;
                }
            }

            return NodeResult.Success;
        }

        protected override void OnStop() {
        }
    }
}
