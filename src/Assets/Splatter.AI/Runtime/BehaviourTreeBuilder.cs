using System;
using System.Collections.Generic;
using System.Linq;

namespace Splatter.AI {
    /// <summary>
    /// Helper for creating behaviour tree
    /// </summary>
    public class BehaviourTreeBuilder {
        private Node currentNode;
        private Stack<Node> stack;

        public BehaviourTree Tree { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviourTreeBuilder"/> class.
        /// </summary>
        public BehaviourTreeBuilder(BehaviourTree tree) {
            this.Tree = tree;
            stack = new Stack<Node>();
        }

        /// <summary>
        /// Adds sequence to the behaviour tree. Add <c>.End()</c> to the end of the sequence.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate</param>
        /// <param name="resetIfInterrupted">Reset sequence if interrupted</param>
        public BehaviourTreeBuilder Sequence(string name = "Sequence") {
            AddNode(new Sequencer(name, Tree));

            return this;
        }

        /// <summary>
        /// Adds selector to the behaviour tree. Add <c>.End()</c> to the end of the selector.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate</param>
        public BehaviourTreeBuilder Selector(string name = "Selector") {
            AddNode(new Selector(name, Tree));

            return this;
        }

        /// <summary>
        /// Adds parallel composite to the behaviour tree. Add <c>.End()</c> to the end of the parallel.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="mode">Parallel mode</param>
        public BehaviourTreeBuilder Parallel(string name = "Parallel", ParallelMode mode = ParallelMode.WaitForAllToSucceed) {
            AddNode(new Parallel(name, Tree, mode));

            return this;
        }

        /// <summary>
        /// Makes current composite node abortable
        /// </summary>
        /// <param name="abortType">Abort type</param>
        /// <param name="condition">Condition to evaluate</param>
        public BehaviourTreeBuilder Abortable(AbortType abortType, Func<bool> condition) {
            var currentNode = stack.Peek();

            if (currentNode is Composite composite) {
                composite.SetAbortType(abortType, condition);
            } else {
                throw new InvalidOperationException("Current node is not abortable");
            }

            return this;
        }

        /// <summary>
        /// Makes current sequence node abortable
        /// </summary>
        public BehaviourTreeBuilder ResetIfInterrupted() {
            var currentNode = stack.Peek();

            if (currentNode is Sequencer composite) {
                composite.ResetIfInterrupted();
            } else {
                throw new InvalidOperationException("Current node is not a sequencer");
            }

            return this;
        }

        /// <summary>
        /// Add node to the behaviour tree.
        /// </summary>
        /// <param name="node">Node to add</param>
        public BehaviourTreeBuilder Do(Node node) {
            AddNode(node);

            return this;
        }

        /// <summary>
        /// Add condition to the behaviour tree. Returns <see cref="NodeResult.Success"/> if true, otherwise <see cref="NodeResult.Failure"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="condition">Condition to evaluate</param>
        public BehaviourTreeBuilder Condition(string name, Func<bool> condition) {
            Do(name, () => condition() ? NodeResult.Success : NodeResult.Failure);

            return this;
        }

        /// <summary>
        /// Invert the result of the child node.
        /// </summary>
        public BehaviourTreeBuilder Invert(){
            AddNode(new InvertDecorator(Tree));

            return this;
        }

        /// <summary>
        /// Add node to the behaviour tree.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="leaf">Function to evaluate</param>
        public BehaviourTreeBuilder Do(string name, Func<NodeResult> leaf) {
            AddNode(new Leaf(name, Tree, leaf));

            return this;
        }

        /// <summary>
        /// Set blackboard value.
        /// </summary>
        /// <param name="key">Blackboard key</param>
        /// <param name="value">Value</param>
        public BehaviourTreeBuilder SetBlackboardValue(string key, object value) {
            AddNode(new SetBlackboardValueNode(Tree, key, value));

            return this;
        }

        /// <summary>
        /// Wait x seconds before continuing.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="seconds">Seconds to wait</param>
        public BehaviourTreeBuilder Wait(string name, float seconds) {
            AddNode(new WaitNode(name, Tree, seconds));

            return this;
        }

        /// <summary>
        /// Wait for a random period of time before continuing.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="minSeconds">Minimum number of seconds to wait</param>
        /// <param name="maxSeconds">Maximum number of seconds to wait</param>
        public BehaviourTreeBuilder Wait(string name, float minSeconds, float maxSeconds) {
            AddNode(new WaitNode(name, Tree, UnityEngine.Random.Range(minSeconds, maxSeconds)));

            return this;
        }

        /// <summary>
        /// Wait until the condition is true to continue
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="condition">Condition to be evaluated</param>
        public BehaviourTreeBuilder WaitUntil(string name, Func<bool> condition) {
            AddNode(new WaitUntilNode(name, Tree, condition));

            return this;
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Success"/>. Add <c>.End()</c> to the end of the decorator.
        /// </summary>
        /// <param name="name">Node name</param>
        public BehaviourTreeBuilder AlwaysSucceed(string name = "Always Succeed") {
            AddNode(new SuccessDecorator("Always Succeed", Tree));

            return this;
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Running"/>. Add <c>.End()</c> to the end of the decorator.
        /// </summary>
        /// <param name="name">Node name</param>
        public BehaviourTreeBuilder AlwaysRunning(string name = "Always Running") {
            AddNode(new RunningDecorator(name, Tree));

            return this;
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Failure"/>. Add <c>.End()</c> to the end of the decorator.
        /// </summary>
        /// <param name="name">Node name</param>
        public BehaviourTreeBuilder AlwaysFail(string name = "Always Fail") {
            AddNode(new FailureDecorator(name, Tree));

            return this;
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Success"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public BehaviourTreeBuilder Success(string name = "Always Success") {
            Do(name, () => NodeResult.Success);

            return this;
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Running"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public BehaviourTreeBuilder Running(string name = "Always Running") {
            Do(name, () => NodeResult.Running);

            return this;
        }

        /// <summary>
        /// Always returns <see cref="NodeResult.Failure"/>.
        /// </summary>
        /// <param name="name">Node name</param>
        public BehaviourTreeBuilder Failure(string name = "Always Failure") {
            Do(name, () => NodeResult.Failure);

            return this;
        }

        private void AddNode(Composite node) {
            AddNode((Node)node);

            stack.Push(node);
        }

        private void AddNode(Decorator node) {
            AddNode((Node)node);

            stack.Push(node);
        }

        private void AddNode(Node node) {
            if (stack.Any()) {
                var currentNode = stack.Peek();

                if (currentNode is Composite compositeNode) {
                    compositeNode.Children.Add(node);
                }

                if (currentNode is Decorator decoratorNode) {
                    if (decoratorNode.Child != null) {
                        throw new InvalidOperationException("Cannot set a decorator nodes child multiple times.");
                    }

                    decoratorNode.Child = node;
                }
            }
        }

        /// <summary>
        /// Add to the end of a composites and decorators.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public BehaviourTreeBuilder End() {
            currentNode = stack.Pop();

            if (currentNode is Composite compositeNode) {
                if (!compositeNode.Children.Any()) {
                    throw new InvalidOperationException("Composite node does not have any children.");
                }
            }

            if (currentNode is Decorator decoratorNode) {
                if (decoratorNode.Child == null) {
                    throw new InvalidOperationException("Decorator node does not have child set.");
                }
            }

            return this;
        }

        /// <summary>
        /// Builds node for behaviour tree.
        /// </summary>
        /// <returns>Built node</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Node Build() {
            if (currentNode == null) {
                throw new InvalidOperationException("Can't create a behaviour tree with zero nodes");
            }

            return currentNode;
        }
    }
}
