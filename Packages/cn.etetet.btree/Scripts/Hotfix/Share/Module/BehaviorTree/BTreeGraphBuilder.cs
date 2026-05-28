using System;
using System.Collections.Generic;

namespace ET
{
    public static class BTreeGraphBuilder
    {
        public static BTreeRoot Build(BTreeExecutionSession session)
        {
            if (session == null || session.EntryDefinition == null)
            {
                return null;
            }

            int nextRuntimeNodeId = 1;
            HashSet<string> buildStack = new(StringComparer.OrdinalIgnoreCase);
            BTreeNode rootNode = BuildNode(session, session.EntryDefinition, session.EntryDefinition.RootNodeId, ref nextRuntimeNodeId, buildStack);
            return rootNode as BTreeRoot;
        }

        private static BTreeNode BuildNode(BTreeExecutionSession session, BTreeDefinition tree, string nodeId, ref int nextRuntimeNodeId, HashSet<string> buildStack)
        {
            if (session == null || tree == null || string.IsNullOrWhiteSpace(nodeId))
            {
                return null;
            }

            BTreeNodeData definition = tree.GetNode(nodeId);
            if (definition == null)
            {
                return null;
            }

            BTreeNode node = CreateNode(definition);
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
                BTreeNode child = BuildNode(session, tree, childId, ref nextRuntimeNodeId, buildStack);
                if (child != null)
                {
                    node.Children.Add(child);
                }
            }

            if (node is BTreeSubTreeCall subTreeCall)
            {
                if (subTreeCall.Definition is not BTreeSubTreeNodeData subTreeNodeData)
                {
                    return node;
                }

                BTreeDefinition subTree = session.ResolveTree(subTreeNodeData.SubTreeId, subTreeNodeData.SubTreeName);
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

        private static BTreeNode CreateNode(BTreeNodeData definition)
        {
            switch (definition)
            {
                case BTreeLogNodeData:
                    return new BTreeLog();
                case BTreeSetBlackboardNodeData:
                    return new BTreeSetBlackboard();
                case BTreeSetBlackboardIfMissingData:
                    return new BTreeSetBlackboardIfMissing();
                case BTreeBlackboardExistsNodeData:
                    return new BTreeBlackboardExists();
                case BTreeBlackboardCompareNodeData:
                    return new BTreeBlackboardCompare();
                case BTreePatrolNodeData:
                    return new BTreePatrol();
                case BTreeHasPatrolPathNodeData:
                    return new BTreeHasPatrolPath();
                case BTreeRootNodeData:
                    return new BTreeRoot();
                case BTreeSequenceNodeData:
                    return new BTreeSequence();
                case BTreeSelectorNodeData:
                    return new BTreeSelector();
                case BTreeParallelNodeData:
                    return new BTreeParallel();
                case BTreeInverterNodeData:
                    return new BTreeInverter();
                case BTreeSucceederNodeData:
                    return new BTreeSucceeder();
                case BTreeFailerNodeData:
                    return new BTreeFailer();
                case BTreeRepeaterNodeData:
                    return new BTreeRepeater();
                case BTreeBlackboardConditionNodeData blackboardConditionNodeData:
                    return new BTreeBlackboardCondition();
                case BTreeServiceNodeData serviceNodeData:
                    return CreateServiceCall(serviceNodeData);
                case BTreeActionNodeData actionNodeData:
                    return CreateActionNode(actionNodeData);
                case BTreeConditionNodeData conditionNodeData:
                    return CreateConditionNode(conditionNodeData);
                case BTreeWaitNodeData:
                    return new BTreeWait();
                case BTreeSubTreeNodeData subTreeNodeData:
                    return new BTreeSubTreeCall();
                default:
                    return null;
            }
        }

        private static BTreeNode CreateActionNode(BTreeActionNodeData definition)
        {
            if (string.Equals(definition.TypeId, BTreeBuiltinNodeTypes.Log, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ActionHandlerName, "Log", StringComparison.OrdinalIgnoreCase))
            {
                return new BTreeLog();
            }

            if (string.Equals(definition.TypeId, BTreeBuiltinNodeTypes.SetBlackboard, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ActionHandlerName, "SetBlackboard", StringComparison.OrdinalIgnoreCase))
            {
                return new BTreeSetBlackboard();
            }

            if (string.Equals(definition.TypeId, BTreeBuiltinNodeTypes.SetBlackboardIfMissing, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ActionHandlerName, "SetBlackboardIfMissing", StringComparison.OrdinalIgnoreCase))
            {
                return new BTreeSetBlackboardIfMissing();
            }

            if (string.Equals(definition.TypeId, BTreePatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ActionHandlerName, "BTreePatrol", StringComparison.OrdinalIgnoreCase))
            {
                return new BTreePatrol();
            }

            return new BTreeActionCall();
        }

        private static BTreeNode CreateConditionNode(BTreeConditionNodeData definition)
        {
            if (string.Equals(definition.TypeId, BTreeBuiltinNodeTypes.BlackboardExists, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ConditionHandlerName, "BlackboardExists", StringComparison.OrdinalIgnoreCase))
            {
                return new BTreeBlackboardExists();
            }

            if (string.Equals(definition.TypeId, BTreeBuiltinNodeTypes.BlackboardCompare, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ConditionHandlerName, "BlackboardCompare", StringComparison.OrdinalIgnoreCase))
            {
                return new BTreeBlackboardCompare();
            }

            if (string.Equals(definition.TypeId, BTreePatrolNodeTypes.HasPatrolPath, StringComparison.OrdinalIgnoreCase)
                || string.Equals(definition.ConditionHandlerName, "BTreeHasPatrolPath", StringComparison.OrdinalIgnoreCase))
            {
                return new BTreeHasPatrolPath();
            }

            return new BTreeConditionCall();
        }

        private static BTreeServiceCall CreateServiceCall(BTreeServiceNodeData definition)
        {
            return new BTreeServiceCall();
        }
    }
}
