namespace ET
{
    internal static class BTreeHandlerUtility
    {
        public static bool TryGetTerminalResult(BTreeExecutionSession session, BTreeNode node, out BTreeExecResult result)
        {
            result = default;
            if (session == null || node == null || !session.Env.TryGetState(node, out BTreeNodeRuntimeState state))
            {
                return false;
            }

            switch (state.State)
            {
                case BTreeNodeState.Success:
                    result = BTreeExecResult.Success;
                    return true;
                case BTreeNodeState.Failure:
                case BTreeNodeState.Aborted:
                    result = BTreeExecResult.Failure;
                    return true;
                default:
                    return false;
            }
        }
    }
}
