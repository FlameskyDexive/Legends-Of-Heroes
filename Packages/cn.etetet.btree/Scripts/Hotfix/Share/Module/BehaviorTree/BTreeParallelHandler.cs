using System;

namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeParallelHandler : ABTreeNodeHandler<BTreeParallel>
    {
        protected override BTreeExecResult Run(BTreeParallel node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            if (node.Definition is not BTreeParallelNodeData definition)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            BTreeNodeRuntimeState state = env.GetState(node);
            if (node.Children.Count == 0)
            {
                session.SetState(node, BTreeNodeState.Success);
                return BTreeExecResult.Success;
            }

            int runningCount = 0;
            int successCount = 0;
            int failureCount = 0;
            BTreeNode terminalChild = null;
            foreach (BTreeNode child in node.Children)
            {
                BTreeExecResult childResult = BTreeDispatcher.Instance.Handle(child, env);
                switch (childResult)
                {
                    case BTreeExecResult.Success:
                        ++successCount;
                        terminalChild = child;
                        if (definition.SuccessPolicy == BTreeParallelPolicy.RequireOne)
                        {
                            AbortOtherChildren(session, node, terminalChild);
                            session.SetState(node, BTreeNodeState.Success);
                            return BTreeExecResult.Success;
                        }
                        break;
                    case BTreeExecResult.Failure:
                        ++failureCount;
                        terminalChild = child;
                        if (definition.FailurePolicy == BTreeParallelPolicy.RequireOne)
                        {
                            AbortOtherChildren(session, node, terminalChild);
                            session.SetState(node, BTreeNodeState.Failure);
                            return BTreeExecResult.Failure;
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
                session.SetState(node, BTreeNodeState.Running);
                return BTreeExecResult.Running;
            }

            bool success = definition.SuccessPolicy == BTreeParallelPolicy.RequireAll ? successCount == node.Children.Count : successCount > 0;
            bool failure = definition.FailurePolicy == BTreeParallelPolicy.RequireAll ? failureCount == node.Children.Count : failureCount > 0;
            if (success && !failure)
            {
                session.SetState(node, BTreeNodeState.Success);
                return BTreeExecResult.Success;
            }

            if (failure && !success)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            if (success)
            {
                session.SetState(node, BTreeNodeState.Success);
                return BTreeExecResult.Success;
            }

            session.SetState(node, BTreeNodeState.Failure);
            return BTreeExecResult.Failure;
        }

        private static void AbortOtherChildren(BTreeExecutionSession session, BTreeParallel node, BTreeNode completedChild)
        {
            foreach (BTreeNode child in node.Children)
            {
                if (ReferenceEquals(child, completedChild))
                {
                    continue;
                }

                BTreeFlowDriver.AbortSubtree(session, child);
            }
        }
    }
}
