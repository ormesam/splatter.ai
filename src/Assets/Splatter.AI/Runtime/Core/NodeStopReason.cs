namespace Splatter.AI {
    /// <summary>
    /// How a node last stopped. <see cref="None"/> until the node stops for the first time.
    /// Unlike <see cref="Node.Result"/>, it is never reset by <see cref="Node.Stop"/>, so
    /// debugging tools can still see aborts and last results after the fact.
    /// </summary>
    public enum NodeStopReason {
        None,
        Success,
        Failure,
        Aborted,
    }
}
