using ET;
using MemoryPack;
using System.Collections.Generic;
namespace ET
{
// 请求匹配
	[ResponseType(nameof(Match2G_StateSyncMatch))]
	[Message(StateSyncInner.G2Match_StateSyncMatch)]
	[MemoryPackable]
	public partial class G2Match_StateSyncMatch: MessageObject, IRequest
	{
		public static G2Match_StateSyncMatch Create(bool isFromPool = true) 
		{ 
			return !isFromPool? new G2Match_StateSyncMatch() : ObjectPool.Instance.Fetch(typeof(G2Match_StateSyncMatch)) as G2Match_StateSyncMatch; 
		}

		[MemoryPackOrder(0)]
		public int RpcId { get; set; }

		[MemoryPackOrder(1)]
		public long Id { get; set; }

		public override void Dispose() 
		{
			if (!this.IsFromPool) return;
			this.RpcId = default;
			this.Id = default;
			
			ObjectPool.Instance.Recycle(this); 
		}

	}

	[Message(StateSyncInner.Match2G_StateSyncMatch)]
	[MemoryPackable]
	public partial class Match2G_StateSyncMatch: MessageObject, IResponse
	{
		public static Match2G_StateSyncMatch Create(bool isFromPool = true) 
		{ 
			return !isFromPool? new Match2G_StateSyncMatch() : ObjectPool.Instance.Fetch(typeof(Match2G_StateSyncMatch)) as Match2G_StateSyncMatch; 
		}

		[MemoryPackOrder(0)]
		public int RpcId { get; set; }

		[MemoryPackOrder(1)]
		public int Error { get; set; }

		[MemoryPackOrder(2)]
		public string Message { get; set; }

		public override void Dispose() 
		{
			if (!this.IsFromPool) return;
			this.RpcId = default;
			this.Error = default;
			this.Message = default;
			
			ObjectPool.Instance.Recycle(this); 
		}

	}

	[ResponseType(nameof(Map2Match_GetRoom))]
	[Message(StateSyncInner.Match2Map_StateSyncGetRoom)]
	[MemoryPackable]
	public partial class Match2Map_StateSyncGetRoom: MessageObject, IRequest
	{
		public static Match2Map_StateSyncGetRoom Create(bool isFromPool = true) 
		{ 
			return !isFromPool? new Match2Map_StateSyncGetRoom() : ObjectPool.Instance.Fetch(typeof(Match2Map_StateSyncGetRoom)) as Match2Map_StateSyncGetRoom; 
		}

		[MemoryPackOrder(0)]
		public int RpcId { get; set; }

		[MemoryPackOrder(1)]
		public List<long> PlayerIds { get; set; } = new();

		public override void Dispose() 
		{
			if (!this.IsFromPool) return;
			this.RpcId = default;
			this.PlayerIds.Clear();
			
			ObjectPool.Instance.Recycle(this); 
		}

	}

	[Message(StateSyncInner.Map2Match_StateSyncGetRoom)]
	[MemoryPackable]
	public partial class Map2Match_StateSyncGetRoom: MessageObject, IResponse
	{
		public static Map2Match_StateSyncGetRoom Create(bool isFromPool = true) 
		{ 
			return !isFromPool? new Map2Match_StateSyncGetRoom() : ObjectPool.Instance.Fetch(typeof(Map2Match_StateSyncGetRoom)) as Map2Match_StateSyncGetRoom; 
		}

		[MemoryPackOrder(0)]
		public int RpcId { get; set; }

		[MemoryPackOrder(1)]
		public int Error { get; set; }

		[MemoryPackOrder(2)]
		public string Message { get; set; }

// 房间的ActorId
		[MemoryPackOrder(3)]
		public ActorId ActorId { get; set; }

		public override void Dispose() 
		{
			if (!this.IsFromPool) return;
			this.RpcId = default;
			this.Error = default;
			this.Message = default;
			this.ActorId = default;
			
			ObjectPool.Instance.Recycle(this); 
		}

	}

	[ResponseType(nameof(Room2G_StateSyncReconnect))]
	[Message(StateSyncInner.G2Room_StateSyncReconnect)]
	[MemoryPackable]
	public partial class G2Room_StateSyncReconnect: MessageObject, IRequest
	{
		public static G2Room_StateSyncReconnect Create(bool isFromPool = true) 
		{ 
			return !isFromPool? new G2Room_StateSyncReconnect() : ObjectPool.Instance.Fetch(typeof(G2Room_StateSyncReconnect)) as G2Room_StateSyncReconnect; 
		}

		[MemoryPackOrder(0)]
		public int RpcId { get; set; }

		[MemoryPackOrder(1)]
		public long PlayerId { get; set; }

		public override void Dispose() 
		{
			if (!this.IsFromPool) return;
			this.RpcId = default;
			this.PlayerId = default;
			
			ObjectPool.Instance.Recycle(this); 
		}

	}

	[Message(StateSyncInner.Room2G_StateSyncReconnect)]
	[MemoryPackable]
	public partial class Room2G_StateSyncReconnect: MessageObject, IResponse
	{
		public static Room2G_StateSyncReconnect Create(bool isFromPool = true) 
		{ 
			return !isFromPool? new Room2G_StateSyncReconnect() : ObjectPool.Instance.Fetch(typeof(Room2G_StateSyncReconnect)) as Room2G_StateSyncReconnect; 
		}

		[MemoryPackOrder(0)]
		public int RpcId { get; set; }

		[MemoryPackOrder(1)]
		public int Error { get; set; }

		[MemoryPackOrder(2)]
		public string Message { get; set; }

		[MemoryPackOrder(3)]
		public long StartTime { get; set; }

		[MemoryPackOrder(4)]
		public List<UnitInfo> UnitInfos { get; set; } = new();

		[MemoryPackOrder(5)]
		public int Frame { get; set; }

		public override void Dispose() 
		{
			if (!this.IsFromPool) return;
			this.RpcId = default;
			this.Error = default;
			this.Message = default;
			this.StartTime = default;
			this.UnitInfos.Clear();
			this.Frame = default;
			
			ObjectPool.Instance.Recycle(this); 
		}

	}

	[ResponseType(nameof(Room2RoomManager_StateSyncInit))]
	[Message(StateSyncInner.RoomManager2Room_StateSynInit)]
	[MemoryPackable]
	public partial class RoomManager2Room_StateSynInit: MessageObject, IRequest
	{
		public static RoomManager2Room_StateSynInit Create(bool isFromPool = true) 
		{ 
			return !isFromPool? new RoomManager2Room_StateSynInit() : ObjectPool.Instance.Fetch(typeof(RoomManager2Room_StateSynInit)) as RoomManager2Room_StateSynInit; 
		}

		[MemoryPackOrder(0)]
		public int RpcId { get; set; }

		[MemoryPackOrder(1)]
		public List<long> PlayerIds { get; set; } = new();

		public override void Dispose() 
		{
			if (!this.IsFromPool) return;
			this.RpcId = default;
			this.PlayerIds.Clear();
			
			ObjectPool.Instance.Recycle(this); 
		}

	}

	[Message(StateSyncInner.Room2RoomManager_StateSyncInit)]
	[MemoryPackable]
	public partial class Room2RoomManager_StateSyncInit: MessageObject, IResponse
	{
		public static Room2RoomManager_StateSyncInit Create(bool isFromPool = true) 
		{ 
			return !isFromPool? new Room2RoomManager_StateSyncInit() : ObjectPool.Instance.Fetch(typeof(Room2RoomManager_StateSyncInit)) as Room2RoomManager_StateSyncInit; 
		}

		[MemoryPackOrder(0)]
		public int RpcId { get; set; }

		[MemoryPackOrder(1)]
		public int Error { get; set; }

		[MemoryPackOrder(2)]
		public string Message { get; set; }

		public override void Dispose() 
		{
			if (!this.IsFromPool) return;
			this.RpcId = default;
			this.Error = default;
			this.Message = default;
			
			ObjectPool.Instance.Recycle(this); 
		}

	}

	public static class StateSyncInner
	{
		 public const ushort G2Match_StateSyncMatch = 22002;
		 public const ushort Match2G_StateSyncMatch = 22003;
		 public const ushort Match2Map_StateSyncGetRoom = 22004;
		 public const ushort Map2Match_StateSyncGetRoom = 22005;
		 public const ushort G2Room_StateSyncReconnect = 22006;
		 public const ushort Room2G_StateSyncReconnect = 22007;
		 public const ushort RoomManager2Room_StateSynInit = 22008;
		 public const ushort Room2RoomManager_StateSyncInit = 22009;
	}
}
