namespace ET
{
    [BTNodeHandler]
    public sealed class BTInverterHandler : ABTNodeHandler<BTInverter>
    {
        protected override BTExecResult Run(BTInverter node, BTEnv env)
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
            if (childResult == BTExecResult.Running)
            {
                session.SetState(node, BTNodeState.Running);
                return BTExecResult.Running;
            }

            BTExecResult resultValue = childResult == BTExecResult.Success ? BTExecResult.Failure : BTExecResult.Success;
            session.SetState(node, resultValue.ToNodeState());
            return resultValue;
        }
    }
}
