using System;

namespace ET
{
    [BTNodeHandler]
    public sealed class BTConditionCallHandler : ABTNodeHandler<BTConditionCall>
    {
        protected override BTExecResult Run(BTConditionCall node, BTEnv env)
        {
            BTExecutionSession session = env.GetSession();
            session.UpdateTreeContext(node);
            if (BTHandlerUtility.TryGetTerminalResult(session, node, out BTExecResult result))
            {
                return result;
            }

            if (node.Definition is not BTConditionNodeData definition)
            {
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            ABTConditionHandler handler = BTConditionDispatcher.Instance.Get(definition.ConditionHandlerName);
            if (handler == null)
            {
                Log.Error($"behavior tree condition handler not found: {definition.ConditionHandlerName}");
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            bool passed;
            try
            {
                passed = handler.Evaluate(env.BindContext(node), node.Definition);
            }
            catch (Exception exception)
            {
                session.LogException(exception, node);
                session.SetState(node, BTNodeState.Failure);
                return BTExecResult.Failure;
            }

            BTExecResult conditionResult = passed ? BTExecResult.Success : BTExecResult.Failure;
            session.SetState(node, conditionResult.ToNodeState());
            return conditionResult;
        }
    }
}
