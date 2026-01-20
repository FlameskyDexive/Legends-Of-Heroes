using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class RoomManagerComponent: Entity, IAwake
    {
        public Dictionary<long, EntityRef<StateSyncRoom>> Rooms { get; set; }

        public long RoomIdGenerator { get; set; }

        public Dictionary<long, long> PlayerToRoom { get; set; }
    }
}