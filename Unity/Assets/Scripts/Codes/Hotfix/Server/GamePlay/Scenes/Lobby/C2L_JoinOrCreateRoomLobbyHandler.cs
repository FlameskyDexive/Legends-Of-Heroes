namespace ET.Server
{
	[MessageHandler(SceneType.Lobby)]
	public class C2L_JoinOrCreateRoomLobbyHandler : AMRpcHandler<C2L_JoinOrCreateRoomLobby, L2C_JoinOrCreateRoomLobby>
	{
		protected override async ETTask Run(Session session, C2L_JoinOrCreateRoomLobby request, L2C_JoinOrCreateRoomLobby response)
		{
			// Player player = session.GetComponent<SessionPlayerComponent>().GetMyPlayer();
            Scene scene = session.DomainScene();
            Log.Info($"C2L_JoinOrCreateRoomLobbyHandler");
            //此处读表房间最大数量来判定，当前只有solo模式，最大数量为2
            Room room = scene.GetComponent<RoomManagerComponent>().GetAvalibleRoom(2);
            if (room != null)
            {
                //玩家加入房间
                Player player = scene.GetComponent<PlayerComponent>().Get(request.PlayerId);
                Log.Info($"player count:{room.AllPlayers.Count}, {player?.PlayerName}");
            }

            await ETTask.CompletedTask;
        }
	}
}