namespace ET.Client
{
    [BehaviorTreeConditionHandler("DemoClientHasXunLuoPath")]
    public sealed class DemoClientHasXunLuoPathCondition : ABehaviorTreeConditionHandler
    {
        public override bool Evaluate(BehaviorTreeExecutionContext context, BehaviorTreeNodeDefinition node)
        {
            return context.TryGetOwner<Unit>(out Unit unit) && !unit.IsDisposed && unit.GetComponent<XunLuoPathComponent>() != null;
        }
    }
}
