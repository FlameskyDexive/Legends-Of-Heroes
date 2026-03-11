using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [Serializable]
    public sealed class BehaviorTreeEditorNodeData
    {
        public string NodeId = Guid.NewGuid().ToString("N");
        public string Title = string.Empty;
        public BehaviorTreeNodeKind NodeKind = BehaviorTreeNodeKind.Action;
        public Rect Position = new(200, 200, 240, 140);
        public List<string> ChildIds = new();
        public string HandlerName = string.Empty;
        public List<BehaviorTreeArgumentDefinition> Arguments = new();
        public string BlackboardKey = string.Empty;
        public BehaviorTreeCompareOperator CompareOperator = BehaviorTreeCompareOperator.IsSet;
        public BehaviorTreeSerializedValue CompareValue = new();
        public BehaviorTreeAbortMode AbortMode = BehaviorTreeAbortMode.Self;
        public int WaitMilliseconds = 1000;
        public int IntervalMilliseconds = 250;
        public int MaxLoopCount;
        public BehaviorTreeParallelPolicy SuccessPolicy = BehaviorTreeParallelPolicy.RequireAll;
        public BehaviorTreeParallelPolicy FailurePolicy = BehaviorTreeParallelPolicy.RequireOne;
        public string Comment = string.Empty;
        public string SubTreeId = string.Empty;
        public string SubTreeName = string.Empty;
        public BehaviorTreeAsset SubTreeAsset;

        public void SyncSubTreeInfo()
        {
            if (this.SubTreeAsset == null)
            {
                return;
            }

            this.SubTreeAsset.EnsureInitialized();
            this.SubTreeId = this.SubTreeAsset.TreeId;
            this.SubTreeName = this.SubTreeAsset.TreeName;
        }
    }

    public sealed class BehaviorTreeAsset : ScriptableObject
    {
        public string TreeId = Guid.NewGuid().ToString("N");
        public string TreeName = "NewBehaviorTree";
        public string Description = string.Empty;
        public string ExportRelativePath = "Config/BehaviorTree/NewBehaviorTree.bytes";
        public List<BehaviorTreeBlackboardEntryDefinition> BlackboardEntries = new();
        public List<BehaviorTreeEditorNodeData> Nodes = new();
        public string RootNodeId = string.Empty;
        public Vector3 ViewPosition;
        public Vector3 ViewScale = Vector3.one;

        [MenuItem("Assets/Create/ET/Behavior Tree Asset", false, 1200)]
        public static void CreateAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Behavior Tree Asset", "NewBehaviorTree", "asset", "Select a location for the behavior tree asset", "Assets/Editor/BehaviorTrees");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            BehaviorTreeAsset asset = CreateInstance<BehaviorTreeAsset>();
            asset.name = System.IO.Path.GetFileNameWithoutExtension(path);
            asset.TreeName = asset.name;
            asset.ExportRelativePath = $"Config/BehaviorTree/{asset.name}.bytes";
            asset.EnsureInitialized();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
            BehaviorTreeEditorWindow.Open(asset);
        }

        private void OnEnable()
        {
            this.EnsureInitialized();
        }

        public void EnsureInitialized()
        {
            if (string.IsNullOrWhiteSpace(this.TreeId))
            {
                this.TreeId = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrWhiteSpace(this.TreeName))
            {
                this.TreeName = string.IsNullOrWhiteSpace(this.name) ? "NewBehaviorTree" : this.name;
            }

            if (string.IsNullOrWhiteSpace(this.ExportRelativePath))
            {
                this.ExportRelativePath = $"Config/BehaviorTree/{this.TreeName}.bytes";
            }

            foreach (BehaviorTreeEditorNodeData node in this.Nodes)
            {
                if (string.IsNullOrWhiteSpace(node.NodeId))
                {
                    node.NodeId = Guid.NewGuid().ToString("N");
                }

                if (string.IsNullOrWhiteSpace(node.Title))
                {
                    node.Title = BehaviorTreeEditorUtility.GetDefaultTitle(node.NodeKind);
                }

                node.SyncSubTreeInfo();
            }

            if (string.IsNullOrWhiteSpace(this.RootNodeId) || this.GetNode(this.RootNodeId) == null)
            {
                this.CreateRootNode();
            }
        }

        public BehaviorTreeEditorNodeData GetNode(string nodeId)
        {
            foreach (BehaviorTreeEditorNodeData node in this.Nodes)
            {
                if (node.NodeId == nodeId)
                {
                    return node;
                }
            }

            return null;
        }

        public BehaviorTreeEditorNodeData GetRootNode()
        {
            return this.GetNode(this.RootNodeId);
        }

        public BehaviorTreeEditorNodeData AddNode(BehaviorTreeNodeKind nodeKind, Vector2 position)
        {
            BehaviorTreeEditorNodeData node = new()
            {
                NodeKind = nodeKind,
                Title = BehaviorTreeEditorUtility.GetDefaultTitle(nodeKind),
                Position = new Rect(position.x, position.y, 240, 140),
            };

            this.Nodes.Add(node);
            return node;
        }

        public void RemoveNode(string nodeId)
        {
            if (this.RootNodeId == nodeId)
            {
                return;
            }

            this.Nodes.RemoveAll(node => node.NodeId == nodeId);
            foreach (BehaviorTreeEditorNodeData node in this.Nodes)
            {
                node.ChildIds.RemoveAll(childId => childId == nodeId);
            }
        }

        public void ResetView(Vector3 position, Vector3 scale)
        {
            this.ViewPosition = position;
            this.ViewScale = scale;
        }

        private void CreateRootNode()
        {
            BehaviorTreeEditorNodeData rootNode = this.GetRootNode();
            if (rootNode == null)
            {
                rootNode = new BehaviorTreeEditorNodeData()
                {
                    NodeKind = BehaviorTreeNodeKind.Root,
                    Title = "Root",
                    Position = new Rect(80, 80, 220, 120),
                };
                this.Nodes.Insert(0, rootNode);
            }
            else
            {
                rootNode.NodeKind = BehaviorTreeNodeKind.Root;
                rootNode.Title = "Root";
            }

            this.RootNodeId = rootNode.NodeId;
        }
    }
}
