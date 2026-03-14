using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class BehaviorTreeExporter
    {
        public static BehaviorTreePackage BuildPackage(BehaviorTreeAsset rootAsset)
        {
            if (rootAsset == null)
            {
                throw new ArgumentNullException(nameof(rootAsset));
            }

            rootAsset.EnsureInitialized();
            HashSet<BehaviorTreeAsset> visitedAssets = new();
            List<BehaviorTreeDefinition> trees = new();
            HashSet<string> treeIds = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> treeNames = new(StringComparer.OrdinalIgnoreCase);

            Collect(rootAsset, visitedAssets, trees, treeIds, treeNames);

            return new BehaviorTreePackage()
            {
                PackageId = rootAsset.TreeId,
                PackageName = rootAsset.TreeName,
                EntryTreeId = rootAsset.TreeId,
                EntryTreeName = rootAsset.TreeName,
                Trees = trees,
            };
        }

        public static string ExportToFile(BehaviorTreeAsset rootAsset)
        {
            return ExportToFiles(rootAsset).ClientFullPath;
        }

        public static BehaviorTreeExportResult ExportToFiles(BehaviorTreeAsset rootAsset)
        {
            BehaviorTreePackage package = BuildPackage(rootAsset);
            byte[] bytes = BehaviorTreeSerializer.Serialize(package);
            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? string.Empty;
            string clientFullPath = Path.GetFullPath(Path.Combine(projectRoot, rootAsset.ExportRelativePath));
            string clientDirectory = Path.GetDirectoryName(clientFullPath) ?? string.Empty;
            if (!Directory.Exists(clientDirectory))
            {
                Directory.CreateDirectory(clientDirectory);
            }

            string serverFileName = Path.GetFileName(rootAsset.ExportRelativePath);
            string serverFullPath = Path.GetFullPath(Path.Combine(projectRoot, "..", BehaviorTreeLoader.ServerBehaviorTreeBytesDir, serverFileName));
            string serverDirectory = Path.GetDirectoryName(serverFullPath) ?? string.Empty;
            if (!Directory.Exists(serverDirectory))
            {
                Directory.CreateDirectory(serverDirectory);
            }

            File.WriteAllBytes(clientFullPath, bytes);
            File.WriteAllBytes(serverFullPath, bytes);
            AssetDatabase.Refresh();
            return new BehaviorTreeExportResult(clientFullPath, serverFullPath);
        }

        public readonly struct BehaviorTreeExportResult
        {
            public BehaviorTreeExportResult(string clientFullPath, string serverFullPath)
            {
                this.ClientFullPath = clientFullPath;
                this.ServerFullPath = serverFullPath;
            }

            public string ClientFullPath { get; }

            public string ServerFullPath { get; }
        }

        private static void Collect(BehaviorTreeAsset asset, HashSet<BehaviorTreeAsset> visitedAssets, List<BehaviorTreeDefinition> trees, HashSet<string> treeIds, HashSet<string> treeNames)
        {
            if (!visitedAssets.Add(asset))
            {
                return;
            }

            ValidateAsset(asset, treeIds, treeNames);
            trees.Add(BuildDefinition(asset));

            foreach (BehaviorTreeEditorNodeData node in asset.Nodes)
            {
                if (node.NodeKind != BehaviorTreeNodeKind.SubTree || node.SubTreeAsset == null)
                {
                    continue;
                }

                node.SubTreeAsset.EnsureInitialized();
                node.SyncSubTreeInfo();
                Collect(node.SubTreeAsset, visitedAssets, trees, treeIds, treeNames);
            }
        }

        private static void ValidateAsset(BehaviorTreeAsset asset, HashSet<string> treeIds, HashSet<string> treeNames)
        {
            if (string.IsNullOrWhiteSpace(asset.TreeId))
            {
                throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' missing TreeId.");
            }

            if (string.IsNullOrWhiteSpace(asset.TreeName))
            {
                throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' missing TreeName.");
            }

            if (!treeIds.Add(asset.TreeId))
            {
                throw new InvalidOperationException($"Duplicate BehaviorTree TreeId: {asset.TreeId}");
            }

            if (!treeNames.Add(asset.TreeName))
            {
                throw new InvalidOperationException($"Duplicate BehaviorTree TreeName: {asset.TreeName}");
            }

            if (asset.GetRootNode() == null)
            {
                throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' missing Root node.");
            }

            HashSet<string> nodeIds = new(StringComparer.OrdinalIgnoreCase);
            foreach (BehaviorTreeEditorNodeData node in asset.Nodes)
            {
                if (string.IsNullOrWhiteSpace(node.NodeId))
                {
                    throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' has node without NodeId.");
                }

                if (!nodeIds.Add(node.NodeId))
                {
                    throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' has duplicated NodeId: {node.NodeId}");
                }
            }

            foreach (BehaviorTreeEditorNodeData node in asset.Nodes)
            {
                foreach (string childId in node.ChildIds)
                {
                    if (!nodeIds.Contains(childId))
                    {
                        throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' node '{node.Title}' references missing child '{childId}'.");
                    }
                }

                if (node.NodeKind == BehaviorTreeNodeKind.SubTree && node.SubTreeAsset == null && string.IsNullOrWhiteSpace(node.SubTreeId) && string.IsNullOrWhiteSpace(node.SubTreeName))
                {
                    throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' subtree node '{node.Title}' missing SubTree reference.");
                }
            }
        }

        private static BehaviorTreeDefinition BuildDefinition(BehaviorTreeAsset asset)
        {
            BehaviorTreeDefinition definition = new()
            {
                TreeId = asset.TreeId,
                TreeName = asset.TreeName,
                Description = asset.Description,
                RootNodeId = asset.RootNodeId,
            };

            foreach (BehaviorTreeBlackboardEntryDefinition entry in asset.BlackboardEntries)
            {
                definition.BlackboardEntries.Add(entry.Clone());
            }

            foreach (BehaviorTreeEditorNodeData node in asset.Nodes)
            {
                definition.Nodes.Add(BuildNode(node));
            }

            return definition;
        }

        private static BTNodeData BuildNode(BehaviorTreeEditorNodeData node)
        {
            node.SyncSubTreeInfo();
            BehaviorTreeEditorUtility.SyncNodeDescriptor(node);

            BTNodeData definition = CreateNodeDefinition(node);
            definition.NodeId = node.NodeId;
            definition.Title = node.Title;
            definition.Comment = node.Comment;
            definition.ChildIds.AddRange(node.ChildIds);
            return definition;
        }

        private static BTNodeData CreateNodeDefinition(BehaviorTreeEditorNodeData node)
        {
            if (string.Equals(node.NodeTypeId, ET.Client.BehaviorTreeDemoNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
            {
                ET.Client.BTDemoPatrolNodeData patrolNode = new();
                foreach (ET.Client.BehaviorTreePatrolPointDefinition patrolPoint in node.PatrolPoints)
                {
                    patrolNode.PatrolPoints.Add(patrolPoint?.Clone() ?? new ET.Client.BehaviorTreePatrolPointDefinition());
                }

                return patrolNode;
            }

            return node.NodeKind switch
            {
                BehaviorTreeNodeKind.Root => new BTRootNodeData(),
                BehaviorTreeNodeKind.Sequence => new BTSequenceNodeData(),
                BehaviorTreeNodeKind.Selector => new BTSelectorNodeData(),
                BehaviorTreeNodeKind.Parallel => new BTParallelNodeData
                {
                    SuccessPolicy = node.SuccessPolicy,
                    FailurePolicy = node.FailurePolicy,
                },
                BehaviorTreeNodeKind.Inverter => new BTInverterNodeData(),
                BehaviorTreeNodeKind.Succeeder => new BTSucceederNodeData(),
                BehaviorTreeNodeKind.Failer => new BTFailerNodeData(),
                BehaviorTreeNodeKind.Repeater => new BTRepeaterNodeData
                {
                    MaxLoopCount = node.MaxLoopCount,
                },
                BehaviorTreeNodeKind.BlackboardCondition => new BTBlackboardConditionNodeData
                {
                    BlackboardKey = node.BlackboardKey,
                    CompareOperator = node.CompareOperator,
                    CompareValue = node.CompareValue?.Clone() ?? new BehaviorTreeSerializedValue(),
                    AbortMode = node.AbortMode,
                },
                BehaviorTreeNodeKind.Service => CreateServiceNodeDefinition(node),
                BehaviorTreeNodeKind.Action => CreateActionNodeDefinition(node),
                BehaviorTreeNodeKind.Condition => CreateConditionNodeDefinition(node),
                BehaviorTreeNodeKind.Wait => new BTWaitNodeData
                {
                    WaitMilliseconds = node.WaitMilliseconds,
                },
                BehaviorTreeNodeKind.SubTree => new BTSubTreeNodeData
                {
                    SubTreeId = node.SubTreeId,
                    SubTreeName = node.SubTreeName,
                },
                _ => throw new InvalidOperationException($"Unsupported behavior tree node kind: {node.NodeKind}"),
            };
        }

        private static BTActionNodeData CreateActionNodeDefinition(BehaviorTreeEditorNodeData node)
        {
            BTActionNodeData definition = new()
            {
                TypeId = node.NodeTypeId,
                ActionHandlerName = node.HandlerName,
            };

            foreach (BehaviorTreeArgumentDefinition argument in node.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BehaviorTreeArgumentDefinition());
            }

            return definition;
        }

        private static BTConditionNodeData CreateConditionNodeDefinition(BehaviorTreeEditorNodeData node)
        {
            BTConditionNodeData definition = new()
            {
                TypeId = node.NodeTypeId,
                ConditionHandlerName = node.HandlerName,
            };

            foreach (BehaviorTreeArgumentDefinition argument in node.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BehaviorTreeArgumentDefinition());
            }

            return definition;
        }

        private static BTServiceNodeData CreateServiceNodeDefinition(BehaviorTreeEditorNodeData node)
        {
            BTServiceNodeData definition = new()
            {
                TypeId = node.NodeTypeId,
                ServiceHandlerName = node.HandlerName,
                IntervalMilliseconds = node.IntervalMilliseconds,
            };

            foreach (BehaviorTreeArgumentDefinition argument in node.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BehaviorTreeArgumentDefinition());
            }

            return definition;
        }
    }
}
