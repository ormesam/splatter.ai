using System;
using System.Collections;
using System.Collections.Generic;

namespace Splatter.AI {
    /// <summary>
    /// Dictionary of behaviour tree variables that notifies per-key observers, but only when a
    /// value actually changes (per <see cref="object.Equals(object, object)"/>). Adding and
    /// removing a key count as changes; re-setting a key to an equal value does not notify.
    /// </summary>
    public class Blackboard : IDictionary<string, object> {
        private readonly IDictionary<string, object> items = new Dictionary<string, object>();
        private readonly IDictionary<string, Action> observers = new Dictionary<string, Action>();

        public object this[string key] {
            get => items[key];
            set {
                if (items.TryGetValue(key, out var existing) && Equals(existing, value)) {
                    return;
                }

                items[key] = value;
                Notify(key);
            }
        }

        public ICollection<string> Keys => items.Keys;

        public ICollection<object> Values => items.Values;

        public int Count => items.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// Registers an observer invoked whenever the value of <paramref name="key"/> changes,
        /// including the key being added or removed.
        /// </summary>
        /// <param name="key">Blackboard key to observe</param>
        /// <param name="observer">Callback invoked on change</param>
        public void Subscribe(string key, Action observer) {
            observers.TryGetValue(key, out var existing);
            observers[key] = existing + observer;
        }

        /// <summary>
        /// Removes a previously subscribed observer. Does nothing if it was not subscribed.
        /// </summary>
        /// <param name="key">Observed blackboard key</param>
        /// <param name="observer">Callback to remove</param>
        public void Unsubscribe(string key, Action observer) {
            if (!observers.TryGetValue(key, out var existing)) {
                return;
            }

            existing -= observer;

            if (existing == null) {
                observers.Remove(key);
            } else {
                observers[key] = existing;
            }
        }

        public void Add(string key, object value) {
            items.Add(key, value);
            Notify(key);
        }

        public bool Remove(string key) {
            if (!items.Remove(key)) {
                return false;
            }

            Notify(key);

            return true;
        }

        public void Clear() {
            if (items.Count == 0) {
                return;
            }

            var keys = new string[items.Count];
            items.Keys.CopyTo(keys, 0);
            items.Clear();

            foreach (var key in keys) {
                Notify(key);
            }
        }

        public bool ContainsKey(string key) => items.ContainsKey(key);

        public bool TryGetValue(string key, out object value) => items.TryGetValue(key, out value);

        public void Add(KeyValuePair<string, object> item) => Add(item.Key, item.Value);

        public bool Contains(KeyValuePair<string, object> item) => items.Contains(item);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, object> item) {
            if (!items.Contains(item)) {
                return false;
            }

            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Notify(string key) {
            // Multicast invocation iterates a snapshot, so observers may safely
            // (un)subscribe or write other keys while being notified.
            if (observers.TryGetValue(key, out var action)) {
                action?.Invoke();
            }
        }
    }
}
