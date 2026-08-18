using UnityEngine;

namespace Splatter.AI {
    /// <summary>
    /// Runs a <see cref="BehaviourTree"/> as a Unity component.
    /// Override <see cref="CreateRoot"/> to create the tree root, and optionally
    /// <see cref="CreateTree"/> to supply a custom tree (e.g. <see cref="ContextBehaviourTree{T}"/>).
    /// </summary>
    public abstract class BehaviourTreeRunner : MonoBehaviour {
        /// <summary>
        /// The behaviour tree this component runs.
        /// </summary>
        public BehaviourTree Tree { get; private set; }

        /// <summary>
        /// Creates the behaviour tree. Override to supply a custom tree type.
        /// </summary>
        protected virtual BehaviourTree CreateTree() {
            return new BehaviourTree();
        }

        /// <summary>
        /// Creates the root of the behaviour tree.
        /// </summary>
        /// <returns>Behaviour tree root</returns>
        protected abstract Node CreateRoot();

        protected virtual void Awake() {
            Tree = CreateTree();
        }

        protected virtual void Start() {
            Tree.Root = CreateRoot();
        }

        protected virtual void Update() {
            Tree.Tick();
        }
    }
}
