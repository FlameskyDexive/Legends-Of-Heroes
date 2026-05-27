using System.Collections.Generic;

namespace ET
{
    // 以下类型为 cn.etetet.room 移植 EUI 时的临时桩，等 cn.etetet.proto 真正移植 Outer 房间消息后删除本文件。

    public enum RoomMode
    {
        StateSync,
        LockStep,
    }

    public enum RoomStatus
    {
        Waiting,
        Playing,
        Closed,
    }

    [EnableClass]
    public class PlayerInfo
    {
        public long PlayerId;
        public string PlayerName;
        public int AvatarIndex;
    }

    [EnableClass]
    public class RoomInfo
    {
        public long RoomId;
        public string RoomName;
        public string Password;
        public RoomMode Mode;
        public RoomStatus Status;
        public int CurrentPlayers;
        public int MaxPlayers;
        public bool HasPassword;
        public bool IsReady;
        public List<PlayerInfo> PlayerInfo = new();
    }

    [EnableClass]
    public class G2C_CreateRoom
    {
        public int Error;
        public string Message;
        public RoomInfo RoomInfo;
    }

    [EnableClass]
    public class G2C_JoinRoom
    {
        public int Error;
        public string Message;
        public RoomInfo RoomInfo;
    }

    [EnableClass]
    public class G2C_GetRoomList
    {
        public int Error;
        public string Message;
        public List<RoomInfo> RoomList = new();
    }
}
