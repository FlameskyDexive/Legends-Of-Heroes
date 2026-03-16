namespace ET
{
    [BTNodeHandler]
    public sealed class BTRepeaterHandler : ABTNodeHandler<BTRepeater>
    {
        protected override BTExecResult Run(BTRepeater node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.Definition is not BTRepeaterNodeData definition)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            if (node.Children.Count == 0)
            {
                session.SetState(node, BTNodeState.Success);
                return BTExecResult.Success;
            }

            BTNodeRuntimeState state = env.GetState(node);
            BTNode child = node.Children[0];
            while (true)
            {
                BTExecResult childResult = BTDispatcher.Instance.Handle(child, env);
                if (childResult == BTExecResult.Running)
                {
                    session.SetState(node, BTNodeState.Running);
                    return BTExecResult.Running;
                }

                ++state.RepeatCounter;
                if (definition.MaxLoopCount > 0 && state.RepeatCounter >= definition.MaxLoopCount)
                {
                    session.SetState(node, childResult.ToNodeState());
                    return childResult;
                }

                BTFlowDriver.ResetSubtree(session, child);
                if (definition.MaxLoopCount <= 0)
                {
                    session.PendingRun = true;
                    session.SetState(node, BTNodeState.Running);
                    return BTExecResult.Running;
                }
            }
        }
    }
}
