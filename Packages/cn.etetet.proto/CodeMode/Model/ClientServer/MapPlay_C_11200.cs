using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    [MemoryPackable]
    [Message(Opcode.PetInfo)]
    public partial class PetInfo : MessageObject
    {
        public static PetInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<PetInfo>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public long OwnerId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.UnitInfo)]
    public partial class UnitInfo : MessageObject
    {
        public static UnitInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<UnitInfo>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public long UnitId { get; set; }
        [MemoryPackOrder(1)]
        public int ConfigId { get; set; }
        [MemoryPackOrder(2)]
        public int Type { get; set; }
        [MemoryPackOrder(3)]
        public Unity.Mathematics.float3 Position { get; set; }
        [MemoryPackOrder(4)]
        public Unity.Mathematics.float3 Forward { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]
        [MemoryPackOrder(5)]
        public Dictionary<int, long> KV { get; set; } = new();
        [MemoryPackOrder(6)]
        public MoveInfo MoveInfo { get; set; }
        [MemoryPackOrder(7)]
        public PetInfo PetInfo { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_CreateUnits)]
    public partial class M2C_CreateUnits : MessageObject, ICurrentMessage
    {
        public static M2C_CreateUnits Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_CreateUnits>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public List<UnitInfo> Units { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_CreateMyUnit)]
    public partial class M2C_CreateMyUnit : MessageObject, ICurrentMessage
    {
        public static M2C_CreateMyUnit Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_CreateMyUnit>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public UnitInfo Unit { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_StartSceneChange)]
    public partial class M2C_StartSceneChange : MessageObject, IMessage
    {
        public static M2C_StartSceneChange Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_StartSceneChange>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public long SceneId { get; set; }
        [MemoryPackOrder(1)]
        public string SceneName { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_RemoveUnits)]
    public partial class M2C_RemoveUnits : MessageObject, ICurrentMessage
    {
        public static M2C_RemoveUnits Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_RemoveUnits>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public List<long> Units { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_Error)]
    public partial class M2C_Error : MessageObject, IMessage
    {
        public static M2C_Error Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_Error>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int Error { get; set; }
        [MemoryPackOrder(1)]
        public List<string> Values { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_NumericChange)]
    public partial class M2C_NumericChange : MessageObject, ICurrentMessage
    {
        public static M2C_NumericChange Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_NumericChange>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public long UnitId { get; set; }
        [MemoryPackOrder(1)]
        public int NumericType { get; set; }
        [MemoryPackOrder(2)]
        public long Value { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2M_PetAttack)]
    public partial class C2M_PetAttack : MessageObject, ILocationMessage
    {
        public static C2M_PetAttack Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2M_PetAttack>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long UnitId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    // 摇杆/按键操作信息(单条)
    [MemoryPackable]
    [Message(Opcode.OperateInfo)]
    public partial class OperateInfo : MessageObject
    {
        public static OperateInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<OperateInfo>(isFromPool);
        }

        /// <summary>
        /// 操作类型, 对应 EOperateType
        /// </summary>
        [MemoryPackOrder(0)]
        public int OperateType { get; set; }
        /// <summary>
        /// 输入类型(按下/抬起), 对应 EInputType
        /// </summary>
        [MemoryPackOrder(1)]
        public int InputType { get; set; }
        /// <summary>
        /// 方向/位置参数(摇杆方向等)
        /// </summary>
        [MemoryPackOrder(2)]
        public Unity.Mathematics.float3 Vec3 { get; set; }
        [MemoryPackOrder(3)]
        public long Value1 { get; set; }
        [MemoryPackOrder(4)]
        public long Value2 { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    // 客户端每帧把收集到的操作批量上报服务端(摇杆移动、技能按键等)
    [MemoryPackable]
    [Message(Opcode.C2M_Operation)]
    public partial class C2M_Operation : MessageObject, ILocationMessage
    {
        public static C2M_Operation Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2M_Operation>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public List<OperateInfo> OperateInfos { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    public static partial class Opcode
    {
        public const ushort PetInfo = 11201;
        public const ushort UnitInfo = 11202;
        public const ushort M2C_CreateUnits = 11203;
        public const ushort M2C_CreateMyUnit = 11204;
        public const ushort M2C_StartSceneChange = 11205;
        public const ushort M2C_RemoveUnits = 11206;
        public const ushort M2C_Error = 11207;
        public const ushort M2C_NumericChange = 11208;
        public const ushort C2M_PetAttack = 11209;
        public const ushort OperateInfo = 11210;
        public const ushort C2M_Operation = 11211;
    }
}