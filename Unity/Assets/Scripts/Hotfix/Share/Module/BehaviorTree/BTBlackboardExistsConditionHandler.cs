namespace ET
{
    [BTConditionHandler("BlackboardExists")]
    public sealed class BTBlackboardExistsConditionHandler : ABTConditionHandler
    {
        public override bool Evaluate(BehaviorTreeExecutionContext context, BTNodeData node)
        {
            string key = context.GetStringArgument(node, "key");
            return !string.IsNullOrWhiteSpace(key) && context.Blackboard.Contains(key);
        }
    }
}
