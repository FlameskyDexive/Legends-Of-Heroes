namespace ET
{
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
}
