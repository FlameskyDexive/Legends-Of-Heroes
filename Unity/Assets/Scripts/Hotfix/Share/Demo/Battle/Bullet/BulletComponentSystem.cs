
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

    [EntitySystemOf(typeof(BulletComponent))]
    [FriendOf(typeof(BulletComponent))]
    public static partial class BulletComponentSystem
    {
        [EntitySystem]
        public static void Destroy(this BulletComponent self)
        {
            self.Root().GetComponent<TimerComponent>()?.Remove(ref self.Timer);
        }
        [EntitySystem]
        public static void Awake(this BulletComponent self)
        {
            //测试子弹，生存时间700ms
            self.Timer = self.Timer = self.Root().GetComponent<TimerComponent>().NewOnceTimer(TimeInfo.Instance.ServerNow() + 700, TimerInvokeType.BulletLifeTimeout, self);

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