using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreePatrolAction : ABTreeNodeHandler<BTreePatrol>
    {
        private const string PatrolIndexBlackboardKeyPrefix = "__bt_demo_patrol_index__";

        protected override BTreeExecResult Run(BTreePatrol node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            BTreeNodeRuntimeState state = env.GetState(node);
            if (state.State == BTreeNodeState.Running)
            {
                return BTreeExecResult.Running;
            }

            if (node.Definition is not BTreePatrolNodeData patrolNode || patrolNode.PatrolPoints.Count == 0)
            {
                return BTreeExecResult.Failure;
            }

            BTreeCoroutineTokenState tokenState = BTreeFlowDriver.StartToken(session, node);
            RunPatrolAsync(session, node, patrolNode, tokenState.Version).Coroutine();
            return BTreeExecResult.Running;
        }

        private static async ETTask RunPatrolAsync(BTreeExecutionSession session, BTreePatrol node, BTreePatrolNodeData patrolNode, long version)
        {
            if (!BTreeFlowDriver.IsTokenValid(session, node, version, out _))
            {
                return;
            }

            BTreeExecutionContext context = session.Env.BindContext(node);

            if (!context.TryGetOwner<Unit>(out Unit unit) || unit.IsDisposed)
            {
                BTreeFlowDriver.Resume(session, node.RuntimeNodeId, BTreeExecResult.Failure);
                return;
            }

            MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
            if (moveComponent == null)
            {
                BTreeFlowDriver.Resume(session, node.RuntimeNodeId, BTreeExecResult.Failure);
                return;
            }

            try
            {
                string blackboardKey = $"{PatrolIndexBlackboardKeyPrefix}{node.SourceNodeId}";
                int index = context.Blackboard.GetBoxed(blackboardKey) is int currentIndex ? currentIndex : 0;
                index = (index % patrolNode.PatrolPoints.Count + patrolNode.PatrolPoints.Count) % patrolNode.PatrolPoints.Count;

                // Share 层移动：直接驱动 MoveComponent，移动协程结束后用 token 版本号判断是否已被打断
                float speed = unit.NumericComponent.GetAsFloat(NumericType.Speed);
                float3 targetPos = patrolNode.PatrolPoints[index].ToFloat3();
                bool moveResult = await moveComponent.MoveToAsync(new List<float3> { targetPos }, speed);

                if (!BTreeFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

                if (!moveResult)
                {
                    BTreeFlowDriver.Resume(session, node.RuntimeNodeId, BTreeExecResult.Failure);
                    return;
                }

                context.Blackboard.SetBoxed(blackboardKey, (index + 1) % patrolNode.PatrolPoints.Count);
                BTreeFlowDriver.Resume(session, node.RuntimeNodeId, BTreeExecResult.Success);
            }
            catch (Exception exception)
            {
                if (!BTreeFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

                session.LogException(exception, node);
                BTreeFlowDriver.Resume(session, node.RuntimeNodeId, BTreeExecResult.Failure);
            }
        }
    }
}
