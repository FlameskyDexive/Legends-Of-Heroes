namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeBlackboardConditionHandler : ABTreeNodeHandler<BTreeBlackboardCondition>
    {
        protected override BTreeExecResult Run(BTreeBlackboardCondition node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);

            if (node.Definition is not BTreeBlackboardConditionNodeData definition)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            BTreeNodeRuntimeState state = env.GetState(node);
            if (state.HasForcedResult)
            {
                CleanupObserver(session, state);
                state.HasForcedResult = false;
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            if (!Evaluate(definition, session.Blackboard))
            {
                CleanupObserver(session, state);
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTreeNodeState.Success);
                return BTreeExecResult.Success;
            }

            if (state.ObserverId == 0 && definition.AbortMode != BTreeAbortMode.None)
            {
                state.ObserverId = session.Blackboard.Observe(definition.BlackboardKey, _ => OnBlackboardChanged(session, node));
            }

            BTreeExecResult childResult = BTreeDispatcher.Instance.Handle(node.Children[0], env);
            if (childResult == BTreeExecResult.Running)
            {
                session.SetState(node, BTreeNodeState.Running);
                return BTreeExecResult.Running;
            }

            CleanupObserver(session, state);
            session.SetState(node, childResult.ToNodeState());
            return childResult;
        }

        private static bool Evaluate(BTreeBlackboardConditionNodeData definition, BTreeBlackboard blackboard)
        {
            object currentValue = blackboard.GetBoxed(definition.BlackboardKey);
            return BTreeValueUtility.Compare(currentValue, definition.CompareOperator, definition.CompareValue);
        }

        private static void OnBlackboardChanged(BTreeExecutionSession session, BTreeBlackboardCondition node)
        {
            if (session == null || session.IsDisposed || session.IsCompleted || !session.Env.TryGetState(node, out BTreeNodeRuntimeState state) || state.State != BTreeNodeState.Running)
            {
                return;
            }

            if (node.Definition is not BTreeBlackboardConditionNodeData definition)
            {
                return;
            }

            if (Evaluate(definition, session.Blackboard))
            {
                return;
            }

            state.HasForcedResult = true;
            state.ForcedResult = BTreeExecResult.Failure;
            if (node.Children.Count > 0)
            {
                BTreeFlowDriver.AbortSubtree(session, node.Children[0]);
            }

            CleanupObserver(session, state);
            BTreeFlowDriver.ScheduleRun(session);
        }

        private static void CleanupObserver(BTreeExecutionSession session, BTreeNodeRuntimeState state)
        {
            if (state.ObserverId == 0)
            {
                return;
            }

            session.Blackboard.RemoveObserver(state.ObserverId);
            state.ObserverId = 0;
        }
    }
}
