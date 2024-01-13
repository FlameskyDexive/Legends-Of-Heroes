using System;
using System.Collections.Generic;

namespace ET.Server
{
	[MessageHandler(SceneType.Map)]
	public class Match2Map_StateSyncGetRoomHandler : MessageHandler<Scene, Match2Map_StateSyncGetRoom, Map2Match_StateSyncGetRoom>
	{
		protected override async ETTask Run(Scene root, Match2Map_StateSyncGetRoom request, Map2Match_StateSyncGetRoom response)
		{
			//RoomManagerComponent roomManagerComponent = root.GetComponent<RoomManagerComponent>();
			
			Fiber fiber = root.Fiber();
			int fiberId = await FiberManager.Instance.Create(SchedulerType.ThreadPool, fiber.Zone, SceneType.RoomRoot, "RoomRoot");
			ActorId roomRootActorId = new(fiber.Process, fiberId);

            // 发送消息给房间纤程，初始化
            RoomManager2Room_StateSyncInit roomManager2RoomInit = RoomManager2Room_StateSyncInit.Create();
			roomManager2RoomInit.PlayerIds.AddRange(request.PlayerIds);
			await root.GetComponent<MessageSender>().Call(roomRootActorId, roomManager2RoomInit);
			
			response.ActorId = roomRootActorId;
			await ETTask.CompletedTask;
		}
	}
}