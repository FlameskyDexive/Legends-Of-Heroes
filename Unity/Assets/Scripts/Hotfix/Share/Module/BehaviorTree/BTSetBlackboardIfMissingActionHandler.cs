namespace ET
{
    [BTNodeHandler]
    public sealed class BTSetBlackboardIfMissingActionHandler : ABTNodeHandler<BTSetBlackboardIfMissing>
    {
        protected override BTExecResult Run(BTSetBlackboardIfMissing node, BTEnv env)
        {
            BTExecutionContext context = env.BindContext(node);
            string key = context.GetStringArgument(node.Definition, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                return BTExecResult.Failure;
            }

            if (context.Blackboard.Contains(key))
            {
                return BTExecResult.Success;
            }

            object value = context.GetArgumentValue(node.Definition, "value");
            context.Blackboard.SetBoxed(key, value);
            return BTExecResult.Success;
        }
    }
}
