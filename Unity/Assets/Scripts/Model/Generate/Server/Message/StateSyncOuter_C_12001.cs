using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    [MemoryPackable]
    [Message(StateSyncOuter.C2G_StateSyncMatch)]
    [ResponseType(nameof(G2C_StateSyncMatch))]
    public partial class C2G_StateSyncMatch : MessageObject, ISessionRequest
    {
        public static C2G_StateSyncMatch Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(C2G_StateSyncMatch), isFromPool) as C2G_StateSyncMatch;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.G2C_StateSyncMatch)]
    public partial class G2C_StateSyncMatch : MessageObject, ISessionResponse
    {
        public static G2C_StateSyncMatch Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2C_StateSyncMatch), isFromPool) as G2C_StateSyncMatch;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public int Error { get; set; }

        [MemoryPackOrder(2)]
        public string Message { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.Error = default;
            this.Message = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    /// <summary>
    /// 刷新匹配信息
    /// </summary>
    [MemoryPackable]
    [Message(StateSyncOuter.G2C_StateSyncRefreshMatch)]
    public partial class G2C_StateSyncRefreshMatch : MessageObject, IMessage
    {
        public static G2C_StateSyncRefreshMatch Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2C_StateSyncRefreshMatch), isFromPool) as G2C_StateSyncRefreshMatch;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        /// <summary>
        /// 房间信息
        /// </summary>
        [MemoryPackOrder(1)]
        public RoomInfo RoomInfo { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.RoomInfo = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    /// <summary>
    /// 匹配成功，通知客户端切换场景
    /// </summary>
    [MemoryPackable]
    [Message(StateSyncOuter.Match2G_StateSyncNotifyMatchSuccess)]
    public partial class Match2G_StateSyncNotifyMatchSuccess : MessageObject, IMessage
    {
        public static Match2G_StateSyncNotifyMatchSuccess Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_StateSyncNotifyMatchSuccess), isFromPool) as Match2G_StateSyncNotifyMatchSuccess;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        /// <summary>
        /// 房间的ActorId
        /// </summary>
        [MemoryPackOrder(1)]
        public ActorId ActorId { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.ActorId = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    /// <summary>
    /// 客户端通知房间切换场景完成
    /// </summary>
    [MemoryPackable]
    [Message(StateSyncOuter.C2Room_StateSyncChangeSceneFinish)]
    public partial class C2Room_StateSyncChangeSceneFinish : MessageObject, IRoomMessage
    {
        public static C2Room_StateSyncChangeSceneFinish Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(C2Room_StateSyncChangeSceneFinish), isFromPool) as C2Room_StateSyncChangeSceneFinish;
        }

        [MemoryPackOrder(0)]
        public long PlayerId { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.PlayerId = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    /// <summary>
    /// 房间通知客户端进入战斗
    /// </summary>
    [MemoryPackable]
    [Message(StateSyncOuter.Room2C_StateSyncStart)]
    public partial class Room2C_StateSyncStart : MessageObject, IMessage
    {
        public static Room2C_StateSyncStart Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Room2C_StateSyncStart), isFromPool) as Room2C_StateSyncStart;
        }

        [MemoryPackOrder(0)]
        public long StartTime { get; set; }

        [MemoryPackOrder(1)]
        public List<UnitInfo> UnitInfo { get; set; } = new();

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.StartTime = default;
            this.UnitInfo.Clear();

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.Room2C_StateSyncAdjustUpdateTime)]
    public partial class Room2C_StateSyncAdjustUpdateTime : MessageObject, IMessage
    {
        public static Room2C_StateSyncAdjustUpdateTime Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Room2C_StateSyncAdjustUpdateTime), isFromPool) as Room2C_StateSyncAdjustUpdateTime;
        }

        [MemoryPackOrder(0)]
        public int DiffTime { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.DiffTime = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.G2C_StateSyncReconnect)]
    public partial class G2C_StateSyncReconnect : MessageObject, IMessage
    {
        public static G2C_StateSyncReconnect Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2C_StateSyncReconnect), isFromPool) as G2C_StateSyncReconnect;
        }

        [MemoryPackOrder(0)]
        public long StartTime { get; set; }

        [MemoryPackOrder(1)]
        public List<UnitInfo> UnitInfos { get; set; } = new();

        [MemoryPackOrder(2)]
        public int Frame { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.StartTime = default;
            this.UnitInfos.Clear();
            this.Frame = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    /// <summary>
    /// Match Start ///////////////////////////////////
    /// </summary>
    [MemoryPackable]
    [Message(StateSyncOuter.G2Match_CreateRoom)]
    [ResponseType(nameof(Match2G_CreateRoom))]
    public partial class G2Match_CreateRoom : MessageObject, ILocationRequest
    {
        public static G2Match_CreateRoom Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2Match_CreateRoom), isFromPool) as G2Match_CreateRoom;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }

        [MemoryPackOrder(2)]
        public string RoomName { get; set; }

        [MemoryPackOrder(3)]
        public RoomMode Mode { get; set; }

        [MemoryPackOrder(4)]
        public int MaxPlayers { get; set; }

        [MemoryPackOrder(5)]
        public string Password { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.PlayerId = default;
            this.RoomName = default;
            this.Mode = default;
            this.MaxPlayers = default;
            this.Password = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.Match2G_CreateRoom)]
    public partial class Match2G_CreateRoom : MessageObject, ILocationResponse
    {
        public static Match2G_CreateRoom Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_CreateRoom), isFromPool) as Match2G_CreateRoom;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public int Error { get; set; }

        [MemoryPackOrder(2)]
        public string Message { get; set; }

        [MemoryPackOrder(3)]
        public RoomInfo RoomInfo { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.RoomInfo = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.G2Match_JoinRoom)]
    [ResponseType(nameof(Match2G_JoinRoom))]
    public partial class G2Match_JoinRoom : MessageObject, ILocationRequest
    {
        public static G2Match_JoinRoom Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2Match_JoinRoom), isFromPool) as G2Match_JoinRoom;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }

        [MemoryPackOrder(2)]
        public long RoomId { get; set; }

        [MemoryPackOrder(3)]
        public string Password { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.PlayerId = default;
            this.RoomId = default;
            this.Password = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.Match2G_JoinRoom)]
    public partial class Match2G_JoinRoom : MessageObject, ILocationResponse
    {
        public static Match2G_JoinRoom Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_JoinRoom), isFromPool) as Match2G_JoinRoom;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public int Error { get; set; }

        [MemoryPackOrder(2)]
        public string Message { get; set; }

        [MemoryPackOrder(3)]
        public RoomInfo RoomInfo { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.RoomInfo = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.G2Match_LeaveRoom)]
    [ResponseType(nameof(Match2G_LeaveRoom))]
    public partial class G2Match_LeaveRoom : MessageObject, ILocationRequest
    {
        public static G2Match_LeaveRoom Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2Match_LeaveRoom), isFromPool) as G2Match_LeaveRoom;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.PlayerId = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.Match2G_LeaveRoom)]
    public partial class Match2G_LeaveRoom : MessageObject, ILocationResponse
    {
        public static Match2G_LeaveRoom Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_LeaveRoom), isFromPool) as Match2G_LeaveRoom;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public int Error { get; set; }

        [MemoryPackOrder(2)]
        public string Message { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.Error = default;
            this.Message = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.G2Match_CancelMatch)]
    [ResponseType(nameof(Match2G_CancelMatch))]
    public partial class G2Match_CancelMatch : MessageObject, ILocationRequest
    {
        public static G2Match_CancelMatch Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2Match_CancelMatch), isFromPool) as G2Match_CancelMatch;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.PlayerId = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.Match2G_CancelMatch)]
    public partial class Match2G_CancelMatch : MessageObject, ILocationResponse
    {
        public static Match2G_CancelMatch Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_CancelMatch), isFromPool) as Match2G_CancelMatch;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public int Error { get; set; }

        [MemoryPackOrder(2)]
        public string Message { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.Error = default;
            this.Message = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.G2Match_GetRoomList)]
    [ResponseType(nameof(Match2G_GetRoomList))]
    public partial class G2Match_GetRoomList : MessageObject, ILocationRequest
    {
        public static G2Match_GetRoomList Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(G2Match_GetRoomList), isFromPool) as G2Match_GetRoomList;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public RoomMode Mode { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.Mode = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.Match2G_GetRoomList)]
    public partial class Match2G_GetRoomList : MessageObject, ILocationResponse
    {
        public static Match2G_GetRoomList Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Match2G_GetRoomList), isFromPool) as Match2G_GetRoomList;
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }

        [MemoryPackOrder(1)]
        public int Error { get; set; }

        [MemoryPackOrder(2)]
        public string Message { get; set; }

        [MemoryPackOrder(3)]
        public List<RoomInfo> RoomList { get; set; } = new();

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.RoomList.Clear();

            ObjectPool.Instance.Recycle(this);
        }
    }

    /// <summary>
    /// Match End ///////////////////////////////////
    /// </summary>
    /// <summary>
    /// Map Start ///////////////////////////////////
    /// </summary>
    [MemoryPackable]
    [Message(StateSyncOuter.C2Room_JoystickMove)]
    public partial class C2Room_JoystickMove : MessageObject, IRoomMessage
    {
        public static C2Room_JoystickMove Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(C2Room_JoystickMove), isFromPool) as C2Room_JoystickMove;
        }

        [MemoryPackOrder(0)]
        public long PlayerId { get; set; }

        [MemoryPackOrder(1)]
        public Unity.Mathematics.float3 MoveForward { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.PlayerId = default;
            this.MoveForward = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.Room2C_JoystickMove)]
    public partial class Room2C_JoystickMove : MessageObject, IMessage
    {
        public static Room2C_JoystickMove Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Room2C_JoystickMove), isFromPool) as Room2C_JoystickMove;
        }

        [MemoryPackOrder(0)]
        public long Id { get; set; }

        [MemoryPackOrder(1)]
        public Unity.Mathematics.float3 Position { get; set; }

        [MemoryPackOrder(2)]
        public Unity.Mathematics.float3 MoveForward { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.Id = default;
            this.Position = default;
            this.MoveForward = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.C2Room_Operation)]
    public partial class C2Room_Operation : MessageObject, IRoomMessage
    {
        public static C2Room_Operation Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(C2Room_Operation), isFromPool) as C2Room_Operation;
        }

        [MemoryPackOrder(0)]
        public long PlayerId { get; set; }

        [MemoryPackOrder(1)]
        public long Id { get; set; }

        [MemoryPackOrder(2)]
        public List<OperateInfo> OperateInfos { get; set; } = new();

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.PlayerId = default;
            this.Id = default;
            this.OperateInfos.Clear();

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.Room2C_Operation)]
    public partial class Room2C_Operation : MessageObject, IMessage
    {
        public static Room2C_Operation Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Room2C_Operation), isFromPool) as Room2C_Operation;
        }

        [MemoryPackOrder(0)]
        public long Id { get; set; }

        [MemoryPackOrder(1)]
        public List<OperateReplyInfo> OperateInfos { get; set; } = new();

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.Id = default;
            this.OperateInfos.Clear();

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.OperateInfo)]
    public partial class OperateInfo : MessageObject
    {
        public static OperateInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(OperateInfo), isFromPool) as OperateInfo;
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        [MemoryPackOrder(0)]
        public int OperateType { get; set; }

        /// <summary>
        /// 输入类型，按下/抬起
        /// </summary>
        [MemoryPackOrder(1)]
        public int InputType { get; set; }

        /// <summary>
        /// v3参数
        /// </summary>
        [MemoryPackOrder(2)]
        public Unity.Mathematics.float3 Vec3 { get; set; }

        [MemoryPackOrder(3)]
        public long Value1 { get; set; }

        [MemoryPackOrder(4)]
        public long Value2 { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.OperateType = default;
            this.InputType = default;
            this.Vec3 = default;
            this.Value1 = default;
            this.Value2 = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.OperateReplyInfo)]
    public partial class OperateReplyInfo : MessageObject
    {
        public static OperateReplyInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(OperateReplyInfo), isFromPool) as OperateReplyInfo;
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        [MemoryPackOrder(0)]
        public int OperateType { get; set; }

        [MemoryPackOrder(1)]
        public int Status { get; set; }

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.OperateType = default;
            this.Status = default;

            ObjectPool.Instance.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(StateSyncOuter.Room2C_SyncUnits)]
    public partial class Room2C_SyncUnits : MessageObject, IMessage
    {
        public static Room2C_SyncUnits Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(Room2C_SyncUnits), isFromPool) as Room2C_SyncUnits;
        }

        [MemoryPackOrder(0)]
        public List<UnitInfo> Units { get; set; } = new();

        public override void Dispose()
        {
            if (!this.IsFromPool)
            {
                return;
            }

            this.Units.Clear();

            ObjectPool.Instance.Recycle(this);
        }
    }

    /// <summary>
    /// Map End ///////////////////////////////////
    /// </summary>
    public static class StateSyncOuter
    {
        public const ushort C2G_StateSyncMatch = 12002;
        public const ushort G2C_StateSyncMatch = 12003;
        public const ushort G2C_StateSyncRefreshMatch = 12004;
        public const ushort Match2G_StateSyncNotifyMatchSuccess = 12005;
        public const ushort C2Room_StateSyncChangeSceneFinish = 12006;
        public const ushort Room2C_StateSyncStart = 12007;
        public const ushort Room2C_StateSyncAdjustUpdateTime = 12008;
        public const ushort G2C_StateSyncReconnect = 12009;
        public const ushort G2Match_CreateRoom = 12010;
        public const ushort Match2G_CreateRoom = 12011;
        public const ushort G2Match_JoinRoom = 12012;
        public const ushort Match2G_JoinRoom = 12013;
        public const ushort G2Match_LeaveRoom = 12014;
        public const ushort Match2G_LeaveRoom = 12015;
        public const ushort G2Match_CancelMatch = 12016;
        public const ushort Match2G_CancelMatch = 12017;
        public const ushort G2Match_GetRoomList = 12018;
        public const ushort Match2G_GetRoomList = 12019;
        public const ushort C2Room_JoystickMove = 12020;
        public const ushort Room2C_JoystickMove = 12021;
        public const ushort C2Room_Operation = 12022;
        public const ushort Room2C_Operation = 12023;
        public const ushort OperateInfo = 12024;
        public const ushort OperateReplyInfo = 12025;
        public const ushort Room2C_SyncUnits = 12026;
    }
}