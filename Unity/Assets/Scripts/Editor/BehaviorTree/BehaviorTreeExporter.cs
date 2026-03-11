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
            BehaviorTreePackage package = BuildPackage(rootAsset);
            byte[] bytes = BehaviorTreeSerializer.Serialize(package);
            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? string.Empty;
            string fullPath = Path.GetFullPath(Path.Combine(projectRoot, rootAsset.ExportRelativePath));
            string directory = Path.GetDirectoryName(fullPath) ?? string.Empty;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(fullPath, bytes);
            AssetDatabase.Refresh();
            return fullPath;
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

        private static BehaviorTreeNodeDefinition BuildNode(BehaviorTreeEditorNodeData node)
        {
            node.SyncSubTreeInfo();

            BehaviorTreeNodeDefinition definition = new()
            {
                NodeId = node.NodeId,
                Title = node.Title,
                NodeKind = node.NodeKind,
                HandlerName = node.HandlerName,
                BlackboardKey = node.BlackboardKey,
                CompareOperator = node.CompareOperator,
                CompareValue = node.CompareValue?.Clone() ?? new BehaviorTreeSerializedValue(),
                AbortMode = node.AbortMode,
                WaitMilliseconds = node.WaitMilliseconds,
                IntervalMilliseconds = node.IntervalMilliseconds,
                MaxLoopCount = node.MaxLoopCount,
                SuccessPolicy = node.SuccessPolicy,
                FailurePolicy = node.FailurePolicy,
                Comment = node.Comment,
                SubTreeId = node.SubTreeId,
                SubTreeName = node.SubTreeName,
            };

            definition.ChildIds.AddRange(node.ChildIds);

            foreach (BehaviorTreeArgumentDefinition argument in node.Arguments)
            {
                definition.Arguments.Add(argument.Clone());
            }

            return definition;
        }
    }
}
