namespace ET.Client
{
	public class SessionComponentDestroySystem: DestroySystem<PlayerSessionComponent>
	{
		protected override void Destroy(PlayerSessionComponent self)
		{
			self.Session?.Dispose();
		}
	}
}
