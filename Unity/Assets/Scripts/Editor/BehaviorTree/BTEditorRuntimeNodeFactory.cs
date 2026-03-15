using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ET
{
    internal static class BTEditorRuntimeNodeFactory
    {
        private const string ModelAssemblyName = "Unity.Model";
        private static readonly Dictionary<string, Type> TypeCache = new(StringComparer.Ordinal);

        public static BTNodeData CreateFromEditorNode(BTEditorNodeData node)
        {
            if (node == null)
            {
                return null;
            }

            if (string.Equals(node.NodeTypeId, BTPatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
            {
                return CreatePatrolNode(node.NodeId, node.Title, node.Comment, node.PatrolPoints, node.ChildIds);
            }

            return node.NodeKind switch
            {
                BTNodeKind.Root => CreateRootNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTNodeKind.Sequence => CreateSequenceNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTNodeKind.Selector => CreateSelectorNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTNodeKind.Parallel => CreateParallelNode(node.NodeId, node.Title, node.Comment, node.SuccessPolicy, node.FailurePolicy, node.ChildIds),
                BTNodeKind.Inverter => CreateInverterNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTNodeKind.Succeeder => CreateSucceederNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTNodeKind.Failer => CreateFailerNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTNodeKind.Repeater => CreateRepeaterNode(node.NodeId, node.Title, node.Comment, node.MaxLoopCount, node.ChildIds),
                BTNodeKind.BlackboardCondition => CreateBlackboardConditionNode(node.NodeId, node.Title, node.Comment, node.BlackboardKey, node.CompareOperator, node.CompareValue, node.AbortMode, node.ChildIds),
                BTNodeKind.Service => CreateServiceNode(node.NodeId, node.Title, node.Comment, node.NodeTypeId, node.HandlerName, node.IntervalMilliseconds, node.Arguments, node.ChildIds),
                BTNodeKind.Action => CreateActionNode(node.NodeId, node.Title, node.Comment, node.NodeTypeId, node.HandlerName, node.Arguments, node.ChildIds),
                BTNodeKind.Condition => CreateConditionNode(node.NodeId, node.Title, node.Comment, node.NodeTypeId, node.HandlerName, node.Arguments, node.ChildIds),
                BTNodeKind.Wait => CreateWaitNode(node.NodeId, node.Title, node.Comment, node.WaitMilliseconds, node.ChildIds),
                BTNodeKind.SubTree => CreateSubTreeNode(node.NodeId, node.Title, node.Comment, node.SubTreeId, node.SubTreeName, node.ChildIds),
                _ => throw new InvalidOperationException($"Unsupported runtime node kind: {node.NodeKind}"),
            };
        }

        public static BTNodeData CreateRootNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTRootNodeData", nodeId, title, comment, childIds);
        }

        public static BTNodeData CreateSequenceNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTSequenceNodeData", nodeId, title, comment, childIds);
        }

        public static BTNodeData CreateSelectorNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTSelectorNodeData", nodeId, title, comment, childIds);
        }

        public static BTNodeData CreateInverterNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTInverterNodeData", nodeId, title, comment, childIds);
        }

        public static BTNodeData CreateSucceederNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTSucceederNodeData", nodeId, title, comment, childIds);
        }

        public static BTNodeData CreateFailerNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTFailerNodeData", nodeId, title, comment, childIds);
        }

        public static BTNodeData CreateWaitNode(string nodeId, string title, string comment, int waitMilliseconds, IEnumerable<string> childIds = null)
        {
            BTNodeData node = CreateSimpleNode("ET.BTWaitNodeData", nodeId, title, comment, childIds);
            SetField(node, "WaitMilliseconds", waitMilliseconds);
            return node;
        }

        public static BTNodeData CreateParallelNode(string nodeId, string title, string comment, BTParallelPolicy successPolicy, BTParallelPolicy failurePolicy, IEnumerable<string> childIds = null)
        {
            BTNodeData node = CreateSimpleNode("ET.BTParallelNodeData", nodeId, title, comment, childIds);
            SetField(node, "SuccessPolicy", successPolicy);
            SetField(node, "FailurePolicy", failurePolicy);
            return node;
        }

        public static BTNodeData CreateRepeaterNode(string nodeId, string title, string comment, int maxLoopCount, IEnumerable<string> childIds = null)
        {
            BTNodeData node = CreateSimpleNode("ET.BTRepeaterNodeData", nodeId, title, comment, childIds);
            SetField(node, "MaxLoopCount", maxLoopCount);
            return node;
        }

        public static BTNodeData CreateBlackboardConditionNode(string nodeId, string title, string comment, string blackboardKey,
            BTCompareOperator compareOperator, BTSerializedValue compareValue, BTAbortMode abortMode, IEnumerable<string> childIds = null)
        {
            BTNodeData node = CreateSimpleNode("ET.BTBlackboardConditionNodeData", nodeId, title, comment, childIds);
            SetField(node, "BlackboardKey", blackboardKey ?? string.Empty);
            SetField(node, "CompareOperator", compareOperator);
            SetField(node, "CompareValue", compareValue?.Clone() ?? new BTSerializedValue());
            SetField(node, "AbortMode", abortMode);
            return node;
        }

        public static BTNodeData CreateActionNode(string nodeId, string title, string comment, string typeId, string handlerName,
            IEnumerable<BTArgumentData> arguments, IEnumerable<string> childIds = null)
        {
            BTNodeData node = CreateSimpleNode("ET.BTActionNodeData", nodeId, title, comment, childIds);
            SetField(node, "TypeId", typeId ?? string.Empty);
            SetField(node, "ActionHandlerName", handlerName ?? string.Empty);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTArgumentData()));
            return node;
        }

        public static BTNodeData CreateConditionNode(string nodeId, string title, string comment, string typeId, string handlerName,
            IEnumerable<BTArgumentData> arguments, IEnumerable<string> childIds = null)
        {
            BTNodeData node = CreateSimpleNode("ET.BTConditionNodeData", nodeId, title, comment, childIds);
            SetField(node, "TypeId", typeId ?? string.Empty);
            SetField(node, "ConditionHandlerName", handlerName ?? string.Empty);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTArgumentData()));
            return node;
        }

        public static BTNodeData CreateServiceNode(string nodeId, string title, string comment, string typeId, string handlerName,
            int intervalMilliseconds, IEnumerable<BTArgumentData> arguments, IEnumerable<string> childIds = null)
        {
            BTNodeData node = CreateSimpleNode("ET.BTServiceNodeData", nodeId, title, comment, childIds);
            SetField(node, "TypeId", typeId ?? string.Empty);
            SetField(node, "ServiceHandlerName", handlerName ?? string.Empty);
            SetField(node, "IntervalMilliseconds", intervalMilliseconds);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTArgumentData()));
            return node;
        }

        public static BTNodeData CreateSubTreeNode(string nodeId, string title, string comment, string subTreeId, string subTreeName, IEnumerable<string> childIds = null)
        {
            BTNodeData node = CreateSimpleNode("ET.BTSubTreeNodeData", nodeId, title, comment, childIds);
            SetField(node, "SubTreeId", subTreeId ?? string.Empty);
            SetField(node, "SubTreeName", subTreeName ?? string.Empty);
            return node;
        }

        public static BTNodeData CreatePatrolNode(string nodeId, string title, string comment, IEnumerable<BTPatrolPointData> patrolPoints, IEnumerable<string> childIds = null)
        {
            BTNodeData node = CreateSimpleNode("ET.BTPatrolNodeData", nodeId, title, comment, childIds);
            FillListField(node, "PatrolPoints", patrolPoints?.Select(point => point?.Clone() ?? new BTPatrolPointData()));
            return node;
        }

        public static bool IsRuntimeNodeType(BTNodeData node, string runtimeTypeName)
        {
            return node != null && string.Equals(node.GetType().Name, runtimeTypeName, StringComparison.Ordinal);
        }

        public static List<BTPatrolPointData> GetPatrolPoints(BTNodeData node)
        {
            FieldInfo fieldInfo = node?.GetType().GetField("PatrolPoints", BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo?.GetValue(node) is not IEnumerable enumerable)
            {
                return new List<BTPatrolPointData>();
            }

            List<BTPatrolPointData> points = new();
            foreach (object value in enumerable)
            {
                if (value is BTPatrolPointData point)
                {
                    points.Add(point);
                }
            }

            return points;
        }

        private static BTNodeData CreateSimpleNode(string fullTypeName, string nodeId, string title, string comment, IEnumerable<string> childIds)
        {
            BTNodeData node = Activator.CreateInstance(ResolveRuntimeType(fullTypeName)) as BTNodeData;
            if (node == null)
            {
                throw new InvalidOperationException($"Failed to create runtime node: {fullTypeName}");
            }

            node.NodeId = nodeId ?? string.Empty;
            node.Title = title ?? string.Empty;
            node.Comment = comment ?? string.Empty;
            node.ChildIds.Clear();
            if (childIds != null)
            {
                node.ChildIds.AddRange(childIds);
            }

            return node;
        }

        private static void FillListField(object target, string fieldName, IEnumerable values)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo?.GetValue(target) is not IList list || values == null)
            {
                return;
            }

            list.Clear();
            foreach (object value in values)
            {
                list.Add(value);
            }
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo == null)
            {
                throw new MissingFieldException(target.GetType().FullName, fieldName);
            }

            fieldInfo.SetValue(target, value);
        }

        private static Type ResolveRuntimeType(string fullTypeName)
        {
            if (TypeCache.TryGetValue(fullTypeName, out Type cachedType))
            {
                return cachedType;
            }

            Assembly modelAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(assembly => assembly.GetName().Name == ModelAssemblyName);
            if (modelAssembly == null)
            {
                throw new InvalidOperationException($"Runtime assembly not found: {ModelAssemblyName}");
            }

            Type runtimeType = modelAssembly.GetType(fullTypeName);
            if (runtimeType == null)
            {
                throw new InvalidOperationException($"Runtime node type not found: {fullTypeName}");
            }

            TypeCache[fullTypeName] = runtimeType;
            return runtimeType;
        }
    }
}
