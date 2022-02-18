namespace Splatter.AI {
    /// <summary>
    /// Modes for parallel nodes.
    /// </summary>
    public enum ParallelMode {
        /// <summary>
        /// Returns <see cref="NodeResult.Success"/> as soon as a child succeeds.
        /// </summary>
        ExitOnSuccess,
        /// <summary>
        /// Returns <see cref="NodeResult.Failure"/> as soon as a child fails.
        /// </summary>
        ExitOnFailure,
        /// <summary>
        /// Returns a childs result if it is <see cref="NodeResult.Success"/> or <see cref="NodeResult.Failure"/>.
        /// </summary>
        ExitOnAnyCompletion,
        /// <summary>
        /// Returns <see cref="NodeResult.Success"/> if all children have finished.
        /// </summary>
        WaitForAll,
        /// <summary>
        /// Returns <see cref="NodeResult.Success"/> if all children have succeeded.
        /// </summary>
        WaitForAllSuccess,
    }
}
