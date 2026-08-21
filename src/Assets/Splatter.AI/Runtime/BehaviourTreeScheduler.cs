using System;
using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// Ticks registered behaviour trees from a single pump. Call <see cref="Tick"/> once per
    /// frame; each tree is ticked every <c>interval</c> pumps, and trees on the same interval
    /// are staggered so they do not all tick on the same pump.
    /// </summary>
    public class BehaviourTreeScheduler {
        private class Entry {
            public BehaviourTree Tree;
            public int Interval;
            public long NextTickAt;
            public bool IsRegistered = true;
        }

        private readonly List<Entry> entries = new List<Entry>();
        private int staggerIndex;
        private long tickCount;
        private bool isTicking;
        private bool hasPendingRemovals;

        /// <summary>
        /// Number of registered trees.
        /// </summary>
        public int Count => entries.Count;

        /// <summary>
        /// Registers a tree to be ticked by <see cref="Tick"/>. Registering an already registered
        /// tree updates its interval.
        /// </summary>
        /// <param name="tree">Tree to tick</param>
        /// <param name="interval">Tick the tree every this many pumps; 1 ticks on every pump</param>
        public void Register(BehaviourTree tree, int interval = 1) {
            if (tree == null) {
                throw new ArgumentNullException(nameof(tree));
            }

            if (interval < 1) {
                throw new ArgumentOutOfRangeException(nameof(interval), interval,
                    "Interval must be >= 1. Use 1 to tick on every pump.");
            }

            Unregister(tree);

            // Offset each tree's first tick within its interval so trees on the same interval
            // spread evenly across pumps instead of all ticking on the same one.
            int offset = staggerIndex++ % interval;

            entries.Add(new Entry {
                Tree = tree,
                Interval = interval,
                NextTickAt = tickCount + 1 + offset,
            });
        }

        /// <summary>
        /// Unregisters a tree so it is no longer ticked. Safe to call for trees that are not
        /// registered, and safe to call from inside a ticking tree.
        /// </summary>
        /// <param name="tree">Tree to unregister</param>
        /// <returns>Whether the tree was registered.</returns>
        public bool Unregister(BehaviourTree tree) {
            int index = entries.FindIndex(e => e.IsRegistered && e.Tree == tree);

            if (index < 0) {
                return false;
            }

            if (isTicking) {
                entries[index].IsRegistered = false;
                hasPendingRemovals = true;
            } else {
                entries.RemoveAt(index);
            }

            return true;
        }

        /// <summary>
        /// Ticks every registered tree that is due this pump. Trees registered or unregistered
        /// during the pump take effect from the next pump.
        /// </summary>
        public void Tick() {
            isTicking = true;
            tickCount++;

            try {
                int count = entries.Count;

                for (int i = 0; i < count; i++) {
                    var entry = entries[i];

                    if (!entry.IsRegistered || tickCount < entry.NextTickAt) {
                        continue;
                    }

                    entry.NextTickAt += entry.Interval;
                    entry.Tree.Tick();
                }
            } finally {
                isTicking = false;

                if (hasPendingRemovals) {
                    entries.RemoveAll(e => !e.IsRegistered);
                    hasPendingRemovals = false;
                }
            }
        }
    }
}
