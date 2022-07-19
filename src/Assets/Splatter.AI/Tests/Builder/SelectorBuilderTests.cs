using System;
using System.Linq;
using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class SelectorBuilderTests : BuilderTestBase {
        [Test]
        public void SelectorBuilder_NoChildren() {
            Assert.Throws<InvalidOperationException>(() =>
                CreateBuilder()
                    .Name("Test")
                .End());
        }

        [Test]
        public void SelectorBuilder_WithChildren() {
            CreateBuilder()
                .Succeed()
                .Fail()
                .Running()
            .End();

            Assert.AreEqual(1, ParentStub.Children.Count);
            Assert.AreEqual(3, (ParentStub.Children.Single() as Composite).Children.Count);
        }

        [Test]
        public void SelectorBuilder_Name() {
            CreateBuilder()
                .Name("Test")
                .Succeed()
            .End();

            Assert.AreEqual("Test", ParentStub.Children.Single().Name);
        }

        [Test]
        public void SelectorBuilder_NestedComposite() {
            CreateBuilder()
                .Succeed()
                .Selector()
                    .Name("Nested")
                    .Succeed()
                .End()
                .Running()
            .End();

            Assert.AreEqual(1, ParentStub.Children.Count);
            Assert.AreEqual(3, (ParentStub.Children.Single() as Composite).Children.Count);
            Assert.AreEqual(typeof(Selector), (ParentStub.Children.Single() as Composite).Children[1].GetType());
        }

        private SelectorBuilder<BehaviourTreeParentStub> CreateBuilder() {
            return new SelectorBuilder<BehaviourTreeParentStub>(ParentStub);
        }
    }
}
