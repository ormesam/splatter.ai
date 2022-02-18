using System.Collections.Generic;
using UnityEngine;

namespace Splatter.AI {
    /// <summary>
    /// Base class for behaviour trees.</para>
    /// Override <see cref="Start"/> to initialise blackboard values. Override <see cref="CreateRoot"/> to create the tree root.
    /// </summary>
    public abstract class BehaviourTree : MonoBehaviour {
        private Node root;

        /// <summary>
        /// Dictionary for storing variables used in the behaviour tree.
        /// </summary>
        public IDictionary<string, object> Blackboard { get; private set; }

        /// <summary>
        /// Number of times the tree has been executed (every update) since start / reset.
        /// </summary>
        public int Ticks { get; private set; }

        protected virtual void Awake() {
            Blackboard = new Dictionary<string, object>();
            Ticks = 0;
        }

        protected virtual void Start() {
            root = CreateRoot();
        }

        /// <summary>
        /// Creates the root of the behaviour tree.
        /// </summary>
        /// <returns>Behaviour tree root</returns>
        protected abstract Node CreateRoot();

        protected virtual void Update() {
            root.Execute();

            Ticks++;
        }

        /// <summary>
        /// Helper to get items from the blackboard, casted to the type passed in.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="key">Item key</param>
        /// <returns>Item</returns>
        public T GetItem<T>(string key) {
            return (T)Blackboard[key];
        }

#if UNITY_INCLUDE_TESTS
        public void IncrementTick() {
            Ticks++;
        }
#endif

#if UNITY_EDITOR
        public Node GetRoot() {
            return root;
        }
#endif
    }
}
