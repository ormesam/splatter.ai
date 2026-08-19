namespace Splatter.AI {
    /// <summary>
    /// A selector without memory: every update re-ticks children from the first, so a
    /// higher-priority child becoming available immediately interrupts a later running child
    /// (which is stopped). Returns <see cref="NodeResult.Success"/> as soon as a child succeeds,
    /// and <see cref="NodeResult.Failure"/> once all children fail in the same update.
    /// Because earlier children re-run every update, they should be cheap condition checks;
    /// use <see cref="Selector"/> for branches that should run to completion once chosen.
    /// </summary>
    public class ReactiveSelector : Composite {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveSelector"/> class.
        /// </summary>
        public ReactiveSelector() : base("Reactive Selector") {
        }

        protected override void OnStart() {
        }

        protected override NodeResult Update() {
            for (int i = 0; i < Children.Count; i++) {
                var result = Children[i].OnUpdate();

                if (result != NodeResult.Failure) {
                    StopChildren(i + 1);

                    return result;
                }
            }

            return NodeResult.Failure;
        }

        protected override void OnStop() {
        }
    }
}
