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
}
