namespace ET
{
    [BTNodeHandler]
    public sealed class BTSetBlackboardActionHandler : ABTNodeHandler<BTSetBlackboard>
    {
        protected override BTExecResult Run(BTSetBlackboard node, BTEnv env)
        {
            BTExecutionContext context = env.BindContext(node);
            string key = context.GetStringArgument(node.Definition, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                return BTExecResult.Failure;
            }

            bool remove = context.GetBoolArgument(node.Definition, "remove");
            if (remove)
            {
                context.Blackboard.Remove(key);
                return BTExecResult.Success;
            }

            object value = context.GetArgumentValue(node.Definition, "value");
            context.Blackboard.SetBoxed(key, value);
            return BTExecResult.Success;
        }
    }
}
