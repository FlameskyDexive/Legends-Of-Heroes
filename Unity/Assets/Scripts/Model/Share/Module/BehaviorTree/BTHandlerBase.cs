namespace ET
{
    [BTActionHandler]
    public abstract class ABTActionHandler : HandlerObject
    {
        public abstract ETTask<BehaviorTreeNodeState> Execute(BehaviorTreeExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken);
    }

    [BTConditionHandler]
    public abstract class ABTConditionHandler : HandlerObject
    {
        public abstract bool Evaluate(BehaviorTreeExecutionContext context, BTNodeData node);
    }

    [BTServiceHandler]
    public abstract class ABTServiceHandler : HandlerObject
    {
        public abstract ETTask Tick(BehaviorTreeExecutionContext context, BTNodeData node, ETCancellationToken cancellationToken);
    }
}
