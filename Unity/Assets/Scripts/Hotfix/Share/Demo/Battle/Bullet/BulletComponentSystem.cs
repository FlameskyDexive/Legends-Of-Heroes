
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
                self.DomainScene().GetComponent<UnitComponent>()?.Remove(self.GetParent<Unit>().Id);
            }
            catch (Exception e)
            {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }

    [ObjectSystem]
    public class BulletAwakeSystem : AwakeSystem<BulletComponent>
    {
        protected override void Awake(BulletComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class BulletFixedUpdateSystem : FixedUpdateSystem<BulletComponent>
    {
        protected override void FixedUpdate(BulletComponent self)
        {
            self.FixedUpdate();
        }
    }
    
    [ObjectSystem]
    public class BulletDestroySystem : DestroySystem<BulletComponent>
    {
        protected override void Destroy(BulletComponent self)
        {
            TimerComponent.Instance.Remove(ref self.Timer);
        }
    }
    [FriendOf(typeof(BulletComponent))]
    public static class BulletComponentSystem
    {
        public static void Awake(this BulletComponent self)
        {
            //测试子弹，生存时间700ms
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 700, TimerInvokeType.BulletLifeTimeout, self);

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