namespace ET.Client
{
	[MessageHandler(SceneType.Client)]
	public class M2C_OperationHandler : AMHandler<M2C_Operation>
	{
		protected override async ETTask Run(Session session, M2C_Operation message)
		{
			Log.Info($" operation back:{message?.OperateInfos?.Count}");
			await ETTask.CompletedTask;
		}
	}
}
