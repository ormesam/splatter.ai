namespace Splatter.AI {
    /// <summary>
    /// Behaviour tree with a strongly typed context.
    /// </summary>
    public class ContextBehaviourTree<T> : BehaviourTree {
        public T Context { get; set; }
    }
}
