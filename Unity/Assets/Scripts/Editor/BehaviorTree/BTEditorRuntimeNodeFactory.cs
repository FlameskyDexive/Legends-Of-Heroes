using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ET
{
    internal static class BTEditorRuntimeNodeFactory
    {
        public static object CreateFromEditorNode(BTEditorNodeData node)
        {
            if (node == null)
            {
                return null;
            }

            if (string.Equals(node.NodeTypeId, BTPatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
            {
                return CreatePatrolNode(node.NodeId, node.Title, node.Comment, node.PatrolPoints, node.ChildIds);
            }

            if (string.Equals(node.NodeTypeId, BTPatrolNodeTypes.HasPatrolPath, StringComparison.OrdinalIgnoreCase))
            {
                return CreateHasPatrolPathNode(node.NodeId, node.Title, node.Comment, node.ChildIds);
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

        public static object CreateRootNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTRootNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateSequenceNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTSequenceNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateSelectorNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTSelectorNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateInverterNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTInverterNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateSucceederNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTSucceederNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateFailerNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTFailerNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateWaitNode(string nodeId, string title, string comment, int waitMilliseconds, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTWaitNodeData", nodeId, title, comment, childIds);
            SetField(node, "WaitMilliseconds", waitMilliseconds);
            return node;
        }

        public static object CreateParallelNode(string nodeId, string title, string comment, BTParallelPolicy successPolicy, BTParallelPolicy failurePolicy, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTParallelNodeData", nodeId, title, comment, childIds);
            SetField(node, "SuccessPolicy", successPolicy);
            SetField(node, "FailurePolicy", failurePolicy);
            return node;
        }

        public static object CreateRepeaterNode(string nodeId, string title, string comment, int maxLoopCount, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTRepeaterNodeData", nodeId, title, comment, childIds);
            SetField(node, "MaxLoopCount", maxLoopCount);
            return node;
        }

        public static object CreateBlackboardConditionNode(string nodeId, string title, string comment, string blackboardKey,
            BTCompareOperator compareOperator, BTSerializedValue compareValue, BTAbortMode abortMode, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTBlackboardConditionNodeData", nodeId, title, comment, childIds);
            SetField(node, "BlackboardKey", blackboardKey ?? string.Empty);
            SetField(node, "CompareOperator", compareOperator);
            SetField(node, "CompareValue", compareValue?.Clone() ?? new BTSerializedValue());
            SetField(node, "AbortMode", abortMode);
            return node;
        }

        public static object CreateActionNode(string nodeId, string title, string comment, string typeId, string handlerName,
            IEnumerable<BTArgumentData> arguments, IEnumerable<string> childIds = null)
        {
            if (string.Equals(typeId, BTBuiltinNodeTypes.Log, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTLogNodeData", nodeId, title, comment, arguments, childIds);
            }

            if (string.Equals(typeId, BTBuiltinNodeTypes.SetBlackboard, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTSetBlackboardNodeData", nodeId, title, comment, arguments, childIds);
            }

            if (string.Equals(typeId, BTBuiltinNodeTypes.SetBlackboardIfMissing, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTSetBlackboardIfMissingData", nodeId, title, comment, arguments, childIds);
            }

            object node = CreateSimpleNode("ET.BTActionNodeData", nodeId, title, comment, childIds);
            SetField(node, "TypeId", typeId ?? string.Empty);
            SetField(node, "ActionHandlerName", handlerName ?? string.Empty);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTArgumentData()));
            return node;
        }

        public static object CreateConditionNode(string nodeId, string title, string comment, string typeId, string handlerName,
            IEnumerable<BTArgumentData> arguments, IEnumerable<string> childIds = null)
        {
            if (string.Equals(typeId, BTBuiltinNodeTypes.BlackboardExists, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTBlackboardExistsNodeData", nodeId, title, comment, arguments, childIds);
            }

            if (string.Equals(typeId, BTBuiltinNodeTypes.BlackboardCompare, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTBlackboardCompareNodeData", nodeId, title, comment, arguments, childIds);
            }

            if (string.Equals(typeId, BTPatrolNodeTypes.HasPatrolPath, StringComparison.OrdinalIgnoreCase))
            {
                return CreateHasPatrolPathNode(nodeId, title, comment, childIds);
            }

            object node = CreateSimpleNode("ET.BTConditionNodeData", nodeId, title, comment, childIds);
            SetField(node, "TypeId", typeId ?? string.Empty);
            SetField(node, "ConditionHandlerName", handlerName ?? string.Empty);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTArgumentData()));
            return node;
        }

        public static object CreateServiceNode(string nodeId, string title, string comment, string typeId, string handlerName,
            int intervalMilliseconds, IEnumerable<BTArgumentData> arguments, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTServiceNodeData", nodeId, title, comment, childIds);
            SetField(node, "TypeId", typeId ?? string.Empty);
            SetField(node, "ServiceHandlerName", handlerName ?? string.Empty);
            SetField(node, "IntervalMilliseconds", intervalMilliseconds);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTArgumentData()));
            return node;
        }

        public static object CreateSubTreeNode(string nodeId, string title, string comment, string subTreeId, string subTreeName, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTSubTreeNodeData", nodeId, title, comment, childIds);
            SetField(node, "SubTreeId", subTreeId ?? string.Empty);
            SetField(node, "SubTreeName", subTreeName ?? string.Empty);
            return node;
        }

        public static object CreatePatrolNode(string nodeId, string title, string comment, IEnumerable<BTPatrolPointData> patrolPoints, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTPatrolNodeData", nodeId, title, comment, childIds);
            FillListField(node, "PatrolPoints", patrolPoints?.Select(point => point?.Clone() ?? new BTPatrolPointData()));
            return node;
        }

        public static object CreateHasPatrolPathNode(string nodeId, string title, string comment, IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTHasPatrolPathNodeData", nodeId, title, comment, childIds);
        }

        public static bool IsRuntimeNodeType(object node, string runtimeTypeName)
        {
            return BTEditorRuntimeBridge.IsRuntimeNodeData(node, runtimeTypeName);
        }

        public static List<BTPatrolPointData> GetPatrolPoints(object node)
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

        private static object CreateSimpleNode(string fullTypeName, string nodeId, string title, string comment, IEnumerable<string> childIds)
        {
            object node = BTEditorRuntimeBridge.CreateInstance(fullTypeName);
            if (node == null)
            {
                throw new InvalidOperationException($"Failed to create runtime node: {fullTypeName}");
            }

            SetField(node, "NodeId", nodeId ?? string.Empty);
            SetField(node, "Title", title ?? string.Empty);
            SetField(node, "Comment", comment ?? string.Empty);
            IList childIdList = BTEditorRuntimeBridge.GetList(node, "ChildIds");
            childIdList.Clear();
            if (childIds != null)
            {
                foreach (string childId in childIds)
                {
                    childIdList.Add(childId);
                }
            }

            return node;
        }

        private static object CreateTypedArgumentNode(string fullTypeName, string nodeId, string title, string comment,
            IEnumerable<BTArgumentData> arguments, IEnumerable<string> childIds)
        {
            object node = CreateSimpleNode(fullTypeName, nodeId, title, comment, childIds);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTArgumentData()));
            return node;
        }

        private static void FillListField(object target, string fieldName, IEnumerable values)
        {
            IList list = BTEditorRuntimeBridge.GetList(target, fieldName);
            list.Clear();
            if (values == null)
            {
                return;
            }

            foreach (object value in values)
            {
                list.Add(value);
            }
        }

        private static void SetField(object target, string fieldName, object value)
        {
            BTEditorRuntimeBridge.SetValue(target, fieldName, value);
        }
    }
}
