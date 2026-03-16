namespace ET
{
    [BTNodeHandler]
    public sealed class BTSequenceHandler : ABTNodeHandler<BTSequence>
    {
        protected override BTExecResult Run(BTSequence node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            BTNodeRuntimeState state = env.GetState(node);
            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            int index = state.CurrentChildIndex;
            while (index < node.Children.Count)
            {
                BTExecResult childResult = BTDispatcher.Instance.Handle(node.Children[index], env);
                if (childResult == BTExecResult.Success)
                {
                    ++index;
                    state.CurrentChildIndex = index;
                    continue;
                }

                session.SetState(node, childResult.ToNodeState());
                return childResult;
            }

            session.SetState(node, BTNodeState.Success);
            return BTExecResult.Success;
        }
    }
}
