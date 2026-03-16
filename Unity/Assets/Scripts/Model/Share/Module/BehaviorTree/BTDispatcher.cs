using System;
using System.Collections.Generic;

namespace ET
{
    public interface IBTHandler
    {
        BTExecResult Handle(BTNode node, BTEnv env);

        Type GetNodeType();
    }

    [BTNodeHandler]
    public abstract class ABTNodeHandler<TNode> : HandlerObject, IBTHandler where TNode : BTNode
    {
        protected abstract BTExecResult Run(TNode node, BTEnv env);

        public BTExecResult Handle(BTNode node, BTEnv env)
        {
            if (node is not TNode typedNode)
            {
                throw new Exception($"behavior tree node type mismatch: {node?.GetType().FullName} -> {typeof(TNode).FullName}");
            }

            return this.Run(typedNode, env);
        }

        public Type GetNodeType()
        {
            return typeof(TNode);
        }
    }

    [Code]
    public class BTDispatcher : Singleton<BTDispatcher>, ISingletonAwake
    {
        private readonly Dictionary<Type, IBTHandler> handlers = new();

        public bool IsInitialized;

        public void Awake()
        {
        }

        public BTExecResult Handle(BTNode node, BTEnv env)
        {
            EnsureInitialized(this);

            if (node == null)
            {
                throw new Exception("behavior tree node is null");
            }

            if (!this.handlers.TryGetValue(node.GetType(), out IBTHandler handler))
            {
                throw new Exception($"behavior tree handler not found: {node.GetType().FullName}");
            }

            return handler.Handle(node, env);
        }

        private static void EnsureInitialized(BTDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(BTNodeHandlerAttribute));
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not IBTHandler handler)
                {
                    Log.Error($"behavior tree node handler invalid: {type.FullName}");
                    continue;
                }

                Type nodeType = handler.GetNodeType();
                if (nodeType == null)
                {
                    Log.Error($"behavior tree node handler missing node type: {type.FullName}");
                    continue;
                }

                if (!self.handlers.TryAdd(nodeType, handler))
                {
                    Log.Error($"behavior tree node handler duplicate: {nodeType.FullName}");
                }
            }
        }
    }
}
