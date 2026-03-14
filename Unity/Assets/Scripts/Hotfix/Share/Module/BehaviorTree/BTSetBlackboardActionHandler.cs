namespace ET
{
    [BTActionHandler("SetBlackboard")]
    public sealed class BTSetBlackboardActionHandler : ABTActionHandler
    {
        public override ETTask<BehaviorTreeNodeState> Execute(BehaviorTreeExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken)
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
