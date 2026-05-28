namespace ET
{
    [BTreeNodeDescriptor]
    public sealed class BTreePatrolNodeDescriptor : ABTreeNodeDescriptor
    {
        public override string TypeId => BTreePatrolNodeTypes.Patrol;

        public override BTreeNodeKind NodeKind => BTreeNodeKind.Action;

        public override string MenuPath => "Behaviors/Demo/Patrol";

        public override string HandlerName => "BTreePatrol";

        public override string Description => "Demo patrol node. Moves the unit by following the patrol points configured on this node.";
    }

    [BTreeNodeDescriptor]
    public sealed class BTreeHasPatrolPathNodeDescriptor : ABTreeNodeDescriptor
    {
        public override string TypeId => BTreePatrolNodeTypes.HasPatrolPath;

        public override BTreeNodeKind NodeKind => BTreeNodeKind.Condition;

        public override string MenuPath => "Conditions/Demo/Has Patrol Path";

        public override string HandlerName => "BTreeHasPatrolPath";

        public override string Description => "Demo condition node. Checks whether the unit currently has a PatrolComponent.";
    }
}
