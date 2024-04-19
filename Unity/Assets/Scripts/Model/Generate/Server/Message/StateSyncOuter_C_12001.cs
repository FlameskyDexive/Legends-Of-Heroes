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
    }
}