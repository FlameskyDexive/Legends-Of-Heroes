using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class BTExporter
    {
        public static BTPackage BuildPackage(BTAsset rootAsset)
        {
            if (rootAsset == null)
            {
                throw new ArgumentNullException(nameof(rootAsset));
            }

            rootAsset.EnsureInitialized();
            HashSet<BTAsset> visitedAssets = new();
            List<BTDefinition> trees = new();
            HashSet<string> treeIds = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> treeNames = new(StringComparer.OrdinalIgnoreCase);

            Collect(rootAsset, visitedAssets, trees, treeIds, treeNames);

            return new BTPackage()
            {
                PackageId = rootAsset.TreeId,
                PackageName = rootAsset.TreeName,
                EntryTreeId = rootAsset.TreeId,
                EntryTreeName = rootAsset.TreeName,
                Trees = trees,
            };
        }

        public static string ExportToFile(BTAsset rootAsset)
        {
            return ExportToFiles(rootAsset).ClientFullPath;
        }

        public static BTExportResult ExportToFiles(BTAsset rootAsset)
        {
            BTPackage package = BuildPackage(rootAsset);
            byte[] bytes = BTSerializer.Serialize(package);
            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? string.Empty;
            string clientFullPath = Path.GetFullPath(Path.Combine(projectRoot, rootAsset.ExportRelativePath));
            string clientDirectory = Path.GetDirectoryName(clientFullPath) ?? string.Empty;
            if (!Directory.Exists(clientDirectory))
            {
                Directory.CreateDirectory(clientDirectory);
            }

            string serverFileName = Path.GetFileName(rootAsset.ExportRelativePath);
            string serverFullPath = Path.GetFullPath(Path.Combine(projectRoot, "..", BTLoader.ServerBehaviorTreeBytesDir, serverFileName));
            string serverDirectory = Path.GetDirectoryName(serverFullPath) ?? string.Empty;
            if (!Directory.Exists(serverDirectory))
            {
                Directory.CreateDirectory(serverDirectory);
            }

            File.WriteAllBytes(clientFullPath, bytes);
            File.WriteAllBytes(serverFullPath, bytes);
            AssetDatabase.Refresh();
            return new BTExportResult(clientFullPath, serverFullPath);
        }

        public readonly struct BTExportResult
        {
            public BTExportResult(string clientFullPath, string serverFullPath)
            {
                this.ClientFullPath = clientFullPath;
                this.ServerFullPath = serverFullPath;
            }

            public string ClientFullPath { get; }

            public string ServerFullPath { get; }
        }

        private static void Collect(BTAsset asset, HashSet<BTAsset> visitedAssets, List<BTDefinition> trees, HashSet<string> treeIds, HashSet<string> treeNames)
        {
            if (!visitedAssets.Add(asset))
            {
                return;
            }

            ValidateAsset(asset, treeIds, treeNames);
            trees.Add(BuildDefinition(asset));

            foreach (BTEditorNodeData node in asset.Nodes)
            {
                if (node.NodeKind != BTNodeKind.SubTree || node.SubTreeAsset == null)
                {
                    continue;
                }

                node.SubTreeAsset.EnsureInitialized();
                node.SyncSubTreeInfo();
                Collect(node.SubTreeAsset, visitedAssets, trees, treeIds, treeNames);
            }
        }

        private static void ValidateAsset(BTAsset asset, HashSet<string> treeIds, HashSet<string> treeNames)
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
            foreach (BTEditorNodeData node in asset.Nodes)
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

            foreach (BTEditorNodeData node in asset.Nodes)
            {
                foreach (string childId in node.ChildIds)
                {
                    if (!nodeIds.Contains(childId))
                    {
                        throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' node '{node.Title}' references missing child '{childId}'.");
                    }
                }

                if (node.NodeKind == BTNodeKind.SubTree && node.SubTreeAsset == null && string.IsNullOrWhiteSpace(node.SubTreeId) && string.IsNullOrWhiteSpace(node.SubTreeName))
                {
                    throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' subtree node '{node.Title}' missing SubTree reference.");
                }
            }
        }

        private static BTDefinition BuildDefinition(BTAsset asset)
        {
            BTDefinition definition = new()
            {
                TreeId = asset.TreeId,
                TreeName = asset.TreeName,
                Description = asset.Description,
                RootNodeId = asset.RootNodeId,
            };

            foreach (BTBlackboardEntryData entry in asset.BlackboardEntries)
            {
                definition.BlackboardEntries.Add(entry.Clone());
            }

            foreach (BTEditorNodeData node in asset.Nodes)
            {
                definition.Nodes.Add(BuildNode(node));
            }

            return definition;
        }

        private static BTNodeData BuildNode(BTEditorNodeData node)
        {
            node.SyncSubTreeInfo();
            BTEditorUtility.SyncNodeDescriptor(node);

            BTNodeData definition = CreateNodeDefinition(node);
            definition.NodeId = node.NodeId;
            definition.Title = node.Title;
            definition.Comment = node.Comment;
            definition.ChildIds.AddRange(node.ChildIds);
            return definition;
        }

        private static BTNodeData CreateNodeDefinition(BTEditorNodeData node)
        {
            if (string.Equals(node.NodeTypeId, ET.BTPatrolNodeTypes.Patrol, StringComparison.OrdinalIgnoreCase))
            {
                ET.BTPatrolNodeData patrolNode = new();
                foreach (ET.BTPatrolPointData patrolPoint in node.PatrolPoints)
                {
                    patrolNode.PatrolPoints.Add(patrolPoint?.Clone() ?? new ET.BTPatrolPointData());
                }

                return patrolNode;
            }

            return node.NodeKind switch
            {
                BTNodeKind.Root => new BTRootNodeData(),
                BTNodeKind.Sequence => new BTSequenceNodeData(),
                BTNodeKind.Selector => new BTSelectorNodeData(),
                BTNodeKind.Parallel => new BTParallelNodeData
                {
                    SuccessPolicy = node.SuccessPolicy,
                    FailurePolicy = node.FailurePolicy,
                },
                BTNodeKind.Inverter => new BTInverterNodeData(),
                BTNodeKind.Succeeder => new BTSucceederNodeData(),
                BTNodeKind.Failer => new BTFailerNodeData(),
                BTNodeKind.Repeater => new BTRepeaterNodeData
                {
                    MaxLoopCount = node.MaxLoopCount,
                },
                BTNodeKind.BlackboardCondition => new BTBlackboardConditionNodeData
                {
                    BlackboardKey = node.BlackboardKey,
                    CompareOperator = node.CompareOperator,
                    CompareValue = node.CompareValue?.Clone() ?? new BTSerializedValue(),
                    AbortMode = node.AbortMode,
                },
                BTNodeKind.Service => CreateServiceNodeDefinition(node),
                BTNodeKind.Action => CreateActionNodeDefinition(node),
                BTNodeKind.Condition => CreateConditionNodeDefinition(node),
                BTNodeKind.Wait => new BTWaitNodeData
                {
                    WaitMilliseconds = node.WaitMilliseconds,
                },
                BTNodeKind.SubTree => new BTSubTreeNodeData
                {
                    SubTreeId = node.SubTreeId,
                    SubTreeName = node.SubTreeName,
                },
                _ => throw new InvalidOperationException($"Unsupported behavior tree node kind: {node.NodeKind}"),
            };
        }

        private static BTActionNodeData CreateActionNodeDefinition(BTEditorNodeData node)
        {
            BTActionNodeData definition = new()
            {
                TypeId = node.NodeTypeId,
                ActionHandlerName = node.HandlerName,
            };

            foreach (BTArgumentData argument in node.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }

        private static BTConditionNodeData CreateConditionNodeDefinition(BTEditorNodeData node)
        {
            BTConditionNodeData definition = new()
            {
                TypeId = node.NodeTypeId,
                ConditionHandlerName = node.HandlerName,
            };

            foreach (BTArgumentData argument in node.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }

        private static BTServiceNodeData CreateServiceNodeDefinition(BTEditorNodeData node)
        {
            BTServiceNodeData definition = new()
            {
                TypeId = node.NodeTypeId,
                ServiceHandlerName = node.HandlerName,
                IntervalMilliseconds = node.IntervalMilliseconds,
            };

            foreach (BTArgumentData argument in node.Arguments)
            {
                definition.Arguments.Add(argument?.Clone() ?? new BTArgumentData());
            }

            return definition;
        }
    }
}
