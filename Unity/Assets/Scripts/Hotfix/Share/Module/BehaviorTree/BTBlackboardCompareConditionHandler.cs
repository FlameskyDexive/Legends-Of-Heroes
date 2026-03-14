namespace ET
{
    [BTConditionHandler("BlackboardCompare")]
    public sealed class BTBlackboardCompareConditionHandler : ABTConditionHandler
    {
        public override bool Evaluate(BehaviorTreeExecutionContext context, BTNodeData node)
        {
            string key = context.GetStringArgument(node, "key");
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            BehaviorTreeCompareOperator compareOperator = (BehaviorTreeCompareOperator)context.GetIntArgument(node, "operator", (int)BehaviorTreeCompareOperator.Equal);
            object currentValue = context.Blackboard.GetBoxed(key);
            if (!context.TryGetArgument(node, "value", out BehaviorTreeArgumentDefinition argument))
            {
                return BehaviorTreeValueUtility.Compare(currentValue, compareOperator, new BehaviorTreeSerializedValue());
            }

            return BehaviorTreeValueUtility.Compare(currentValue, compareOperator, argument.Value);
        }
    }
}
