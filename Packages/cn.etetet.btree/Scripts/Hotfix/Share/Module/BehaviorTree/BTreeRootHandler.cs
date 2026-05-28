namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeRootHandler : ABTreeNodeHandler<BTreeRoot>
    {
        protected override BTreeExecResult Run(BTreeRoot node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            BTreeExecResult childResult = BTreeDispatcher.Instance.Handle(node.Children[0], env);
            session.SetState(node, childResult.ToNodeState());
            return childResult;
        }
    }
}
