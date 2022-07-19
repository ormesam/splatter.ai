using System;
using System.Linq;
using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class ParallelBuilderTests : BuilderTestBase {
        [Test]
        public void ParallelBuilder_NoChildren() {
            Assert.Throws<InvalidOperationException>(() =>
                CreateBuilder()
                    .Name("Test")
                .End());
        }

        [Test]
        public void ParallelBuilder_WithChildren() {
            CreateBuilder()
                .Succeed()
                .Fail()
                .Running()
            .End();

            Assert.AreEqual(1, ParentStub.Children.Count);
            Assert.AreEqual(3, (ParentStub.Children.Single() as Composite).Children.Count);
        }

        [Test]
        public void ParallelBuilder_Name() {
            CreateBuilder()
                .Name("Test")
                .Succeed()
            .End();

            Assert.AreEqual("Test", ParentStub.Children.Single().Name);
        }

        [Test]
        public void ParallelBuilder_NestedComposite() {
            CreateBuilder()
                .Succeed()
                .Parallel(ParallelMode.WaitForAllToSucceed)
                    .Name("Nested")
                    .Succeed()
                .End()
                .Running()
            .End();

            Assert.AreEqual(1, ParentStub.Children.Count);
            Assert.AreEqual(3, (ParentStub.Children.Single() as Composite).Children.Count);
            Assert.AreEqual(typeof(Parallel), (ParentStub.Children.Single() as Composite).Children[1].GetType());
        }

        private ParallelBuilder<BehaviourTreeParentStub> CreateBuilder() {
            return new ParallelBuilder<BehaviourTreeParentStub>(ParentStub, ParallelMode.WaitForAllToSucceed);
        }
    }
}
