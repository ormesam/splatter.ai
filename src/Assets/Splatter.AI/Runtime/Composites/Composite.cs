using System.Collections;
using System.Collections.Generic;
using Splatter.AI.Decorators;

namespace Splatter.AI.Composites {
    /// <summary>
    /// Node with multiple children.
    /// </summary>
    public abstract class Composite : Node, IEnumerable<Node> {
        private readonly List<Node> children = new List<Node>();

        /// <summary>
        /// Children of the composite node.
        /// </summary>
        protected IReadOnlyList<Node> Children => children;

        /// <summary>
        /// Number of children.
        /// </summary>
        public int Count => children.Count;

        /// <summary>
        /// Index of the node to be executed. Readable by debugging tools; for randomised
        /// composites this is progress through the shuffled order, not a child index.
        /// </summary>
        public int CurrentNodeIdx { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Composite"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        public Composite(string name) : base(name) {
        }

        /// <summary>
        /// Adds a child to the composite.
        /// </summary>
        /// <param name="node">Child node</param>
        public void Add(Node node) {
            children.Add(node);
        }

        public IEnumerator<Node> GetEnumerator() => children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected void StopChildren() {
            StopChildren(0);
        }

        /// <summary>
        /// Stops children at or after the given index.
        /// </summary>
        /// <param name="fromIndex">Index of the first child to stop</param>
        protected void StopChildren(int fromIndex) {
            for (int i = fromIndex; i < children.Count; i++) {
                children[i].Stop();
            }
        }

        /// <summary>
        /// Applies a pending lower-priority abort from an already-passed observing child,
        /// stopping later children and rewinding <see cref="CurrentNodeIdx"/> to the observer.
        /// The lowest index (highest priority) wins.
        /// </summary>
        protected void ApplyObserverAborts() {
            for (int i = 0; i < CurrentNodeIdx && i < children.Count; i++) {
                if (children[i] is ObservingDecorator observer && observer.ShouldAbortLowerPriority()) {
                    StopChildren(i + 1);
                    CurrentNodeIdx = i;

                    return;
                }
            }
        }

        /// <summary>
        /// Ends observation on observing children when this composite stops, so
        /// lower-priority observers don't outlive the composite that scopes them.
        /// </summary>
        protected void CancelObservers() {
            for (int i = 0; i < children.Count; i++) {
                if (children[i] is ObservingDecorator observer) {
                    observer.CancelObservation();
                }
            }
        }
    }
}
