namespace ET
{
    [BTNodeHandler]
    public sealed class BTBlackboardExistsConditionHandler : ABTNodeHandler<BTBlackboardExists>
    {
        protected override BTExecResult Run(BTBlackboardExists node, BTEnv env)
        {
            BTExecutionContext context = env.BindContext(node);
            string key = context.GetStringArgument(node.Definition, "key");
            return !string.IsNullOrWhiteSpace(key) && context.Blackboard.Contains(key) ? BTExecResult.Success : BTExecResult.Failure;
        }
    }
}
