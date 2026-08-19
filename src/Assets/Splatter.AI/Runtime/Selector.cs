namespace Splatter.AI {
    /// <summary>
    /// Returns <see cref="NodeResult.Running"/> until a child returns <see cref="NodeResult.Success"/>.
    /// If no children succeed, <see cref="NodeResult.Failure"/> is returned.
    /// This selector has memory: it resumes at the running child each update and does not
    /// re-evaluate earlier children, so a chosen branch runs to completion once picked.
    /// Use <see cref="ReactiveSelector"/> if higher-priority children should be re-checked
    /// every update and be able to interrupt a later running child.
    /// </summary>
    public class Selector : Composite {
        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// </summary>
        public Selector()
            : base("Selector") {
        }

        protected override void OnStart() {
            CurrentNodeIdx = 0;
        }

        protected override NodeResult Update() {
            while (CurrentNodeIdx < Children.Count) {
                var result = Children[CurrentNodeIdx].OnUpdate();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                }

                if (result == NodeResult.Success) {
                    return NodeResult.Success;
                }

                CurrentNodeIdx++;
            }

            return NodeResult.Failure;
        }

        protected override void OnStop() {
        }
    }
}
