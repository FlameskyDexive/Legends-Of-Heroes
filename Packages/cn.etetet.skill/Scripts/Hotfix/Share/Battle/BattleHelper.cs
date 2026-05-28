using System.Collections;
using System.Collections.Generic;

namespace ET
{
    public static class BattleHelper
    {
        // TODO(配置缺失): 本项目 numeric 数值配置当前没有"攻击力"(Attack)数值类型。
        // 在 numeric 配置补一个 Attack 后, 把 PlaceholderAttack 改回 from.NumericComponent.GetAsInt(NumericType.Attack)。
        private const int PlaceholderAttack = 1;

        /// <summary>
        /// 命中结算
        /// </summary>
        public static void HitSettle(Unit from, Unit to, EHitFromType hitType = EHitFromType.Skill_Normal, Unit bullet = null)
        {
            switch (hitType)
            {
                case EHitFromType.Skill_Normal:
                case EHitFromType.Skill_Bullet:
                {
                    int dmg = PlaceholderAttack; // 原: from.NumericComponent.GetAsInt(NumericType.Attack)
                    int finalHp = to.NumericComponent.GetAsInt(NumericType.HP) - dmg;
                    if (finalHp <= 0)
                    {
                        //死亡发事件通知
                    }
                    Log.Info($"hit settle, from:{from?.Id}, to:{to?.Id}, value:{dmg}");
                    //命中结算结果发事件通知，处理一系列逻辑/表现（飘血，血量触发引发的其他事件等）
                    EventSystem.Instance.Publish(from.Root(), new HitResult() { hitResultType = EHitResultType.Damage, value = dmg });
                    break;
                }
            }
        }
    }
}
