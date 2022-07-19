namespace Splatter.AI {
    /// <summary>
    /// Always returns <see cref="NodeResult.Running"/> until all children succeed. 
    /// If a child fails, <see cref="NodeResult.Failure"/> is returned.
    /// </summary>
    public class Sequencer : Composite {
        private bool resetIfInterrupted;
        private int lastRanOnTick = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequencer"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="tree">Behaviour tree</param>
        public Sequencer(BehaviourTree tree)
            : base("Sequence", tree) {
        }

        protected override NodeResult ExecuteNode() {
            if (CanAbortSelf && !Condition()) {
                return NodeResult.Failure;
            }

            UpdateCurrentIdxIfInterrupted();

            if (resetIfInterrupted && lastRanOnTick != Tree.Ticks - 1) {
                CurrentNodeIdx = 0;
            }

            lastRanOnTick = Tree.Ticks;

            if (CurrentNodeIdx < Children.Count) {
                var result = Children[CurrentNodeIdx].Execute();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                } else if (result == NodeResult.Failure) {
                    CurrentNodeIdx = 0;
                    return NodeResult.Failure;
                } else {
                    CurrentNodeIdx++;

                    if (CurrentNodeIdx < Children.Count) {
                        return NodeResult.Running;
                    } else {
                        CurrentNodeIdx = 0;
                        return NodeResult.Success;
                    }
                }
            }

            return NodeResult.Success;
        }

        public void SetResetIfInterrupted(bool reset) {
            resetIfInterrupted = reset;
        }

#if UNITY_INCLUDE_TESTS
        // Useful for debugging tests
        public int CurrentIndex => CurrentNodeIdx;
        public bool ResetIfInterrupted => resetIfInterrupted;
#endif
    }
}
