namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeSequenceHandler : ABTreeNodeHandler<BTreeSequence>
    {
        protected override BTreeExecResult Run(BTreeSequence node, BTreeEnv env)
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
                session.SetState(node, BTreeNodeState.Success);
                return BTreeExecResult.Success;
            }

            int index = state.CurrentChildIndex;
            while (index < node.Children.Count)
            {
                BTreeExecResult childResult = BTreeDispatcher.Instance.Handle(node.Children[index], env);
                if (childResult == BTreeExecResult.Success)
                {
                    ++index;
                    state.CurrentChildIndex = index;
                    continue;
                }

                session.SetState(node, childResult.ToNodeState());
                return childResult;
            }

            session.SetState(node, BTreeNodeState.Success);
            return BTreeExecResult.Success;
        }
    }
}
