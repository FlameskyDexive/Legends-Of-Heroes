using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Server
{
    [MessageHandler(SceneType.RoomRoot)]
    [FriendOf(typeof (RoomServerComponent))]
    public class C2Room_StateSyncChangeSceneFinishHandler: MessageHandler<Scene, C2Room_StateSyncChangeSceneFinish>
    {
        protected override async ETTask Run(Scene root, C2Room_StateSyncChangeSceneFinish message)
        {
            Room room = root.GetComponent<Room>();
            RoomServerComponent roomServerComponent = room.GetComponent<RoomServerComponent>();
            RoomPlayer roomPlayer = room.GetComponent<RoomServerComponent>().GetChild<RoomPlayer>(message.PlayerId);
            roomPlayer.Progress = 100;
            
            if (!roomServerComponent.IsAllPlayerProgress100())
            {
                return;
            }
            
            await room.Fiber.TimerComponent.WaitAsync(1000);

            Room2C_StateSyncStart room2CStart = new() { StartTime = TimeInfo.Instance.ServerFrameTime() };
            foreach (RoomPlayer rp in roomServerComponent.Children.Values)
            {
                room2CStart.UnitInfo.Add(new StateSyncUnitInfo()
                {
                    PlayerId = rp.Id, Position = new float3(20, 0, -10), Rotation = new float3(0, 0, 0)
                });
            }

            room.Init(room2CStart.UnitInfo, room2CStart.StartTime);

            room.AddComponent<LSServerUpdater>();

            RoomMessageHelper.BroadCast(room, room2CStart);
        }
    }
}