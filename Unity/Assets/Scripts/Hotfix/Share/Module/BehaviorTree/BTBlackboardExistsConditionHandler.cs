namespace ET
{
    [BTConditionHandler("BlackboardExists")]
    public sealed class BTBlackboardExistsConditionHandler : ABTConditionHandler
    {
        public override bool Evaluate(BTExecutionContext context, BTNodeData node)
        {
            string key = context.GetStringArgument(node, "key");
            return !string.IsNullOrWhiteSpace(key) && context.Blackboard.Contains(key);
        }
    }
}
