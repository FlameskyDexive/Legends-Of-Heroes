using System;
using System.Collections.Generic;
using Nino.Core;
namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public abstract partial class BTreeNodeData
    {
        public string NodeId = string.Empty;

        public string Title = string.Empty;

        public BTreeNodeKind NodeKind;

        public List<string> ChildIds = new();

        public string Comment = string.Empty;

        protected T CloneBaseTo<T>(T definition) where T : BTreeNodeData
        {
            definition.NodeId = this.NodeId;
            definition.Title = this.Title;
            definition.Comment = this.Comment;
            definition.ChildIds.AddRange(this.ChildIds);
            return definition;
        }

        public abstract BTreeNodeData Clone();
    }

    public interface IBTreeHandlerNodeData
    {
        string NodeTypeId { get; }

        string HandlerName { get; }
    }

    public interface IBTreeArgumentNodeData
    {
        List<BTreeArgumentData> Arguments { get; }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BTreeDefinition
    {
        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public string Description = string.Empty;

        public string RootNodeId = string.Empty;

        public List<BTreeBlackboardEntryData> BlackboardEntries = new();

        public List<BTreeNodeData> Nodes = new();

        private Dictionary<string, BTreeNodeData> nodeLookup;

        private int nodeLookupCount = -1;

        public BTreeNodeData GetNode(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                return null;
            }

            if (this.nodeLookup == null || this.nodeLookupCount != this.Nodes.Count)
            {
                RebuildNodeLookup();
            }

            if (this.nodeLookup != null && this.nodeLookup.TryGetValue(nodeId, out BTreeNodeData cachedNode) && cachedNode?.NodeId == nodeId)
            {
                return cachedNode;
            }

            RebuildNodeLookup();
            return this.nodeLookup != null && this.nodeLookup.TryGetValue(nodeId, out BTreeNodeData rebuiltNode) ? rebuiltNode : null;
        }

        private void RebuildNodeLookup()
        {
            this.nodeLookup ??= new Dictionary<string, BTreeNodeData>(StringComparer.OrdinalIgnoreCase);
            this.nodeLookup.Clear();
            this.nodeLookupCount = this.Nodes.Count;
            foreach (BTreeNodeData node in this.Nodes)
            {
                if (node == null || string.IsNullOrWhiteSpace(node.NodeId))
                {
                    continue;
                }

                this.nodeLookup[node.NodeId] = node;
            }
        }

        public BTreeDefinition Clone()
        {
            BTreeDefinition definition = new()
            {
                TreeId = this.TreeId,
                TreeName = this.TreeName,
                Description = this.Description,
                RootNodeId = this.RootNodeId,
            };

            foreach (BTreeBlackboardEntryData blackboardEntry in this.BlackboardEntries)
            {
                definition.BlackboardEntries.Add(blackboardEntry.Clone());
            }

            foreach (BTreeNodeData node in this.Nodes)
            {
                definition.Nodes.Add(node.Clone());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BTreePackage
    {
        public string PackageId = string.Empty;

        public string PackageName = string.Empty;

        public string EntryTreeId = string.Empty;

        public string EntryTreeName = string.Empty;

        public List<BTreeDefinition> Trees = new();

        public BTreeDefinition GetTree(string treeIdOrName)
        {
            if (string.IsNullOrWhiteSpace(treeIdOrName))
            {
                return null;
            }

            foreach (BTreeDefinition tree in this.Trees)
            {
                if (tree.TreeId == treeIdOrName)
                {
                    return tree;
                }

                if (string.Equals(tree.TreeName, treeIdOrName, StringComparison.OrdinalIgnoreCase))
                {
                    return tree;
                }
            }

            return null;
        }

        public BTreeDefinition GetEntryTree()
        {
            BTreeDefinition definition = this.GetTree(this.EntryTreeId);
            if (definition != null)
            {
                return definition;
            }

            return this.GetTree(this.EntryTreeName);
        }

        public BTreePackage Clone()
        {
            BTreePackage package = new()
            {
                PackageId = this.PackageId,
                PackageName = this.PackageName,
                EntryTreeId = this.EntryTreeId,
                EntryTreeName = this.EntryTreeName,
            };

            foreach (BTreeDefinition tree in this.Trees)
            {
                package.Trees.Add(tree.Clone());
            }

            return package;
        }
    }
}
