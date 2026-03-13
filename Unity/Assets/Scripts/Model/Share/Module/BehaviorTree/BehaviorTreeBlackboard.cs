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
    public sealed class BehaviorTreeBlackboardObserver
    {
        public long ObserverId;

        public string Key = string.Empty;

        public Action<BehaviorTreeBlackboardChange> Callback;
    }

    [EnableClass]
    public sealed class BehaviorTreeBlackboard
    {
        public long NextObserverId;

        public readonly Dictionary<string, object> Values = new(StringComparer.OrdinalIgnoreCase);

        public readonly Dictionary<long, BehaviorTreeBlackboardObserver> Observers = new();

        public readonly Dictionary<string, List<long>> ObserverKeys = new(StringComparer.OrdinalIgnoreCase);
    }
}
