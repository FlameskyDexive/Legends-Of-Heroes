using System.Collections.Generic;

namespace ET.Server
{
	[ActorMessageHandler(SceneType.Lobby)]
    public class G2Lobby_JoinOrCreateRoomHandler : AMActorRpcHandler<Scene, G2Lobby_JoinOrCreateRoom, Lobby2G_JoinOrCreateRoom>
    {
		protected override async ETTask Run(Scene scene, G2Lobby_JoinOrCreateRoom request, Lobby2G_JoinOrCreateRoom response)
		{
            //gate与lobby进行内网通信，获取可用房间


            Log.Info($"G2Lobby_JoinOrCreateRoom");
            //此处读表房间最大数量来判定，当前只有solo模式，最大数量为2
            Room room = scene.GetComponent<RoomManagerComponent>().GetAvalibleRoom(request.PlayerId, 2);
            int campId = room?.AllPlayers?.Count > 0 ? 0 : 1;
            if (room != null)
            {
                //玩家加入房间
                PlayerInfoRoom PlayerInfoRoom = new PlayerInfoRoom()
                {
                    PlayerId = request.PlayerId,
                    RoomId = room.Id,
                    CampId = campId,
                };
                room.AllPlayers[request.PlayerId] = PlayerInfoRoom;
                room.CampPlayers[campId] = new List<long>(){ request.PlayerId };
                // Player player = scene.GetComponent<PlayerComponent>().Get(request.PlayerId);
                // Log.Info($"player count:{room.AllPlayers.Count}, {player?.PlayerName}");
            }

            response.roomInfo = new RoomInfoProto();
            response.roomInfo.RoomId = room.Id;
            response.roomInfo.CampId = campId;
            response.roomInfo.RoomOwnerId = room.RoomOwnerPlayerId;
            response.roomInfo.playerInfoRoom = new List<PlayerInfoRoom>(room.AllPlayers.Values);
            await ETTask.CompletedTask;
        }
	}
}