namespace ET
{
    [BTActionHandler("SetBlackboard")]
    public sealed class BTSetBlackboardActionHandler : ABTActionHandler
    {
        public override ETTask<BTNodeState> Execute(BTExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken)
        {
            string key = context.GetStringArgument(node, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                ETTask<BTNodeState> failedTask = ETTask<BTNodeState>.Create();
                failedTask.SetResult(BTNodeState.Failure);
                return failedTask;
            }

            bool remove = context.GetBoolArgument(node, "remove");
            if (remove)
            {
                context.Blackboard.Remove(key);
                ETTask<BTNodeState> removedTask = ETTask<BTNodeState>.Create();
                removedTask.SetResult(BTNodeState.Success);
                return removedTask;
            }

            object value = context.GetArgumentValue(node, "value");
            context.Blackboard.SetBoxed(key, value);
            ETTask<BTNodeState> successTask = ETTask<BTNodeState>.Create();
            successTask.SetResult(BTNodeState.Success);
            return successTask;
        }
    }
}
