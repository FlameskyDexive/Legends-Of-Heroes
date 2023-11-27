using System;
using System.Collections.Generic;

namespace ET
{
    [EntitySystemOf(typeof(SkillComponent))]
    [FriendOf(typeof(SkillComponent))]
    [FriendOf(typeof(BattleUnitComponent))]
    public static partial class SkillComponentSystem
    {

        [EntitySystem]
        private static void Awake(this SkillComponent self)
        {
            
        }
        
        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="self"></param>
        /// <param name="absType"></param>
        /// <param name="index"></param>
        public static bool SpellSkill(this SkillComponent self, ESkillAbstractType absType, int index = 0)
        {
            //
            self.Fiber().Log.Info($"spell skill {index}");
            Skill skill = null;
            self.GetParent<BattleUnitComponent>()?.TryGetSkill(absType, index, out skill);
            if (skill == null || skill.IsInCd())
                return false;
            skill.StartSpell();
            return true;
        }
    }

   
}