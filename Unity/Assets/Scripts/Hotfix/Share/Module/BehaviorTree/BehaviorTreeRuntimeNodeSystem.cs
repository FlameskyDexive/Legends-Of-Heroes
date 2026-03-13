namespace ET
{
    public static partial class BehaviorTreeRuntimeNodeSystem
    {
        public static void Start(this BehaviorTreeRuntimeNode self)
        {
            if (self == null || self.Runner.IsDisposed || self.State == BehaviorTreeNodeState.Running)
            {
                return;
            }

            self.CancellationToken = new ETCancellationToken();
            self.State = BehaviorTreeNodeState.Running;
            RecordState(self.Runner, self, self.State);
            self.DispatchOnStart();
        }

        public static void Stop(this BehaviorTreeRuntimeNode self)
        {
            if (self == null || self.State != BehaviorTreeNodeState.Running)
            {
                return;
            }

            ETCancellationToken token = self.CancellationToken;
            self.CancellationToken = null;
            token?.Cancel();
            self.DispatchOnStop();
            self.State = BehaviorTreeNodeState.Aborted;
            RecordState(self.Runner, self, self.State);
        }

        public static void Succeed(this BehaviorTreeRuntimeNode self)
        {
            self.Complete(BehaviorTreeNodeState.Success);
        }

        public static void Fail(this BehaviorTreeRuntimeNode self)
        {
            self.Complete(BehaviorTreeNodeState.Failure);
        }

        public static void HandleChildCompleted(this BehaviorTreeRuntimeNode self, BehaviorTreeRuntimeNode child, BehaviorTreeNodeState state)
        {
            switch (self)
            {
                case BehaviorTreeRootNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeSequenceNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeSelectorNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeParallelNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeInverterNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeSucceederNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeFailerNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeRepeaterNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeBlackboardConditionNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeServiceNode node:
                    HandleChildCompleted(node, child, state);
                    return;
                case BehaviorTreeSubTreeNode node:
                    HandleChildCompleted(node, child, state);
                    return;
            }
        }

        public static void StopChildren(this BehaviorTreeRuntimeNode self)
        {
            foreach (BehaviorTreeRuntimeNode child in self.Children)
            {
                child.Stop();
            }
        }

        private static void Complete(this BehaviorTreeRuntimeNode self, BehaviorTreeNodeState state)
        {
            if (self.State != BehaviorTreeNodeState.Running)
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

        private static void DispatchOnStart(this BehaviorTreeRuntimeNode self)
        {
            switch (self)
            {
                case BehaviorTreeRootNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeSequenceNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeSelectorNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeParallelNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeInverterNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeSucceederNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeFailerNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeRepeaterNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeBlackboardConditionNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeServiceNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeActionNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeConditionNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeWaitNode node:
                    OnStart(node);
                    return;
                case BehaviorTreeSubTreeNode node:
                    OnStart(node);
                    return;
            }
        }

        private static void DispatchOnStop(this BehaviorTreeRuntimeNode self)
        {
            switch (self)
            {
                case BehaviorTreeBlackboardConditionNode node:
                    OnStop(node);
                    return;
                case BehaviorTreeSubTreeNode node:
                    OnStop(node);
                    return;
                default:
                    self.StopChildren();
                    return;
            }
        }
    }
}
