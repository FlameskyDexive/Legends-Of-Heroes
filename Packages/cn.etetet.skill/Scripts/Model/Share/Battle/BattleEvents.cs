namespace ET
{
    // 战斗事件：命中结算结果。从 Ori Model/Share/GamePlay/EventType.cs 迁出（其余事件本项目已有，不重复迁移）。
    public struct HitResult
    {
        public EHitResultType hitResultType;
        public int value;
    }

    // 行为事件数据。原 Ori 在 ET.EventType 命名空间，本项目按目录约定(Share→ET)放在 ET；
    // Entity 成员用 EntityRef<Unit> 持有(ET0111)。
    public struct ActionEventData
    {
        public EActionEventType actionEventType;
        public EntityRef<Unit> owner;
        public EntityRef<Unit> target;
    }
}
