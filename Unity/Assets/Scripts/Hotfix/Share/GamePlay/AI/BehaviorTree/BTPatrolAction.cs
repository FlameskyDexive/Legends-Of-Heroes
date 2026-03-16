using System;
using ET.Client;

namespace ET
{
    [BTNodeHandler]
    public sealed class BTPatrolAction : ABTNodeHandler<BTPatrol>
    {
        private const string PatrolIndexBlackboardKeyPrefix = "__bt_demo_patrol_index__";

        protected override BTExecResult Run(BTPatrol node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            BTNodeRuntimeState state = env.GetState(node);
            if (state.State == BTNodeState.Running)
            {
                return BTExecResult.Running;
            }

            if (node.Definition is not BTPatrolNodeData patrolNode || patrolNode.PatrolPoints.Count == 0)
            {
                return BTExecResult.Failure;
            }

            BTCoroutineTokenState tokenState = BTFlowDriver.StartToken(session, node);
            RunPatrolAsync(session, node, patrolNode, tokenState.Version).Coroutine();
            return BTExecResult.Running;
        }

        private static async ETTask RunPatrolAsync(BTExecutionSession session, BTPatrol node, BTPatrolNodeData patrolNode, long version)
        {
            if (!BTFlowDriver.IsTokenValid(session, node, version, out BTCoroutineTokenState tokenState))
            {
                return;
            }

            BTExecutionContext context = session.Env.BindContext(node);

            if (!context.TryGetOwner<Unit>(out Unit unit) || unit.IsDisposed)
            {
                BTFlowDriver.Resume(session, node.RuntimeNodeId, BTExecResult.Failure);
                return;
            }

            try
            {
                string blackboardKey = $"{PatrolIndexBlackboardKeyPrefix}{node.SourceNodeId}";
                int index = context.Blackboard.GetBoxed(blackboardKey) is int currentIndex ? currentIndex : 0;
                index = (index % patrolNode.PatrolPoints.Count + patrolNode.PatrolPoints.Count) % patrolNode.PatrolPoints.Count;

                int moveResult = await unit.MoveToAsync(patrolNode.PatrolPoints[index].ToFloat3(), tokenState.Token);

                if (!BTFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

                if (moveResult != WaitTypeError.Success)
                {
                    BTFlowDriver.Resume(session, node.RuntimeNodeId, BTExecResult.Failure);
                    return;
                }

                context.Blackboard.SetBoxed(blackboardKey, (index + 1) % patrolNode.PatrolPoints.Count);
                BTFlowDriver.Resume(session, node.RuntimeNodeId, BTExecResult.Success);
            }
            catch (Exception exception)
            {
                if (!BTFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

                session.LogException(exception, node);
                BTFlowDriver.Resume(session, node.RuntimeNodeId, BTExecResult.Failure);
            }
        }
    }
}
