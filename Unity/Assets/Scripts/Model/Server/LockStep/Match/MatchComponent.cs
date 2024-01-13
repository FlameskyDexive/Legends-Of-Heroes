using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class MatchComponent: Entity, IAwake
    {
        public List<long> waitMatchPlayers = new List<long>();
        public List<long> waitMatchStateSyncPlayers = new List<long>();
    }

}