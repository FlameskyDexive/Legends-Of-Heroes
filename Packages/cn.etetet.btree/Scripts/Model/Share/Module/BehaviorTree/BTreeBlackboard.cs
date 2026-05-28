using System;
using System.Collections.Generic;

namespace ET
{
    public readonly struct BTreeBlackboardChange
    {
        public readonly string Key;
        public readonly object PreviousValue;
        public readonly object CurrentValue;
        public readonly bool IsRemoved;

        public BTreeBlackboardChange(string key, object previousValue, object currentValue, bool isRemoved)
        {
            this.Key = key;
            this.PreviousValue = previousValue;
            this.CurrentValue = currentValue;
            this.IsRemoved = isRemoved;
        }
    }

    [EnableClass]
    public sealed class BTreeBlackboardObserver
    {
        public long ObserverId;

        public string Key = string.Empty;

        public Action<BTreeBlackboardChange> Callback;
    }

    [EnableClass]
    public sealed class BTreeBlackboard
    {
        public long NextObserverId;

        public readonly Dictionary<string, object> Values = new(StringComparer.OrdinalIgnoreCase);

        public readonly Dictionary<long, BTreeBlackboardObserver> Observers = new();

        public readonly Dictionary<string, List<long>> ObserverKeys = new(StringComparer.OrdinalIgnoreCase);
    }
}
