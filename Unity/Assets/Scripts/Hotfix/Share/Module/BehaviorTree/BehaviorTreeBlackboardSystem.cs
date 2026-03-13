using System;
using System.Collections.Generic;

namespace ET
{
    public static class BehaviorTreeBlackboardSystem
    {
        public static void ApplyDefaults(this BehaviorTreeBlackboard self, IEnumerable<BehaviorTreeBlackboardEntryDefinition> defaults)
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

                self.Values[entry.Key] = BehaviorTreeValueUtility.GetValue(entry.DefaultValue);
            }
        }

        public static bool Contains(this BehaviorTreeBlackboard self, string key)
        {
            return !string.IsNullOrWhiteSpace(key) && self.Values.ContainsKey(key);
        }

        public static object GetBoxed(this BehaviorTreeBlackboard self, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            self.Values.TryGetValue(key, out object value);
            return value;
        }

        public static T Get<T>(this BehaviorTreeBlackboard self, string key, T defaultValue = default)
        {
            object value = self.GetBoxed(key);
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

        public static void Set<T>(this BehaviorTreeBlackboard self, string key, T value)
        {
            self.SetBoxed(key, value);
        }

        public static void SetBoxed(this BehaviorTreeBlackboard self, string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            self.Values.TryGetValue(key, out object previousValue);
            if (Equals(previousValue, value))
            {
                return;
            }

            self.Values[key] = value;
            self.Notify(new BehaviorTreeBlackboardChange(key, previousValue, value, false));
        }

        public static bool Remove(this BehaviorTreeBlackboard self, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (!self.Values.Remove(key, out object previousValue))
            {
                return false;
            }

            self.Notify(new BehaviorTreeBlackboardChange(key, previousValue, null, true));
            return true;
        }

        public static long Observe(this BehaviorTreeBlackboard self, string key, Action<BehaviorTreeBlackboardChange> callback, bool fireImmediately = false)
        {
            if (string.IsNullOrWhiteSpace(key) || callback == null)
            {
                return 0;
            }

            long observerId = ++self.NextObserverId;
            BehaviorTreeBlackboardObserver observer = new()
            {
                ObserverId = observerId,
                Key = key,
                Callback = callback,
            };
            self.Observers.Add(observerId, observer);

            if (!self.ObserverKeys.TryGetValue(key, out List<long> observerIds))
            {
                observerIds = new List<long>();
                self.ObserverKeys.Add(key, observerIds);
            }

            observerIds.Add(observerId);

            if (fireImmediately)
            {
                callback.Invoke(new BehaviorTreeBlackboardChange(key, null, self.GetBoxed(key), false));
            }

            return observerId;
        }

        public static void RemoveObserver(this BehaviorTreeBlackboard self, long observerId)
        {
            if (observerId == 0 || !self.Observers.Remove(observerId, out BehaviorTreeBlackboardObserver observer) || observer == null)
            {
                return;
            }

            if (!self.ObserverKeys.TryGetValue(observer.Key, out List<long> observerIds))
            {
                return;
            }

            observerIds.Remove(observerId);
            if (observerIds.Count == 0)
            {
                self.ObserverKeys.Remove(observer.Key);
            }
        }

        public static void Notify(this BehaviorTreeBlackboard self, BehaviorTreeBlackboardChange change)
        {
            if (!self.ObserverKeys.TryGetValue(change.Key, out List<long> observerIds) || observerIds.Count == 0)
            {
                return;
            }

            long[] copiedIds = observerIds.ToArray();
            foreach (long observerId in copiedIds)
            {
                if (!self.Observers.TryGetValue(observerId, out BehaviorTreeBlackboardObserver observer))
                {
                    continue;
                }

                observer.Callback?.Invoke(change);
            }
        }
    }
}
