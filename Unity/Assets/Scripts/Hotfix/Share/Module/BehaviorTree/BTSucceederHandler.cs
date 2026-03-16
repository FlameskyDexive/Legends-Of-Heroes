namespace ET
{
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
}
