using System.Collections;
using System.Collections.Generic;

namespace ET.Server
{
    public class RoomManagerComponent : Entity, IAwake, IDestroy
    {
        public int RoomIdNum { get; set; }

        public Dictionary<long, Room> Rooms = new Dictionary<long, Room>();

    }
}