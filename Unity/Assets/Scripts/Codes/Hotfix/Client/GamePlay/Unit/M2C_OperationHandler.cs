namespace ET.Client
{
	[MessageHandler(SceneType.Client)]
	public class M2C_OperationHandler : AMHandler<M2C_Operation>
	{
		protected override async ETTask Run(Session session, M2C_Operation message)
		{
			Log.Info($" operation back:{message?.OperateInfos?.Count}");
            if (message?.OperateInfos?.Count > 0)
            {
                Unit unit = UnitHelper.GetMyUnitFromClientScene(session.DomainScene());
                foreach (OperateReplyInfo info in message.OperateInfos)
                {
                    EOperateType operateType = (EOperateType)info.OperateType;
                    switch (operateType)
                    {
                        case EOperateType.Skill1:
                            unit.GetComponent<BattleUnitComponent>().GetComponent<SkillComponent>().SpellSkill(ESkillAbstractType.ActiveSkill);
                            break;
                        case EOperateType.Skill2:
                            unit.GetComponent<BattleUnitComponent>().GetComponent<SkillComponent>().SpellSkill(ESkillAbstractType.ActiveSkill, 1);
                            break;
                    }
                }
            }
			await ETTask.CompletedTask;
		}
	}
}
