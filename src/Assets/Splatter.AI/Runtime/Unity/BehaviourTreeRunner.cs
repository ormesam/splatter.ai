using UnityEngine;

namespace Splatter.AI {
    /// <summary>
    /// Runs a <see cref="BehaviourTree"/> as a Unity component, ticked centrally by
    /// <see cref="BehaviourTreeManager"/>. Override <see cref="CreateRoot"/> to create the tree
    /// root, and optionally <see cref="CreateTree"/> to supply a custom tree
    /// (e.g. <see cref="ContextBehaviourTree{T}"/>).
    /// Disabling the component pauses the tree in place; re-enabling resumes it.
    /// </summary>
    public abstract class BehaviourTreeRunner : MonoBehaviour {
        [SerializeField, Min(1), Tooltip("Tick the tree every this many frames. 1 ticks every frame.")]
        private int tickInterval = 1;

        /// <summary>
        /// The behaviour tree this component runs.
        /// </summary>
        public BehaviourTree Tree { get; private set; }

        /// <summary>
        /// The tree is ticked every this many frames; 1 ticks every frame.
        /// </summary>
        public int TickInterval => tickInterval;

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
            BehaviourTreeManager.Register(Tree, tickInterval);
        }

        protected virtual void OnEnable() {
            // On first activation the root does not exist yet; Start registers instead.
            if (Tree?.Root != null) {
                BehaviourTreeManager.Register(Tree, tickInterval);
            }
        }

        protected virtual void OnDisable() {
            BehaviourTreeManager.Unregister(Tree);
        }
    }
}
