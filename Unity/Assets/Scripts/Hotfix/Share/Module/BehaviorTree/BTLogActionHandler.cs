namespace ET
{
    [BTActionHandler("Log")]
    public sealed class BTLogActionHandler : ABTActionHandler
    {
        public override ETTask<BehaviorTreeNodeState> Execute(BehaviorTreeExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken)
        {
            string message = context.GetStringArgument(node, "message", node.Title);
            Log.Info($"[BehaviorTree][{context.TreeName}] {message}");
            ETTask<BehaviorTreeNodeState> task = ETTask<BehaviorTreeNodeState>.Create();
            task.SetResult(BehaviorTreeNodeState.Success);
            return task;
        }
    }
}
