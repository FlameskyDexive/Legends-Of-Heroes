namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeSetBlackboardIfMissingActionHandler : ABTreeNodeHandler<BTreeSetBlackboardIfMissing>
    {
        protected override BTreeExecResult Run(BTreeSetBlackboardIfMissing node, BTreeEnv env)
        {
            BTreeExecutionContext context = env.BindContext(node);
            string key = context.GetStringArgument(node.Definition, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                return BTreeExecResult.Failure;
            }

            if (context.Blackboard.Contains(key))
            {
                return BTreeExecResult.Success;
            }

            object value = context.GetArgumentValue(node.Definition, "value");
            context.Blackboard.SetBoxed(key, value);
            return BTreeExecResult.Success;
        }
    }
}
