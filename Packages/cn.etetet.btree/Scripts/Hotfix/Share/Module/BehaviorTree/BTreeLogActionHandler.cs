namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeLogActionHandler : ABTreeNodeHandler<BTreeLog>
    {
        protected override BTreeExecResult Run(BTreeLog node, BTreeEnv env)
        {
            BTreeExecutionContext context = env.BindContext(node);
            string message = context.GetStringArgument(node.Definition, "message", node.Definition?.Title);
            Log.Info($"[BehaviorTree][{context.TreeName}] {message}");
            return BTreeExecResult.Success;
        }
    }
}
