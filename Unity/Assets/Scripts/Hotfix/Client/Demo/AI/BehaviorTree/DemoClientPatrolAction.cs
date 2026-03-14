namespace ET.Client
{
    [BTActionHandler("DemoClientPatrol")]
    public sealed class DemoClientPatrolAction : ABTActionHandler
    {
        private const string PatrolIndexBlackboardKeyPrefix = "__bt_demo_patrol_index__";

        public override async ETTask<BehaviorTreeNodeState> Execute(BehaviorTreeExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken)
        {
            if (node is not BTDemoPatrolNodeData patrolNode || patrolNode.PatrolPoints.Count == 0)
            {
                return BehaviorTreeNodeState.Failure;
            }

            if (!context.TryGetOwner<Unit>(out Unit unit) || unit.IsDisposed)
            {
                return BehaviorTreeNodeState.Failure;
            }

            string blackboardKey = $"{PatrolIndexBlackboardKeyPrefix}{node.NodeId}";
            int index = context.Blackboard.GetBoxed(blackboardKey) is int currentIndex ? currentIndex : 0;
            index = (index % patrolNode.PatrolPoints.Count + patrolNode.PatrolPoints.Count) % patrolNode.PatrolPoints.Count;

            int moveResult = await unit.MoveToAsync(patrolNode.PatrolPoints[index].ToFloat3(), cancellationToken);

            if (cancellationToken.IsCancel())
            {
                return BehaviorTreeNodeState.Aborted;
            }

            if (moveResult != WaitTypeError.Success)
            {
                return BehaviorTreeNodeState.Failure;
            }

            context.Blackboard.SetBoxed(blackboardKey, (index + 1) % patrolNode.PatrolPoints.Count);
            return BehaviorTreeNodeState.Success;
        }
    }
}
