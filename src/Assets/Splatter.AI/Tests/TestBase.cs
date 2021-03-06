using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class TestBase {
        protected static BehaviourTree Tree = new BehaviourTreeStub();

        protected static Node CreateSuccessNode() {
            return new SuccessNode(Tree);
        }

        protected static Node CreateFailureNode() {
            return new FailureNode(Tree);
        }

        protected static Node CreateRunningNode() {
            return new RunningNode(Tree);
        }

        protected static Node[] GetNodes() {
            return new[]{
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateRunningNode(),
            };
        }
    }
}