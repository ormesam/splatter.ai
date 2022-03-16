namespace Splatter.AI {
    /// <summary>
    /// Modes for parallel nodes.
    /// </summary>
    public enum ParallelMode {
        /// <summary>
        /// Returns <see cref="NodeResult.Success"/> as soon as a child succeeds.
        /// </summary>
        ExitOnAnySuccess,
        /// <summary>
        /// Returns <see cref="NodeResult.Failure"/> as soon as a child fails.
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
        /// Returns <see cref="NodeResult.Success"/> if all children have succeeded.
        /// </summary>
        WaitForAllToSucceed,
    }
}
