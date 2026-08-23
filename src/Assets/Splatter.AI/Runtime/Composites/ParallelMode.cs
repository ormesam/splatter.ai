namespace Splatter.AI.Composites {
    /// <summary>
    /// Modes for parallel nodes.
    /// </summary>
    public enum ParallelMode {
        /// <summary>
        /// Returns <see cref="NodeResult.Success"/> as soon as a child succeeds,
        /// or <see cref="NodeResult.Failure"/> once all children have completed without succeeding.
        /// </summary>
        ExitOnAnySuccess,
        /// <summary>
        /// Returns <see cref="NodeResult.Failure"/> as soon as a child fails,
        /// or <see cref="NodeResult.Success"/> once all children have completed without failing.
        /// </summary>
        ExitOnAnyFailure,
        /// <summary>
        /// Returns a childs result if it is <see cref="NodeResult.Success"/> or <see cref="NodeResult.Failure"/>.
        /// </summary>
        ExitOnAnyCompletion,
        /// <summary>
        /// Returns <see cref="NodeResult.Success"/> if all children have finished.
        /// </summary>
        WaitForAllToComplete,
        /// <summary>
        /// Returns <see cref="NodeResult.Success"/> once all children have succeeded,
        /// or <see cref="NodeResult.Failure"/> as soon as a child fails.
        /// </summary>
        WaitForAllToSucceed,
    }
}
