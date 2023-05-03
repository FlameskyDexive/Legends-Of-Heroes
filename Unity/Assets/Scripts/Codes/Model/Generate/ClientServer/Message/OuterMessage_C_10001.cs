using ET;
using ProtoBuf;
using System.Collections.Generic;
namespace ET
{
	[Message(OuterMessage.HttpGetRouterResponse)]
	[ProtoContract]
	public partial class HttpGetRouterResponse: ProtoObject
	{
		[ProtoMember(1)]
		public List<string> Realms { get; set; }

		[ProtoMember(2)]
		public List<string> Routers { get; set; }

	}

	[Message(OuterMessage.RouterSync)]
	[ProtoContract]
	public partial class RouterSync: ProtoObject
	{
		[ProtoMember(1)]
		public uint ConnectId { get; set; }

		[ProtoMember(2)]
		public string Address { get; set; }

	}

	[ResponseType(nameof(M2C_TestResponse))]
	[Message(OuterMessage.C2M_TestRequest)]
	[ProtoContract]
	public partial class C2M_TestRequest: ProtoObject, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public string request { get; set; }

	}

	[Message(OuterMessage.M2C_TestResponse)]
	[ProtoContract]
	public partial class M2C_TestResponse: ProtoObject, IActorResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public string response { get; set; }

	}

	[ResponseType(nameof(Actor_TransferResponse))]
	[Message(OuterMessage.Actor_TransferRequest)]
	[ProtoContract]
	public partial class Actor_TransferRequest: ProtoObject, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int MapIndex { get; set; }

	}

	[Message(OuterMessage.Actor_TransferResponse)]
	[ProtoContract]
	public partial class Actor_TransferResponse: ProtoObject, IActorLocationResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2C_EnterMap))]
	[Message(OuterMessage.C2G_EnterMap)]
	[ProtoContract]
	public partial class C2G_EnterMap: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.G2C_EnterMap)]
	[ProtoContract]
	public partial class G2C_EnterMap: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

// 自己unitId
		[ProtoMember(4)]
		public long MyId { get; set; }

	}

	[Message(OuterMessage.MoveInfo)]
	[ProtoContract]
	public partial class MoveInfo: ProtoObject
	{
		[ProtoMember(1)]
		public List<Unity.Mathematics.float3> Points { get; set; }

		[ProtoMember(2)]
		public Unity.Mathematics.quaternion Rotation { get; set; }

		[ProtoMember(3)]
		public int TurnSpeed { get; set; }

	}

	[Message(OuterMessage.UnitInfo)]
	[ProtoContract]
	public partial class UnitInfo: ProtoObject
	{
		[ProtoMember(1)]
		public long UnitId { get; set; }

		[ProtoMember(2)]
		public int ConfigId { get; set; }

		[ProtoMember(3)]
		public int Type { get; set; }

		[ProtoMember(4)]
		public Unity.Mathematics.float3 Position { get; set; }

		[ProtoMember(5)]
		public Unity.Mathematics.float3 Forward { get; set; }

		[MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]
		[ProtoMember(6)]
		public Dictionary<int, long> KV { get; set; }
		[ProtoMember(7)]
		public MoveInfo MoveInfo { get; set; }

		[ProtoMember(8)]
		public long OwnerId { get; set; }

		[MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]
		[ProtoMember(9)]
		public Dictionary<int, int> Skills { get; set; }
	}

	[Message(OuterMessage.M2C_CreateUnits)]
	[ProtoContract]
	public partial class M2C_CreateUnits: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public List<UnitInfo> Units { get; set; }

	}

	[Message(OuterMessage.M2C_CreateMyUnit)]
	[ProtoContract]
	public partial class M2C_CreateMyUnit: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public UnitInfo Unit { get; set; }

	}

	[Message(OuterMessage.M2C_StartSceneChange)]
	[ProtoContract]
	public partial class M2C_StartSceneChange: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public long SceneInstanceId { get; set; }

		[ProtoMember(2)]
		public string SceneName { get; set; }

	}

	[Message(OuterMessage.M2C_RemoveUnits)]
	[ProtoContract]
	public partial class M2C_RemoveUnits: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public List<long> Units { get; set; }

	}

	[Message(OuterMessage.C2M_PathfindingResult)]
	[ProtoContract]
	public partial class C2M_PathfindingResult: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public Unity.Mathematics.float3 Position { get; set; }

	}

	[Message(OuterMessage.C2M_Stop)]
	[ProtoContract]
	public partial class C2M_Stop: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.M2C_PathfindingResult)]
	[ProtoContract]
	public partial class M2C_PathfindingResult: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public long Id { get; set; }

		[ProtoMember(2)]
		public Unity.Mathematics.float3 Position { get; set; }

		[ProtoMember(3)]
		public List<Unity.Mathematics.float3> Points { get; set; }

	}

	[Message(OuterMessage.M2C_Stop)]
	[ProtoContract]
	public partial class M2C_Stop: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public int Error { get; set; }

		[ProtoMember(2)]
		public long Id { get; set; }

		[ProtoMember(3)]
		public Unity.Mathematics.float3 Position { get; set; }

		[ProtoMember(4)]
		public Unity.Mathematics.quaternion Rotation { get; set; }

	}

	[ResponseType(nameof(G2C_Ping))]
	[Message(OuterMessage.C2G_Ping)]
	[ProtoContract]
	public partial class C2G_Ping: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.G2C_Ping)]
	[ProtoContract]
	public partial class G2C_Ping: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public long Time { get; set; }

	}

	[Message(OuterMessage.G2C_Test)]
	[ProtoContract]
	public partial class G2C_Test: ProtoObject, IMessage
	{
	}

	[ResponseType(nameof(M2C_Reload))]
	[Message(OuterMessage.C2M_Reload)]
	[ProtoContract]
	public partial class C2M_Reload: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public string Account { get; set; }

		[ProtoMember(3)]
		public string Password { get; set; }

	}

	[Message(OuterMessage.M2C_Reload)]
	[ProtoContract]
	public partial class M2C_Reload: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(R2C_Login))]
	[Message(OuterMessage.C2R_Login)]
	[ProtoContract]
	public partial class C2R_Login: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public string Account { get; set; }

		[ProtoMember(3)]
		public string Password { get; set; }

	}

	[Message(OuterMessage.R2C_Login)]
	[ProtoContract]
	public partial class R2C_Login: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public string Address { get; set; }

		[ProtoMember(5)]
		public long Key { get; set; }

		[ProtoMember(6)]
		public long GateId { get; set; }

	}

	[ResponseType(nameof(G2C_LoginGate))]
	[Message(OuterMessage.C2G_LoginGate)]
	[ProtoContract]
	public partial class C2G_LoginGate: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long Key { get; set; }

		[ProtoMember(3)]
		public long GateId { get; set; }

	}

	[Message(OuterMessage.G2C_LoginGate)]
	[ProtoContract]
	public partial class G2C_LoginGate: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public long PlayerId { get; set; }

		[ProtoMember(5)]
		public string LobbyAddress { get; set; }

		[ProtoMember(6)]
		public string PlayerName { get; set; }

		[ProtoMember(7)]
		public int AvatarIndex { get; set; }

		[ProtoMember(8)]
		public long LobbyActorId { get; set; }

	}

	[Message(OuterMessage.G2C_TestHotfixMessage)]
	[ProtoContract]
	public partial class G2C_TestHotfixMessage: ProtoObject, IMessage
	{
		[ProtoMember(1)]
		public string Info { get; set; }

	}

	[ResponseType(nameof(M2C_TestRobotCase))]
	[Message(OuterMessage.C2M_TestRobotCase)]
	[ProtoContract]
	public partial class C2M_TestRobotCase: ProtoObject, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int N { get; set; }

	}

	[Message(OuterMessage.M2C_TestRobotCase)]
	[ProtoContract]
	public partial class M2C_TestRobotCase: ProtoObject, IActorLocationResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public int N { get; set; }

	}

	[Message(OuterMessage.C2M_TestRobotCase2)]
	[ProtoContract]
	public partial class C2M_TestRobotCase2: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int N { get; set; }

	}

	[Message(OuterMessage.M2C_TestRobotCase2)]
	[ProtoContract]
	public partial class M2C_TestRobotCase2: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int N { get; set; }

	}

	[ResponseType(nameof(M2C_TransferMap))]
	[Message(OuterMessage.C2M_TransferMap)]
	[ProtoContract]
	public partial class C2M_TransferMap: ProtoObject, IActorLocationRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.M2C_TransferMap)]
	[ProtoContract]
	public partial class M2C_TransferMap: ProtoObject, IActorLocationResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

	}

	[ResponseType(nameof(G2C_Benchmark))]
	[Message(OuterMessage.C2G_Benchmark)]
	[ProtoContract]
	public partial class C2G_Benchmark: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

	}

	[Message(OuterMessage.G2C_Benchmark)]
	[ProtoContract]
	public partial class G2C_Benchmark: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

	}

///////////////////////////////// Map Start ///////////////////////////////////
	[ResponseType(nameof(M2C_JoystickMove))]
	[Message(OuterMessage.C2M_JoystickMove)]
	[ProtoContract]
	public partial class C2M_JoystickMove: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public Unity.Mathematics.float3 MoveForward { get; set; }

	}

	[Message(OuterMessage.M2C_JoystickMove)]
	[ProtoContract]
	public partial class M2C_JoystickMove: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public long Id { get; set; }

		[ProtoMember(2)]
		public Unity.Mathematics.float3 Position { get; set; }

		[ProtoMember(3)]
		public Unity.Mathematics.float3 MoveForward { get; set; }

	}

	[ResponseType(nameof(M2C_Operation))]
	[Message(OuterMessage.C2M_Operation)]
	[ProtoContract]
	public partial class C2M_Operation: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long Id { get; set; }

		[ProtoMember(3)]
		public List<OperateInfo> OperateInfos { get; set; }

	}

	[Message(OuterMessage.M2C_Operation)]
	[ProtoContract]
	public partial class M2C_Operation: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public long Id { get; set; }

		[ProtoMember(2)]
		public List<OperateReplyInfo> OperateInfos { get; set; }

	}

	[Message(OuterMessage.OperateInfo)]
	[ProtoContract]
	public partial class OperateInfo: ProtoObject
	{
		[ProtoMember(1)]
		public int OperateType { get; set; }

		[ProtoMember(2)]
		public int InputType { get; set; }

		[ProtoMember(3)]
		public Unity.Mathematics.float3 Vec3 { get; set; }

		[ProtoMember(4)]
		public long Value1 { get; set; }

		[ProtoMember(5)]
		public long Value2 { get; set; }

	}

	[Message(OuterMessage.OperateReplyInfo)]
	[ProtoContract]
	public partial class OperateReplyInfo: ProtoObject
	{
		[ProtoMember(1)]
		public int OperateType { get; set; }

		[ProtoMember(2)]
		public int Status { get; set; }

	}

	[Message(OuterMessage.M2C_SyncUnits)]
	[ProtoContract]
	public partial class M2C_SyncUnits: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public List<UnitInfo> Units { get; set; }

	}

///////////////////////////////// Map End ///////////////////////////////////
///////////////////////////////// 房间相关  START ///////////////////////////////////
	[Message(OuterMessage.C2L_JoinOrCreateRoom)]
	[ProtoContract]
	public partial class C2L_JoinOrCreateRoom: ProtoObject, IActorLocationMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long PlayerId { get; set; }

		[ProtoMember(3)]
		public long RoomId { get; set; }

	}

	[Message(OuterMessage.L2C_JoinOrCreateRoom)]
	[ProtoContract]
	public partial class L2C_JoinOrCreateRoom: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public RoomInfoProto roomInfo { get; set; }

	}

	[ResponseType(nameof(G2C_JoinOrCreateRoom))]
	[Message(OuterMessage.C2G_JoinOrCreateRoom)]
	[ProtoContract]
	public partial class C2G_JoinOrCreateRoom: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long PlayerId { get; set; }

		[ProtoMember(3)]
		public long RoomId { get; set; }

		[ProtoMember(4)]
		public int PlayMode { get; set; }

	}

	[Message(OuterMessage.G2C_JoinOrCreateRoom)]
	[ProtoContract]
	public partial class G2C_JoinOrCreateRoom: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public RoomInfoProto roomInfo { get; set; }

		[ProtoMember(5)]
		public int PlayMode { get; set; }

	}

	[Message(OuterMessage.G2C_UpdateRoomPlayers)]
	[ProtoContract]
	public partial class G2C_UpdateRoomPlayers: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public RoomInfoProto roomInfo { get; set; }

		[ProtoMember(5)]
		public int PlayMode { get; set; }

	}

	[Message(OuterMessage.RoomInfoProto)]
	[ProtoContract]
	public partial class RoomInfoProto: ProtoObject
	{
		[ProtoMember(1)]
		public long RoomId { get; set; }

		[ProtoMember(2)]
		public int CampId { get; set; }

		[ProtoMember(3)]
		public long RoomOwnerId { get; set; }

		[ProtoMember(4)]
		public string RoomName { get; set; }

		[ProtoMember(5)]
		public List<PlayerInfoRoom> playerInfoRoom { get; set; }

		[ProtoMember(6)]
		public int PlayMode { get; set; }

		[ProtoMember(7)]
		public bool IsReady { get; set; }

	}

	[Message(OuterMessage.PlayerInfoRoom)]
	[ProtoContract]
	public partial class PlayerInfoRoom: ProtoObject
	{
		[ProtoMember(1)]
		public string Account { get; set; }

		[ProtoMember(2)]
		public long UnitId { get; set; }

		[ProtoMember(3)]
		public long SessionId { get; set; }

		[ProtoMember(4)]
		public long RoomId { get; set; }

		[ProtoMember(5)]
		public int CampId { get; set; }

		[ProtoMember(6)]
		public long PlayerId { get; set; }

		[ProtoMember(7)]
		public string PlayerName { get; set; }

		[ProtoMember(8)]
		public int AvatarIndex { get; set; }

	}

	[ResponseType(nameof(G2C_CreateNewRoom))]
	[Message(OuterMessage.C2G_CreateNewRoom)]
	[ProtoContract]
	public partial class C2G_CreateNewRoom: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long PlayerId { get; set; }

		[ProtoMember(3)]
		public int PlayMode { get; set; }

	}

	[Message(OuterMessage.G2C_CreateNewRoom)]
	[ProtoContract]
	public partial class G2C_CreateNewRoom: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public int RoomId { get; set; }

		[ProtoMember(5)]
		public int PlayMode { get; set; }

		[ProtoMember(6)]
		public int CampId { get; set; }

	}

	[Message(OuterMessage.G2C_PlayerTriggerRoom)]
	[ProtoContract]
	public partial class G2C_PlayerTriggerRoom: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long ActorId { get; set; }

		[ProtoMember(3)]
		public PlayerInfoRoom playerInfoRoom { get; set; }

		[ProtoMember(4)]
		public bool JoinOrLeave { get; set; }

	}

	[ResponseType(nameof(G2C_LeaveRoom))]
	[Message(OuterMessage.C2G_LeaveRoom)]
	[ProtoContract]
	public partial class C2G_LeaveRoom: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long PlayerId { get; set; }

	}

	[Message(OuterMessage.G2C_LeaveRoom)]
	[ProtoContract]
	public partial class G2C_LeaveRoom: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public int CampId { get; set; }

		[ProtoMember(5)]
		public long newRoomOwner { get; set; }

		[ProtoMember(6)]
		public long RoomId { get; set; }

		[ProtoMember(7)]
		public long PlayerId { get; set; }

		[ProtoMember(8)]
		public bool IsDestory { get; set; }

	}

	[Message(OuterMessage.PlayerBattleInfo)]
	[ProtoContract]
	public partial class PlayerBattleInfo: ProtoObject
	{
		[ProtoMember(1)]
		public long PlayerId { get; set; }

		[ProtoMember(2)]
		public int HeroConfig { get; set; }

	}

	[Message(OuterMessage.G2C_StartEnterMap)]
	[ProtoContract]
	public partial class G2C_StartEnterMap: ProtoObject, IActorMessage
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

// 自己unitId
		[ProtoMember(4)]
		public long MyId { get; set; }

	}

	[ResponseType(nameof(G2C_StartGame))]
	[Message(OuterMessage.C2G_StartGame)]
	[ProtoContract]
	public partial class C2G_StartGame: ProtoObject, IRequest
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public long PlayerId { get; set; }

		[ProtoMember(3)]
		public List<PlayerBattleInfo> PlayerBattleInfos { get; set; }

	}

	[Message(OuterMessage.G2C_StartGame)]
	[ProtoContract]
	public partial class G2C_StartGame: ProtoObject, IResponse
	{
		[ProtoMember(1)]
		public int RpcId { get; set; }

		[ProtoMember(2)]
		public int Error { get; set; }

		[ProtoMember(3)]
		public string Message { get; set; }

		[ProtoMember(4)]
		public List<long> list { get; set; }

	}

	[Message(OuterMessage.PlayerBattlePoint)]
	[ProtoContract]
	public partial class PlayerBattlePoint: ProtoObject
	{
		[ProtoMember(1)]
		public long PlayerId { get; set; }

		[ProtoMember(2)]
		public int Point { get; set; }

		[ProtoMember(3)]
		public int SingleMatchCount { get; set; }

		[ProtoMember(4)]
		public string Account { get; set; }

		[ProtoMember(5)]
		public long UnitId { get; set; }

	}

	[Message(OuterMessage.G2C_PrepareToEnterBattle)]
	[ProtoContract]
	public partial class G2C_PrepareToEnterBattle: ProtoObject, IMessage
	{
	}

	[Message(OuterMessage.C2G_PreparedToEnterBattle)]
	[ProtoContract]
	public partial class C2G_PreparedToEnterBattle: ProtoObject, IMessage
	{
		[ProtoMember(1)]
		public long PlayerId { get; set; }

	}

	[Message(OuterMessage.G2C_AllowToEnterMap)]
	[ProtoContract]
	public partial class G2C_AllowToEnterMap: ProtoObject, IMessage
	{
	}

///////////////////////////////// 房间相关  END ///////////////////////////////////
	public static class OuterMessage
	{
		 public const ushort HttpGetRouterResponse = 10002;
		 public const ushort RouterSync = 10003;
		 public const ushort C2M_TestRequest = 10004;
		 public const ushort M2C_TestResponse = 10005;
		 public const ushort Actor_TransferRequest = 10006;
		 public const ushort Actor_TransferResponse = 10007;
		 public const ushort C2G_EnterMap = 10008;
		 public const ushort G2C_EnterMap = 10009;
		 public const ushort MoveInfo = 10010;
		 public const ushort UnitInfo = 10011;
		 public const ushort M2C_CreateUnits = 10012;
		 public const ushort M2C_CreateMyUnit = 10013;
		 public const ushort M2C_StartSceneChange = 10014;
		 public const ushort M2C_RemoveUnits = 10015;
		 public const ushort C2M_PathfindingResult = 10016;
		 public const ushort C2M_Stop = 10017;
		 public const ushort M2C_PathfindingResult = 10018;
		 public const ushort M2C_Stop = 10019;
		 public const ushort C2G_Ping = 10020;
		 public const ushort G2C_Ping = 10021;
		 public const ushort G2C_Test = 10022;
		 public const ushort C2M_Reload = 10023;
		 public const ushort M2C_Reload = 10024;
		 public const ushort C2R_Login = 10025;
		 public const ushort R2C_Login = 10026;
		 public const ushort C2G_LoginGate = 10027;
		 public const ushort G2C_LoginGate = 10028;
		 public const ushort G2C_TestHotfixMessage = 10029;
		 public const ushort C2M_TestRobotCase = 10030;
		 public const ushort M2C_TestRobotCase = 10031;
		 public const ushort C2M_TestRobotCase2 = 10032;
		 public const ushort M2C_TestRobotCase2 = 10033;
		 public const ushort C2M_TransferMap = 10034;
		 public const ushort M2C_TransferMap = 10035;
		 public const ushort C2G_Benchmark = 10036;
		 public const ushort G2C_Benchmark = 10037;
		 public const ushort C2M_JoystickMove = 10038;
		 public const ushort M2C_JoystickMove = 10039;
		 public const ushort C2M_Operation = 10040;
		 public const ushort M2C_Operation = 10041;
		 public const ushort OperateInfo = 10042;
		 public const ushort OperateReplyInfo = 10043;
		 public const ushort M2C_SyncUnits = 10044;
		 public const ushort C2L_JoinOrCreateRoom = 10045;
		 public const ushort L2C_JoinOrCreateRoom = 10046;
		 public const ushort C2G_JoinOrCreateRoom = 10047;
		 public const ushort G2C_JoinOrCreateRoom = 10048;
		 public const ushort G2C_UpdateRoomPlayers = 10049;
		 public const ushort RoomInfoProto = 10050;
		 public const ushort PlayerInfoRoom = 10051;
		 public const ushort C2G_CreateNewRoom = 10052;
		 public const ushort G2C_CreateNewRoom = 10053;
		 public const ushort G2C_PlayerTriggerRoom = 10054;
		 public const ushort C2G_LeaveRoom = 10055;
		 public const ushort G2C_LeaveRoom = 10056;
		 public const ushort PlayerBattleInfo = 10057;
		 public const ushort G2C_StartEnterMap = 10058;
		 public const ushort C2G_StartGame = 10059;
		 public const ushort G2C_StartGame = 10060;
		 public const ushort PlayerBattlePoint = 10061;
		 public const ushort G2C_PrepareToEnterBattle = 10062;
		 public const ushort C2G_PreparedToEnterBattle = 10063;
		 public const ushort G2C_AllowToEnterMap = 10064;
	}
}
