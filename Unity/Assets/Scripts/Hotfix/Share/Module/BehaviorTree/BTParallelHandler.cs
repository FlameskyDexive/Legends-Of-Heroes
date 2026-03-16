using System;

namespace ET
{
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
}
