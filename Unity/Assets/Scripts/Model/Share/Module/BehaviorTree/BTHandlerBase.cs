namespace ET
{
    [BTActionHandler]
    public abstract class ABTActionHandler : HandlerObject
    {
        public abstract ETTask<BTNodeState> Execute(BTExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken);
    }

    [BTConditionHandler]
    public abstract class ABTConditionHandler : HandlerObject
    {
        public abstract bool Evaluate(BTExecutionContext context, BTNodeData node);
    }

    [BTServiceHandler]
    public abstract class ABTServiceHandler : HandlerObject
    {
        public abstract ETTask Tick(BTExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken);
    }
}
