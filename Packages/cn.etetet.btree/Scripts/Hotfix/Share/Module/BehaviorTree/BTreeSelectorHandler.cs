namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeSelectorHandler : ABTreeNodeHandler<BTreeSelector>
    {
        protected override BTreeExecResult Run(BTreeSelector node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            BTreeNodeRuntimeState state = env.GetState(node);
            if (node.Children.Count == 0)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            int index = state.CurrentChildIndex;
            while (index < node.Children.Count)
            {
                BTreeExecResult childResult = BTreeDispatcher.Instance.Handle(node.Children[index], env);
                if (childResult == BTreeExecResult.Failure)
                {
                    ++index;
                    state.CurrentChildIndex = index;
                    continue;
                }

                session.SetState(node, childResult.ToNodeState());
                return childResult;
            }

            session.SetState(node, BTreeNodeState.Failure);
            return BTreeExecResult.Failure;
        }
    }
}
