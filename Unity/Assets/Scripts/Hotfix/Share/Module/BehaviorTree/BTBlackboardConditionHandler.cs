namespace ET
{
    [BTNodeHandler]
    public sealed class BTBlackboardConditionHandler : ABTNodeHandler<BTBlackboardCondition>
    {
        protected override BTExecResult Run(BTBlackboardCondition node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);

            BTNodeRuntimeState state = env.GetState(node);
            if (state.HasForcedResult)
            {
                CleanupObserver(session, state);
                state.HasForcedResult = false;
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (!Evaluate(node, session.Blackboard))
            {
                CleanupObserver(session, state);
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            if (state.ObserverId == 0 && node.AbortMode != BTAbortMode.None)
            {
                state.ObserverId = session.Blackboard.Observe(node.BlackboardKey, _ => OnBlackboardChanged(session, node));
            }

            BTExecResult childResult = BTDispatcher.Instance.Handle(node.Children[0], env);
            if (childResult == BTExecResult.Running)
            {
                session.SetState(node, BTNodeState.Running);
                return BTExecResult.Running;
            }

            CleanupObserver(session, state);
            session.SetState(node, childResult.ToNodeState());
            return childResult;
        }

        private static bool Evaluate(BTBlackboardCondition node, BTBlackboard blackboard)
        {
            object currentValue = blackboard.GetBoxed(node.BlackboardKey);
            return BTValueUtility.Compare(currentValue, node.CompareOperator, node.CompareValue);
        }

        private static void OnBlackboardChanged(BTExecutionSession session, BTBlackboardCondition node)
        {
            if (session == null || session.IsDisposed || session.IsCompleted || !session.Env.TryGetState(node, out BTNodeRuntimeState state) || state.State != BTNodeState.Running)
            {
                return;
            }

            if (Evaluate(node, session.Blackboard))
            {
                return;
            }

            state.HasForcedResult = true;
            state.ForcedResult = BTExecResult.Failure;
            if (node.Children.Count > 0)
            {
                BTFlowDriver.AbortSubtree(session, node.Children[0]);
            }

            CleanupObserver(session, state);
            BTFlowDriver.ScheduleRun(session);
        }

        private static void CleanupObserver(BTExecutionSession session, BTNodeRuntimeState state)
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
