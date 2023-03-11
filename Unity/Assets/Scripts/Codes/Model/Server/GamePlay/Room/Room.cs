using System.Collections;
using System.Collections.Generic;

namespace ET.Server
{
    [ChildOf(typeof(RoomManagerComponent))]
    public class Room : Entity, IAwake
    {
        public Player RoomOwner;
        
        public string RoomName { get; set; }
        public int CurPlayerNum { get; set; }
        public int MaxPlayerNum { get; set; }

        public Dictionary<long, Player> AllPlayers = new Dictionary<long, Player>();

        public Dictionary<int, List<long>> CampPlayers = new Dictionary<int, List<long>>();
    }
}
