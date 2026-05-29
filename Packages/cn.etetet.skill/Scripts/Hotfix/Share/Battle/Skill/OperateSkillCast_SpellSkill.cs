namespace ET
{
    // 订阅操作触发的技能释放事件(由 cn.etetet.mapplay 的 C2M_OperationHandler 发布)。
    // 只在服务端地图场景(SceneType.Map)触发, 调用本包的 SkillComponent.SpellSkill 真正释放技能。
    // 这样 mapplay(低层) 无需反向依赖 skill(高层): mapplay 只发事件, skill 订阅响应。
    [Event(SceneType.Map)]
    public class OperateSkillCast_SpellSkill : AEvent<Scene, OperateSkillCast>
    {
        protected override async ETTask Run(Scene scene, OperateSkillCast a)
        {
            Unit unit = a.Unit;
            if (unit == null || unit.IsDisposed)
            {
                return;
            }

            SkillComponent skillComponent = unit.GetComponent<SkillComponent>();
            if (skillComponent == null)
            {
                return;
            }

            // Ori 中 Skill1/Skill2 均为 ActiveSkill, 以 index 区分槽位。
            skillComponent.SpellSkill(ESkillAbstractType.ActiveSkill, a.Index);

            await ETTask.CompletedTask;
        }
    }
}
