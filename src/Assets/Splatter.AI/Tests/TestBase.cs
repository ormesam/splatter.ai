using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class TestBase {
        protected static Node CreateSuccessNode() {
            return new SuccessNode();
        }

        protected static Node CreateFailureNode() {
            return new FailureNode();
        }

        protected static Node CreateRunningNode() {
            return new RunningNode();
        }

        protected static Node[] GetNodes() {
            return new[]{
                CreateSuccessNode(),
                CreateFailureNode(),
                CreateRunningNode(),
            };
        }

        protected static Node[] GetCompletedNodes() {
            return new[]{
                CreateSuccessNode(),
                CreateFailureNode(),
            };
        }
    }
}
