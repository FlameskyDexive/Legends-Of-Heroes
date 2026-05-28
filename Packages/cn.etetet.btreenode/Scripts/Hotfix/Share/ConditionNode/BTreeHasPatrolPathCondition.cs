namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeHasPatrolPathCondition : ABTreeNodeHandler<BTreeHasPatrolPath>
    {
        protected override BTreeExecResult Run(BTreeHasPatrolPath node, BTreeEnv env)
        {
            BTreeExecutionContext context = env.BindContext(node);
            return context.TryGetOwner<Unit>(out Unit unit) && !unit.IsDisposed && unit.GetComponent<PatrolComponent>() != null
                    ? BTreeExecResult.Success
                    : BTreeExecResult.Failure;
        }
    }
}
