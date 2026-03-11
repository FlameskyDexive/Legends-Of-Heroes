using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ET
{
    public static class BehaviorTreeEditorUtility
    {
        private static string[] actionHandlerNames;
        private static string[] conditionHandlerNames;
        private static string[] serviceHandlerNames;

        public static string GetDefaultTitle(BehaviorTreeNodeKind nodeKind)
        {
            return nodeKind switch
            {
                BehaviorTreeNodeKind.Root => "Root",
                BehaviorTreeNodeKind.Sequence => "Sequence",
                BehaviorTreeNodeKind.Selector => "Selector",
                BehaviorTreeNodeKind.Parallel => "Parallel",
                BehaviorTreeNodeKind.Inverter => "Inverter",
                BehaviorTreeNodeKind.Succeeder => "Succeeder",
                BehaviorTreeNodeKind.Failer => "Failer",
                BehaviorTreeNodeKind.Repeater => "Repeater",
                BehaviorTreeNodeKind.BlackboardCondition => "Blackboard Condition",
                BehaviorTreeNodeKind.Service => "Service",
                BehaviorTreeNodeKind.Action => "Action",
                BehaviorTreeNodeKind.Condition => "Condition",
                BehaviorTreeNodeKind.Wait => "Wait",
                BehaviorTreeNodeKind.SubTree => "SubTree",
                _ => nodeKind.ToString(),
            };
        }

        public static bool CanDelete(BehaviorTreeEditorNodeData node)
        {
            return node != null && node.NodeKind != BehaviorTreeNodeKind.Root;
        }

        public static bool HasInputPort(BehaviorTreeNodeKind nodeKind)
        {
            return nodeKind != BehaviorTreeNodeKind.Root;
        }

        public static bool HasOutputPort(BehaviorTreeNodeKind nodeKind)
        {
            return nodeKind is not (BehaviorTreeNodeKind.Action or BehaviorTreeNodeKind.Condition or BehaviorTreeNodeKind.Wait or BehaviorTreeNodeKind.SubTree);
        }

        public static Port.Capacity GetOutputCapacity(BehaviorTreeNodeKind nodeKind)
        {
            return nodeKind is BehaviorTreeNodeKind.Root or BehaviorTreeNodeKind.Inverter or BehaviorTreeNodeKind.Succeeder or BehaviorTreeNodeKind.Failer or BehaviorTreeNodeKind.Repeater or BehaviorTreeNodeKind.BlackboardCondition or BehaviorTreeNodeKind.Service
                    ? Port.Capacity.Single
                    : Port.Capacity.Multi;
        }

        public static string[] GetActionHandlerNames()
        {
            return actionHandlerNames ??= GetHandlerNames<BehaviorTreeActionHandlerAttribute>();
        }

        public static string[] GetConditionHandlerNames()
        {
            return conditionHandlerNames ??= GetHandlerNames<BehaviorTreeConditionHandlerAttribute>();
        }

        public static string[] GetServiceHandlerNames()
        {
            return serviceHandlerNames ??= GetHandlerNames<BehaviorTreeServiceHandlerAttribute>();
        }

        public static void InvalidateHandlerCaches()
        {
            actionHandlerNames = null;
            conditionHandlerNames = null;
            serviceHandlerNames = null;
        }

        public static Color GetNodeColor(BehaviorTreeNodeState state)
        {
            return state switch
            {
                BehaviorTreeNodeState.Running => new Color(0.22f, 0.51f, 0.95f),
                BehaviorTreeNodeState.Success => new Color(0.20f, 0.70f, 0.30f),
                BehaviorTreeNodeState.Failure => new Color(0.82f, 0.25f, 0.25f),
                BehaviorTreeNodeState.Aborted => new Color(0.90f, 0.58f, 0.12f),
                _ => new Color(0.24f, 0.24f, 0.24f),
            };
        }

        private static string[] GetHandlerNames<T>() where T : Attribute
        {
            List<string> names = new();
            foreach (Type type in TypeCache.GetTypesWithAttribute<T>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (Attribute.GetCustomAttribute(type, typeof(T)) is BehaviorTreeHandlerAttribute handlerAttribute && !string.IsNullOrWhiteSpace(handlerAttribute.Name))
                {
                    names.Add(handlerAttribute.Name);
                }
                else
                {
                    names.Add(type.Name);
                }
            }

            names.Sort(StringComparer.OrdinalIgnoreCase);
            return names.ToArray();
        }
    }
}
