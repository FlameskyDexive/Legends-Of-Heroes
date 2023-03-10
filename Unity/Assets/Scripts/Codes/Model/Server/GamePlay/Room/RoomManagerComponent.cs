using System.Collections;
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class RoomManagerComponent : Entity, IAwake, IDestroy
    {
        public int RoomIdNum { get; set; }

        public Dictionary<long, Room> Rooms = new Dictionary<long, Room>();

    }
}