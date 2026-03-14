namespace ET
{
    [BTActionHandler("Log")]
    public sealed class BTLogActionHandler : ABTActionHandler
    {
        public override ETTask<BTNodeState> Execute(BTExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken)
        {
            string message = context.GetStringArgument(node, "message", node.Title);
            Log.Info($"[BehaviorTree][{context.TreeName}] {message}");
            ETTask<BTNodeState> task = ETTask<BTNodeState>.Create();
            task.SetResult(BTNodeState.Success);
            return task;
        }
    }
}
