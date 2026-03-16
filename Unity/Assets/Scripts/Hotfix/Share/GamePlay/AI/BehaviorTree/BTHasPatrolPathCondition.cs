namespace ET
{
    [BTNodeHandler]
    public sealed class BTHasPatrolPathCondition : ABTNodeHandler<BTHasPatrolPath>
    {
        protected override BTExecResult Run(BTHasPatrolPath node, BTEnv env)
        {
            BTExecutionContext context = env.BindContext(node);
            return context.TryGetOwner<Unit>(out Unit unit) && !unit.IsDisposed && unit.GetComponent<PatrolComponent>() != null
                    ? BTExecResult.Success
                    : BTExecResult.Failure;
        }
    }
}
