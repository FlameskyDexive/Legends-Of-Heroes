namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeSucceederHandler : ABTreeNodeHandler<BTreeSucceeder>
    {
        protected override BTreeExecResult Run(BTreeSucceeder node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTreeNodeState.Success);
                return BTreeExecResult.Success;
            }

            BTreeExecResult childResult = BTreeDispatcher.Instance.Handle(node.Children[0], env);
            if (childResult == BTreeExecResult.Running)
            {
                session.SetState(node, BTreeNodeState.Running);
                return BTreeExecResult.Running;
            }

            session.SetState(node, BTreeNodeState.Success);
            return BTreeExecResult.Success;
        }
    }
}
