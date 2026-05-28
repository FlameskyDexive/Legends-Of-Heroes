namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeRepeaterHandler : ABTreeNodeHandler<BTreeRepeater>
    {
        protected override BTreeExecResult Run(BTreeRepeater node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            if (node.Definition is not BTreeRepeaterNodeData definition)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTreeNodeState.Success);
                return BTreeExecResult.Success;
            }

            BTreeNodeRuntimeState state = env.GetState(node);
            BTreeNode child = node.Children[0];
            while (true)
            {
                BTreeExecResult childResult = BTreeDispatcher.Instance.Handle(child, env);
                if (childResult == BTreeExecResult.Running)
                {
                    session.SetState(node, BTreeNodeState.Running);
                    return BTreeExecResult.Running;
                }

                ++state.RepeatCounter;
                if (definition.MaxLoopCount > 0 && state.RepeatCounter >= definition.MaxLoopCount)
                {
                    session.SetState(node, childResult.ToNodeState());
                    return childResult;
                }

                BTreeFlowDriver.ResetSubtree(session, child);
                if (definition.MaxLoopCount <= 0)
                {
                    session.PendingRun = true;
                    session.SetState(node, BTreeNodeState.Running);
                    return BTreeExecResult.Running;
                }
            }
        }
    }
}
