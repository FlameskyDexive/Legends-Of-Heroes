using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class BTreeComponent : Entity, IAwake<byte[], string>, IDestroy
    {
        public byte[] TreeBytes;

        public string TreeIdOrName;

        public long RuntimeId;

        public readonly Dictionary<string, BTreeSerializedValue> BlackboardOverrides = new();
    }
}
