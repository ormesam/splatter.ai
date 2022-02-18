namespace Splatter.AI {
    /// <summary>
    /// Composite node abort types.
    /// </summary>
    public enum AbortType {
        /// <summary>
        /// Default abort type, does not abort the composite..
        /// </summary>
        None,
        /// <summary>
        /// Abort if the condition returns false.
        /// </summary>
        Self,
        /// <summary>
        /// Abort lower nodes if the condition returns true.
        /// </summary>
        Lower,
        /// <summary>
        /// Abort if the condition returns false. 
        /// Abort lower nodes if the condition returns true.
        /// </summary>
        SelfAndLower,
    }
}
