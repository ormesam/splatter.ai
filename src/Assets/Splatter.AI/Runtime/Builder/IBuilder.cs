namespace Splatter.AI {
    public interface IBuilder {
        BehaviourTree Tree { get; }
        void AddNode(Node node);
        void SetName(string name);
    }
}
