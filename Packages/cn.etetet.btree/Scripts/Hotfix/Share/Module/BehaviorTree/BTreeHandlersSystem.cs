using System;
using System.Collections.Generic;

namespace ET
{
    public static class BTreeHandlersSystem
    {
        public static bool TryGetOwner<T>(this BTreeExecutionContext self, out T entity) where T : Entity
        {
            entity = self.Owner as T;
            return entity != null;
        }

        public static Scene Root(this BTreeExecutionContext self)
        {
            Entity owner = self.Owner;
            return owner?.Root();
        }

        public static bool TryGetArgument(this BTreeExecutionContext self, BTreeNodeData node, string argumentName,
            out BTreeArgumentData argument)
        {
            argument = null;
            if (node is not IBTreeArgumentNodeData argumentNode || string.IsNullOrWhiteSpace(argumentName))
            {
                return false;
            }

            foreach (BTreeArgumentData nodeArgument in argumentNode.Arguments)
            {
                if (string.Equals(nodeArgument.Name, argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    argument = nodeArgument;
                    return true;
                }
            }

            return false;
        }

        public static string GetStringArgument(this BTreeExecutionContext self, BTreeNodeData node, string argumentName, string defaultValue = "")
        {
            return self.TryGetArgument(node, argumentName, out BTreeArgumentData argument)
                    ? BTreeValueUtility.GetString(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static int GetIntArgument(this BTreeExecutionContext self, BTreeNodeData node, string argumentName, int defaultValue = 0)
        {
            return self.TryGetArgument(node, argumentName, out BTreeArgumentData argument)
                    ? BTreeValueUtility.GetInt(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static long GetLongArgument(this BTreeExecutionContext self, BTreeNodeData node, string argumentName, long defaultValue = 0)
        {
            return self.TryGetArgument(node, argumentName, out BTreeArgumentData argument)
                    ? BTreeValueUtility.GetLong(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static float GetFloatArgument(this BTreeExecutionContext self, BTreeNodeData node, string argumentName, float defaultValue = 0)
        {
            return self.TryGetArgument(node, argumentName, out BTreeArgumentData argument)
                    ? BTreeValueUtility.GetFloat(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static bool GetBoolArgument(this BTreeExecutionContext self, BTreeNodeData node, string argumentName, bool defaultValue = false)
        {
            return self.TryGetArgument(node, argumentName, out BTreeArgumentData argument)
                    ? BTreeValueUtility.GetBool(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static object GetArgumentValue(this BTreeExecutionContext self, BTreeNodeData node, string argumentName)
        {
            return self.TryGetArgument(node, argumentName, out BTreeArgumentData argument)
                    ? BTreeValueUtility.GetValue(argument.Value)
                    : null;
        }

        public static string GetHandlerName(this BTreeNodeData node)
        {
            return node is IBTreeHandlerNodeData handlerNode ? handlerNode.HandlerName : string.Empty;
        }

        public static ABTreeActionHandler Get(this BTreeActionDispatcher self, string handlerName)
        {
            EnsureInitialized(self);
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            self.Handlers.TryGetValue(handlerName, out ABTreeActionHandler handler);
            return handler;
        }

        public static ABTreeConditionHandler Get(this BTreeConditionDispatcher self, string handlerName)
        {
            EnsureInitialized(self);
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            self.Handlers.TryGetValue(handlerName, out ABTreeConditionHandler handler);
            return handler;
        }

        public static ABTreeServiceHandler Get(this BTreeServiceDispatcher self, string handlerName)
        {
            EnsureInitialized(self);
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            self.Handlers.TryGetValue(handlerName, out ABTreeServiceHandler handler);
            return handler;
        }

        private static void EnsureInitialized(BTreeActionDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            FillHandlers<BTreeActionHandlerAttribute, ABTreeActionHandler>(self.Handlers);
        }

        private static void EnsureInitialized(BTreeConditionDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            FillHandlers<BTreeConditionHandlerAttribute, ABTreeConditionHandler>(self.Handlers);
        }

        private static void EnsureInitialized(BTreeServiceDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            FillHandlers<BTreeServiceHandlerAttribute, ABTreeServiceHandler>(self.Handlers);
        }

        private static void FillHandlers<TAttribute, THandler>(Dictionary<string, THandler> handlers)
            where TAttribute : BTreeHandlerAttribute where THandler : HandlerObject
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
            if (Attribute.GetCustomAttribute(type, attributeType) is BTreeHandlerAttribute attribute && !string.IsNullOrWhiteSpace(attribute.Name))
            {
                return attribute.Name;
            }

            return type.Name;
        }
    }
}
