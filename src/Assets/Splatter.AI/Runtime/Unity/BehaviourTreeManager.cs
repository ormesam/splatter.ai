using UnityEngine;

namespace Splatter.AI {
    /// <summary>
    /// Central manager that ticks all registered behaviour trees from a single update loop.
    /// <see cref="BehaviourTreeRunner"/> registers with it automatically; register directly to
    /// run a tree without a runner.
    /// </summary>
    public static class BehaviourTreeManager {
        private static BehaviourTreeScheduler scheduler = new BehaviourTreeScheduler();
        private static Driver driver;

        /// <summary>
        /// Registers a tree to be ticked centrally. Registering an already registered tree
        /// updates its interval.
        /// </summary>
        /// <param name="tree">Tree to tick</param>
        /// <param name="interval">Tick the tree every this many frames; 1 ticks every frame</param>
        public static void Register(BehaviourTree tree, int interval = 1) {
            if (driver == null) {
                var go = new GameObject("Splatter.AI Manager") {
                    hideFlags = HideFlags.HideInHierarchy,
                };

                Object.DontDestroyOnLoad(go);
                driver = go.AddComponent<Driver>();
            }

            scheduler.Register(tree, interval);
        }

        /// <summary>
        /// Unregisters a tree so it is no longer ticked. Safe to call for trees that are not
        /// registered.
        /// </summary>
        /// <param name="tree">Tree to unregister</param>
        /// <returns>Whether the tree was registered.</returns>
        public static bool Unregister(BehaviourTree tree) {
            return scheduler.Unregister(tree);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() {
            scheduler = new BehaviourTreeScheduler();
            driver = null;
        }

        private sealed class Driver : MonoBehaviour {
            private void Update() {
                scheduler.Tick();
            }
        }
    }
}
