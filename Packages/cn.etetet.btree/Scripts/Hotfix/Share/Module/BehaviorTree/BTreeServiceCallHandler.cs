using System;

namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeServiceCallHandler : ABTreeNodeHandler<BTreeServiceCall>
    {
        protected override BTreeExecResult Run(BTreeServiceCall node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);

            if (node.Definition is not BTreeServiceNodeData definition)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            BTreeNodeRuntimeState state = env.GetState(node);
            if (state.HasForcedResult)
            {
                Cleanup(session, node, state);
                state.HasForcedResult = false;
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            ABTreeServiceHandler handler = BTreeServiceDispatcher.Instance.Get(definition.ServiceHandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree service handler not found: {definition.ServiceHandlerName}");
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            if (!state.ServiceStarted)
            {
                state.ServiceStarted = true;
                BTreeCoroutineTokenState tokenState = BTreeFlowDriver.StartToken(session, node);
                RunServiceLoop(session, node, definition, handler, tokenState.Version).Coroutine();
            }

            BTreeExecResult childResult = BTreeDispatcher.Instance.Handle(node.Children[0], env);
            if (childResult == BTreeExecResult.Running)
            {
                session.SetState(node, BTreeNodeState.Running);
                return BTreeExecResult.Running;
            }

            Cleanup(session, node, state);
            session.SetState(node, childResult.ToNodeState());
            return childResult;
        }

        private static async ETTask RunServiceLoop(BTreeExecutionSession session, BTreeServiceCall node, BTreeServiceNodeData definition, ABTreeServiceHandler handler, long version)
        {
            try
            {
                while (BTreeFlowDriver.IsTokenValid(session, node, version, out BTreeCoroutineTokenState tokenState))
                {
                    await handler.Tick(session.Env.BindContext(node), node.Definition, tokenState.Token);
                    if (!BTreeFlowDriver.IsTokenValid(session, node, version, out tokenState))
                    {
                        return;
                    }

                    Entity owner = session.Owner;
                    TimerComponent timerComponent = owner?.Root()?.GetComponent<TimerComponent>();
                    if (timerComponent == null)
                    {
                        return;
                    }

                    await timerComponent.WaitAsync(definition.IntervalMilliseconds).AddCancel(tokenState.Token);
                }
            }
            catch (Exception exception)
            {
                if (!BTreeFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

                session.LogException(exception, node);
                BTreeNodeRuntimeState state = session.Env.GetState(node);
                state.HasForcedResult = true;
                state.ForcedResult = BTreeExecResult.Failure;
                if (node.Children.Count > 0)
                {
                    BTreeFlowDriver.AbortSubtree(session, node.Children[0]);
                }

                BTreeFlowDriver.ScheduleRun(session);
            }
        }

        private static void Cleanup(BTreeExecutionSession session, BTreeNode node, BTreeNodeRuntimeState state)
        {
            state.ServiceStarted = false;
            if (node.Children.Count > 0)
            {
                BTreeFlowDriver.AbortSubtree(session, node.Children[0]);
            }

            if (session.CoroutineStates.TryGetValue(node.RuntimeNodeId, out BTreeCoroutineTokenState tokenState))
            {
                tokenState.Token?.Cancel();
                session.CoroutineStates.Remove(node.RuntimeNodeId);
            }
        }
    }
}
