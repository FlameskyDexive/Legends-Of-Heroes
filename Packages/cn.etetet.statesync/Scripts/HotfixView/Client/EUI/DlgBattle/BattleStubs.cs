using UnityEngine;

namespace ET.Client
{
    // BattleStubs：DlgBattle 战斗逻辑所需的最小桩实现。
    // - Skill / SkillComponent / ESkillAbstractType 在 ModelView/BattleModelStubs.cs。
    // - 这里只放 HotfixView 层的扩展（CD 判断、SkillComponent.TryGetSkill、OperaComponent 战斗操作）。
    // 等 cn.etetet.spell 与 cn.etetet.mapplay 提供真实业务后，删除本文件。

    [EntitySystemOf(typeof(Skill))]
    [FriendOf(typeof(Skill))]
    public static partial class SkillStubSystem
    {
        [EntitySystem]
        private static void Awake(this Skill self)
        {
        }

        public static bool IsInCd(this Skill self) => self != null && self.CurrentCD > 0;
    }

    [EntitySystemOf(typeof(SkillComponent))]
    [FriendOf(typeof(SkillComponent))]
    public static partial class SkillComponentStubSystem
    {
        [EntitySystem]
        private static void Awake(this SkillComponent self)
        {
        }

        public static bool TryGetSkill(this SkillComponent self, ESkillAbstractType type, int index, out Skill skill)
        {
            skill = null;
            return false;
        }
    }

    public static class OperaComponentBattleStubs
    {
        public static void OnMove(this OperaComponent self, Vector2 v)
        {
            // TODO: 接入真实操控逻辑（cn.etetet.mapplay）
        }

        public static void StopMove(this OperaComponent self)
        {
            // TODO: 接入真实操控逻辑
        }

        public static void OnClickSkill1(this OperaComponent self)
        {
            // TODO: 接入真实操控逻辑
        }

        public static void OnClickSkill2(this OperaComponent self)
        {
            // TODO: 接入真实操控逻辑
        }
    }
}
