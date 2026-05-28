using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    [MemoryPackable]
    [Message(Opcode.Console2Robot_LogoutRequest)]
    [ResponseType(nameof(Console2Robot_LogoutResponse))]
    public partial class Console2Robot_LogoutRequest : MessageObject, IRequest
    {
        public static Console2Robot_LogoutRequest Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<Console2Robot_LogoutRequest>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.Console2Robot_LogoutResponse)]
    public partial class Console2Robot_LogoutResponse : MessageObject, IResponse
    {
        public static Console2Robot_LogoutResponse Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<Console2Robot_LogoutResponse>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.Robot_LoginRequest)]
    [ResponseType(nameof(Robot_LoginResponse))]
    public partial class Robot_LoginRequest : MessageObject, IRequest
    {
        public static Robot_LoginRequest Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<Robot_LoginRequest>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string Account { get; set; }
        [MemoryPackOrder(2)]
        public string Password { get; set; }
        [MemoryPackOrder(3)]
        public string Address { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.Robot_LoginResponse)]
    public partial class Robot_LoginResponse : MessageObject, IResponse
    {
        public static Robot_LoginResponse Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<Robot_LoginResponse>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    // 客户端匹配超时，请求服务端创建一个机器人玩家当对手（机器人登录后进同一张共享地图）
    [MemoryPackable]
    [Message(Opcode.C2G_RequestMatchRobot)]
    [ResponseType(nameof(G2C_RequestMatchRobot))]
    public partial class C2G_RequestMatchRobot : MessageObject, ISessionRequest
    {
        public static C2G_RequestMatchRobot Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_RequestMatchRobot>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_RequestMatchRobot)]
    public partial class G2C_RequestMatchRobot : MessageObject, ISessionResponse
    {
        public static G2C_RequestMatchRobot Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_RequestMatchRobot>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    public static partial class Opcode
    {
        public const ushort Console2Robot_LogoutRequest = 20301;
        public const ushort Console2Robot_LogoutResponse = 20302;
        public const ushort Robot_LoginRequest = 20303;
        public const ushort Robot_LoginResponse = 20304;
        public const ushort C2G_RequestMatchRobot = 20305;
        public const ushort G2C_RequestMatchRobot = 20306;
    }
}