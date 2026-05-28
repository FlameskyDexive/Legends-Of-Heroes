namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeSetBlackboardActionHandler : ABTreeNodeHandler<BTreeSetBlackboard>
    {
        protected override BTreeExecResult Run(BTreeSetBlackboard node, BTreeEnv env)
        {
            BTreeExecutionContext context = env.BindContext(node);
            string key = context.GetStringArgument(node.Definition, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                return BTreeExecResult.Failure;
            }

            bool remove = context.GetBoolArgument(node.Definition, "remove");
            if (remove)
            {
                context.Blackboard.Remove(key);
                return BTreeExecResult.Success;
            }

            object value = context.GetArgumentValue(node.Definition, "value");
            context.Blackboard.SetBoxed(key, value);
            return BTreeExecResult.Success;
        }
    }
}
