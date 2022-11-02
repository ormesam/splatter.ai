namespace Splatter.AI {
    /// <summary>
    /// Base class for behaviour trees with a stringly typed context.</para>
    /// Override <see cref="Start"/> to initialise blackboard values. Override <see cref="CreateRoot"/> to create the tree root.
    /// </summary>
    public abstract class ContextBehaviourTree<T> : BehaviourTree {
        public T Context { get; set; }
    }
}
