using System.Collections.Generic;
using UnityEngine;

namespace Splatter.AI {
    /// <summary>
    /// Base class for behaviour trees.</para>
    /// Override <see cref="Start"/> to initialise blackboard values. Override <see cref="CreateRoot"/> to create the tree root.
    /// </summary>
    public abstract class BehaviourTree : MonoBehaviour {
        public Node Root;

        /// <summary>
        /// Dictionary for storing variables used in the behaviour tree.
        /// </summary>
        public IDictionary<string, object> Blackboard { get; private set; }

        protected virtual void Awake() {
            Blackboard = new Dictionary<string, object>();
        }

        protected virtual void Start() {
            Root = CreateRoot();
        }

        /// <summary>
        /// Creates the root of the behaviour tree.
        /// </summary>
        /// <returns>Behaviour tree root</returns>
        protected abstract Node CreateRoot();

        protected virtual void Update() {
            Root.OnUpdate();
        }

        /// <summary>
        /// Helper to get items from the blackboard, casted to the type passed in.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="key">Item key</param>
        /// <returns>Item</returns>
        public T GetItem<T>(string key) {
            if (!Blackboard.ContainsKey(key)) {
                Debug.LogError($"Dictionary item not found with key: {key}");
            }

            return (T)Blackboard[key];
        }
    }
}
