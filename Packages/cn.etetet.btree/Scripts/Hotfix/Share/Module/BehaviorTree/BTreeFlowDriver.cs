namespace ET
{
    public static class BTreeFlowDriver
    {
        public static void RunRoot(BTreeExecutionSession session)
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
                    BTreeExecResult result = BTreeDispatcher.Instance.Handle(session.Root, session.Env);
                    if (result == BTreeExecResult.Running)
                    {
                        session.SetState(session.Root, BTreeNodeState.Running);
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

        public static void ScheduleRun(BTreeExecutionSession session)
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

        public static void Resume(BTreeExecutionSession session, int runtimeNodeId, BTreeExecResult result)
        {
            if (session == null || session.IsDisposed || session.IsCompleted || !session.Nodes.TryGetValue(runtimeNodeId, out BTreeNode node) || node == null)
            {
                return;
            }

            BTreeNodeRuntimeState state = session.Env.GetState(node);
            state.HasForcedResult = false;
            state.State = result.ToNodeState();
            state.IsActive = false;
            RemoveToken(session, runtimeNodeId, false);
            session.PublishDebug();
            ScheduleRun(session);
        }

        public static void Dispose(BTreeExecutionSession session)
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
            BTreeDebugHub.Instance.Remove(session.RuntimeId);
        }

        public static void AbortSubtree(BTreeExecutionSession session, BTreeNode node)
        {
            if (session == null || session.IsDisposed || node == null)
            {
                return;
            }

            AbortNodeRecursive(session, node);
            session.PublishDebug();
        }

        public static void ResetSubtree(BTreeExecutionSession session, BTreeNode node)
        {
            if (session == null || node == null)
            {
                return;
            }

            ResetNodeRecursive(session, node);
            session.PublishDebug();
        }

        public static BTreeCoroutineTokenState StartToken(BTreeExecutionSession session, BTreeNode node)
        {
            long version = 1;
            if (session.CoroutineStates.Remove(node.RuntimeNodeId, out BTreeCoroutineTokenState oldState))
            {
                version = oldState.Version + 1;
                oldState.Token?.Cancel();
            }

            BTreeCoroutineTokenState state = new()
            {
                RuntimeNodeId = node.RuntimeNodeId,
                Version = version,
                Token = new ETCancellationToken(),
            };
            session.CoroutineStates[node.RuntimeNodeId] = state;
            return state;
        }

        public static bool IsTokenValid(BTreeExecutionSession session, BTreeNode node, long version, out BTreeCoroutineTokenState tokenState)
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

        private static void AbortNodeRecursive(BTreeExecutionSession session, BTreeNode node)
        {
            RemoveToken(session, node.RuntimeNodeId, true);
            if (session.Env.TryGetState(node, out BTreeNodeRuntimeState state))
            {
                if (state.ObserverId != 0)
                {
                    session.Blackboard.RemoveObserver(state.ObserverId);
                    state.ObserverId = 0;
                }

                if (state.State == BTreeNodeState.Running)
                {
                    state.State = BTreeNodeState.Aborted;
                }

                state.IsActive = false;
                state.HasForcedResult = false;
                state.ServiceStarted = false;
            }

            foreach (BTreeNode child in node.Children)
            {
                AbortNodeRecursive(session, child);
            }

            if (node is BTreeSubTreeCall subTreeCall && subTreeCall.SubTreeRoot != null)
            {
                AbortNodeRecursive(session, subTreeCall.SubTreeRoot);
            }
        }

        private static void ResetNodeRecursive(BTreeExecutionSession session, BTreeNode node)
        {
            RemoveToken(session, node.RuntimeNodeId, true);
            if (session.Env.TryGetState(node, out BTreeNodeRuntimeState state) && state.ObserverId != 0)
            {
                session.Blackboard.RemoveObserver(state.ObserverId);
            }

            session.NodeStates.Remove(node.RuntimeNodeId);
            session.Env.RemoveState(node);

            foreach (BTreeNode child in node.Children)
            {
                ResetNodeRecursive(session, child);
            }

            if (node is BTreeSubTreeCall subTreeCall && subTreeCall.SubTreeRoot != null)
            {
                ResetNodeRecursive(session, subTreeCall.SubTreeRoot);
            }
        }

        private static void RemoveToken(BTreeExecutionSession session, int runtimeNodeId, bool cancel)
        {
            if (session == null)
            {
                return;
            }

            if (!session.CoroutineStates.Remove(runtimeNodeId, out BTreeCoroutineTokenState state) || state == null)
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
