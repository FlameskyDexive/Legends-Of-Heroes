namespace ET
{
    [BehaviorTreeConditionHandler("BlackboardExists")]
    public sealed class BehaviorTreeBlackboardExistsConditionHandler : ABehaviorTreeConditionHandler
    {
        public override bool Evaluate(BehaviorTreeExecutionContext context, BehaviorTreeNodeDefinition node)
        {
            string key = context.GetStringArgument(node, "key");
            return !string.IsNullOrWhiteSpace(key) && context.Blackboard.Contains(key);
        }
    }
}
