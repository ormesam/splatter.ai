using System;
using System.Linq;
using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class SequenceBuilderTests : BuilderTestBase {
        [Test]
        public void SequenceBuilder_NoChildren() {
            Assert.Throws<InvalidOperationException>(() =>
                CreateBuilder()
                    .Name("Test")
                .End());
        }

        [Test]
        public void SequenceBuilder_WithChildren() {
            CreateBuilder()
                .Succeed()
                .Fail()
                .Running()
            .End();

            Assert.AreEqual(1, ParentStub.Children.Count);
            Assert.AreEqual(3, (ParentStub.Children.Single() as Composite).Children.Count);
        }

        [Test]
        public void SequenceBuilder_Name() {
            CreateBuilder()
                .Name("Test")
                .Succeed()
            .End();

            Assert.AreEqual("Test", ParentStub.Children.Single().Name);
        }

        [Test]
        public void SequenceBuilder_NestedComposite() {
            CreateBuilder()
                .Succeed()
                .Sequence()
                    .Name("Nested")
                    .Succeed()
                .End()
                .Running()
            .End();

            Assert.AreEqual(1, ParentStub.Children.Count);
            Assert.AreEqual(3, (ParentStub.Children.Single() as Composite).Children.Count);
            Assert.AreEqual(typeof(Sequencer), (ParentStub.Children.Single() as Composite).Children[1].GetType());
        }

        private SequenceBuilder<BehaviourTreeParentStub> CreateBuilder() {
            return new SequenceBuilder<BehaviourTreeParentStub>(ParentStub);
        }
    }
}
