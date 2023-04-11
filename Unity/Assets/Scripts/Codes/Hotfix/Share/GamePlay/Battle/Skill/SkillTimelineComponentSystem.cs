using System;
using System.Collections.Generic;

namespace ET
{
    [FriendOf(typeof(SkillComponent))]
    public static class SkillTimeLineComponentSystem
    {
        [ObjectSystem]
        public class SkillTimeLineComponentAwakeSystem : AwakeSystem<SkillTimeLineComponent, int, int>
        {
            protected override void Awake(SkillTimeLineComponent self, int skillId, int skillLevel)
            {
                self.Awake(skillId, skillLevel);
            }
        }
        [ObjectSystem]
        public class SkillTimeLineComponentFixedUpdateSystem : FixedUpdateSystem<SkillTimeLineComponent>
        {
            protected override void FixedUpdate(SkillTimeLineComponent self)
            {
                self.FixedUpdate();
            }
        }

	

        private static void Awake(this SkillTimeLineComponent self, int skillId, int skillLevel)
        {
            //当前测试，一个事件一个字段，可以自己换成二维数组一个字段存多条事件数据
            SkillConfig skillconfig = SkillConfigCategory.Instance.GetByKeys(skillId, skillLevel);
            if (skillconfig?.Params.Length > 0)
            {
                self.AddChild<SkillEvent, SkillConfig>(skillconfig);
            }
        }
        
        /// <summary>
        /// 固定帧驱动
        /// </summary>
        /// <param name="self"></param>
        public static void FixedUpdate(this SkillTimeLineComponent self)
        {
            
        }
    }

   
}