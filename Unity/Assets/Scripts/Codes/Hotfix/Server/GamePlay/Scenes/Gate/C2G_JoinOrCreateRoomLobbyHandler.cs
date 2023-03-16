using System.Collections.Generic;

namespace ET.Server
{
	[MessageHandler(SceneType.Gate)]
	public class C2G_JoinOrCreateRoomLobbyHandler : AMRpcHandler<C2G_JoinOrCreateRoom, G2C_JoinOrCreateRoom>
	{
		protected override async ETTask Run(Session session, C2G_JoinOrCreateRoom request, G2C_JoinOrCreateRoom response)
		{
            Scene scene = session.DomainScene();
            PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();

            //gate与lobby进行内网通信，获取可用房间在返回给Client
            StartSceneConfig config = StartSceneConfigCategory.Instance.GetBySceneName(scene.Zone, "Lobby");
            Lobby2G_JoinOrCreateRoom lobby2GJoinOrCreateRoom = (Lobby2G_JoinOrCreateRoom)await ActorMessageSenderComponent.Instance.Call(
                config.InstanceId, new G2Lobby_JoinOrCreateRoom() {PlayerId = request.PlayerId, RoomId = request.RoomId});

            Log.Info($"C2G_JoinOrCreateRoomLobbyHandler, {lobby2GJoinOrCreateRoom.roomInfo.playerInfoRoom?.Count}");
            //此处根据返回来房间的playerId获取Player的信息，赋值，并返回给Client
            if (lobby2GJoinOrCreateRoom.roomInfo?.playerInfoRoom?.Count > 0)
            {
                foreach (PlayerInfoRoom playerInfoRoom in lobby2GJoinOrCreateRoom.roomInfo.playerInfoRoom)
                {
                    Player player = playerComponent.Get(playerInfoRoom.PlayerId);
                    playerInfoRoom.Account = player.Account;
                    playerInfoRoom.PlayerName = player.PlayerName;
                    playerInfoRoom.AvatarIndex = player.AvatarIndex;
                    if (request.PlayerId == playerInfoRoom.PlayerId)
                    {
                        playerInfoRoom.SessionId = session.InstanceId;
                    }
                    Log.Info($"room player:{player.PlayerName}");
                }

                //广播给房间其他的玩家，通知消息房间信息变动
                G2C_UpdateRoomPlayers updateRoomPlayers = new G2C_UpdateRoomPlayers()
                {
                    roomInfo = lobby2GJoinOrCreateRoom.roomInfo

                };
                foreach (PlayerInfoRoom playerInfoRoom in lobby2GJoinOrCreateRoom.roomInfo.playerInfoRoom)
                {
                    MessageHelper.SendActor(playerInfoRoom.SessionId, updateRoomPlayers);
                }
            }

            response.roomInfo = lobby2GJoinOrCreateRoom.roomInfo;

            await ETTask.CompletedTask;
        }
	}
}