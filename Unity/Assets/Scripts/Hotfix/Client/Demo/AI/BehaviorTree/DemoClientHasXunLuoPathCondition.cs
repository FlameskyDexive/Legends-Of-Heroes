namespace ET.Client
{
    [BTConditionHandler("DemoClientHasXunLuoPath")]
    public sealed class DemoClientHasXunLuoPathCondition : ABTConditionHandler
    {
        public override bool Evaluate(BehaviorTreeExecutionContext context, BTNodeData node)
        {
            return context.TryGetOwner<Unit>(out Unit unit) && !unit.IsDisposed && unit.GetComponent<XunLuoPathComponent>() != null;
        }
    }
}
