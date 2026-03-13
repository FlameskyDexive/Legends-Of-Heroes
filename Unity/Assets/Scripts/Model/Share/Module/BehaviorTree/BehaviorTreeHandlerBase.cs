namespace ET
{
    [BehaviorTreeActionHandler]
    public abstract class ABehaviorTreeActionHandler : HandlerObject
    {
        public abstract ETTask<BehaviorTreeNodeState> Execute(BehaviorTreeExecutionContext context, BehaviorTreeNodeDefinition node, ETCancellationToken cancellationToken);
    }

    [BehaviorTreeConditionHandler]
    public abstract class ABehaviorTreeConditionHandler : HandlerObject
    {
        public abstract bool Evaluate(BehaviorTreeExecutionContext context, BehaviorTreeNodeDefinition node);
    }

    [BehaviorTreeServiceHandler]
    public abstract class ABehaviorTreeServiceHandler : HandlerObject
    {
        public abstract ETTask Tick(BehaviorTreeExecutionContext context, BehaviorTreeNodeDefinition node, ETCancellationToken cancellationToken);
    }
}
