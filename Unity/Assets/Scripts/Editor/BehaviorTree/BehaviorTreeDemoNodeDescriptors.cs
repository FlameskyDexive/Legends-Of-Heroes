namespace ET.Client
{
    [BehaviorTreeNodeDescriptor]
    public sealed class DemoClientPatrolNodeDescriptor : ABehaviorTreeNodeDescriptor
    {
        public override string TypeId => BehaviorTreeDemoNodeTypes.Patrol;

        public override BehaviorTreeNodeKind NodeKind => BehaviorTreeNodeKind.Action;

        public override string MenuPath => "Behaviors/Demo/Patrol";

        public override string HandlerName => "DemoClientPatrol";

        public override string Description => "客户端巡逻示例节点，按照节点自己的巡逻点配置驱动单位移动。";
    }

    [BehaviorTreeNodeDescriptor]
    public sealed class DemoClientHasPatrolPathNodeDescriptor : ABehaviorTreeNodeDescriptor
    {
        public override string TypeId => BehaviorTreeDemoNodeTypes.HasPatrolPath;

        public override BehaviorTreeNodeKind NodeKind => BehaviorTreeNodeKind.Condition;

        public override string MenuPath => "Conditions/Demo/Has Patrol Path";

        public override string HandlerName => "DemoClientHasXunLuoPath";

        public override string Description => "客户端示例条件节点，判断单位是否存在巡逻路径组件。";
    }
}
