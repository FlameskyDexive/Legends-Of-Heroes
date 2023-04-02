using System;
using System.Collections.Generic;

namespace ET
{
    [FriendOf(typeof(SkillComponent))]
    public static class SkillComponentSystem
    {
        [ObjectSystem]
        public class SkillComponentAwakeSystem : AwakeSystem<SkillComponent>
        {
            protected override void Awake(SkillComponent self)
            {
                self.Init();
            }
        }

	

        private static void Init(this SkillComponent self)
        {
            
        }
        
        public static void PlaySkill(this SkillComponent self)
        {
            
        }
    }

   
}