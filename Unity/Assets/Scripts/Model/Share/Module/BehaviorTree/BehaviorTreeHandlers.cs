using System;
using System.Collections.Generic;

namespace ET
{
    public abstract class BehaviorTreeHandlerAttribute : BaseAttribute
    {
        public string Name { get; }

        protected BehaviorTreeHandlerAttribute(string name)
        {
            this.Name = name;
        }
    }

    public sealed class BehaviorTreeActionHandlerAttribute : BehaviorTreeHandlerAttribute
    {
        public BehaviorTreeActionHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BehaviorTreeConditionHandlerAttribute : BehaviorTreeHandlerAttribute
    {
        public BehaviorTreeConditionHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BehaviorTreeServiceHandlerAttribute : BehaviorTreeHandlerAttribute
    {
        public BehaviorTreeServiceHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    [EnableClass]
    public sealed class BehaviorTreeExecutionContext
    {
        private readonly EntityRef<Entity> owner;

        public BehaviorTreeExecutionContext(long runtimeId, string treeId, string treeName, Entity owner, BehaviorTreeBlackboard blackboard)
        {
            this.RuntimeId = runtimeId;
            this.TreeId = treeId;
            this.TreeName = treeName;
            this.owner = owner;
            this.Blackboard = blackboard;
        }

        public long RuntimeId { get; }

        public string TreeId { get; }

        public string TreeName { get; }

        public BehaviorTreeBlackboard Blackboard { get; }

        public Entity Owner => this.owner;

        public Scene Root => this.Owner?.Root();

        public bool TryGetOwner<T>(out T entity) where T : Entity
        {
            entity = this.Owner as T;
            return entity != null;
        }

        public bool TryGetArgument(BehaviorTreeNodeDefinition node, string argumentName, out BehaviorTreeArgumentDefinition argument)
        {
            argument = null;
            if (node == null || string.IsNullOrWhiteSpace(argumentName))
            {
                return false;
            }

            foreach (BehaviorTreeArgumentDefinition nodeArgument in node.Arguments)
            {
                if (string.Equals(nodeArgument.Name, argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    argument = nodeArgument;
                    return true;
                }
            }

            return false;
        }

        public string GetStringArgument(BehaviorTreeNodeDefinition node, string argumentName, string defaultValue = "")
        {
            return this.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetString(argument.Value, defaultValue)
                    : defaultValue;
        }

        public int GetIntArgument(BehaviorTreeNodeDefinition node, string argumentName, int defaultValue = 0)
        {
            return this.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetInt(argument.Value, defaultValue)
                    : defaultValue;
        }

        public long GetLongArgument(BehaviorTreeNodeDefinition node, string argumentName, long defaultValue = 0)
        {
            return this.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetLong(argument.Value, defaultValue)
                    : defaultValue;
        }

        public float GetFloatArgument(BehaviorTreeNodeDefinition node, string argumentName, float defaultValue = 0)
        {
            return this.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetFloat(argument.Value, defaultValue)
                    : defaultValue;
        }

        public bool GetBoolArgument(BehaviorTreeNodeDefinition node, string argumentName, bool defaultValue = false)
        {
            return this.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetBool(argument.Value, defaultValue)
                    : defaultValue;
        }

        public object GetArgumentValue(BehaviorTreeNodeDefinition node, string argumentName)
        {
            return this.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetValue(argument.Value)
                    : null;
        }
    }

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

    [Code]
    public class BehaviorTreeActionDispatcher : Singleton<BehaviorTreeActionDispatcher>, ISingletonAwake
    {
        private readonly Dictionary<string, ABehaviorTreeActionHandler> handlers = new(StringComparer.OrdinalIgnoreCase);

        public void Awake()
        {
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(BehaviorTreeActionHandlerAttribute));
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not ABehaviorTreeActionHandler handler)
                {
                    Log.Error($"behavior tree action handler invalid: {type.FullName}");
                    continue;
                }

                string handlerName = GetHandlerName(type, typeof(BehaviorTreeActionHandlerAttribute));
                if (this.handlers.ContainsKey(handlerName))
                {
                    Log.Error($"behavior tree action handler duplicate: {handlerName}");
                    continue;
                }

                this.handlers.Add(handlerName, handler);
            }
        }

        public ABehaviorTreeActionHandler Get(string handlerName)
        {
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            this.handlers.TryGetValue(handlerName, out ABehaviorTreeActionHandler handler);
            return handler;
        }

        private static string GetHandlerName(Type type, Type attributeType)
        {
            if (Attribute.GetCustomAttribute(type, attributeType) is BehaviorTreeHandlerAttribute attribute && !string.IsNullOrWhiteSpace(attribute.Name))
            {
                return attribute.Name;
            }

            return type.Name;
        }
    }

    [Code]
    public class BehaviorTreeConditionDispatcher : Singleton<BehaviorTreeConditionDispatcher>, ISingletonAwake
    {
        private readonly Dictionary<string, ABehaviorTreeConditionHandler> handlers = new(StringComparer.OrdinalIgnoreCase);

        public void Awake()
        {
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(BehaviorTreeConditionHandlerAttribute));
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not ABehaviorTreeConditionHandler handler)
                {
                    Log.Error($"behavior tree condition handler invalid: {type.FullName}");
                    continue;
                }

                string handlerName = GetHandlerName(type, typeof(BehaviorTreeConditionHandlerAttribute));
                if (this.handlers.ContainsKey(handlerName))
                {
                    Log.Error($"behavior tree condition handler duplicate: {handlerName}");
                    continue;
                }

                this.handlers.Add(handlerName, handler);
            }
        }

        public ABehaviorTreeConditionHandler Get(string handlerName)
        {
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            this.handlers.TryGetValue(handlerName, out ABehaviorTreeConditionHandler handler);
            return handler;
        }

        private static string GetHandlerName(Type type, Type attributeType)
        {
            if (Attribute.GetCustomAttribute(type, attributeType) is BehaviorTreeHandlerAttribute attribute && !string.IsNullOrWhiteSpace(attribute.Name))
            {
                return attribute.Name;
            }

            return type.Name;
        }
    }

    [Code]
    public class BehaviorTreeServiceDispatcher : Singleton<BehaviorTreeServiceDispatcher>, ISingletonAwake
    {
        private readonly Dictionary<string, ABehaviorTreeServiceHandler> handlers = new(StringComparer.OrdinalIgnoreCase);

        public void Awake()
        {
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(BehaviorTreeServiceHandlerAttribute));
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not ABehaviorTreeServiceHandler handler)
                {
                    Log.Error($"behavior tree service handler invalid: {type.FullName}");
                    continue;
                }

                string handlerName = GetHandlerName(type, typeof(BehaviorTreeServiceHandlerAttribute));
                if (this.handlers.ContainsKey(handlerName))
                {
                    Log.Error($"behavior tree service handler duplicate: {handlerName}");
                    continue;
                }

                this.handlers.Add(handlerName, handler);
            }
        }

        public ABehaviorTreeServiceHandler Get(string handlerName)
        {
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            this.handlers.TryGetValue(handlerName, out ABehaviorTreeServiceHandler handler);
            return handler;
        }

        private static string GetHandlerName(Type type, Type attributeType)
        {
            if (Attribute.GetCustomAttribute(type, attributeType) is BehaviorTreeHandlerAttribute attribute && !string.IsNullOrWhiteSpace(attribute.Name))
            {
                return attribute.Name;
            }

            return type.Name;
        }
    }
}
