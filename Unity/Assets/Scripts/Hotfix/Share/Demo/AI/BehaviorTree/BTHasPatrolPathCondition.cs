namespace ET
{
    [BTConditionHandler("BTHasPatrolPath")]
    public sealed class BTHasPatrolPathCondition : ABTConditionHandler
    {
        public override bool Evaluate(BTExecutionContext context, BTNodeData node)
        {
            return context.TryGetOwner<Unit>(out Unit unit) && !unit.IsDisposed && unit.GetComponent<PatrolComponent>() != null;
        }
    }
}
