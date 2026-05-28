namespace ET
{
    [BTreeActionHandler]
    public abstract class ABTreeActionHandler : HandlerObject
    {
        public abstract ETTask<BTreeNodeState> Execute(BTreeExecutionContext context, BTreeNodeData node, ETCancellationToken cancellationToken);
    }

    [BTreeConditionHandler]
    public abstract class ABTreeConditionHandler : HandlerObject
    {
        public abstract bool Evaluate(BTreeExecutionContext context, BTreeNodeData node);
    }

    [BTreeServiceHandler]
    public abstract class ABTreeServiceHandler : HandlerObject
    {
        public abstract ETTask Tick(BTreeExecutionContext context, BTreeNodeData node, ETCancellationToken cancellationToken);
    }
}
