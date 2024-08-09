using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(StateSyncRoom))]
    public class StateSyncRoomServerComponent : Entity, IAwake<List<long>>, IFixedUpdate, IDestroy
    {
    }
}