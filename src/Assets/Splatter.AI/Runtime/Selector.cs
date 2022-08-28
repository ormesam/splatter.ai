namespace Splatter.AI {
    /// <summary>
    /// Returns <see cref="NodeResult.Running"/> until a child returns <see cref="NodeResult.Success"/>. 
    /// If no children succeed, <see cref="NodeResult.Failure"/> is returned.
    /// </summary>
    public class Selector : Composite {
        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="tree">Behaviour tree</param>
        public Selector(BehaviourTree tree)
            : base("Selector", tree) {
        }

        protected override void OnStart() {
            CurrentNodeIdx = 0;
        }

        protected override NodeResult Update() {
            if (CanAbortSelf && !Condition()) {
                return NodeResult.Failure;
            }

            UpdateCurrentIdxIfInterrupted();

            if (CurrentNodeIdx < Children.Count) {
                var result = Children[CurrentNodeIdx].OnUpdate();

                if (result == NodeResult.Running) {
                    return NodeResult.Running;
                } else if (result == NodeResult.Success) {
                    return NodeResult.Success;
                } else {
                    CurrentNodeIdx++;

                    if (CurrentNodeIdx < Children.Count) {
                        return NodeResult.Running;
                    } else {
                        return NodeResult.Failure;
                    }
                }
            }

            return NodeResult.Failure;
        }

        protected override void OnStop() {
        }
    }
}
