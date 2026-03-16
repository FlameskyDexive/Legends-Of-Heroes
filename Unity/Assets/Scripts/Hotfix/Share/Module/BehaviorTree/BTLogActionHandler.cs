namespace ET
{
    [BTNodeHandler]
    public sealed class BTLogActionHandler : ABTNodeHandler<BTLog>
    {
        protected override BTExecResult Run(BTLog node, BTEnv env)
        {
            BTExecutionContext context = env.BindContext(node);
            string message = context.GetStringArgument(node.Definition, "message", node.Definition?.Title);
            Log.Info($"[BehaviorTree][{context.TreeName}] {message}");
            return BTExecResult.Success;
        }
    }
}
