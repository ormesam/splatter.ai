using Splatter.AI.Decorators;

namespace Splatter.AI.Composites {
    /// <summary>
    /// Always returns <see cref="NodeResult.Running"/> until all children succeed.
    /// If a child fails, <see cref="NodeResult.Failure"/> is returned.
    /// This sequence has memory: it resumes at the running child each update and does not
    /// re-evaluate earlier children, so completed steps run exactly once per pass.
    /// Use <see cref="ReactiveSequencer"/> if earlier children should be re-checked every
    /// update and be able to interrupt a later running child, or make an earlier child an
    /// <see cref="ObservingDecorator"/> with <see cref="AbortMode.LowerPriority"/> to
    /// interrupt only when an observed value actually changes.
    /// </summary>
    public class Sequencer : Composite {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sequencer"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        public Sequencer(string name = "Sequence")
            : base(name) {
        }

        protected override void OnStart() {
            CurrentNodeIdx = 0;
        }

        protected override NodeResult Update() {
            ApplyObserverAborts();

            while (CurrentNodeIdx < Children.Count) {
                var result = Children[CurrentNodeIdx].OnUpdate();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                }

                if (result == NodeResult.Failure) {
                    return NodeResult.Failure;
                }

                CurrentNodeIdx++;
            }

            return NodeResult.Success;
        }

        protected override void OnStop() {
            CancelObservers();
        }
    }
}
