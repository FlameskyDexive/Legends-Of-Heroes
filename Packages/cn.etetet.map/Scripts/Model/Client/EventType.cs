namespace ET.Client
{
    public struct AfterCreateClientScene
    {
    }

    public struct AfterCreateCurrentScene
    {
    }

    public struct AppStartInit
    {
    }

    public struct AppStartInitFinish
    {
    }

    public struct AfterUnitCreate
    {
        public EntityRef<Unit> Unit;
    }

    public struct AfterMyUnitCreate
    {
        public EntityRef<Unit> Unit;
    }

    // 单位"视图(GameObject)"创建完成后发布的通用扩展点。
    // 由 mapplay 的通用视图创建(AfterUnitCreate_CreateUnitView)在挂好 GameObjectComponent 后发布,
    // 供上层玩法(如球球大作战)做专属表现:按 id 换皮肤、切俯视相机等,避免在通用视图里写玩法逻辑。
    public struct AfterUnitViewCreate
    {
        public EntityRef<Unit> Unit;
    }
}
