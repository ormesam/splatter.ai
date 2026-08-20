using System;
using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// A behaviour tree. Assign <see cref="Root"/>, then call <see cref="Tick"/> each update.
    /// </summary>
    public class BehaviourTree {
        private Node root;
        private bool isValidated;

        /// <summary>
        /// Root node of the behaviour tree.
        /// </summary>
        public Node Root {
            get => root;
            set {
                root = value;
                isValidated = false;
            }
        }

        /// <summary>
        /// Dictionary for storing variables used in the behaviour tree. Notifies per-key
        /// observers when a value actually changes.
        /// </summary>
        public Blackboard Blackboard { get; } = new Blackboard();

        /// <summary>
        /// Updates the tree by one tick.
        /// </summary>
        /// <returns>The result of the root node's execution.</returns>
        public NodeResult Tick() {
            if (Root == null) {
                throw new InvalidOperationException("Tree has no root");
            }

            if (!isValidated) {
                Validate();
                isValidated = true;
            }

            return Root.OnUpdate();
        }

        /// <summary>
        /// Stops all running nodes and ends every observation, including inside subtrees.
        /// </summary>
        public void Stop() {
            Root?.Stop();
        }

        /// <summary>
        /// Helper to get items from the blackboard, casted to the type passed in.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="key">Item key</param>
        /// <returns>Item</returns>
        public T GetItem<T>(string key) {
            if (!Blackboard.TryGetValue(key, out var value)) {
                throw new KeyNotFoundException($"Blackboard item not found with key: {key}");
            }

            return (T)value;
        }

        private void Validate() {
            if (Root is ObservingDecorator rootObserver && rootObserver.StopsLowerPriority) {
                throw MisplacedObserver(rootObserver);
            }

            Traverse(Root, (node) => {
                foreach (var child in GetChildren(node)) {
                    if (child is ObservingDecorator observer && observer.StopsLowerPriority
                        && !(node is Selector || node is Sequencer)) {

                        throw MisplacedObserver(observer);
                    }
                }
            });
        }

        private static InvalidOperationException MisplacedObserver(ObservingDecorator observer) {
            return new InvalidOperationException(
                $"'{observer.Name}' uses AbortMode.{observer.Mode} but is not a direct child of a " +
                "Selector or Sequencer, so its lower-priority aborts would never be applied. " +
                "Move it to the top of a memory composite branch, or use AbortMode.Self, a " +
                "GuardDecorator, or a reactive composite instead.");
        }

        public static void Traverse(Node node, Action<Node> visitor) {
            visitor.Invoke(node);

            var children = GetChildren(node);

            foreach (var child in children) {
                Traverse(child, visitor);
            }
        }

        public static IEnumerable<Node> GetChildren(Node parent) {
            if (parent is Decorator decorator && decorator.Child != null) {
                return new[] { decorator.Child };
            }

            if (parent is Composite composite) {
                return composite;
            }

            if (parent is SubtreeNode subtree && subtree.Tree?.Root != null) {
                return new[] { subtree.Tree.Root };
            }

            return Array.Empty<Node>();
        }
    }
}
