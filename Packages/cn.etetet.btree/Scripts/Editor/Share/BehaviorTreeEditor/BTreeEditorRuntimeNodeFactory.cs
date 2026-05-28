using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ET
{
    internal static class BTreeEditorRuntimeNodeFactory
    {
        public static object CreateFromEditorNode(BTreeEditorNodeData node)
        {
            if (node == null)
            {
                return null;
            }

            if (string.Equals(node.NodeTypeId, BTreePatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
            {
                return CreatePatrolNode(node.NodeId, node.Title, node.Comment, node.PatrolPoints, node.ChildIds);
            }

            if (string.Equals(node.NodeTypeId, BTreePatrolNodeTypes.HasPatrolPath, StringComparison.OrdinalIgnoreCase))
            {
                return CreateHasPatrolPathNode(node.NodeId, node.Title, node.Comment, node.ChildIds);
            }

            return node.NodeKind switch
            {
                BTreeNodeKind.Root => CreateRootNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTreeNodeKind.Sequence => CreateSequenceNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTreeNodeKind.Selector => CreateSelectorNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTreeNodeKind.Parallel => CreateParallelNode(node.NodeId, node.Title, node.Comment, node.SuccessPolicy, node.FailurePolicy, node.ChildIds),
                BTreeNodeKind.Inverter => CreateInverterNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTreeNodeKind.Succeeder => CreateSucceederNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTreeNodeKind.Failer => CreateFailerNode(node.NodeId, node.Title, node.Comment, node.ChildIds),
                BTreeNodeKind.Repeater => CreateRepeaterNode(node.NodeId, node.Title, node.Comment, node.MaxLoopCount, node.ChildIds),
                BTreeNodeKind.BlackboardCondition => CreateBlackboardConditionNode(node.NodeId, node.Title, node.Comment, node.BlackboardKey, node.CompareOperator, node.CompareValue, node.AbortMode, node.ChildIds),
                BTreeNodeKind.Service => CreateServiceNode(node.NodeId, node.Title, node.Comment, node.NodeTypeId, node.HandlerName, node.IntervalMilliseconds, node.Arguments, node.ChildIds),
                BTreeNodeKind.Action => CreateActionNode(node.NodeId, node.Title, node.Comment, node.NodeTypeId, node.HandlerName, node.Arguments, node.ChildIds),
                BTreeNodeKind.Condition => CreateConditionNode(node.NodeId, node.Title, node.Comment, node.NodeTypeId, node.HandlerName, node.Arguments, node.ChildIds),
                BTreeNodeKind.Wait => CreateWaitNode(node.NodeId, node.Title, node.Comment, node.WaitMilliseconds, node.ChildIds),
                BTreeNodeKind.SubTree => CreateSubTreeNode(node.NodeId, node.Title, node.Comment, node.SubTreeId, node.SubTreeName, node.ChildIds),
                _ => throw new InvalidOperationException($"Unsupported runtime node kind: {node.NodeKind}"),
            };
        }

        public static object CreateRootNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTreeRootNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateSequenceNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTreeSequenceNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateSelectorNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTreeSelectorNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateInverterNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTreeInverterNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateSucceederNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTreeSucceederNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateFailerNode(string nodeId, string title, string comment = "", IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTreeFailerNodeData", nodeId, title, comment, childIds);
        }

        public static object CreateWaitNode(string nodeId, string title, string comment, int waitMilliseconds, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTreeWaitNodeData", nodeId, title, comment, childIds);
            SetField(node, "WaitMilliseconds", waitMilliseconds);
            return node;
        }

        public static object CreateParallelNode(string nodeId, string title, string comment, BTreeParallelPolicy successPolicy, BTreeParallelPolicy failurePolicy, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTreeParallelNodeData", nodeId, title, comment, childIds);
            SetField(node, "SuccessPolicy", successPolicy);
            SetField(node, "FailurePolicy", failurePolicy);
            return node;
        }

        public static object CreateRepeaterNode(string nodeId, string title, string comment, int maxLoopCount, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTreeRepeaterNodeData", nodeId, title, comment, childIds);
            SetField(node, "MaxLoopCount", maxLoopCount);
            return node;
        }

        public static object CreateBlackboardConditionNode(string nodeId, string title, string comment, string blackboardKey,
            BTreeCompareOperator compareOperator, BTreeSerializedValue compareValue, BTreeAbortMode abortMode, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTreeBlackboardConditionNodeData", nodeId, title, comment, childIds);
            SetField(node, "BlackboardKey", blackboardKey ?? string.Empty);
            SetField(node, "CompareOperator", compareOperator);
            SetField(node, "CompareValue", compareValue?.Clone() ?? new BTreeSerializedValue());
            SetField(node, "AbortMode", abortMode);
            return node;
        }

        public static object CreateActionNode(string nodeId, string title, string comment, string typeId, string handlerName,
            IEnumerable<BTreeArgumentData> arguments, IEnumerable<string> childIds = null)
        {
            if (string.Equals(typeId, BTreeBuiltinNodeTypes.Log, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTreeLogNodeData", nodeId, title, comment, arguments, childIds);
            }

            if (string.Equals(typeId, BTreeBuiltinNodeTypes.SetBlackboard, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTreeSetBlackboardNodeData", nodeId, title, comment, arguments, childIds);
            }

            if (string.Equals(typeId, BTreeBuiltinNodeTypes.SetBlackboardIfMissing, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTreeSetBlackboardIfMissingData", nodeId, title, comment, arguments, childIds);
            }

            object node = CreateSimpleNode("ET.BTreeActionNodeData", nodeId, title, comment, childIds);
            SetField(node, "TypeId", typeId ?? string.Empty);
            SetField(node, "ActionHandlerName", handlerName ?? string.Empty);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTreeArgumentData()));
            return node;
        }

        public static object CreateConditionNode(string nodeId, string title, string comment, string typeId, string handlerName,
            IEnumerable<BTreeArgumentData> arguments, IEnumerable<string> childIds = null)
        {
            if (string.Equals(typeId, BTreeBuiltinNodeTypes.BlackboardExists, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTreeBlackboardExistsNodeData", nodeId, title, comment, arguments, childIds);
            }

            if (string.Equals(typeId, BTreeBuiltinNodeTypes.BlackboardCompare, StringComparison.OrdinalIgnoreCase))
            {
                return CreateTypedArgumentNode("ET.BTreeBlackboardCompareNodeData", nodeId, title, comment, arguments, childIds);
            }

            if (string.Equals(typeId, BTreePatrolNodeTypes.HasPatrolPath, StringComparison.OrdinalIgnoreCase))
            {
                return CreateHasPatrolPathNode(nodeId, title, comment, childIds);
            }

            object node = CreateSimpleNode("ET.BTreeConditionNodeData", nodeId, title, comment, childIds);
            SetField(node, "TypeId", typeId ?? string.Empty);
            SetField(node, "ConditionHandlerName", handlerName ?? string.Empty);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTreeArgumentData()));
            return node;
        }

        public static object CreateServiceNode(string nodeId, string title, string comment, string typeId, string handlerName,
            int intervalMilliseconds, IEnumerable<BTreeArgumentData> arguments, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTreeServiceNodeData", nodeId, title, comment, childIds);
            SetField(node, "TypeId", typeId ?? string.Empty);
            SetField(node, "ServiceHandlerName", handlerName ?? string.Empty);
            SetField(node, "IntervalMilliseconds", intervalMilliseconds);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTreeArgumentData()));
            return node;
        }

        public static object CreateSubTreeNode(string nodeId, string title, string comment, string subTreeId, string subTreeName, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTreeSubTreeNodeData", nodeId, title, comment, childIds);
            SetField(node, "SubTreeId", subTreeId ?? string.Empty);
            SetField(node, "SubTreeName", subTreeName ?? string.Empty);
            return node;
        }

        public static object CreatePatrolNode(string nodeId, string title, string comment, IEnumerable<BTreePatrolPointData> patrolPoints, IEnumerable<string> childIds = null)
        {
            object node = CreateSimpleNode("ET.BTreePatrolNodeData", nodeId, title, comment, childIds);
            FillListField(node, "PatrolPoints", patrolPoints?.Select(point => point?.Clone() ?? new BTreePatrolPointData()));
            return node;
        }

        public static object CreateHasPatrolPathNode(string nodeId, string title, string comment, IEnumerable<string> childIds = null)
        {
            return CreateSimpleNode("ET.BTreeHasPatrolPathNodeData", nodeId, title, comment, childIds);
        }

        public static bool IsRuntimeNodeType(object node, string runtimeTypeName)
        {
            return BTreeEditorRuntimeBridge.IsRuntimeNodeData(node, runtimeTypeName);
        }

        public static List<BTreePatrolPointData> GetPatrolPoints(object node)
        {
            FieldInfo fieldInfo = node?.GetType().GetField("PatrolPoints", BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo?.GetValue(node) is not IEnumerable enumerable)
            {
                return new List<BTreePatrolPointData>();
            }

            List<BTreePatrolPointData> points = new();
            foreach (object value in enumerable)
            {
                if (value is BTreePatrolPointData point)
                {
                    points.Add(point);
                }
            }

            return points;
        }

        private static object CreateSimpleNode(string fullTypeName, string nodeId, string title, string comment, IEnumerable<string> childIds)
        {
            object node = BTreeEditorRuntimeBridge.CreateInstance(fullTypeName);
            if (node == null)
            {
                throw new InvalidOperationException($"Failed to create runtime node: {fullTypeName}");
            }

            SetField(node, "NodeId", nodeId ?? string.Empty);
            SetField(node, "Title", title ?? string.Empty);
            SetField(node, "Comment", comment ?? string.Empty);
            IList childIdList = BTreeEditorRuntimeBridge.GetList(node, "ChildIds");
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
            IEnumerable<BTreeArgumentData> arguments, IEnumerable<string> childIds)
        {
            object node = CreateSimpleNode(fullTypeName, nodeId, title, comment, childIds);
            FillListField(node, "Arguments", arguments?.Select(argument => argument?.Clone() ?? new BTreeArgumentData()));
            return node;
        }

        private static void FillListField(object target, string fieldName, IEnumerable values)
        {
            IList list = BTreeEditorRuntimeBridge.GetList(target, fieldName);
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
            BTreeEditorRuntimeBridge.SetValue(target, fieldName, value);
        }
    }
}
