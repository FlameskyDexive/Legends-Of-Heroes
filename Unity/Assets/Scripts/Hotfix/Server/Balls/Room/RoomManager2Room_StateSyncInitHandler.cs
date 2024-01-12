using System.Collections.Generic;

namespace ET.Server
{
    [MessageHandler(SceneType.RoomRoot)]
    public class RoomManager2Room_StateSyncInitHandler : MessageHandler<Scene, RoomManager2Room_StateSyncInit, Room2RoomManager_StateSyncInit>
    {
        protected override async ETTask Run(Scene root, RoomManager2Room_StateSyncInit request, Room2RoomManager_StateSyncInit response)
        {
            StateSyncRoom room = root.AddComponent<StateSyncRoom>();
            room.Name = "Server";
            room.AddComponent<StateSyncRoomServerComponent, List<long>>(request.PlayerIds);

            room.World = new StateSyncWorld(SceneType.Demo);
            await ETTask.CompletedTask;
        }
    }
}