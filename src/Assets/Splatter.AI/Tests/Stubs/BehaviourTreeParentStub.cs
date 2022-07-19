using System.Collections.Generic;

namespace Splatter.AI.Tests.Stubs {
    public class BehaviourTreeParentStub : IBuilder {
        public BehaviourTree Tree => new BehaviourTreeStub();
        public IList<Node> Children = new List<Node>();
        public string Name { get; set; }

        public void AddNode(Node node) {
            Children.Add(node);
        }

        public void SetName(string name) {
            Name = name;
        }
    }
}
