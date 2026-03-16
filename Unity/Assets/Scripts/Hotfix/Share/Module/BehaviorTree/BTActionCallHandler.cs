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

            if (node.Definition is not BTActionNodeData definition)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            ABTActionHandler handler = BTActionDispatcher.Instance.Get(definition.ActionHandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree action handler not found: {definition.ActionHandlerName}");
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
}
