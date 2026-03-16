using System;
using System.Collections.Generic;

namespace ET
{
    public static class BTGraphBuilder
    {
        public static BTRoot Build(BTExecutionSession session)
        {
            if (session == null || session.EntryDefinition == null)
            {
                return null;
            }

            int nextRuntimeNodeId = 1;
            HashSet<string> buildStack = new(StringComparer.OrdinalIgnoreCase);
            BTNode rootNode = BuildNode(session, session.EntryDefinition, session.EntryDefinition.RootNodeId, ref nextRuntimeNodeId, buildStack);
            return rootNode as BTRoot;
        }

        private static BTNode BuildNode(BTExecutionSession session, BTDefinition tree, string nodeId, ref int nextRuntimeNodeId, HashSet<string> buildStack)
        {
            if (session == null || tree == null || string.IsNullOrWhiteSpace(nodeId))
            {
                return null;
            }

            BTNodeData definition = tree.GetNode(nodeId);
            if (definition == null)
            {
                return null;
            }

            BTNode node = CreateNode(definition);
            if (node == null)
            {
                return null;
            }

            node.RuntimeNodeId = nextRuntimeNodeId++;
            node.SourceNodeId = definition.NodeId ?? string.Empty;
            node.TreeId = tree.TreeId ?? string.Empty;
            node.TreeName = tree.TreeName ?? string.Empty;
            node.Definition = definition;
            session.Nodes[node.RuntimeNodeId] = node;

            foreach (string childId in definition.ChildIds)
            {
                BTNode child = BuildNode(session, tree, childId, ref nextRuntimeNodeId, buildStack);
                if (child != null)
                {
                    node.Children.Add(child);
                }
            }

            if (node is BTSubTreeCall subTreeCall)
            {
                if (subTreeCall.Definition is not BTSubTreeNodeData subTreeNodeData)
                {
                    return node;
                }

                BTDefinition subTree = session.ResolveTree(subTreeNodeData.SubTreeId, subTreeNodeData.SubTreeName);
                if (subTree == null)
                {
                    return node;
                }

                string guardKey = !string.IsNullOrWhiteSpace(subTree.TreeId) ? subTree.TreeId : subTree.TreeName;
                if (!string.IsNullOrWhiteSpace(guardKey))
                {
                    if (!buildStack.Add(guardKey))
                    {
                        throw new Exception($"behavior tree subtree cycle detected: {guardKey}");
                    }
                }

                try
                {
                    subTreeCall.SubTreeRoot = BuildNode(session, subTree, subTree.RootNodeId, ref nextRuntimeNodeId, buildStack);
                }
                finally
                {
                    if (!string.IsNullOrWhiteSpace(guardKey))
                    {
                        buildStack.Remove(guardKey);
                    }
                }
            }

            return node;
        }

        private static BTNode CreateNode(BTNodeData definition)
        {
            switch (definition)
            {
                case BTLogNodeData:
                    return new BTLog();
                case BTSetBlackboardNodeData:
                    return new BTSetBlackboard();
                case BTSetBlackboardIfMissingData:
                    return new BTSetBlackboardIfMissing();
                case BTBlackboardExistsNodeData:
                    return new BTBlackboardExists();
                case BTBlackboardCompareNodeData:
                    return new BTBlackboardCompare();
                case BTPatrolNodeData:
                    return new BTPatrol();
                case BTHasPatrolPathNodeData:
                    return new BTHasPatrolPath();
                case BTRootNodeData:
                    return new BTRoot();
                case BTSequenceNodeData:
                    return new BTSequence();
                case BTSelectorNodeData:
                    return new BTSelector();
                case BTParallelNodeData:
                    return new BTParallel();
                case BTInverterNodeData:
                    return new BTInverter();
                case BTSucceederNodeData:
                    return new BTSucceeder();
                case BTFailerNodeData:
                    return new BTFailer();
                case BTRepeaterNodeData:
                    return new BTRepeater();
                case BTBlackboardConditionNodeData blackboardConditionNodeData:
                    return new BTBlackboardCondition();
                case BTServiceNodeData serviceNodeData:
                    return CreateServiceCall(serviceNodeData);
                case BTActionNodeData actionNodeData:
                    return CreateActionNode(actionNodeData);
                case BTConditionNodeData conditionNodeData:
                    return CreateConditionNode(conditionNodeData);
                case BTWaitNodeData:
                    return new BTWait();
                case BTSubTreeNodeData subTreeNodeData:
                    return new BTSubTreeCall();
                default:
                    return null;
            }
        }

        private static BTNode CreateActionNode(BTActionNodeData definition)
        {
            if (string.Equals(definition.TypeId, BTBuiltinNodeTypes.Log, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ActionHandlerName, "Log", StringComparison.OrdinalIgnoreCase))
            {
                return new BTLog();
            }

            if (string.Equals(definition.TypeId, BTBuiltinNodeTypes.SetBlackboard, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ActionHandlerName, "SetBlackboard", StringComparison.OrdinalIgnoreCase))
            {
                return new BTSetBlackboard();
            }

            if (string.Equals(definition.TypeId, BTBuiltinNodeTypes.SetBlackboardIfMissing, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ActionHandlerName, "SetBlackboardIfMissing", StringComparison.OrdinalIgnoreCase))
            {
                return new BTSetBlackboardIfMissing();
            }

            if (string.Equals(definition.TypeId, BTPatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ActionHandlerName, "BTPatrol", StringComparison.OrdinalIgnoreCase))
            {
                return new BTPatrol();
            }

            return new BTActionCall();
        }

        private static BTNode CreateConditionNode(BTConditionNodeData definition)
        {
            if (string.Equals(definition.TypeId, BTBuiltinNodeTypes.BlackboardExists, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ConditionHandlerName, "BlackboardExists", StringComparison.OrdinalIgnoreCase))
            {
                return new BTBlackboardExists();
            }

            if (string.Equals(definition.TypeId, BTBuiltinNodeTypes.BlackboardCompare, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ConditionHandlerName, "BlackboardCompare", StringComparison.OrdinalIgnoreCase))
            {
                return new BTBlackboardCompare();
            }

            if (string.Equals(definition.TypeId, BTPatrolNodeTypes.HasPatrolPath, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ConditionHandlerName, "BTHasPatrolPath", StringComparison.OrdinalIgnoreCase))
            {
                return new BTHasPatrolPath();
            }

            return new BTConditionCall();
        }

        private static BTServiceCall CreateServiceCall(BTServiceNodeData definition)
        {
            return new BTServiceCall();
        }
    }
}
