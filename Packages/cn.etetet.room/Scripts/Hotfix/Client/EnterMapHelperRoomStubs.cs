namespace ET.Client
{
    // 房间相关 EnterMapHelper 扩展桩，等 proto/业务 helper 真正移植后替换。
    public static partial class EnterMapHelper
    {
        public static async ETTask<G2C_CreateRoom> CreateRoomAsync(Fiber fiber, string roomName, RoomMode mode, int maxPlayers, string password)
        {
            Log.Warning("EnterMapHelper.CreateRoomAsync 仍为桩实现，需要等待 proto 房间消息移植完成。");
            await ETTask.CompletedTask;
            return new G2C_CreateRoom { Error = -1, Message = "stub: proto not ported" };
        }

        public static async ETTask<G2C_JoinRoom> JoinRoomAsync(Fiber fiber, long roomId, string password = null)
        {
            Log.Warning("EnterMapHelper.JoinRoomAsync 仍为桩实现，需要等待 proto 房间消息移植完成。");
            await ETTask.CompletedTask;
            return new G2C_JoinRoom { Error = -1, Message = "stub: proto not ported" };
        }

        public static async ETTask<G2C_GetRoomList> GetRoomListAsync(Fiber fiber, RoomMode? mode = null)
        {
            Log.Warning("EnterMapHelper.GetRoomListAsync 仍为桩实现，需要等待 proto 房间消息移植完成。");
            await ETTask.CompletedTask;
            return new G2C_GetRoomList { Error = -1, Message = "stub: proto not ported" };
        }
    }
}
