using System.Collections.Generic;

namespace ET.Client
{
    public struct CreateRoomSuccess
    {
        public RoomInfo RoomInfo;
    }

    public struct CreateRoomFailed
    {
        public int Error;
        public string Message;
    }

    public struct JoinRoomSuccess
    {
        public RoomInfo RoomInfo;
    }

    public struct JoinRoomFailed
    {
        public int Error;
        public string Message;
    }

    public struct LeaveRoomSuccess
    {
    }

    public struct LeaveRoomFailed
    {
        public int Error;
        public string Message;
    }

    public struct CancelMatchSuccess
    {
    }

    public struct CancelMatchFailed
    {
        public int Error;
        public string Message;
    }

    public struct GetRoomListSuccess
    {
        public List<RoomInfo> RoomList;
    }

    public struct GetRoomListFailed
    {
        public int Error;
        public string Message;
    }

    public struct RoomInfoChanged
    {
        public RoomInfo RoomInfo;
    }

    public struct PlayerJoinRoom
    {
        public PlayerInfo PlayerInfo;
    }

    public struct PlayerLeaveRoom
    {
        public long PlayerId;
    }
}