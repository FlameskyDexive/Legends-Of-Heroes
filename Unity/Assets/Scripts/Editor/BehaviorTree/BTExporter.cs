using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class BTExporter
    {
        public static object BuildPackage(BTAsset rootAsset)
        {
            if (rootAsset == null)
            {
                throw new ArgumentNullException(nameof(rootAsset));
            }

            rootAsset.EnsureInitialized();
            HashSet<BTAsset> visitedAssets = new();
            List<object> trees = new();
            HashSet<string> treeIds = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> treeNames = new(StringComparer.OrdinalIgnoreCase);

            Collect(rootAsset, visitedAssets, trees, treeIds, treeNames);

            object package = BTEditorRuntimeBridge.CreateInstance("ET.BTPackage");
            BTEditorRuntimeBridge.SetValue(package, "PackageId", rootAsset.TreeId);
            BTEditorRuntimeBridge.SetValue(package, "PackageName", rootAsset.TreeName);
            BTEditorRuntimeBridge.SetValue(package, "EntryTreeId", rootAsset.TreeId);
            BTEditorRuntimeBridge.SetValue(package, "EntryTreeName", rootAsset.TreeName);

            IList packageTrees = BTEditorRuntimeBridge.GetList(package, "Trees");
            foreach (object tree in trees)
            {
                packageTrees.Add(tree);
            }

            return package;
        }

        public static byte[] BuildBytes(BTAsset rootAsset)
        {
            object package = BuildPackage(rootAsset);
            return BTEditorRuntimeBridge.SerializePackage(package);
        }

        public static string ExportToFile(BTAsset rootAsset)
        {
            return ExportToFiles(rootAsset).ClientFullPath;
        }

        public static BTExportResult ExportToFiles(BTAsset rootAsset)
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
            string serverFullPath = Path.GetFullPath(Path.Combine(projectRoot, "..", BTBytesLoader.ServerBehaviorTreeBytesDir, serverFileName));
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

        private static void Collect(BTAsset asset, HashSet<BTAsset> visitedAssets, List<object> trees, HashSet<string> treeIds, HashSet<string> treeNames)
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

        private static object BuildDefinition(BTAsset asset)
        {
            object definition = BTEditorRuntimeBridge.CreateInstance("ET.BTDefinition");
            BTEditorRuntimeBridge.SetValue(definition, "TreeId", asset.TreeId);
            BTEditorRuntimeBridge.SetValue(definition, "TreeName", asset.TreeName);
            BTEditorRuntimeBridge.SetValue(definition, "Description", asset.Description);
            BTEditorRuntimeBridge.SetValue(definition, "RootNodeId", asset.RootNodeId);

            IList blackboardEntries = BTEditorRuntimeBridge.GetList(definition, "BlackboardEntries");
            foreach (BTBlackboardEntryData entry in asset.BlackboardEntries)
            {
                blackboardEntries.Add(entry.Clone());
            }

            IList nodes = BTEditorRuntimeBridge.GetList(definition, "Nodes");
            foreach (BTEditorNodeData node in asset.Nodes)
            {
                nodes.Add(BuildNode(node));
            }

            return definition;
        }

        private static object BuildNode(BTEditorNodeData node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            BTEditorUtility.SyncNodeDescriptor(node);
            return BTEditorRuntimeNodeFactory.CreateFromEditorNode(node);
        }
    }
}
