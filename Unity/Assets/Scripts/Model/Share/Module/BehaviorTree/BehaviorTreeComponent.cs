using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class BehaviorTreeComponent : Entity, IAwake<byte[], string>, IDestroy
    {
        public byte[] TreeBytes;

        public string TreeIdOrName;

        public long RuntimeId;

        public readonly Dictionary<string, BehaviorTreeSerializedValue> BlackboardOverrides = new();
    }
}
