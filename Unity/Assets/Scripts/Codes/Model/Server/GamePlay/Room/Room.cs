using System.Collections;
using System.Collections.Generic;

namespace ET.Server
{
    [ChildOf(typeof(RoomManagerComponent))]
    public class Room : Entity, IAwake, IDestroy
    {
        public long RoomOwnerPlayerId;
        
        public string RoomName { get; set; }
        public int CurPlayerNum { get; set; }
        public int MaxPlayerNum { get; set; }

        public Dictionary<long, PlayerInfoRoom> AllPlayers = new Dictionary<long, PlayerInfoRoom>();

        public Dictionary<int, List<long>> CampPlayers = new Dictionary<int, List<long>>();
    }
}
