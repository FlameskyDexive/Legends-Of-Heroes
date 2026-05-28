using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    // 使用物品请求
    [MemoryPackable]
    [Message(Opcode.C2M_UseItem)]
    [ResponseType(nameof(M2C_UseItem))]
    public partial class C2M_UseItem : MessageObject, ILocationRequest
    {
        public static C2M_UseItem Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2M_UseItem>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        /// <summary>
        /// 物品ID
        /// </summary>
        [MemoryPackOrder(1)]
        public long ItemId { get; set; }
        /// <summary>
        /// 使用数量
        /// </summary>
        [MemoryPackOrder(2)]
        public int Count { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_UseItem)]
    public partial class M2C_UseItem : MessageObject, ILocationResponse
    {
        public static M2C_UseItem Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_UseItem>(isFromPool);
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

    // 物品信息
    [MemoryPackable]
    [Message(Opcode.ItemData)]
    public partial class ItemData : MessageObject
    {
        public static ItemData Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<ItemData>(isFromPool);
        }

        /// <summary>
        /// 物品ID
        /// </summary>
        [MemoryPackOrder(0)]
        public long ItemId { get; set; }
        /// <summary>
        /// 槽位索引
        /// </summary>
        [MemoryPackOrder(1)]
        public int SlotIndex { get; set; }
        /// <summary>
        /// 物品配置ID
        /// </summary>
        [MemoryPackOrder(2)]
        public int ConfigId { get; set; }
        /// <summary>
        /// 物品数量
        /// </summary>
        [MemoryPackOrder(3)]
        public int Count { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    // 同步背包数据请求
    [MemoryPackable]
    [Message(Opcode.C2M_SyncBagData)]
    [ResponseType(nameof(M2C_SyncBagData))]
    public partial class C2M_SyncBagData : MessageObject, ILocationRequest
    {
        public static C2M_SyncBagData Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2M_SyncBagData>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_SyncBagData)]
    public partial class M2C_SyncBagData : MessageObject, ILocationResponse
    {
        public static M2C_SyncBagData Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_SyncBagData>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        /// <summary>
        /// 背包容量
        /// </summary>
        [MemoryPackOrder(3)]
        public int Capacity { get; set; }
        /// <summary>
        /// 物品列表
        /// </summary>
        [MemoryPackOrder(4)]
        public List<ItemData> Items { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    // 更新物品通知（服务器推送）
    [MemoryPackable]
    [Message(Opcode.M2C_UpdateItem)]
    public partial class M2C_UpdateItem : MessageObject, IMessage
    {
        public static M2C_UpdateItem Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_UpdateItem>(isFromPool);
        }

        /// <summary>
        /// 物品ID
        /// </summary>
        [MemoryPackOrder(0)]
        public long ItemId { get; set; }
        /// <summary>
        /// 槽位索引
        /// </summary>
        [MemoryPackOrder(1)]
        public int SlotIndex { get; set; }
        /// <summary>
        /// 物品配置ID
        /// </summary>
        [MemoryPackOrder(2)]
        public int ConfigId { get; set; }
        /// <summary>
        /// 物品数量
        /// </summary>
        [MemoryPackOrder(3)]
        public int Count { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    // 背包容量变化通知
    [MemoryPackable]
    [Message(Opcode.M2C_UpdateBagCapacity)]
    public partial class M2C_UpdateBagCapacity : MessageObject, IMessage
    {
        public static M2C_UpdateBagCapacity Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_UpdateBagCapacity>(isFromPool);
        }

        /// <summary>
        /// 新的背包容量
        /// </summary>
        [MemoryPackOrder(0)]
        public int Capacity { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    // 物品移动/交换槽位
    [MemoryPackable]
    [Message(Opcode.C2M_MoveItem)]
    [ResponseType(nameof(M2C_MoveItem))]
    public partial class C2M_MoveItem : MessageObject, ILocationRequest
    {
        public static C2M_MoveItem Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2M_MoveItem>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        /// <summary>
        /// 要移动的物品ID
        /// </summary>
        [MemoryPackOrder(1)]
        public long ItemId { get; set; }
        /// <summary>
        /// 目标槽位
        /// </summary>
        [MemoryPackOrder(2)]
        public int ToSlot { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_MoveItem)]
    public partial class M2C_MoveItem : MessageObject, ILocationResponse
    {
        public static M2C_MoveItem Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_MoveItem>(isFromPool);
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

    // 丢弃物品
    [MemoryPackable]
    [Message(Opcode.C2M_DiscardItem)]
    [ResponseType(nameof(M2C_DiscardItem))]
    public partial class C2M_DiscardItem : MessageObject, ILocationRequest
    {
        public static C2M_DiscardItem Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2M_DiscardItem>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        /// <summary>
        /// 物品ID
        /// </summary>
        [MemoryPackOrder(1)]
        public long ItemId { get; set; }
        /// <summary>
        /// 丢弃数量
        /// </summary>
        [MemoryPackOrder(2)]
        public int Count { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_DiscardItem)]
    public partial class M2C_DiscardItem : MessageObject, ILocationResponse
    {
        public static M2C_DiscardItem Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_DiscardItem>(isFromPool);
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

    // 整理背包
    [MemoryPackable]
    [Message(Opcode.C2M_SortBag)]
    [ResponseType(nameof(M2C_SortBag))]
    public partial class C2M_SortBag : MessageObject, ILocationRequest
    {
        public static C2M_SortBag Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2M_SortBag>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.M2C_SortBag)]
    public partial class M2C_SortBag : MessageObject, ILocationResponse
    {
        public static M2C_SortBag Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<M2C_SortBag>(isFromPool);
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
        public const ushort C2M_UseItem = 10801;
        public const ushort M2C_UseItem = 10802;
        public const ushort ItemData = 10803;
        public const ushort C2M_SyncBagData = 10804;
        public const ushort M2C_SyncBagData = 10805;
        public const ushort M2C_UpdateItem = 10806;
        public const ushort M2C_UpdateBagCapacity = 10807;
        public const ushort C2M_MoveItem = 10808;
        public const ushort M2C_MoveItem = 10809;
        public const ushort C2M_DiscardItem = 10810;
        public const ushort M2C_DiscardItem = 10811;
        public const ushort C2M_SortBag = 10812;
        public const ushort M2C_SortBag = 10813;
    }
}