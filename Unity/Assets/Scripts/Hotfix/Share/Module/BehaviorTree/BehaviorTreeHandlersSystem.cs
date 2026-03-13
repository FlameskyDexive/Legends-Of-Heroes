using System;
using System.Collections.Generic;

namespace ET
{
    public static class BehaviorTreeHandlersSystem
    {
        public static bool TryGetOwner<T>(this BehaviorTreeExecutionContext self, out T entity) where T : Entity
        {
            entity = self.Owner as T;
            return entity != null;
        }

        public static Scene Root(this BehaviorTreeExecutionContext self)
        {
            Entity owner = self.Owner;
            return owner?.Root();
        }

        public static bool TryGetArgument(this BehaviorTreeExecutionContext self, BehaviorTreeNodeDefinition node, string argumentName,
            out BehaviorTreeArgumentDefinition argument)
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

        public static string GetStringArgument(this BehaviorTreeExecutionContext self, BehaviorTreeNodeDefinition node, string argumentName, string defaultValue = "")
        {
            return self.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetString(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static int GetIntArgument(this BehaviorTreeExecutionContext self, BehaviorTreeNodeDefinition node, string argumentName, int defaultValue = 0)
        {
            return self.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetInt(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static long GetLongArgument(this BehaviorTreeExecutionContext self, BehaviorTreeNodeDefinition node, string argumentName, long defaultValue = 0)
        {
            return self.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetLong(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static float GetFloatArgument(this BehaviorTreeExecutionContext self, BehaviorTreeNodeDefinition node, string argumentName, float defaultValue = 0)
        {
            return self.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetFloat(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static bool GetBoolArgument(this BehaviorTreeExecutionContext self, BehaviorTreeNodeDefinition node, string argumentName, bool defaultValue = false)
        {
            return self.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetBool(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static object GetArgumentValue(this BehaviorTreeExecutionContext self, BehaviorTreeNodeDefinition node, string argumentName)
        {
            return self.TryGetArgument(node, argumentName, out BehaviorTreeArgumentDefinition argument)
                    ? BehaviorTreeValueUtility.GetValue(argument.Value)
                    : null;
        }

        public static ABehaviorTreeActionHandler Get(this BehaviorTreeActionDispatcher self, string handlerName)
        {
            EnsureInitialized(self);
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            self.Handlers.TryGetValue(handlerName, out ABehaviorTreeActionHandler handler);
            return handler;
        }

        public static ABehaviorTreeConditionHandler Get(this BehaviorTreeConditionDispatcher self, string handlerName)
        {
            EnsureInitialized(self);
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            self.Handlers.TryGetValue(handlerName, out ABehaviorTreeConditionHandler handler);
            return handler;
        }

        public static ABehaviorTreeServiceHandler Get(this BehaviorTreeServiceDispatcher self, string handlerName)
        {
            EnsureInitialized(self);
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            self.Handlers.TryGetValue(handlerName, out ABehaviorTreeServiceHandler handler);
            return handler;
        }

        private static void EnsureInitialized(BehaviorTreeActionDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            FillHandlers<BehaviorTreeActionHandlerAttribute, ABehaviorTreeActionHandler>(self.Handlers);
        }

        private static void EnsureInitialized(BehaviorTreeConditionDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            FillHandlers<BehaviorTreeConditionHandlerAttribute, ABehaviorTreeConditionHandler>(self.Handlers);
        }

        private static void EnsureInitialized(BehaviorTreeServiceDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            FillHandlers<BehaviorTreeServiceHandlerAttribute, ABehaviorTreeServiceHandler>(self.Handlers);
        }

        private static void FillHandlers<TAttribute, THandler>(Dictionary<string, THandler> handlers)
            where TAttribute : BehaviorTreeHandlerAttribute where THandler : HandlerObject
        {
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(TAttribute));
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is not THandler handler)
                {
                    Log.Error($"behavior tree handler invalid: {type.FullName}");
                    continue;
                }

                string handlerName = GetHandlerName(type, typeof(TAttribute));
                if (handlers.ContainsKey(handlerName))
                {
                    Log.Error($"behavior tree handler duplicate: {handlerName}");
                    continue;
                }

                handlers.Add(handlerName, handler);
            }
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
