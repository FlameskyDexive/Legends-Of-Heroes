using System;

namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeActionCallHandler : ABTreeNodeHandler<BTreeActionCall>
    {
        protected override BTreeExecResult Run(BTreeActionCall node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            BTreeNodeRuntimeState state = env.GetState(node);
            if (state.State == BTreeNodeState.Running)
            {
                return BTreeExecResult.Running;
            }

            if (node.Definition is not BTreeActionNodeData definition)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            ABTreeActionHandler handler = BTreeActionDispatcher.Instance.Get(definition.ActionHandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree action handler not found: {definition.ActionHandlerName}");
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            BTreeCoroutineTokenState tokenState = BTreeFlowDriver.StartToken(session, node);
            ETTask<BTreeNodeState> task;
            try
            {
                task = handler.Execute(env.BindContext(node), node.Definition, tokenState.Token);
                if (task.IsCompleted)
                {
                    BTreeExecResult completedResult = ConvertHandlerResult(task.GetResult());
                    session.CoroutineStates.Remove(node.RuntimeNodeId);
                    session.SetState(node, completedResult.ToNodeState());
                    return completedResult;
                }
            }
            catch (Exception exception)
            {
                session.CoroutineStates.Remove(node.RuntimeNodeId);
                session.LogException(exception, node);
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            session.SetState(node, BTreeNodeState.Running);
            AwaitActionAsync(session, node, task, tokenState.Version).Coroutine();
            return BTreeExecResult.Running;
        }

        private static async ETTask AwaitActionAsync(BTreeExecutionSession session, BTreeActionCall node, ETTask<BTreeNodeState> task, long version)
        {
            try
            {
                BTreeNodeState handlerResult = await task;
                if (!BTreeFlowDriver.IsTokenValid(session, node, version, out _))
                {
                    return;
                }

                BTreeFlowDriver.Resume(session, node.RuntimeNodeId, ConvertHandlerResult(handlerResult));
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

        private static BTreeExecResult ConvertHandlerResult(BTreeNodeState state)
        {
            return state == BTreeNodeState.Success ? BTreeExecResult.Success : BTreeExecResult.Failure;
        }
    }
}
