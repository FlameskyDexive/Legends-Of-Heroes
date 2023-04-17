using System.Collections.Generic;
namespace ET
{
    [ObjectSystem]
    public class BattleUnitAwakeSystem : AwakeSystem<BattleUnitComponent,List<int>>
    {
        protected override void Awake(BattleUnitComponent self,List<int> skills)
        {
            self.Awake(skills);
        }
    }
    [ObjectSystem]
    public class BattleUnitAwakeSystem2 : AwakeSystem<BattleUnitComponent>
    {
        protected override void Awake(BattleUnitComponent self)
        {
            
        }
    }
    
    [ObjectSystem]
    public class BattleUnitDestroySystem : DestroySystem<BattleUnitComponent>
    {
        protected override void Destroy(BattleUnitComponent self)
        {
            self.IdSkillMap.Clear();
            self.AbstractTypeSkills.Clear();
        }
    }
    [FriendOf(typeof(BattleUnitComponent))]
    public static class BattleUnitComponentSystem
    {
        public static void Awake(this BattleUnitComponent self, List<int> skillIds)
        {
            // int activeSkillIndex = 0;
            foreach (int skillId in skillIds)
            {
                //测试先默认都1级技能，后续再做等级切换，同一个技能id高等级覆盖低等级。
                Skill skill = self.AddSkill(skillId);
            }
            self.AddComponent<SkillComponent>();
        }
        /// <summary>
        /// 添加技能
        /// </summary>
        /// <param name="self"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public static Skill AddSkill(this BattleUnitComponent self,int configId, int skillLevel = 1)
        {
            if (!self.IdSkillMap.TryGetValue(configId, out long skillId))
            {
                Skill skill = self.AddChild<Skill, int, int>(configId, skillLevel);
                self.IdSkillMap.Add(configId, skill.Id);
                SkillConfig skillConfig = SkillConfigCategory.Instance.GetByKeys(configId, skillLevel);
                ESkillAbstractType abstractType = (ESkillAbstractType)skillConfig.AbstractType;
                if (!self.AbstractTypeSkills.TryGetValue(abstractType, out List<long> skills))
                {
                    skills = new List<long>();
                    self.AbstractTypeSkills[abstractType] = skills;
                }
                self.AbstractTypeSkills[abstractType].Add(skill.Id);
                
            }
            return self.GetChild<Skill>(self.IdSkillMap[configId]);
        }

        public static bool TryGetSkill(this BattleUnitComponent self, int configId,out Skill skill)
        {
            if (self.IdSkillMap.TryGetValue(configId, out long skillId))
            {
                skill = self.GetChild<Skill>(self.IdSkillMap[configId]);
                return true;
            }
            skill = null;
            return false;
        }

        /// <summary>
        /// 通过技能类型获取技能
        /// </summary>
        /// <param name="self"></param>
        /// <param name="abstractType"></param>
        /// <param name="index"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static bool TryGetSkill(this BattleUnitComponent self, ESkillAbstractType abstractType, int index,out Skill skill)
        {
            if (self.AbstractTypeSkills.TryGetValue(abstractType, out List<long> skillIds))
            {
                if (skillIds?.Count > index)
                {
                    skill = self.GetChild<Skill>(skillIds[index]);
                    return true;
                }
            }
            skill = null;
            return false;
        }


        public static bool IsDead(this BattleUnitComponent self)
        {
            return self.Unit.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp) <= 0;
        }
    }
}