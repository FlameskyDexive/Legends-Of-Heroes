
using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;

namespace ET
{

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
            
            
        }
    }
    [FriendOf(typeof(BulletComponent))]
    public static class BulletComponentSystem
    {
        public static void Awake(this BulletComponent self)
        {
            //测试子弹，生存时间700ms

            self.EndTime = TimeInfo.Instance.ServerNow() + 1000;

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
            if (TimeInfo.Instance.ServerNow() > self.EndTime)
            {
                self.DomainScene().GetComponent<UnitComponent>()?.Remove(self.GetParent<Unit>().Id);
            }
        }

    }
}