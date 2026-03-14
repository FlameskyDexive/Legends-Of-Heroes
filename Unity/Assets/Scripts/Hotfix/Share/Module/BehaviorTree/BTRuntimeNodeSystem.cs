namespace ET
{
    public static partial class BTRuntimeNodeSystem
    {
        public static void Start(this BTRuntimeNode self)
        {
            if (self == null || self.Runner.IsDisposed || self.State == BTNodeState.Running)
            {
                return;
            }

            self.CancellationToken = new ETCancellationToken();
            self.State = BTNodeState.Running;
            RecordState(self.Runner, self, self.State);
            self.DispatchOnStart();
        }

        public static void Stop(this BTRuntimeNode self)
        {
            if (self == null || self.State != BTNodeState.Running)
            {
                return;
            }

            ETCancellationToken token = self.CancellationToken;
            self.CancellationToken = null;
            token?.Cancel();
            self.DispatchOnStop();
            self.State = BTNodeState.Aborted;
            RecordState(self.Runner, self, self.State);
        }

        public static void Succeed(this BTRuntimeNode self)
        {
            self.Complete(BTNodeState.Success);
        }

        public static void Fail(this BTRuntimeNode self)
        {
            self.Complete(BTNodeState.Failure);
        }

        public static void HandleChildCompleted(this BTRuntimeNode self, BTRuntimeNode child, BTNodeState state)
        {
            switch (self)
            {
                case BTRootNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTSequenceNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTSelectorNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTParallelNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTInverterNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTSucceederNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTFailerNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTRepeaterNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTBlackboardConditionNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTServiceNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BTSubTreeNode node:
                    HandleChildCompleted(node, child, state);
                    return;
            }
        }

        public static void StopChildren(this BTRuntimeNode self)
        {
            foreach (BTRuntimeNode child in self.Children)
            {
                child.Stop();
            }
        }

        private static void Complete(this BTRuntimeNode self, BTNodeState state)
        {
            if (self.State != BTNodeState.Running)
            {
                return;
            }

            ETCancellationToken token = self.CancellationToken;
            self.CancellationToken = null;
            token?.Cancel();
            self.State = state;
            RecordState(self.Runner, self, state);
            self.Parent?.HandleChildCompleted(self, state);
        }

        private static void DispatchOnStart(this BTRuntimeNode self)
        {
            switch (self)
            {
                case BTRootNode node:
                    OnStart(node);
                    return;
                case BTSequenceNode node:
                    OnStart(node);
                    return;
                case BTSelectorNode node:
                    OnStart(node);
                    return;
                case BTParallelNode node:
                    OnStart(node);
                    return;
                case BTInverterNode node:
                    OnStart(node);
                    return;
                case BTSucceederNode node:
                    OnStart(node);
                    return;
                case BTFailerNode node:
                    OnStart(node);
                    return;
                case BTRepeaterNode node:
                    OnStart(node);
                    return;
                case BTBlackboardConditionNode node:
                    OnStart(node);
                    return;
                case BTServiceNode node:
                    OnStart(node);
                    return;
                case BTActionNode node:
                    OnStart(node);
                    return;
                case BTConditionNode node:
                    OnStart(node);
                    return;
                case BTWaitNode node:
                    OnStart(node);
                    return;
                case BTSubTreeNode node:
                    OnStart(node);
                    return;
            }
        }

        private static void DispatchOnStop(this BTRuntimeNode self)
        {
            switch (self)
            {
                case BTBlackboardConditionNode node:
                    OnStop(node);
                    return;
                case BTSubTreeNode node:
                    OnStop(node);
                    return;
                default:
                    self.StopChildren();
                    return;
            }
        }
    }
}
