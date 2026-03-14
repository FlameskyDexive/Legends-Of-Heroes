using ET.Client;

namespace ET
{
    [BTActionHandler("BTPatrol")]
    public sealed class BTPatrolAction : ABTActionHandler
    {
        private const string PatrolIndexBlackboardKeyPrefix = "__bt_demo_patrol_index__";

        public override async ETTask<BTNodeState> Execute(BTExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken)
        {
            if (node is not BTPatrolNodeData patrolNode || patrolNode.PatrolPoints.Count == 0)
            {
                return BTNodeState.Failure;
            }

            if (!context.TryGetOwner<Unit>(out Unit unit) || unit.IsDisposed)
            {
                return BTNodeState.Failure;
            }

            string blackboardKey = $"{PatrolIndexBlackboardKeyPrefix}{node.NodeId}";
            int index = context.Blackboard.GetBoxed(blackboardKey) is int currentIndex ? currentIndex : 0;
            index = (index % patrolNode.PatrolPoints.Count + patrolNode.PatrolPoints.Count) % patrolNode.PatrolPoints.Count;

            int moveResult = await unit.MoveToAsync(patrolNode.PatrolPoints[index].ToFloat3(), cancellationToken);

            if (cancellationToken.IsCancel())
            {
                return BTNodeState.Aborted;
            }

            if (moveResult != WaitTypeError.Success)
            {
                return BTNodeState.Failure;
            }

            context.Blackboard.SetBoxed(blackboardKey, (index + 1) % patrolNode.PatrolPoints.Count);
            return BTNodeState.Success;
        }
    }
}
