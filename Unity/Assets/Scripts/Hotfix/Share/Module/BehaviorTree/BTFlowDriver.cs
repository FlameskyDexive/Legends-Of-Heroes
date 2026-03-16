namespace ET
{
    public static class BTFlowDriver
    {
        public static void RunRoot(BTExecutionSession session)
        {
            if (session == null || session.IsDisposed || session.Root == null || session.IsCompleted)
            {
                return;
            }

            if (session.IsDispatching)
            {
                session.PendingRun = true;
                return;
            }

            session.IsDispatching = true;
            try
            {
                do
                {
                    session.PendingRun = false;
                    session.UpdateTreeContext(session.Root);
                    BTExecResult result = BTDispatcher.Instance.Handle(session.Root, session.Env);
                    if (result == BTExecResult.Running)
                    {
                        session.SetState(session.Root, BTNodeState.Running);
                        continue;
                    }

                    session.SetState(session.Root, result.ToNodeState());
                    session.IsCompleted = true;
                }
                while (session.PendingRun && !session.IsDisposed && !session.IsCompleted);
            }
            finally
            {
                session.IsDispatching = false;
            }
        }

        public static void ScheduleRun(BTExecutionSession session)
        {
            if (session == null || session.IsDisposed || session.IsCompleted)
            {
                return;
            }

            if (session.IsDispatching)
            {
                session.PendingRun = true;
                return;
            }

            RunRoot(session);
        }

        public static void Resume(BTExecutionSession session, int runtimeNodeId, BTExecResult result)
        {
            if (session == null || session.IsDisposed || session.IsCompleted || !session.Nodes.TryGetValue(runtimeNodeId, out BTNode node) || node == null)
            {
                return;
            }

            BTNodeRuntimeState state = session.Env.GetState(node);
            state.HasForcedResult = false;
            state.State = result.ToNodeState();
            state.IsActive = false;
            RemoveToken(session, runtimeNodeId, false);
            session.PublishDebug();
            ScheduleRun(session);
        }

        public static void Dispose(BTExecutionSession session)
        {
            if (session == null || session.IsDisposed)
            {
                return;
            }

            session.IsDisposed = true;
            if (session.Root != null)
            {
                ResetSubtree(session, session.Root);
            }

            session.NodeStates.Clear();
            session.CoroutineStates.Clear();
            session.Env?.States.Clear();
            BTDebugHub.Instance.Remove(session.RuntimeId);
        }

        public static void AbortSubtree(BTExecutionSession session, BTNode node)
        {
            if (session == null || session.IsDisposed || node == null)
            {
                return;
            }

            AbortNodeRecursive(session, node);
            session.PublishDebug();
        }

        public static void ResetSubtree(BTExecutionSession session, BTNode node)
        {
            if (session == null || node == null)
            {
                return;
            }

            ResetNodeRecursive(session, node);
            session.PublishDebug();
        }

        public static BTCoroutineTokenState StartToken(BTExecutionSession session, BTNode node)
        {
            long version = 1;
            if (session.CoroutineStates.Remove(node.RuntimeNodeId, out BTCoroutineTokenState oldState))
            {
                version = oldState.Version + 1;
                oldState.Token?.Cancel();
            }

            BTCoroutineTokenState state = new()
            {
                RuntimeNodeId = node.RuntimeNodeId,
                Version = version,
                Token = new ETCancellationToken(),
            };
            session.CoroutineStates[node.RuntimeNodeId] = state;
            return state;
        }

        public static bool IsTokenValid(BTExecutionSession session, BTNode node, long version, out BTCoroutineTokenState tokenState)
        {
            if (session == null || node == null)
            {
                tokenState = null;
                return false;
            }

            if (!session.CoroutineStates.TryGetValue(node.RuntimeNodeId, out tokenState))
            {
                return false;
            }

            return tokenState.Version == version && !tokenState.Token.IsCancel() && !session.IsDisposed;
        }

        private static void AbortNodeRecursive(BTExecutionSession session, BTNode node)
        {
            RemoveToken(session, node.RuntimeNodeId, true);
            if (session.Env.TryGetState(node, out BTNodeRuntimeState state))
            {
                if (state.ObserverId != 0)
                {
                    session.Blackboard.RemoveObserver(state.ObserverId);
                    state.ObserverId = 0;
                }

                if (state.State == BTNodeState.Running)
                {
                    state.State = BTNodeState.Aborted;
                }

                state.IsActive = false;
                state.HasForcedResult = false;
                state.ServiceStarted = false;
            }

            foreach (BTNode child in node.Children)
            {
                AbortNodeRecursive(session, child);
            }

            if (node is BTSubTreeCall subTreeCall && subTreeCall.SubTreeRoot != null)
            {
                AbortNodeRecursive(session, subTreeCall.SubTreeRoot);
            }
        }

        private static void ResetNodeRecursive(BTExecutionSession session, BTNode node)
        {
            RemoveToken(session, node.RuntimeNodeId, true);
            if (session.Env.TryGetState(node, out BTNodeRuntimeState state) && state.ObserverId != 0)
            {
                session.Blackboard.RemoveObserver(state.ObserverId);
            }

            session.NodeStates.Remove(node.RuntimeNodeId);
            session.Env.RemoveState(node);

            foreach (BTNode child in node.Children)
            {
                ResetNodeRecursive(session, child);
            }

            if (node is BTSubTreeCall subTreeCall && subTreeCall.SubTreeRoot != null)
            {
                ResetNodeRecursive(session, subTreeCall.SubTreeRoot);
            }
        }

        private static void RemoveToken(BTExecutionSession session, int runtimeNodeId, bool cancel)
        {
            if (session == null)
            {
                return;
            }

            if (!session.CoroutineStates.Remove(runtimeNodeId, out BTCoroutineTokenState state) || state == null)
            {
                return;
            }

            if (cancel)
            {
                state.Token?.Cancel();
            }
        }
    }
}
