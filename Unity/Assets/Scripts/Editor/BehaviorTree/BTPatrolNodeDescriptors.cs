namespace ET
{
    [BTNodeDescriptor]
    public sealed class BTPatrolNodeDescriptor : ABTNodeDescriptor
    {
        public override string TypeId => BTPatrolNodeTypes.Patrol;

        public override BTNodeKind NodeKind => BTNodeKind.Action;

        public override string MenuPath => "Behaviors/Demo/Patrol";

        public override string HandlerName => "BTPatrol";

        public override string Description => "Demo patrol node. Moves the unit by following the patrol points configured on this node.";
    }

    [BTNodeDescriptor]
    public sealed class BTHasPatrolPathNodeDescriptor : ABTNodeDescriptor
    {
        public override string TypeId => BTPatrolNodeTypes.HasPatrolPath;

        public override BTNodeKind NodeKind => BTNodeKind.Condition;

        public override string MenuPath => "Conditions/Demo/Has Patrol Path";

        public override string HandlerName => "BTHasPatrolPath";

        public override string Description => "Demo condition node. Checks whether the unit currently has a PatrolComponent.";
    }
}
