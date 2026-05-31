using Unity.Mathematics;

namespace ET
{
    // 输入/操作枚举。原在 Ori 的 Battle/BattleDefine, 因属于"操控"语义(OperaComponent 使用),
    // 迁移时下沉到 mapplay(OperaComponent 所在包), 避免战斗包(cn.etetet.skill, 更高层)被反向依赖。

    /// <summary>
    /// 输入操作类型
    /// </summary>
    public enum EInputType : byte
    {
        Key,
        KeyDown,
        KeyUp,
    }

    public enum EOperateStatus : byte
    {
        Success = 0,
        Error = 1,
    }

    public enum EOperateType : byte
    {
        Move = 0,
        Jump = 1,
        Attack = 2, // 普攻
        Skill1,
        Skill2,
        Skill3,
        Skill4,
    }

    // 操作的普通数据结构(非 MessageObject)。Entity(OperaComponent)禁止持有 MessageObject(ETMO001),
    // 故每帧操作先暂存为此结构, LateUpdate 时再转成 proto 的 OperateInfo 发送。
    public struct OperateInfoData
    {
        public int OperateType;
        public int InputType;
        public float3 Vec3;
    }

    // 操作触发技能释放的事件。用于解耦: 操作处理器(mapplay, 低层)发布此事件,
    // 战斗包(cn.etetet.skill, 高层)订阅并真正释放技能, 从而 mapplay 不需反向依赖 skill。
    // Index: 技能槽位(Skill1=0, Skill2=1 ...), 对应 SkillComponent.SpellSkill 的 index 参数。
    public struct OperateSkillCast
    {
        public EntityRef<Unit> Unit { get; set; }
        public int Index { get; set; }
    }

    // 地图移动边界(X/Z 平面方形,居中)。挂 Scene。某些玩法(如球球大作战)需要把角色移动 clamp 在地面范围内:
    // 由玩法包在场景上添加并设值(取自该玩法地图地面的实际占地),C2M_OperationHandler 读它 clamp 移动目标;
    // 没有此组件的地图不做限制(普通 RPG 走 Recast 寻路,本就不会越界)。放 mapplay/Model(通用),不依赖具体玩法包。
    [ComponentOf(typeof(Scene))]
    public class MapBoundsComponent : Entity, IAwake
    {
        public float Min; // X/Z 下限
        public float Max; // X/Z 上限
    }
}
