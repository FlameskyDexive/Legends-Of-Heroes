using System;
using System.Collections.Generic;

namespace ET
{
    public interface IBTreeHandler
    {
        BTreeExecResult Handle(BTreeNode node, BTreeEnv env);

        Type GetNodeType();
    }

    [BTreeNodeHandler]
    public abstract class ABTreeNodeHandler<TNode> : HandlerObject, IBTreeHandler where TNode : BTreeNode
    {
        protected abstract BTreeExecResult Run(TNode node, BTreeEnv env);

        public BTreeExecResult Handle(BTreeNode node, BTreeEnv env)
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

    [CodeProcess]
    [AllowInstance]
    public class BTreeDispatcher : Singleton<BTreeDispatcher>, ISingletonAwake
    {
        private readonly Dictionary<Type, IBTreeHandler> handlers = new();

        public bool IsInitialized;

        public void Awake()
        {
        }

        public BTreeExecResult Handle(BTreeNode node, BTreeEnv env)
        {
            EnsureInitialized(this);

            if (node == null)
            {
                throw new Exception("behavior tree node is null");
            }

            if (!this.handlers.TryGetValue(node.GetType(), out IBTreeHandler handler))
            {
                throw new Exception($"behavior tree handler not found: {node.GetType().FullName}");
            }

            return handler.Handle(node, env);
        }

        private static void EnsureInitialized(BTreeDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(BTreeNodeHandlerAttribute));
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not IBTreeHandler handler)
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
