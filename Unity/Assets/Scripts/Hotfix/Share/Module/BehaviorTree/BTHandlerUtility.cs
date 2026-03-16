namespace ET
{
    internal static class BTHandlerUtility
    {
        public static bool TryGetTerminalResult(BTExecutionSession session, BTNode node, out BTExecResult result)
        {
            result = default;
            if (session == null || node == null || !session.Env.TryGetState(node, out BTNodeRuntimeState state))
            {
                return false;
            }

            switch (state.State)
            {
                case BTNodeState.Success:
                    result = BTExecResult.Success;
                    return true;
                case BTNodeState.Failure:
                case BTNodeState.Aborted:
                    result = BTExecResult.Failure;
                    return true;
                default:
                    return false;
            }
        }
    }
}
