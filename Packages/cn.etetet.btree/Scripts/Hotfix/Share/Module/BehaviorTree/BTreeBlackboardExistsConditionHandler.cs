namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeBlackboardExistsConditionHandler : ABTreeNodeHandler<BTreeBlackboardExists>
    {
        protected override BTreeExecResult Run(BTreeBlackboardExists node, BTreeEnv env)
        {
            BTreeExecutionContext context = env.BindContext(node);
            string key = context.GetStringArgument(node.Definition, "key");
            return !string.IsNullOrWhiteSpace(key) && context.Blackboard.Contains(key) ? BTreeExecResult.Success : BTreeExecResult.Failure;
        }
    }
}
