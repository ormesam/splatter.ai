using System.Collections.Generic;
using NUnit.Framework;
using Splatter.AI.Leaves;

namespace Splatter.AI.Tests {
    public class BlackboardTests : TestBase {
        [Test]
        public void Blackboard_SetNewKey_Notifies() {
            var blackboard = new Blackboard();
            int notifications = 0;
            blackboard.Subscribe("key", () => notifications++);

            blackboard["key"] = 1;

            Assert.AreEqual(1, notifications);
        }

        [Test]
        public void Blackboard_SetSameValue_DoesNotNotify() {
            var blackboard = new Blackboard();
            blackboard["number"] = 42;
            blackboard["text"] = "value";

            int notifications = 0;
            blackboard.Subscribe("number", () => notifications++);
            blackboard.Subscribe("text", () => notifications++);

            blackboard["number"] = 42;
            blackboard["text"] = "value";

            Assert.AreEqual(0, notifications);
        }

        [Test]
        public void Blackboard_SetDifferentValue_Notifies() {
            var blackboard = new Blackboard();
            blackboard["key"] = 1;

            int notifications = 0;
            blackboard.Subscribe("key", () => notifications++);

            blackboard["key"] = 2;

            Assert.AreEqual(1, notifications);
        }

        [Test]
        public void Blackboard_SetNullOverNull_DoesNotNotify() {
            var blackboard = new Blackboard();
            blackboard["key"] = null;

            int notifications = 0;
            blackboard.Subscribe("key", () => notifications++);

            blackboard["key"] = null;

            Assert.AreEqual(0, notifications);
        }

        [Test]
        public void Blackboard_SetNullOnMissingKey_Notifies() {
            var blackboard = new Blackboard();
            int notifications = 0;
            blackboard.Subscribe("key", () => notifications++);

            blackboard["key"] = null;

            Assert.AreEqual(1, notifications);
        }

        [Test]
        public void Blackboard_RemoveExistingKey_Notifies() {
            var blackboard = new Blackboard();
            blackboard["key"] = 1;

            int notifications = 0;
            blackboard.Subscribe("key", () => notifications++);

            blackboard.Remove("key");

            Assert.AreEqual(1, notifications);
        }

        [Test]
        public void Blackboard_RemoveMissingKey_DoesNotNotify() {
            var blackboard = new Blackboard();
            int notifications = 0;
            blackboard.Subscribe("key", () => notifications++);

            blackboard.Remove("key");

            Assert.AreEqual(0, notifications);
        }

        [Test]
        public void Blackboard_Add_Notifies() {
            var blackboard = new Blackboard();
            int notifications = 0;
            blackboard.Subscribe("key", () => notifications++);

            blackboard.Add("key", 1);

            Assert.AreEqual(1, notifications);
        }

        [Test]
        public void Blackboard_Clear_NotifiesEachKey() {
            var blackboard = new Blackboard();
            blackboard["a"] = 1;
            blackboard["b"] = 2;

            int notifications = 0;
            blackboard.Subscribe("a", () => notifications++);
            blackboard.Subscribe("b", () => notifications++);

            blackboard.Clear();

            Assert.AreEqual(2, notifications);
            Assert.AreEqual(0, blackboard.Count);
        }

        [Test]
        public void Blackboard_OtherKeyChange_DoesNotNotify() {
            var blackboard = new Blackboard();
            int notifications = 0;
            blackboard.Subscribe("key", () => notifications++);

            blackboard["other"] = 1;

            Assert.AreEqual(0, notifications);
        }

        [Test]
        public void Blackboard_Unsubscribe_StopsNotifications() {
            var blackboard = new Blackboard();
            int notifications = 0;
            void Observer() => notifications++;

            blackboard.Subscribe("key", Observer);
            blackboard["key"] = 1;

            blackboard.Unsubscribe("key", Observer);
            blackboard["key"] = 2;

            Assert.AreEqual(1, notifications);
        }

        [Test]
        public void Blackboard_TwoSubscribers_BothNotified() {
            var blackboard = new Blackboard();
            int first = 0;
            int second = 0;
            blackboard.Subscribe("key", () => first++);
            blackboard.Subscribe("key", () => second++);

            blackboard["key"] = 1;

            Assert.AreEqual(1, first);
            Assert.AreEqual(1, second);
        }

        [Test]
        public void Blackboard_UnsubscribeDuringNotification_DoesNotThrow() {
            var blackboard = new Blackboard();
            int notifications = 0;
            void Observer() {
                notifications++;
                blackboard.Unsubscribe("key", Observer);
            }

            blackboard.Subscribe("key", Observer);

            blackboard["key"] = 1;
            blackboard["key"] = 2;

            Assert.AreEqual(1, notifications);
        }

        [Test]
        public void Blackboard_WriteThroughDictionaryInterface_Notifies() {
            var blackboard = new Blackboard();
            int notifications = 0;
            blackboard.Subscribe("key", () => notifications++);

            var node = new SetBlackboardValueNode(blackboard, "key", 1);
            node.OnUpdate();

            Assert.AreEqual(1, notifications);

            IDictionary<string, object> asInterface = blackboard;
            asInterface["key"] = 2;

            Assert.AreEqual(2, notifications);
        }
    }
}
