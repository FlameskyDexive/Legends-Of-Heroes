using System;

namespace ET
{
    [BTNodeHandler]
    public sealed class BTServiceCallHandler : ABTNodeHandler<BTServiceCall>
    {
        protected override BTExecResult Run(BTServiceCall node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);

            BTNodeRuntimeState state = env.GetState(node);
            if (state.HasForcedResult)
            {
                Cleanup(session, node, state);
                state.HasForcedResult = false;
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            ABTServiceHandler handler = BTServiceDispatcher.Instance.Get(node.HandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree service handler not found: {node.HandlerName}");
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            if (!state.ServiceStarted)
            {
                state.ServiceStarted = true;
                BTCoroutineTokenState tokenState = BTFlowDriver.StartToken(session, node);
                RunServiceLoop(session, node, handler, tokenState.Version).Coroutine();
            }

            BTExecResult childResult = BTDispatcher.Instance.Handle(node.Children[0], env);
            if (childResult == BTExecResult.Running)
            {
                session.SetState(node, BTNodeState.Running);
                return BTExecResult.Running;
            }

            Cleanup(session, node, state);
            session.SetState(node, childResult.ToNodeState());
            return childResult;
        }

        private static async ETTask RunServiceLoop(BTExecutionSession session, BTServiceCall node, ABTServiceHandler handler, long version)
        {
            try
            {
                while (BTFlowDriver.IsTokenValid(session, node, version, out BTCoroutineTokenState tokenState))
                {
                    await handler.Tick(session.Env.BindContext(node), node.Definition, tokenState.Token);
                    if (!BTFlowDriver.IsTokenValid(session, node, version, out tokenState))
                    {
                        return;
                    }

                    Entity owner = session.Owner;
                    TimerComponent timerComponent = owner?.Root()?.GetComponent<TimerComponent>();
                    if (timerComponent == null)
                    {
                        return;
                    }

                    await timerComponent.WaitAsync(node.IntervalMilliseconds, tokenState.Token);
                }
            }
            catch (Exception exception)
            {
                if (!BTFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

                session.LogException(exception, node);
                BTNodeRuntimeState state = session.Env.GetState(node);
                state.HasForcedResult = true;
                state.ForcedResult = BTExecResult.Failure;
                if (node.Children.Count > 0)
                {
                    BTFlowDriver.AbortSubtree(session, node.Children[0]);
                }

                BTFlowDriver.ScheduleRun(session);
            }
        }

        private static void Cleanup(BTExecutionSession session, BTNode node, BTNodeRuntimeState state)
        {
            state.ServiceStarted = false;
            if (node.Children.Count > 0)
            {
                BTFlowDriver.AbortSubtree(session, node.Children[0]);
            }

            if (session.CoroutineStates.TryGetValue(node.RuntimeNodeId, out BTCoroutineTokenState tokenState))
            {
                tokenState.Token?.Cancel();
                session.CoroutineStates.Remove(node.RuntimeNodeId);
            }
        }
    }
}
