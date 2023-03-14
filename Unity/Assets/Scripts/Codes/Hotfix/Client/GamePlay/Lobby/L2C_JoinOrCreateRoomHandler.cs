

namespace ET.Client
{
	[MessageHandler(SceneType.Client)]
	public class L2C_JoinOrCreateRoomHandler : AMHandler<L2C_JoinOrCreateRoom>
	{
		protected override async ETTask Run(Session session, L2C_JoinOrCreateRoom message)
		{
			Log.Info($"L2C_JoinOrCreateRoom, {message.playerInfoRoom?.Count}");
            await ETTask.CompletedTask;
        }
	}
}
