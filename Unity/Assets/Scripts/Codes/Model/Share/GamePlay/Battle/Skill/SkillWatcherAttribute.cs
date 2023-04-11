namespace ET
{
	public class SkillWatcherAttribute : BaseAttribute
	{
		
		public ESkillEventType SkillEventType { get; }

		public SkillWatcherAttribute(ESkillEventType eventType)
		{
			this.SkillEventType = eventType;
		}
	}
}