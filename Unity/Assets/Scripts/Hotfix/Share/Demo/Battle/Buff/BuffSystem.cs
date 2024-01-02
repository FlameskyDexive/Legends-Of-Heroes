using System;
using System.Collections.Generic;

namespace ET
{

    [Invoke(TimerInvokeType.BuffLifeTimeout)]
    public class BuffLifeTimeout : ATimer<Buff>
    {
        protected override void Run(Buff self)
        {
            try
            {
                self.LifeTimeout();
            }
            catch (Exception e)
            {
                self.Fiber().Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }
    [Invoke(TimerInvokeType.BuffInterval)]
    public class BuffIntervalTimer : ATimer<Buff>
    {
        protected override void Run(Buff self)
        {
            try
            {
                self.TriggerBuff();
            }
            catch (Exception e)
            {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }

    [EntitySystemOf(typeof(Buff))]
    [FriendOf(typeof(Buff))]
    [FriendOf(typeof(NumericComponent))]
    public static partial class BuffSystem
    {

        [EntitySystem]
        public static void Awake(this Buff self, int BuffId)
        {
            self.BuffId = BuffId;
            self.LifeTimer = self.Root().GetComponent<TimerComponent>().NewOnceTimer(TimeInfo.Instance.ServerNow() + self.BuffConfig.Duration, TimerInvokeType.BuffLifeTimeout, self);
            //常规buff添加则立即出发一次，时间到销毁。如果有触发间隔，则间隔固定的时间再次出发buff行为
            if(self.IntervalTimer > 0)
                self.IntervalTimer = self.Root().GetComponent<TimerComponent>().NewRepeatedTimer(self.BuffConfig.Interval, TimerInvokeType.BuffInterval, self);
            //初始默认触发一次buff效果
            self.TriggerBuff();
        }
        [EntitySystem]
        public static void Destroy(this Buff self)
        {
            self.Root().GetComponent<TimerComponent>().Remove(ref self.LifeTimer);
            self.Root().GetComponent<TimerComponent>().Remove(ref self.IntervalTimer);

        }
        /// <summary>
        /// 每帧更新检测buff的周期、触发事件等. 如果表现层需要获取当前buff的剩余时间进度等，此处更新
        /// </summary>
        /// <param name="self"></param>
        [EntitySystem]
        public static void FixedUpdate(this Buff self)
        {
            
        }

        public static void LifeTimeout(this Buff self)
        {
            //layerCount > 0时，减少层数量，重新计时buff
            --self.LayerCount;
            if (self.LayerCount > 0)
            {
                self.UpdateLayerCount();
                return;
            }
            //移除Buff
            self.GetParent<BuffComponent>().RemoveBuff(self.BuffId);
        }
        public static void UpdateLayerCount(this Buff self)
        {
            
        }

        /// <summary>
        /// 触发buff行为
        /// </summary>
        /// <param name="self"></param>
        public static void TriggerBuff(this Buff self)
        {
            //常规buff只修改属性
            switch (self.BuffType)
            {
                case EBuffType.ChangeNumeric:
                    NumericComponent numericComponent = self.Unit.GetComponent<NumericComponent>();
                    numericComponent[self.BuffConfig.NumericType] += self.BuffConfig.NumericValue;
                    break;
            }
            
            //buff修改状态
            
            //如果buff携带可触发技能事件，则触发事件
            
        }

        /// <summary>
        /// 触发技能事件
        /// </summary>
        /// <param name="self"></param>
        public static void TriggerEvent(this Buff self)
        {
            
        }
        
        


        public static Unit GetOwnerUnit(this Buff self)
        {
            return self.GetParent<BattleUnitComponent>().Unit;
        }
        
    }

   
}