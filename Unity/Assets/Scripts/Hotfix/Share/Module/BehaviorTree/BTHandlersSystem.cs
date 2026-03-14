using System;
using System.Collections.Generic;

namespace ET
{
    public static class BTHandlersSystem
    {
        public static bool TryGetOwner<T>(this BTExecutionContext self, out T entity) where T : Entity
        {
            entity = self.Owner as T;
            return entity != null;
        }

        public static Scene Root(this BTExecutionContext self)
        {
            Entity owner = self.Owner;
            return owner?.Root();
        }

        public static bool TryGetArgument(this BTExecutionContext self, BTNodeData node, string argumentName,
            out BTArgumentData argument)
        {
            argument = null;
            if (node is not IBTArgumentNodeData argumentNode || string.IsNullOrWhiteSpace(argumentName))
            {
                return false;
            }

            foreach (BTArgumentData nodeArgument in argumentNode.Arguments)
            {
                if (string.Equals(nodeArgument.Name, argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    argument = nodeArgument;
                    return true;
                }
            }

            return false;
        }

        public static string GetStringArgument(this BTExecutionContext self, BTNodeData node, string argumentName, string defaultValue = "")
        {
            return self.TryGetArgument(node, argumentName, out BTArgumentData argument)
                    ? BTValueUtility.GetString(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static int GetIntArgument(this BTExecutionContext self, BTNodeData node, string argumentName, int defaultValue = 0)
        {
            return self.TryGetArgument(node, argumentName, out BTArgumentData argument)
                    ? BTValueUtility.GetInt(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static long GetLongArgument(this BTExecutionContext self, BTNodeData node, string argumentName, long defaultValue = 0)
        {
            return self.TryGetArgument(node, argumentName, out BTArgumentData argument)
                    ? BTValueUtility.GetLong(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static float GetFloatArgument(this BTExecutionContext self, BTNodeData node, string argumentName, float defaultValue = 0)
        {
            return self.TryGetArgument(node, argumentName, out BTArgumentData argument)
                    ? BTValueUtility.GetFloat(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static bool GetBoolArgument(this BTExecutionContext self, BTNodeData node, string argumentName, bool defaultValue = false)
        {
            return self.TryGetArgument(node, argumentName, out BTArgumentData argument)
                    ? BTValueUtility.GetBool(argument.Value, defaultValue)
                    : defaultValue;
        }

        public static object GetArgumentValue(this BTExecutionContext self, BTNodeData node, string argumentName)
        {
            return self.TryGetArgument(node, argumentName, out BTArgumentData argument)
                    ? BTValueUtility.GetValue(argument.Value)
                    : null;
        }

        public static string GetHandlerName(this BTNodeData node)
        {
            return node is IBTHandlerNodeData handlerNode ? handlerNode.HandlerName : string.Empty;
        }

        public static ABTActionHandler Get(this BTActionDispatcher self, string handlerName)
        {
            EnsureInitialized(self);
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            self.Handlers.TryGetValue(handlerName, out ABTActionHandler handler);
            return handler;
        }

        public static ABTConditionHandler Get(this BTConditionDispatcher self, string handlerName)
        {
            EnsureInitialized(self);
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            self.Handlers.TryGetValue(handlerName, out ABTConditionHandler handler);
            return handler;
        }

        public static ABTServiceHandler Get(this BTServiceDispatcher self, string handlerName)
        {
            EnsureInitialized(self);
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                return null;
            }

            self.Handlers.TryGetValue(handlerName, out ABTServiceHandler handler);
            return handler;
        }

        private static void EnsureInitialized(BTActionDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            FillHandlers<BTActionHandlerAttribute, ABTActionHandler>(self.Handlers);
        }

        private static void EnsureInitialized(BTConditionDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            FillHandlers<BTConditionHandlerAttribute, ABTConditionHandler>(self.Handlers);
        }

        private static void EnsureInitialized(BTServiceDispatcher self)
        {
            if (self.IsInitialized)
            {
                return;
            }

            self.IsInitialized = true;
            FillHandlers<BTServiceHandlerAttribute, ABTServiceHandler>(self.Handlers);
        }

        private static void FillHandlers<TAttribute, THandler>(Dictionary<string, THandler> handlers)
            where TAttribute : BTHandlerAttribute where THandler : HandlerObject
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
            if (Attribute.GetCustomAttribute(type, attributeType) is BTHandlerAttribute attribute && !string.IsNullOrWhiteSpace(attribute.Name))
            {
                return attribute.Name;
            }

            return type.Name;
        }
    }
}
