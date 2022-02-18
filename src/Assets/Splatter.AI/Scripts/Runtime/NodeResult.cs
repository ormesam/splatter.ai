namespace Splatter.AI {
    /// <summary>
    /// Node return types of the execute function.
    /// </summary>
    public enum NodeResult {
        /// <summary>
        /// The node did not meet the criteria to carry on.
        /// </summary>
        Running,
        /// <summary>
        /// The node failed.
        /// </summary>
        Failure,
        /// <summary>
        /// The node succeeded.
        /// </summary>
        Success
    }
}
