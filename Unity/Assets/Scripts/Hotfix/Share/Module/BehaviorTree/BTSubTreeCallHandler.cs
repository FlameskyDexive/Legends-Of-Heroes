namespace ET
{
    [BTNodeHandler]
    public sealed class BTSubTreeCallHandler : ABTNodeHandler<BTSubTreeCall>
    {
        protected override BTExecResult Run(BTSubTreeCall node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.Definition is not BTSubTreeNodeData definition)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            if (node.SubTreeRoot == null)
            {
                Log.Error($"behavior tree subtree not found: {definition.SubTreeId}/{definition.SubTreeName}");
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            BTDefinition previousTree = session.Env.CurrentTree;
            string previousTreeId = session.Env.TreeId;
            string previousTreeName = session.Env.TreeName;
            try
            {
                session.UpdateTreeContext(node.SubTreeRoot);
                BTExecResult childResult = BTDispatcher.Instance.Handle(node.SubTreeRoot, env);
                session.SetState(node, childResult.ToNodeState());
                return childResult;
            }
            finally
            {
                session.Env.CurrentTree = previousTree;
                session.Env.TreeId = previousTreeId;
                session.Env.TreeName = previousTreeName;
            }
        }
    }
}
