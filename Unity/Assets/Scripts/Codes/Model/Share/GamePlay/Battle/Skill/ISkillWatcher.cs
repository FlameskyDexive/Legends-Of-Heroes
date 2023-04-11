namespace ET
{
	public interface ISkillWatcher
	{
		void Run(Unit unit, EventType.SkillEventType args);
	}
}
