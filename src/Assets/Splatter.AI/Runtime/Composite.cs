using System;
using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// Node with multiple children.
    /// </summary>
    public abstract class Composite : Node {
        /// <summary>
        /// Condition used to evaluate if this composite should be aborted.
        /// </summary>
        protected Func<bool> Condition { get; private set; }

        /// <summary>
        /// Abort type of this composite node.
        /// </summary>
        protected AbortType AbortType { get; private set; }

        /// <summary>
        /// Can this composite node be aborted.
        /// </summary>
        protected bool CanAbortSelf => AbortType == AbortType.SelfAndLower || AbortType == AbortType.Self;

        /// <summary>
        /// Can this composite abort lower priority nodes.
        /// </summary>
        protected bool CanAbortLower => AbortType == AbortType.SelfAndLower || AbortType == AbortType.Lower;

        /// <summary>
        /// Children of the composite node.
        /// </summary>
        public IList<Node> Children { get; set; }

        /// <summary>
        /// Index of the node to be executed.
        /// </summary>
        protected int CurrentNodeIdx = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Composite"/> class.
        /// </summary>
        /// <param name="tree">Behaviour tree</param>
        public Composite(string name, BehaviourTree tree) : base(name, tree) {
            Children = new List<Node>();
            AbortType = AbortType.None;
        }

        /// <summary>
        /// Update index if a higher priority task has interrupted.
        /// </summary>
        protected void UpdateCurrentIdxIfInterrupted() {
            for (int i = 0; i < CurrentNodeIdx; i++) {
                if (CanInterrupt(Children[i] as Composite)) {
                    CurrentNodeIdx = i;
                    return;
                }
            }
        }

        private bool CanInterrupt(Composite composite) {
            if (composite == null) {
                return false;
            }

            if (!composite.CanAbortLower) {
                return false;
            }

            return composite.Condition();
        }

        /// <summary>
        /// Sets the composite's abort type and condition
        /// </summary>
        /// <param name="abortType">Abort type (optional)</param>
        /// <param name="condition">Condition evaluated for aborting (optional)</param>
        public void SetAbortType(AbortType abortType, Func<bool> condition) {
            AbortType = abortType;
            Condition = condition;

            if (AbortType != AbortType.None && condition == null) {
                throw new InvalidOperationException($"{nameof(Condition)} cannot be null if {nameof(AbortType)} is not set to none");
            }
        }
    }
}
