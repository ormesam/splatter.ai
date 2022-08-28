namespace Splatter.AI {
    /// <summary>
    /// Always returns <see cref="NodeResult.Running"/> until all children succeed. 
    /// If a child fails, <see cref="NodeResult.Failure"/> is returned.
    /// </summary>
    public class Sequencer : Composite {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sequencer"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="tree">Behaviour tree</param>
        public Sequencer(BehaviourTree tree)
            : base("Sequence", tree) {
        }

        protected override void OnStart() {
            CurrentNodeIdx = 0;
        }

        protected override NodeResult Update() {
            if (CanAbortSelf && !Condition()) {
                Abort();

                return NodeResult.Failure;
            }

            UpdateCurrentIdxIfInterrupted();

            if (CurrentNodeIdx < Children.Count) {
                var result = Children[CurrentNodeIdx].OnUpdate();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                } else if (result == NodeResult.Failure) {
                    return NodeResult.Failure;
                } else {
                    CurrentNodeIdx++;

                    if (CurrentNodeIdx < Children.Count) {
                        return NodeResult.Running;
                    } else {
                        return NodeResult.Success;
                    }
                }
            }

            return NodeResult.Success;
        }

        protected override void OnStop() {
        }
    }
}
