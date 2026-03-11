namespace ET
{
    [BehaviorTreeActionHandler("Log")]
    public sealed class BehaviorTreeLogActionHandler : ABehaviorTreeActionHandler
    {
        public override ETTask<BehaviorTreeNodeState> Execute(BehaviorTreeExecutionContext context, BehaviorTreeNodeDefinition node, ETCancellationToken cancellationToken)
        {
            string message = context.GetStringArgument(node, "message", node.Title);
            Log.Info($"[BehaviorTree][{context.TreeName}] {message}");
            ETTask<BehaviorTreeNodeState> task = ETTask<BehaviorTreeNodeState>.Create();
            task.SetResult(BehaviorTreeNodeState.Success);
            return task;
        }
    }

    [BehaviorTreeActionHandler("SetBlackboard")]
    public sealed class BehaviorTreeSetBlackboardActionHandler : ABehaviorTreeActionHandler
    {
        public override ETTask<BehaviorTreeNodeState> Execute(BehaviorTreeExecutionContext context, BehaviorTreeNodeDefinition node, ETCancellationToken cancellationToken)
        {
            string key = context.GetStringArgument(node, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                ETTask<BehaviorTreeNodeState> failedTask = ETTask<BehaviorTreeNodeState>.Create();
                failedTask.SetResult(BehaviorTreeNodeState.Failure);
                return failedTask;
            }

            bool remove = context.GetBoolArgument(node, "remove");
            if (remove)
            {
                context.Blackboard.Remove(key);
                ETTask<BehaviorTreeNodeState> removedTask = ETTask<BehaviorTreeNodeState>.Create();
                removedTask.SetResult(BehaviorTreeNodeState.Success);
                return removedTask;
            }

            object value = context.GetArgumentValue(node, "value");
            context.Blackboard.SetBoxed(key, value);
            ETTask<BehaviorTreeNodeState> successTask = ETTask<BehaviorTreeNodeState>.Create();
            successTask.SetResult(BehaviorTreeNodeState.Success);
            return successTask;
        }
    }
}
