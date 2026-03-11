using System;
using System.Collections.Generic;

namespace ET
{
    public readonly struct BehaviorTreeBlackboardChange
    {
        public readonly string Key;
        public readonly object PreviousValue;
        public readonly object CurrentValue;
        public readonly bool IsRemoved;

        public BehaviorTreeBlackboardChange(string key, object previousValue, object currentValue, bool isRemoved)
        {
            this.Key = key;
            this.PreviousValue = previousValue;
            this.CurrentValue = currentValue;
            this.IsRemoved = isRemoved;
        }
    }

    [EnableClass]
    public sealed class BehaviorTreeBlackboard
    {
        [EnableClass]
        private sealed class ObserverHandle : IDisposable
        {
            private readonly BehaviorTreeBlackboard blackboard;
            private readonly string key;
            private readonly Action<BehaviorTreeBlackboardChange> callback;

            public ObserverHandle(BehaviorTreeBlackboard blackboard, string key, Action<BehaviorTreeBlackboardChange> callback)
            {
                this.blackboard = blackboard;
                this.key = key;
                this.callback = callback;
            }

            public void Dispose()
            {
                this.blackboard?.RemoveObserver(this.key, this.callback);
            }
        }

        private readonly Dictionary<string, object> values = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<Action<BehaviorTreeBlackboardChange>>> observers = new(StringComparer.OrdinalIgnoreCase);

        public BehaviorTreeBlackboard()
        {
        }

        public BehaviorTreeBlackboard(IEnumerable<BehaviorTreeBlackboardEntryDefinition> defaults)
        {
            this.ApplyDefaults(defaults);
        }

        public IEnumerable<KeyValuePair<string, object>> Values => this.values;

        public void ApplyDefaults(IEnumerable<BehaviorTreeBlackboardEntryDefinition> defaults)
        {
            if (defaults == null)
            {
                return;
            }

            foreach (BehaviorTreeBlackboardEntryDefinition entry in defaults)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Key))
                {
                    continue;
                }

                this.values[entry.Key] = BehaviorTreeValueUtility.GetValue(entry.DefaultValue);
            }
        }

        public bool Contains(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && this.values.ContainsKey(key);
        }

        public object GetBoxed(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            this.values.TryGetValue(key, out object value);
            return value;
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            object value = this.GetBoxed(key);
            if (value == null)
            {
                return defaultValue;
            }

            if (value is T matched)
            {
                return matched;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public void Set<T>(string key, T value)
        {
            this.SetBoxed(key, value);
        }

        public void SetBoxed(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            this.values.TryGetValue(key, out object previousValue);
            if (Equals(previousValue, value))
            {
                return;
            }

            this.values[key] = value;
            this.Notify(new BehaviorTreeBlackboardChange(key, previousValue, value, false));
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (!this.values.Remove(key, out object previousValue))
            {
                return false;
            }

            this.Notify(new BehaviorTreeBlackboardChange(key, previousValue, null, true));
            return true;
        }

        public IDisposable Observe(string key, Action<BehaviorTreeBlackboardChange> callback, bool fireImmediately = false)
        {
            if (string.IsNullOrWhiteSpace(key) || callback == null)
            {
                return null;
            }

            if (!this.observers.TryGetValue(key, out List<Action<BehaviorTreeBlackboardChange>> callbacks))
            {
                callbacks = new List<Action<BehaviorTreeBlackboardChange>>();
                this.observers.Add(key, callbacks);
            }

            callbacks.Add(callback);

            if (fireImmediately)
            {
                callback.Invoke(new BehaviorTreeBlackboardChange(key, null, this.GetBoxed(key), false));
            }

            return new ObserverHandle(this, key, callback);
        }

        private void RemoveObserver(string key, Action<BehaviorTreeBlackboardChange> callback)
        {
            if (string.IsNullOrWhiteSpace(key) || callback == null)
            {
                return;
            }

            if (!this.observers.TryGetValue(key, out List<Action<BehaviorTreeBlackboardChange>> callbacks))
            {
                return;
            }

            callbacks.Remove(callback);
            if (callbacks.Count == 0)
            {
                this.observers.Remove(key);
            }
        }

        private void Notify(BehaviorTreeBlackboardChange change)
        {
            if (!this.observers.TryGetValue(change.Key, out List<Action<BehaviorTreeBlackboardChange>> callbacks) || callbacks.Count == 0)
            {
                return;
            }

            Action<BehaviorTreeBlackboardChange>[] copiedCallbacks = callbacks.ToArray();
            foreach (Action<BehaviorTreeBlackboardChange> callback in copiedCallbacks)
            {
                callback?.Invoke(change);
            }
        }
    }
}
