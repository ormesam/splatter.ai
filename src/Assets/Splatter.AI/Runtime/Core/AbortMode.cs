using Splatter.AI.Composites;
using Splatter.AI.Decorators;

namespace Splatter.AI {
    /// <summary>
    /// How an <see cref="ObservingDecorator"/> reacts when an observed value changes.
    /// Aborts are applied on the next tick, never at notification time, so changes made
    /// mid-tick (and changes that revert before the next tick) cannot tear down nodes
    /// that are currently executing.
    /// </summary>
    public enum AbortMode {
        /// <summary>
        /// Never aborts. The condition gates entry only, evaluated when the decorator starts.
        /// </summary>
        None,
        /// <summary>
        /// While the child is running, a change that makes the condition false stops the
        /// child on the next tick and the decorator returns <see cref="NodeResult.Failure"/>.
        /// </summary>
        Self,
        /// <summary>
        /// While the branch is not running, a change that would flip the branch's last result
        /// makes the parent composite stop lower-priority children and re-evaluate from this
        /// branch on its next tick: under a <see cref="Selector"/> a failed branch reactivates
        /// when its condition becomes true; under a <see cref="Sequencer"/> a passed condition
        /// becoming false rewinds and fails the sequence. The decorator must be a direct child
        /// of a <see cref="Selector"/> or <see cref="Sequencer"/>. To fail a whole sequence
        /// when a guard breaks mid-run, wrap the sequence in a <see cref="Self"/> observer
        /// instead.
        /// </summary>
        LowerPriority,
        /// <summary>
        /// Both <see cref="Self"/> and <see cref="LowerPriority"/>.
        /// </summary>
        Both,
    }
}
