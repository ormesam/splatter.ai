namespace Splatter.AI.Tests.Stubs {
    public class BehaviourTreeStub : BehaviourTree {
        protected override Node CreateRoot() {
            return new Leaf("Leaf", this, () => NodeResult.Failure);
        }
    }
}
