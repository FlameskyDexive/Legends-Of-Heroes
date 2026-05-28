namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeInverterHandler : ABTreeNodeHandler<BTreeInverter>
    {
        protected override BTreeExecResult Run(BTreeInverter node, BTreeEnv env)
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
            if (childResult == BTreeExecResult.Running)
            {
                session.SetState(node, BTreeNodeState.Running);
                return BTreeExecResult.Running;
            }

            BTreeExecResult resultValue = childResult == BTreeExecResult.Success ? BTreeExecResult.Failure : BTreeExecResult.Success;
            session.SetState(node, resultValue.ToNodeState());
            return resultValue;
        }
    }
}
