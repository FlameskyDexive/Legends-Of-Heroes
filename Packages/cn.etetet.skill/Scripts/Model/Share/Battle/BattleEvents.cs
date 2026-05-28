namespace ET
{
    // 战斗事件：命中结算结果。从 Ori Model/Share/GamePlay/EventType.cs 迁出（其余事件本项目已有，不重复迁移）。
    public struct HitResult
    {
        public EHitResultType hitResultType;
        public int value;
    }
}
