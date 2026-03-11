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

    [BehaviorTreeActionHandler("DemoClientPatrol")]
    public sealed class DemoClientPatrolAction : ABehaviorTreeActionHandler
    {
        public override async ETTask<BehaviorTreeNodeState> Execute(BehaviorTreeExecutionContext context, BehaviorTreeNodeDefinition node, ETCancellationToken cancellationToken)
        {
            if (!context.TryGetOwner<Unit>(out Unit unit) || unit.IsDisposed)
            {
                return BehaviorTreeNodeState.Failure;
            }

            XunLuoPathComponent xunLuoPathComponent = unit.GetComponent<XunLuoPathComponent>();
            if (xunLuoPathComponent == null)
            {
                return BehaviorTreeNodeState.Failure;
            }

            int moveResult = await unit.MoveToAsync(xunLuoPathComponent.GetCurrent(), cancellationToken);
            if (cancellationToken.IsCancel())
            {
                return BehaviorTreeNodeState.Aborted;
            }

            if (moveResult != WaitTypeError.Success)
            {
                return BehaviorTreeNodeState.Failure;
            }

            xunLuoPathComponent.MoveNext();
            return BehaviorTreeNodeState.Success;
        }
    }
}
