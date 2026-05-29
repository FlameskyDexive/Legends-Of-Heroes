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

    // OperaComponent 的摇杆操作(OnMove/StopMove/OnClickSkill1/2)已在 cn.etetet.mapplay
    // 的 OperaComponentSystem 中实现真实逻辑(上报 C2M_Operation), 此处不再提供空桩。
}
