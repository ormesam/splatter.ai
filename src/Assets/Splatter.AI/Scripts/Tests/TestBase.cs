using NUnit.Framework;
using Splatter.AI;
using Splatter.Tests.Stubs;

namespace Splatter.Tests {
    public class TestBase {
        protected BehaviourTree Tree;

        [SetUp]
        public void Init() {
            // Runs before each test
            Tree = new BehaviourTreeStub();
        }

        protected Node CreateSuccessNode() {
            return new SuccessNode(Tree);
        }

        protected Node CreateFailureNode() {
            return new FailureNode(Tree);
        }

        protected Node CreateRunningNode() {
            return new RunningNode(Tree);
        }
    }
}