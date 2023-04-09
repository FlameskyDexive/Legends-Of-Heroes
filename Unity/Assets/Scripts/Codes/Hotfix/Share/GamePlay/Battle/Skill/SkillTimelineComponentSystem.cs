using System;
using System.Collections.Generic;

namespace ET
{
    [FriendOf(typeof(SkillComponent))]
    public static class SkillTimeLineComponentSystem
    {
        [ObjectSystem]
        public class SkillTimeLineComponentAwakeSystem : AwakeSystem<SkillTimeLineComponent>
        {
            protected override void Awake(SkillTimeLineComponent self)
            {
                self.Awake();
            }
        }

	

        private static void Awake(this SkillTimeLineComponent self)
        {
            
        }
        
        public static void Update(this SkillTimeLineComponent self)
        {
            
        }
    }

   
}