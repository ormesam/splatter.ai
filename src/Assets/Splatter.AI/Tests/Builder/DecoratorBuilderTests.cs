using System;
using System.Linq;
using NUnit.Framework;
using Splatter.AI.Tests.Stubs;

namespace Splatter.AI.Tests {
    public class DecoratorBuilderTests : BuilderTestBase {
        [Test]
        public void DecoratorBuilder_NoChild() {
            Assert.Throws<InvalidOperationException>(() =>
                CreateBuilder()
                    .Name("Test")
                .End());
        }

        [Test]
        public void DecoratorBuilder_Child() {
            CreateBuilder()
                .Name("Test")
                .Succeed()
            .End();

            Assert.IsNotNull((ParentStub.Children.Single() as Decorator).Child);
        }

        [Test]
        public void DecoratorBuilder_MultipleChildren() {
            Assert.Throws<InvalidOperationException>(() =>
                CreateBuilder()
                    .Name("Test")
                    .Succeed()
                    .Succeed()
                .End());
        }

        private DecoratorBuilder<BehaviourTreeParentStub> CreateBuilder() {
            return new DecoratorBuilder<BehaviourTreeParentStub>(ParentStub, new RunningDecorator(new BehaviourTreeStub()));
        }
    }
}
