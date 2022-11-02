using System;

namespace Splatter.AI {
    public static class BuilderExtensions {
        /// <summary>
        /// Adds sequence composite to the behaviour tree.
        /// </summary>
        public static SequenceBuilder<T> Sequence<T>(this T builder) where T : IBuilder {
            return new SequenceBuilder<T>(builder);
        }

        /// <summary>
        /// Adds selector composite to the behaviour tree.
        /// </summary>
        public static SelectorBuilder<T> Selector<T>(this T builder) where T : IBuilder {
            return new SelectorBuilder<T>(builder);
        }

        /// <summary>
        /// Adds parallel composite to the behaviour tree.
        /// </summary>
        public static ParallelBuilder<T> Parallel<T>(this T builder, ParallelMode mode) where T : IBuilder {
            return new ParallelBuilder<T>(builder, mode);
        }

        /// <summary>
        /// Add node to the behaviour tree.
        /// </summary>
        /// <param name="node">Node to add</param>
        public static T Do<T>(this T builder, Node node) where T : IBuilder {
            builder.AddNode(node);

            return builder;
        }

        /// <summary>
        /// Add node to the behaviour tree.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="leaf">Function to execute</param>
        public static T Do<T>(this T builder, string name, Func<NodeResult> leaf) where T : IBuilder {
            builder.AddNode(new Leaf(name, builder.Tree, leaf));

            return builder;
        }

        /// <summary>
        /// Add condition to the behaviour tree. Returns <see cref="NodeResult.Success"/> if true, otherwise <see cref="NodeResult.Failure"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="condition">Condition to evaluate</param>
        public static T Condition<T>(this T builder, string name, Func<bool> condition) where T : IBuilder {
            return builder.Do(name, () => condition() ? NodeResult.Success : NodeResult.Failure);
        }

        /// <summary>
        /// Invert the result of the child node.
        /// </summary>
        public static DecoratorBuilder<T> Invert<T>(this T builder) where T : IBuilder {
            return new DecoratorBuilder<T>(builder, new InvertDecorator(builder.Tree));
        }

        /// <summary>
        /// Set blackboard value.
        /// </summary>
        /// <param name="key">Blackboard key</param>
        /// <param name="value">Value</param>
        public static T SetBlackboardValue<T>(this T builder, string key, object value) where T : IBuilder {
            builder.AddNode(new SetBlackboardValueNode(builder.Tree, key, value));

            return builder;
        }

        /// <summary>
        /// Wait x seconds before continuing.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="seconds">Seconds to wait</param>
        public static T Wait<T>(this T builder, string name, float seconds) where T : IBuilder {
            builder.AddNode(new WaitNode(name, builder.Tree, seconds, seconds));

            return builder;
        }

        /// <summary>
        /// Wait for a random period of time before continuing.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="minSeconds">Minimum number of seconds to wait</param>
        /// <param name="maxSeconds">Maximum number of seconds to wait</param>
        public static T Wait<T>(this T builder, string name, float minSeconds, float maxSeconds) where T : IBuilder {
            builder.AddNode(new WaitNode(name, builder.Tree, minSeconds, maxSeconds));

            return builder;
        }

        /// <summary>
        /// Wait until the condition is true to continue.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="condition">Condition to be evaluated</param>
        public static T WaitUntil<T>(this T builder, string name, Func<bool> condition) where T : IBuilder {
            builder.AddNode(new WaitUntilNode(name, builder.Tree, condition));

            return builder;
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Success"/>.
        /// </summary>
        public static DecoratorBuilder<T> AlwaysSucceed<T>(this T builder) where T : IBuilder {
            return new DecoratorBuilder<T>(builder, new SuccessDecorator(builder.Tree));
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Running"/>.
        /// </summary>
        public static DecoratorBuilder<T> AlwaysRunning<T>(this T builder) where T : IBuilder {
            return new DecoratorBuilder<T>(builder, new RunningDecorator(builder.Tree));
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Failure"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public static DecoratorBuilder<T> AlwaysFail<T>(this T builder) where T : IBuilder {
            return new DecoratorBuilder<T>(builder, new FailureDecorator(builder.Tree));
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Success"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public static T Succeed<T>(this T builder, string name = "Succeed") where T : IBuilder {
            return builder.Do(name, () => NodeResult.Success);
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Running"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public static T Running<T>(this T builder, string name = "Running") where T : IBuilder {
            return builder.Do(name, () => NodeResult.Running);
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Failure"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public static T Fail<T>(this T builder, string name = "Fail") where T : IBuilder {
            return builder.Do(name, () => NodeResult.Failure);
        }

        /// <summary>
        /// Set the node name.
        /// </summary>
        /// <param name="name">Node name</param>
        public static T Name<T>(this T builder, string name) where T : IBuilder {
            builder.SetName(name);

            return builder;
        }
    }
}
