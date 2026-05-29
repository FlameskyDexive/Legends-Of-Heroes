namespace ET
{
    // 服务端"单位创建完成"事件。客户端有 ET.Client.AfterUnitCreate, 但服务端原本没有对应钩子;
    // 这里补一个通用扩展点(由服务端 UnitFactory.Create 发布), 供高层玩法包(如 cn.etetet.ballbattle)
    // 订阅做单位创建后的装配, 避免低层 mapplay 反向依赖高层包。
    public struct AfterUnitCreateServer
    {
        public EntityRef<Unit> Unit { get; set; }
    }
}
