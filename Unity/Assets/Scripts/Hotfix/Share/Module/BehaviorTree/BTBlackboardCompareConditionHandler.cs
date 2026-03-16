namespace ET
{
    [BTNodeHandler]
    public sealed class BTBlackboardCompareConditionHandler : ABTNodeHandler<BTBlackboardCompare>
    {
        protected override BTExecResult Run(BTBlackboardCompare node, BTEnv env)
        {
            BTExecutionContext context = env.BindContext(node);
            string key = context.GetStringArgument(node.Definition, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                return BTExecResult.Failure;
            }

            BTCompareOperator compareOperator = (BTCompareOperator)context.GetIntArgument(node.Definition, "operator", (int)BTCompareOperator.Equal);
            object currentValue = context.Blackboard.GetBoxed(key);
            if (!context.TryGetArgument(node.Definition, "value", out BTArgumentData argument))
            {
                return BTValueUtility.Compare(currentValue, compareOperator, new BTSerializedValue()) ? BTExecResult.Success : BTExecResult.Failure;
            }

            return BTValueUtility.Compare(currentValue, compareOperator, argument.Value) ? BTExecResult.Success : BTExecResult.Failure;
        }
    }
}
