using System;
using System.Collections.Generic;

namespace ET
{
    public readonly struct BTBlackboardChange
    {
        public readonly string Key;
        public readonly object PreviousValue;
        public readonly object CurrentValue;
        public readonly bool IsRemoved;

        public BTBlackboardChange(string key, object previousValue, object currentValue, bool isRemoved)
        {
            this.Key = key;
            this.PreviousValue = previousValue;
            this.CurrentValue = currentValue;
            this.IsRemoved = isRemoved;
        }
    }

    [EnableClass]
    public sealed class BTBlackboardObserver
    {
        public long ObserverId;

        public string Key = string.Empty;

        public Action<BTBlackboardChange> Callback;
    }

    [EnableClass]
    public sealed class BTBlackboard
    {
        public long NextObserverId;

        public readonly Dictionary<string, object> Values = new(StringComparer.OrdinalIgnoreCase);

        public readonly Dictionary<long, BTBlackboardObserver> Observers = new();

        public readonly Dictionary<string, List<long>> ObserverKeys = new(StringComparer.OrdinalIgnoreCase);
    }
}
