namespace ET
{
    internal static class BTHandlerUtility
    {
        public static bool TryGetTerminalResult(BTExecutionSession session, BTNode node, out BTExecResult result)
        {
            result = default;
            if (session == null || node == null || !session.Env.TryGetState(node, out BTNodeRuntimeState state))
            {
                return false;
            }

            switch (state.State)
            {
                case BTNodeState.Success:
                    result = BTExecResult.Success;
                    return true;
                case BTNodeState.Failure:
                case BTNodeState.Aborted:
                    result = BTExecResult.Failure;
                    return true;
                default:
                    return false;
            }
        }
    }

    [BTNodeHandler]
    public sealed class BTRootHandler : ABTNodeHandler<BTRoot>
    {
        protected override BTExecResult Run(BTRoot node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            BTExecResult childResult = BTDispatcher.Instance.Handle(node.Children[0], env);
            session.SetState(node, childResult.ToNodeState());
            return childResult;
        }
    }

    [BTNodeHandler]
    public sealed class BTSequenceHandler : ABTNodeHandler<BTSequence>
    {
        protected override BTExecResult Run(BTSequence node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            BTNodeRuntimeState state = env.GetState(node);
            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            int index = state.CurrentChildIndex;
            while (index < node.Children.Count)
            {
                BTExecResult childResult = BTDispatcher.Instance.Handle(node.Children[index], env);
                if (childResult == BTExecResult.Success)
                {
                    ++index;
                    state.CurrentChildIndex = index;
                    continue;
                }

                session.SetState(node, childResult.ToNodeState());
                return childResult;
            }

            session.SetState(node, BTNodeState.Success);
            return BTExecResult.Success;
        }
    }

    [BTNodeHandler]
    public sealed class BTSelectorHandler : ABTNodeHandler<BTSelector>
    {
        protected override BTExecResult Run(BTSelector node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            BTNodeRuntimeState state = env.GetState(node);
            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            int index = state.CurrentChildIndex;
            while (index < node.Children.Count)
            {
                BTExecResult childResult = BTDispatcher.Instance.Handle(node.Children[index], env);
                if (childResult == BTExecResult.Failure)
                {
                    ++index;
                    state.CurrentChildIndex = index;
                    continue;
                }

                session.SetState(node, childResult.ToNodeState());
                return childResult;
            }

            session.SetState(node, BTNodeState.Failure);
            return BTExecResult.Failure;
        }
    }

    [BTNodeHandler]
    public sealed class BTParallelHandler : ABTNodeHandler<BTParallel>
    {
        protected override BTExecResult Run(BTParallel node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            BTNodeRuntimeState state = env.GetState(node);
            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            int runningCount = 0;
            int successCount = 0;
            int failureCount = 0;
            BTNode terminalChild = null;
            foreach (BTNode child in node.Children)
            {
                BTExecResult childResult = BTDispatcher.Instance.Handle(child, env);
                switch (childResult)
                {
                    case BTExecResult.Success:
                        ++successCount;
                        terminalChild = child;
                        if (node.SuccessPolicy == BTParallelPolicy.RequireOne)
                        {
                            AbortOtherChildren(session, node, terminalChild);
                            session.SetState(node, BTNodeState.Success);
                            return BTExecResult.Success;
                        }
                        break;
                    case BTExecResult.Failure:
                        ++failureCount;
                        terminalChild = child;
                        if (node.FailurePolicy == BTParallelPolicy.RequireOne)
                        {
                            AbortOtherChildren(session, node, terminalChild);
                            session.SetState(node, BTNodeState.Failure);
                            return BTExecResult.Failure;
                        }
                        break;
                    default:
                        ++runningCount;
                        break;
                }
            }

            state.CompletedCount = successCount + failureCount;
            state.SuccessCount = successCount;
            state.FailureCount = failureCount;
            if (runningCount > 0)
            {
                session.SetState(node, BTNodeState.Running);
                return BTExecResult.Running;
            }

            bool success = node.SuccessPolicy == BTParallelPolicy.RequireAll ? successCount == node.Children.Count : successCount > 0;
            bool failure = node.FailurePolicy == BTParallelPolicy.RequireAll ? failureCount == node.Children.Count : failureCount > 0;
            if (success && !failure)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            if (failure && !success)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            if (success)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            session.SetState(node, BTNodeState.Failure);
            return BTExecResult.Failure;
        }

        private static void AbortOtherChildren(BTExecutionSession session, BTParallel node, BTNode completedChild)
        {
            foreach (BTNode child in node.Children)
            {
                if (ReferenceEquals(child, completedChild))
                {
                    continue;
                }

                BTFlowDriver.AbortSubtree(session, child);
            }
        }
    }

    [BTNodeHandler]
    public sealed class BTInverterHandler : ABTNodeHandler<BTInverter>
    {
        protected override BTExecResult Run(BTInverter node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            BTExecResult childResult = BTDispatcher.Instance.Handle(node.Children[0], env);
            if (childResult == BTExecResult.Running)
            {
                session.SetState(node, BTNodeState.Running);
                return BTExecResult.Running;
            }

            BTExecResult resultValue = childResult == BTExecResult.Success ? BTExecResult.Failure : BTExecResult.Success;
            session.SetState(node, resultValue.ToNodeState());
            return resultValue;
        }
    }

    [BTNodeHandler]
    public sealed class BTSucceederHandler : ABTNodeHandler<BTSucceeder>
    {
        protected override BTExecResult Run(BTSucceeder node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            BTExecResult childResult = BTDispatcher.Instance.Handle(node.Children[0], env);
            if (childResult == BTExecResult.Running)
            {
                session.SetState(node, BTNodeState.Running);
                return BTExecResult.Running;
            }

            session.SetState(node, BTNodeState.Success);
            return BTExecResult.Success;
        }
    }

    [BTNodeHandler]
    public sealed class BTFailerHandler : ABTNodeHandler<BTFailer>
    {
        protected override BTExecResult Run(BTFailer node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            BTExecResult childResult = BTDispatcher.Instance.Handle(node.Children[0], env);
            if (childResult == BTExecResult.Running)
            {
                session.SetState(node, BTNodeState.Running);
                return BTExecResult.Running;
            }

            session.SetState(node, BTNodeState.Failure);
            return BTExecResult.Failure;
        }
    }

    [BTNodeHandler]
    public sealed class BTRepeaterHandler : ABTNodeHandler<BTRepeater>
    {
        protected override BTExecResult Run(BTRepeater node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            BTNodeRuntimeState state = env.GetState(node);
            BTNode child = node.Children[0];
            while (true)
            {
                BTExecResult childResult = BTDispatcher.Instance.Handle(child, env);
                if (childResult == BTExecResult.Running)
                {
                    session.SetState(node, BTNodeState.Running);
                    return BTExecResult.Running;
                }

                ++state.RepeatCounter;
                if (node.MaxLoopCount > 0 && state.RepeatCounter >= node.MaxLoopCount)
                {
                    session.SetState(node, childResult.ToNodeState());
                    return childResult;
                }

                BTFlowDriver.ResetSubtree(session, child);
                if (node.MaxLoopCount <= 0)
                {
                    session.PendingRun = true;
                    session.SetState(node, BTNodeState.Running);
                    return BTExecResult.Running;
                }
            }
        }
    }

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

    [BTNodeHandler]
    public sealed class BTSubTreeCallHandler : ABTNodeHandler<BTSubTreeCall>
    {
        protected override BTExecResult Run(BTSubTreeCall node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.SubTreeRoot == null)
            {
                Log.Error($"behavior tree subtree not found: {node.SubTreeId}/{node.SubTreeName}");
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            BTDefinition previousTree = session.Env.CurrentTree;
            string previousTreeId = session.Env.TreeId;
            string previousTreeName = session.Env.TreeName;
            try
            {
                session.UpdateTreeContext(node.SubTreeRoot);
                BTExecResult childResult = BTDispatcher.Instance.Handle(node.SubTreeRoot, env);
                session.SetState(node, childResult.ToNodeState());
                return childResult;
            }
            finally
            {
                session.Env.CurrentTree = previousTree;
                session.Env.TreeId = previousTreeId;
                session.Env.TreeName = previousTreeName;
            }
        }
    }
}
