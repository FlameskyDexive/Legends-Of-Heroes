using System.Collections;
using System.Collections.Generic;
using ET.EventType;

namespace ET
{
    public static class BattleHelper 
    {
        /// <summary>
        /// 命中结算
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="hitType"></param>
        /// <param name="bullet"></param>
        public static void HitSettle(Unit from, Unit to, EHitFromType hitType = EHitFromType.Skill_Normal, Unit bullet = null)
        {
            switch (hitType)
            {
                case EHitFromType.Skill_Normal:
                {
                    int dmg = from.GetComponent<NumericComponent>().GetAsInt(NumericType.Attack);
                    int finalHp = to.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp) - dmg;
                    if (finalHp <= 0)
                    {
                        //死亡发事件通知
                    }
                    Log.Console($"hit settle, from:{from?.Id}, to:{to?.Id}, value:{dmg}");
                    //命中结算结果发事件通知，处理一系列逻辑/表现（飘血，血量触发引发的其他事件等，当前球球会重新更新大小）
                    EventSystem.Instance.Publish(from.DomainScene(), new HitResult(){hitResultType = EHitResultType.Damage, value = dmg});
                    break;
                }
                case EHitFromType.Skill_Bullet:
                {
                    int dmg = from.GetComponent<NumericComponent>().GetAsInt(NumericType.Attack);
                    int finalHp = to.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp) - dmg;
                    if (finalHp <= 0)
                    {
                        //死亡发事件通知
                    }
                    Log.Info($"hit settle, from:{from?.Id}, to:{to?.Id}, value:{dmg}");
                    EventSystem.Instance.Publish(from.DomainScene(), new HitResult(){hitResultType = EHitResultType.Damage, value = dmg});
                    break;
                }
                    
            }
        }
    }
}
