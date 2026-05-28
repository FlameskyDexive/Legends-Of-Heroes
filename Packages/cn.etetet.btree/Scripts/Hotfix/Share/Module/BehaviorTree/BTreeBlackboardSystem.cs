using System;
using System.Collections.Generic;

namespace ET
{
    public static class BTreeBlackboardSystem
    {
        public static void ApplyDefaults(this BTreeBlackboard self, IEnumerable<BTreeBlackboardEntryData> defaults)
        {
            if (defaults == null)
            {
                return;
            }

            foreach (BTreeBlackboardEntryData entry in defaults)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Key))
                {
                    continue;
                }

                self.Values[entry.Key] = BTreeValueUtility.GetValue(entry.DefaultValue);
            }
        }

        public static bool Contains(this BTreeBlackboard self, string key)
        {
            return !string.IsNullOrWhiteSpace(key) && self.Values.ContainsKey(key);
        }

        public static object GetBoxed(this BTreeBlackboard self, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            self.Values.TryGetValue(key, out object value);
            return value;
        }

        public static T Get<T>(this BTreeBlackboard self, string key, T defaultValue = default)
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

        public static void Set<T>(this BTreeBlackboard self, string key, T value)
        {
            self.SetBoxed(key, value);
        }

        public static void SetBoxed(this BTreeBlackboard self, string key, object value)
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
            self.Notify(new BTreeBlackboardChange(key, previousValue, value, false));
        }

        public static bool Remove(this BTreeBlackboard self, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (!self.Values.Remove(key, out object previousValue))
            {
                return false;
            }

            self.Notify(new BTreeBlackboardChange(key, previousValue, null, true));
            return true;
        }

        public static long Observe(this BTreeBlackboard self, string key, Action<BTreeBlackboardChange> callback, bool fireImmediately = false)
        {
            if (string.IsNullOrWhiteSpace(key) || callback == null)
            {
                return 0;
            }

            long observerId = ++self.NextObserverId;
            BTreeBlackboardObserver observer = new()
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
                callback.Invoke(new BTreeBlackboardChange(key, null, self.GetBoxed(key), false));
            }

            return observerId;
        }

        public static void RemoveObserver(this BTreeBlackboard self, long observerId)
        {
            if (observerId == 0 || !self.Observers.Remove(observerId, out BTreeBlackboardObserver observer) || observer == null)
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

        public static void Notify(this BTreeBlackboard self, BTreeBlackboardChange change)
        {
            if (!self.ObserverKeys.TryGetValue(change.Key, out List<long> observerIds) || observerIds.Count == 0)
            {
                return;
            }

            long[] copiedIds = observerIds.ToArray();
            foreach (long observerId in copiedIds)
            {
                if (!self.Observers.TryGetValue(observerId, out BTreeBlackboardObserver observer))
                {
                    continue;
                }

                observer.Callback?.Invoke(change);
            }
        }
    }
}
