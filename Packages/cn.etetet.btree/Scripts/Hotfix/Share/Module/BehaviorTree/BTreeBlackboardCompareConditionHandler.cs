namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeBlackboardCompareConditionHandler : ABTreeNodeHandler<BTreeBlackboardCompare>
    {
        protected override BTreeExecResult Run(BTreeBlackboardCompare node, BTreeEnv env)
        {
            BTreeExecutionContext context = env.BindContext(node);
            string key = context.GetStringArgument(node.Definition, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                return BTreeExecResult.Failure;
            }

            BTreeCompareOperator compareOperator = (BTreeCompareOperator)context.GetIntArgument(node.Definition, "operator", (int)BTreeCompareOperator.Equal);
            object currentValue = context.Blackboard.GetBoxed(key);
            if (!context.TryGetArgument(node.Definition, "value", out BTreeArgumentData argument))
            {
                return BTreeValueUtility.Compare(currentValue, compareOperator, new BTreeSerializedValue()) ? BTreeExecResult.Success : BTreeExecResult.Failure;
            }

            return BTreeValueUtility.Compare(currentValue, compareOperator, argument.Value) ? BTreeExecResult.Success : BTreeExecResult.Failure;
        }
    }
}
