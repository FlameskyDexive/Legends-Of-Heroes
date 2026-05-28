using System;

namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeWaitHandler : ABTreeNodeHandler<BTreeWait>
    {
        protected override BTreeExecResult Run(BTreeWait node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            if (node.Definition is not BTreeWaitNodeData)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            BTreeNodeRuntimeState state = env.GetState(node);
            if (state.State == BTreeNodeState.Running)
            {
                return BTreeExecResult.Running;
            }

            Entity owner = session.Owner;
            TimerComponent timerComponent = owner?.Root()?.GetComponent<TimerComponent>();
            if (timerComponent == null)
            {
                session.SetState(node, BTreeNodeState.Success);
                return BTreeExecResult.Success;
            }

            BTreeCoroutineTokenState tokenState = BTreeFlowDriver.StartToken(session, node);
            session.SetState(node, BTreeNodeState.Running);
            RunWaitAsync(session, node, timerComponent, tokenState.Version).Coroutine();
            return BTreeExecResult.Running;
        }

        private static async ETTask RunWaitAsync(BTreeExecutionSession session, BTreeWait node, TimerComponent timerComponent, long version)
        {
            if (!BTreeFlowDriver.IsTokenValid(session, node, version, out BTreeCoroutineTokenState tokenState))
            {
                return;
            }

            try
            {
                if (node.Definition is not BTreeWaitNodeData definition)
                {
                    BTreeFlowDriver.Resume(session, node.RuntimeNodeId, BTreeExecResult.Failure);
                    return;
                }

                await timerComponent.WaitAsync(definition.WaitMilliseconds).AddCancel(tokenState.Token);
                if (!BTreeFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

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
