using System.Collections;
using System.Collections.Generic;

namespace ET.Server
{
    public class Room : Entity, IAwake
    {
        public Player RoomOwner;
        
        public string RoomName { get; set; }
        public int CurPlayerNum { get; set; }
        public int MaxPlayerNum { get; set; }

        public Dictionary<long, Player> AllPlayers = new Dictionary<long, Player>();

        public Dictionary<int, long> CampPlayers = new Dictionary<int, long>();
    }
}
