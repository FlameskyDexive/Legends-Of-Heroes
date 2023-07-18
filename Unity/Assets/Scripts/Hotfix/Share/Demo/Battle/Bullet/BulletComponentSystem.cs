
using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;

namespace ET
{

    [Invoke(TimerInvokeType.BulletLifeTimeout)]
    public class BulletLifeTimeout : ATimer<BulletComponent>
    {
        protected override void Run(BulletComponent self)
        {
            try
            {
                self.Root().GetComponent<UnitComponent>()?.Remove(self.GetParent<Unit>().Id);
            }
            catch (Exception e)
            {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }

    [FriendOf(typeof(BulletComponent))]
    public static class BulletComponentSystem
    {
        public static void Destroy(this BulletComponent self)
        {
            self.Fiber().TimerComponent.Remove(ref self.Timer);
        }
        public static void Awake(this BulletComponent self)
        {
            //测试子弹，生存时间700ms
            self.Timer = self.Fiber().TimerComponent.NewOnceTimer(self.Fiber().TimeInfo.ServerNow() + 700, TimerInvokeType.BulletLifeTimeout, self);

        }

        public static void Init(this BulletComponent self, Skill skill, Unit owner)
        {
            self.OwnerSkill = skill;
            self.OwnerUnit = owner;
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="self"></param>
        public static void FixedUpdate(this BulletComponent self)
        {
            
        }

    }
}