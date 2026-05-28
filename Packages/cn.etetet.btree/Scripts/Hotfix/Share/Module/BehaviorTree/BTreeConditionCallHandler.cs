using System;

namespace ET
{
    [BTreeNodeHandler]
    public sealed class BTreeConditionCallHandler : ABTreeNodeHandler<BTreeConditionCall>
    {
        protected override BTreeExecResult Run(BTreeConditionCall node, BTreeEnv env)
        {
            BTreeExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTreeHandlerUtility.TryGetTerminalResult(session, node, out BTreeExecResult result))
            {
                return result;
            }

            if (node.Definition is not BTreeConditionNodeData definition)
            {
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            ABTreeConditionHandler handler = BTreeConditionDispatcher.Instance.Get(definition.ConditionHandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree condition handler not found: {definition.ConditionHandlerName}");
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            bool passed;
            try
            {
                passed = handler.Evaluate(env.BindContext(node), node.Definition);
            }
            catch (Exception exception)
            {
                session.LogException(exception, node);
                session.SetState(node, BTreeNodeState.Failure);
                return BTreeExecResult.Failure;
            }

            BTreeExecResult conditionResult = passed ? BTreeExecResult.Success : BTreeExecResult.Failure;
            session.SetState(node, conditionResult.ToNodeState());
            return conditionResult;
        }
    }
}
