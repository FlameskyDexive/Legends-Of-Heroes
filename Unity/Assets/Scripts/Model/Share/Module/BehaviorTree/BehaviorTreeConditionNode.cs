using System;

namespace ET
{
    [EnableClass]
    internal sealed class BehaviorTreeConditionNode : BehaviorTreeRuntimeNode
    {
        public BehaviorTreeConditionNode(BehaviorTreeRunner runner, BehaviorTreeNodeDefinition definition, BehaviorTreeRuntimeNode parent) : base(runner, definition, parent)
        {
        }

        protected override void OnStart()
        {
            ABehaviorTreeConditionHandler handler = BehaviorTreeConditionDispatcher.Instance.Get(this.Definition.HandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree condition handler not found: {this.Definition.HandlerName}");
                this.Fail();
                return;
            }

            try
            {
                if (handler.Evaluate(this.Runner.Context, this.Definition))
                {
                    this.Succeed();
                    return;
                }

                this.Fail();
            }
            catch (Exception exception)
            {
                this.Runner.LogException(exception, this.Definition);
                this.Fail();
            }
        }
    }
}
