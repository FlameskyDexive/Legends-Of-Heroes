namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeSubTreeCallHandler : ABTreeNodeHandler<BTreeSubTreeCall>
    {
        protected override BTreeExecResult Run(BTreeSubTreeCall node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            if (node.Definition is not BTreeSubTreeNodeData definition)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            if (node.SubTreeRoot == null)
            {
                Log.Error($"behavior tree subtree not found: {definition.SubTreeId}/{definition.SubTreeName}");
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            BTreeDefinition previousTree = session.Env.CurrentTree;
            string previousTreeId = session.Env.TreeId;
            string previousTreeName = session.Env.TreeName;
            try
            {
                session.UpdateTreeContext(node.SubTreeRoot);
                BTreeExecResult childResult = BTreeDispatcher.Instance.Handle(node.SubTreeRoot, env);
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
