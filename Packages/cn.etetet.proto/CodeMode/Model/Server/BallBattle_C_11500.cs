using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    // 排行榜单行(一个玩家球)
    [MemoryPackable]
    [Message(Opcode.BallRankInfo)]
    public partial class BallRankInfo : MessageObject
    {
        public static BallRankInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<BallRankInfo>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public long UnitId { get; set; }
        /// <summary>
        /// 当前 HP(体型/质量)
        /// </summary>
        [MemoryPackOrder(1)]
        public int Hp { get; set; }
        /// <summary>
        /// 累计击杀
        /// </summary>
        [MemoryPackOrder(2)]
        public int Kills { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    // 服务端→客户端 广播排行榜(Top N, 按 HP 降序)。每隔固定时间由竞技场广播给场景内真人玩家。
    [MemoryPackable]
    [Message(Opcode.M2C_BallLeaderboard)]
    public partial class M2C_BallLeaderboard : MessageObject, ICurrentMessage
    {
        public static M2C_BallLeaderboard Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_BallLeaderboard>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public List<BallRankInfo> Ranks { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    public static partial class Opcode
    {
        public const ushort BallRankInfo = 11501;
        public const ushort M2C_BallLeaderboard = 11502;
    }
}