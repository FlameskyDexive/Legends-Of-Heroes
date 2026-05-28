using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class BTreeExporter
    {
        public static object BuildPackage(BTreeAsset rootAsset)
        {
            if (rootAsset == null)
            {
                throw new ArgumentNullException(nameof(rootAsset));
            }

            rootAsset.EnsureInitialized();
            HashSet<BTreeAsset> visitedAssets = new();
            List<object> trees = new();
            HashSet<string> treeIds = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> treeNames = new(StringComparer.OrdinalIgnoreCase);

            Collect(rootAsset, visitedAssets, trees, treeIds, treeNames);

            object package = BTreeEditorRuntimeBridge.CreateInstance("ET.BTreePackage");
            BTreeEditorRuntimeBridge.SetValue(package, "PackageId", rootAsset.TreeId);
            BTreeEditorRuntimeBridge.SetValue(package, "PackageName", rootAsset.TreeName);
            BTreeEditorRuntimeBridge.SetValue(package, "EntryTreeId", rootAsset.TreeId);
            BTreeEditorRuntimeBridge.SetValue(package, "EntryTreeName", rootAsset.TreeName);

            IList packageTrees = BTreeEditorRuntimeBridge.GetList(package, "Trees");
            foreach (object tree in trees)
            {
                packageTrees.Add(tree);
            }

            return package;
        }

        public static byte[] BuildBytes(BTreeAsset rootAsset)
        {
            object package = BuildPackage(rootAsset);
            return BTreeEditorRuntimeBridge.SerializePackage(package);
        }

        public static string ExportToFile(BTreeAsset rootAsset)
        {
            return ExportToFiles(rootAsset).ClientFullPath;
        }

        public static BTreeExportResult ExportToFiles(BTreeAsset rootAsset)
        {
            byte[] bytes = BuildBytes(rootAsset);
            string projectRoot = Path.GetDirectoryName(Application.dataPath) ?? string.Empty;
            string clientFullPath = Path.GetFullPath(Path.Combine(projectRoot, rootAsset.ExportRelativePath));
            string clientDirectory = Path.GetDirectoryName(clientFullPath) ?? string.Empty;
            if (!Directory.Exists(clientDirectory))
            {
                Directory.CreateDirectory(clientDirectory);
            }

            string serverFileName = Path.GetFileName(rootAsset.ExportRelativePath);
            string serverFullPath = Path.GetFullPath(Path.Combine(projectRoot, "..", BTreeBytesLoader.ServerBehaviorTreeBytesDir, serverFileName));
            string serverDirectory = Path.GetDirectoryName(serverFullPath) ?? string.Empty;
            if (!Directory.Exists(serverDirectory))
            {
                Directory.CreateDirectory(serverDirectory);
            }

            File.WriteAllBytes(clientFullPath, bytes);
            File.WriteAllBytes(serverFullPath, bytes);
            AssetDatabase.Refresh();
            return new BTreeExportResult(clientFullPath, serverFullPath);
        }

        public readonly struct BTreeExportResult
        {
            public BTreeExportResult(string clientFullPath, string serverFullPath)
            {
                this.ClientFullPath = clientFullPath;
                this.ServerFullPath = serverFullPath;
            }

            public string ClientFullPath { get; }

            public string ServerFullPath { get; }
        }

        private static void Collect(BTreeAsset asset, HashSet<BTreeAsset> visitedAssets, List<object> trees, HashSet<string> treeIds, HashSet<string> treeNames)
        {
            if (!visitedAssets.Add(asset))
            {
                return;
            }

            ValidateAsset(asset, treeIds, treeNames);
            trees.Add(BuildDefinition(asset));

            foreach (BTreeEditorNodeData node in asset.Nodes)
            {
                if (node.NodeKind != BTreeNodeKind.SubTree || node.SubTreeAsset == null)
                {
                    continue;
                }

                node.SubTreeAsset.EnsureInitialized();
                node.SyncSubTreeInfo();
                Collect(node.SubTreeAsset, visitedAssets, trees, treeIds, treeNames);
            }
        }

        private static void ValidateAsset(BTreeAsset asset, HashSet<string> treeIds, HashSet<string> treeNames)
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
            foreach (BTreeEditorNodeData node in asset.Nodes)
            {
                if (node == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(node.NodeId))
                {
                    throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' has node without NodeId.");
                }

                if (!nodeIds.Add(node.NodeId))
                {
                    throw new InvalidOperationException($"BehaviorTree asset '{asset.name}' contains duplicate NodeId: {node.NodeId}");
                }
            }
        }

        private static object BuildDefinition(BTreeAsset asset)
        {
            object definition = BTreeEditorRuntimeBridge.CreateInstance("ET.BTreeDefinition");
            BTreeEditorRuntimeBridge.SetValue(definition, "TreeId", asset.TreeId);
            BTreeEditorRuntimeBridge.SetValue(definition, "TreeName", asset.TreeName);
            BTreeEditorRuntimeBridge.SetValue(definition, "Description", asset.Description);
            BTreeEditorRuntimeBridge.SetValue(definition, "RootNodeId", asset.RootNodeId);

            IList blackboardEntries = BTreeEditorRuntimeBridge.GetList(definition, "BlackboardEntries");
            foreach (BTreeBlackboardEntryData entry in asset.BlackboardEntries)
            {
                blackboardEntries.Add(entry.Clone());
            }

            IList nodes = BTreeEditorRuntimeBridge.GetList(definition, "Nodes");
            foreach (BTreeEditorNodeData node in asset.Nodes)
            {
                nodes.Add(BuildNode(node));
            }

            return definition;
        }

        private static object BuildNode(BTreeEditorNodeData node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            BTreeEditorUtility.SyncNodeDescriptor(node);
            return BTreeEditorRuntimeNodeFactory.CreateFromEditorNode(node);
        }
    }
}
