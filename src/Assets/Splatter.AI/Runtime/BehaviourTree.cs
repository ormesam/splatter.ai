using System;
using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// A behaviour tree. Assign <see cref="Root"/>, then call <see cref="Tick"/> each update.
    /// </summary>
    public class BehaviourTree {
        /// <summary>
        /// Root node of the behaviour tree.
        /// </summary>
        public Node Root { get; set; }

        /// <summary>
        /// Dictionary for storing variables used in the behaviour tree.
        /// </summary>
        public IDictionary<string, object> Blackboard { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Updates the tree by one tick.
        /// </summary>
        /// <returns>The result of the root node's execution.</returns>
        public NodeResult Tick() {
            if (Root == null) {
                throw new InvalidOperationException("Tree has no root");
            }

            return Root.OnUpdate();
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

            return Array.Empty<Node>();
        }
    }
}
