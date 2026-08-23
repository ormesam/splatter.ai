using System.Collections.Generic;
using NUnit.Framework;
using Splatter.AI.Leaves;

namespace Splatter.AI.Tests {
    public class ConditionNodeTests : TestBase {
        [Test]
        public void Condition_True_ReturnsSuccess() {
            var condition = new ConditionNode("Condition", () => true);

            Assert.AreEqual(NodeResult.Success, condition.OnUpdate());
        }

        [Test]
        public void Condition_False_ReturnsFailure() {
            var condition = new ConditionNode("Condition", () => false);

            Assert.AreEqual(NodeResult.Failure, condition.OnUpdate());
        }

        [Test]
        public void Condition_ReEvaluatedEachUpdate() {
            var value = false;
            var condition = new ConditionNode("Condition", () => value);

            Assert.AreEqual(NodeResult.Failure, condition.OnUpdate());

            value = true;

            Assert.AreEqual(NodeResult.Success, condition.OnUpdate());
        }

        [Test]
        public void BlackboardCondition_KeySet_ReturnsSuccess() {
            var blackboard = new Dictionary<string, object> { ["Target"] = "Player" };

            var condition = new BlackboardConditionNode(blackboard, "Target");

            Assert.AreEqual(NodeResult.Success, condition.OnUpdate());
        }

        [Test]
        public void BlackboardCondition_KeyMissing_ReturnsFailure() {
            var blackboard = new Dictionary<string, object>();

            var condition = new BlackboardConditionNode(blackboard, "Target");

            Assert.AreEqual(NodeResult.Failure, condition.OnUpdate());
        }

        [Test]
        public void BlackboardCondition_ValueMatches_ReturnsSuccess() {
            var blackboard = new Dictionary<string, object> { ["Health"] = 100 };

            var condition = new BlackboardConditionNode(blackboard, "Health", 100);

            Assert.AreEqual(NodeResult.Success, condition.OnUpdate());
        }

        [Test]
        public void BlackboardCondition_ValueMismatch_ReturnsFailure() {
            var blackboard = new Dictionary<string, object> { ["Health"] = 100 };

            var condition = new BlackboardConditionNode(blackboard, "Health", 50);

            Assert.AreEqual(NodeResult.Failure, condition.OnUpdate());
        }

        [Test]
        public void BlackboardCondition_KeyMissing_WithExpectedValue_ReturnsFailure() {
            var blackboard = new Dictionary<string, object>();

            var condition = new BlackboardConditionNode(blackboard, "Health", 100);

            Assert.AreEqual(NodeResult.Failure, condition.OnUpdate());
        }

        [Test]
        public void BlackboardCondition_NullValueMatchesNull() {
            var blackboard = new Dictionary<string, object> { ["Target"] = null };

            var condition = new BlackboardConditionNode(blackboard, "Target", null);

            Assert.AreEqual(NodeResult.Success, condition.OnUpdate());
        }
    }
}
