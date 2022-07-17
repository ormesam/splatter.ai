using System;

namespace Splatter.AI {
    public abstract class BuilderBase {
        public BehaviourTree Tree;

        protected BuilderBase(BehaviourTree tree) {
            this.Tree = tree;
        }

        protected abstract void AddNode(Node node);

        /// <summary>
        /// Adds sequence to the behaviour tree.
        /// </summary>
        public void Sequence(Action<SequenceBuilder> configurator) {
            var builder = new SequenceBuilder(Tree);
            configurator(builder);
            AddNode(builder.Build());
        }

        /// <summary>
        /// Adds selector to the behaviour tree.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate</param>
        public void Selector(Action<SelectorBuilder> configurator) {
            var builder = new SelectorBuilder(Tree);
            configurator(builder);
            AddNode(builder.Build());
        }

        /// <summary>
        /// Adds parallel composite to the behaviour tree.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="mode">Parallel mode</param>
        public void Parallel(ParallelMode mode, Action<ParallelBuilder> configurator) {
            var builder = new ParallelBuilder(Tree, mode);
            configurator(builder);
            AddNode(builder.Build());
        }

        /// <summary>
        /// Add node to the behaviour tree.
        /// </summary>
        /// <param name="node">Node to add</param>
        public void Do(Node node) {
            AddNode(node);
        }

        /// <summary>
        /// Add condition to the behaviour tree. Returns <see cref="NodeResult.Success"/> if true, otherwise <see cref="NodeResult.Failure"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="condition">Condition to evaluate</param>
        public void Condition(string name, Func<bool> condition) {
            Do(name, () => condition() ? NodeResult.Success : NodeResult.Failure);
        }

        /// <summary>
        /// Invert the result of the child node.
        /// </summary>
        public void Invert() {
            AddNode(new InvertDecorator(Tree));
        }

        /// <summary>
        /// Add node to the behaviour tree.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="leaf">Function to evaluate</param>
        public void Do(string name, Func<NodeResult> leaf) {
            AddNode(new Leaf(name, Tree, leaf));
        }

        /// <summary>
        /// Set blackboard value.
        /// </summary>
        /// <param name="key">Blackboard key</param>
        /// <param name="value">Value</param>
        public void SetBlackboardValue(string key, object value) {
            AddNode(new SetBlackboardValueNode(Tree, key, value));
        }

        /// <summary>
        /// Wait x seconds before continuing.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="seconds">Seconds to wait</param>
        public void Wait(string name, float seconds) {
            AddNode(new WaitNode(name, Tree, seconds));
        }

        /// <summary>
        /// Wait for a random period of time before continuing.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="minSeconds">Minimum number of seconds to wait</param>
        /// <param name="maxSeconds">Maximum number of seconds to wait</param>
        public void Wait(string name, float minSeconds, float maxSeconds) {
            AddNode(new WaitNode(name, Tree, UnityEngine.Random.Range(minSeconds, maxSeconds)));
        }

        /// <summary>
        /// Wait until the condition is true to continue
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="condition">Condition to be evaluated</param>
        public void WaitUntil(string name, Func<bool> condition) {
            AddNode(new WaitUntilNode(name, Tree, condition));
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Success"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public void AlwaysSucceed(string name = "Always Succeed") {
            AddNode(new SuccessDecorator("Always Succeed", Tree));
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Running"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public void AlwaysRunning(string name = "Always Running") {
            AddNode(new RunningDecorator(name, Tree));
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Failure"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public void AlwaysFail(string name = "Always Fail") {
            AddNode(new FailureDecorator(name, Tree));
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Success"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public void Success(string name = "Always Success") {
            Do(name, () => NodeResult.Success);
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Running"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public void Running(string name = "Always Running") {
            Do(name, () => NodeResult.Running);
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Failure"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public void Failure(string name = "Always Failure") {
            Do(name, () => NodeResult.Failure);
        }
    }
}
