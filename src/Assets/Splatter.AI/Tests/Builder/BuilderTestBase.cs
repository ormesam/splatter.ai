using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class BuilderTestBase {
        protected BehaviourTreeParentStub ParentStub;

        [SetUp]
        public void Setup() {
            ParentStub = new BehaviourTreeParentStub();
        }
    }
}
