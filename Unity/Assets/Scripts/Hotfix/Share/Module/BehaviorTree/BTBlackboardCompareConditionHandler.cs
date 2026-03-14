namespace ET
{
    [BTConditionHandler("BlackboardCompare")]
    public sealed class BTBlackboardCompareConditionHandler : ABTConditionHandler
    {
        public override bool Evaluate(BTExecutionContext context, BTNodeData node)
        {
            string key = context.GetStringArgument(node, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            BTCompareOperator compareOperator = (BTCompareOperator)context.GetIntArgument(node, "operator", (int)BTCompareOperator.Equal);
            object currentValue = context.Blackboard.GetBoxed(key);
            if (!context.TryGetArgument(node, "value", out BTArgumentData argument))
            {
                return BTValueUtility.Compare(currentValue, compareOperator, new BTSerializedValue());
            }

            return BTValueUtility.Compare(currentValue, compareOperator, argument.Value);
        }
    }
}
