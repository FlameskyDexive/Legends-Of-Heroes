namespace ET
{
	/// <summary>
	/// 技能发射子弹
	/// </summary>
	[SkillWatcher(ESkillEventType.Bullet)]
	public class SkillWatcher_Bullet : ISkillWatcher
	{
		public void Run(Entity entity, EventType.SkillEventType args)
		{
            Log.Info($"emit a bullet");
		}
	}
}
