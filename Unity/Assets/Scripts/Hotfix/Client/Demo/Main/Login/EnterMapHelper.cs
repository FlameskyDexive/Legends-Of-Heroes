using System;


namespace ET.Client
{
    public static partial class EnterMapHelper
    {
        public static async ETTask EnterMapAsync(Scene root)
        {
            try
            {
                G2C_EnterMap g2CEnterMap = await root.GetComponent<ClientSenderComponent>().Call(C2G_EnterMap.Create()) as G2C_EnterMap;

                // 等待场景切换完成
                await root.GetComponent<ObjectWait>().Wait<Wait_SceneChangeFinish>();

                EventSystem.Instance.Publish(root, new EnterMapFinish());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ETTask Match(Fiber fiber)
        {
            try
            {
                G2C_Match g2CEnterMap = await fiber.Root.GetComponent<ClientSenderComponent>().Call(C2G_Match.Create()) as G2C_Match;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ETTask StateSyncMatch(Fiber fiber)
        {
            try
            {
                G2C_StateSyncMatch g2CEnterMap = await fiber.Root.GetComponent<ClientSenderComponent>().Call(C2G_StateSyncMatch.Create()) as G2C_StateSyncMatch;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ETTask<G2C_CreateRoom> CreateRoomAsync(Fiber fiber, string roomName, RoomMode mode, int maxPlayers, string password)
        {
            try
            {
                C2G_CreateRoom c2GCreateRoom = C2G_CreateRoom.Create();
                c2GCreateRoom.RoomName = roomName;
                c2GCreateRoom.Mode = mode;
                c2GCreateRoom.MaxPlayers = maxPlayers;
                c2GCreateRoom.Password = password;
                return await fiber.Root.GetComponent<ClientSenderComponent>().Call(c2GCreateRoom) as G2C_CreateRoom;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public static async ETTask<G2C_JoinRoom> JoinRoomAsync(Fiber fiber, long roomId, string password = null)
        {
            try
            {
                C2G_JoinRoom c2GJoinRoom = C2G_JoinRoom.Create();
                c2GJoinRoom.RoomId = roomId;
                c2GJoinRoom.Password = password;
                return await fiber.Root.GetComponent<ClientSenderComponent>().Call(c2GJoinRoom) as G2C_JoinRoom;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public static async ETTask<G2C_LeaveRoom> LeaveRoomAsync(Fiber fiber)
        {
            try
            {
                C2G_LeaveRoom c2GLeaveRoom = C2G_LeaveRoom.Create();
                return await fiber.Root.GetComponent<ClientSenderComponent>().Call(c2GLeaveRoom) as G2C_LeaveRoom;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public static async ETTask<G2C_CancelMatch> CancelMatchAsync(Fiber fiber)
        {
            try
            {
                C2G_CancelMatch c2GCancelMatch = C2G_CancelMatch.Create();
                return await fiber.Root.GetComponent<ClientSenderComponent>().Call(c2GCancelMatch) as G2C_CancelMatch;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public static async ETTask<G2C_GetRoomList> GetRoomListAsync(Fiber fiber, RoomMode? mode = null)
        {
            try
            {
                C2G_GetRoomList c2GGetRoomList = C2G_GetRoomList.Create();
                if (mode.HasValue)
                {
                    c2GGetRoomList.Mode = mode.Value;
                }
                return await fiber.Root.GetComponent<ClientSenderComponent>().Call(c2GGetRoomList) as G2C_GetRoomList;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
    }
}