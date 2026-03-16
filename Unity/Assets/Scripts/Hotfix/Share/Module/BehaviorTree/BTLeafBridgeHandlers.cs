using System;

namespace ET
{
    [BTNodeHandler]
    public sealed class BTActionCallHandler : ABTNodeHandler<BTActionCall>
    {
        protected override BTExecResult Run(BTActionCall node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            BTNodeRuntimeState state = env.GetState(node);
            if (state.State == BTNodeState.Running)
            {
                return BTExecResult.Running;
            }

            ABTActionHandler handler = BTActionDispatcher.Instance.Get(node.HandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree action handler not found: {node.HandlerName}");
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            BTCoroutineTokenState tokenState = BTFlowDriver.StartToken(session, node);
            ETTask<BTNodeState> task;
            try
            {
                task = handler.Execute(env.BindContext(node), node.Definition, tokenState.Token);
                if (task.IsCompleted)
                {
                    BTExecResult completedResult = ConvertHandlerResult(task.GetResult());
                    session.CoroutineStates.Remove(node.RuntimeNodeId);
                    session.SetState(node, completedResult.ToNodeState());
                    return completedResult;
                }
            }
            catch (Exception exception)
            {
                session.CoroutineStates.Remove(node.RuntimeNodeId);
                session.LogException(exception, node);
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            session.SetState(node, BTNodeState.Running);
            AwaitActionAsync(session, node, task, tokenState.Version).Coroutine();
            return BTExecResult.Running;
        }

        private static async ETTask AwaitActionAsync(BTExecutionSession session, BTActionCall node, ETTask<BTNodeState> task, long version)
        {
            try
            {
                BTNodeState handlerResult = await task;
                if (!BTFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

                BTFlowDriver.Resume(session, node.RuntimeNodeId, ConvertHandlerResult(handlerResult));
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

        private static BTExecResult ConvertHandlerResult(BTNodeState state)
        {
            return state == BTNodeState.Success ? BTExecResult.Success : BTExecResult.Failure;
        }
    }

    [BTNodeHandler]
    public sealed class BTConditionCallHandler : ABTNodeHandler<BTConditionCall>
    {
        protected override BTExecResult Run(BTConditionCall node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            ABTConditionHandler handler = BTConditionDispatcher.Instance.Get(node.HandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree condition handler not found: {node.HandlerName}");
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            bool passed;
            try
            {
                passed = handler.Evaluate(env.BindContext(node), node.Definition);
            }
            catch (Exception exception)
            {
                session.LogException(exception, node);
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            BTExecResult conditionResult = passed ? BTExecResult.Success : BTExecResult.Failure;
            session.SetState(node, conditionResult.ToNodeState());
            return conditionResult;
        }
    }

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

    [BTNodeHandler]
    public sealed class BTWaitHandler : ABTNodeHandler<BTWait>
    {
        protected override BTExecResult Run(BTWait node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            BTNodeRuntimeState state = env.GetState(node);
            if (state.State == BTNodeState.Running)
            {
                return BTExecResult.Running;
            }

            Entity owner = session.Owner;
            TimerComponent timerComponent = owner?.Root()?.GetComponent<TimerComponent>();
            if (timerComponent == null)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            BTCoroutineTokenState tokenState = BTFlowDriver.StartToken(session, node);
            session.SetState(node, BTNodeState.Running);
            RunWaitAsync(session, node, timerComponent, tokenState.Version).Coroutine();
            return BTExecResult.Running;
        }

        private static async ETTask RunWaitAsync(BTExecutionSession session, BTWait node, TimerComponent timerComponent, long version)
        {
            if (!BTFlowDriver.IsTokenValid(session, node, version, out BTCoroutineTokenState tokenState))
            {
                return;
            }

            try
            {
                await timerComponent.WaitAsync(node.WaitMilliseconds, tokenState.Token);
                if (!BTFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

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
