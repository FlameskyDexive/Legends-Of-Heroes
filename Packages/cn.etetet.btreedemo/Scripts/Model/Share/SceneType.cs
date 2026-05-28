namespace ET
{
    public static partial class SceneType
    {
        // 行为树 Demo 专用场景类型。AfterMyUnitCreate_BTreeDemo 仅在该场景下挂载示例行为树，
        // 默认不会在正式玩法场景触发；需要体验 Demo 时把客户端根场景的 SceneType 设为此值。
        public const int Demo = PackageType.BTreeDemo * 1000 + 1;
    }
}
