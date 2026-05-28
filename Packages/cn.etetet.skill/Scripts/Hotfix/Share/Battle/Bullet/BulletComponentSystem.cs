
using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ET
{

    [EntitySystemOf(typeof(BulletComponent))]
    [FriendOf(typeof(BulletComponent))]
    public static partial class BulletComponentSystem
    {
        [EntitySystem]
        public static void Destroy(this BulletComponent self)
        {
            
        }
        [EntitySystem]
        public static void Awake(this BulletComponent self)
        {
            //测试子弹，生存时间700ms
            self.EndTime = self.GetSingleton<TimeInfo>().ServerNow() + 1000;

        }

        public static void Init(this BulletComponent self, Skill skill, Unit owner)
        {
            self.OwnerSkill = skill;
            self.OwnerUnit = owner;
        }

        /// <summary>
        /// 每帧更新（原 IFixedUpdate，本项目用 IUpdate）
        /// </summary>
        /// <param name="self"></param>
        [EntitySystem]
        public static void Update(this BulletComponent self)
        {
            if (self.GetSingleton<TimeInfo>().ServerNow() > self.EndTime)
            {
                self.Root().GetComponent<UnitComponent>()?.Remove(self.GetParent<Unit>().Id);
            }
        }

    }
}