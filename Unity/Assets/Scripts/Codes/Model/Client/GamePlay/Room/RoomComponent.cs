using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public class RoomComponent : Entity, IAwake, IDestroy
    {
        public List<PlayerInfoRoom> PlayerInfos = new List<PlayerInfoRoom>();
        
        
    }
}