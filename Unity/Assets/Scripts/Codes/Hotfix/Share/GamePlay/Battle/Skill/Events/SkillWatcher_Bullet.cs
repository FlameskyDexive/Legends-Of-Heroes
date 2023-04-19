namespace ET
{
	/// <summary>
	/// 技能发射子弹
	/// </summary>
	[SkillWatcher(ESkillEventType.Bullet)]
	[FriendOf(typeof(SkillEvent))]
	public class SkillWatcher_Bullet : ISkillWatcher
	{
		public void Run(SkillEvent skillEvent, EventType.SkillEventType args)
		{
            Log.Info($"emit a bullet");
            Unit owner = args.owner;
            if (owner == null)
                return;
#if DOTNET
            Unit bullet = Server.UnitFactory.CreateBullet(skillEvent.DomainScene(), IdGenerater.Instance.GenerateUnitId(skillEvent.DomainZone()), skillEvent.Skill, -1000, skillEvent.EventData);

            // 通知客户端创建子弹Unit
            M2C_CreateMyUnit m2CCreateUnits = new M2C_CreateMyUnit();
            m2CCreateUnits.Unit = Server.UnitHelper.CreateUnitInfo(bullet);
            Server.MessageHelper.SendToClient(bullet, m2CCreateUnits);
#endif
        }
	}
}
