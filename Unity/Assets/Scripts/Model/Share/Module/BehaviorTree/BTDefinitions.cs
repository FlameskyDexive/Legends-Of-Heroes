using System;
using System.Collections.Generic;
using Nino.Core;
namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public abstract partial class BTNodeData
    {
        public string NodeId = string.Empty;

        public string Title = string.Empty;

        public BTNodeKind NodeKind;

        public List<string> ChildIds = new();

        public string Comment = string.Empty;

        protected T CloneBaseTo<T>(T definition) where T : BTNodeData
        {
            definition.NodeId = this.NodeId;
            definition.Title = this.Title;
            definition.Comment = this.Comment;
            definition.ChildIds.AddRange(this.ChildIds);
            return definition;
        }

        public abstract BTNodeData Clone();
    }

    public interface IBTHandlerNodeData
    {
        string NodeTypeId { get; }

        string HandlerName { get; }
    }

    public interface IBTArgumentNodeData
    {
        List<BTArgumentData> Arguments { get; }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BTDefinition
    {
        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public string Description = string.Empty;

        public string RootNodeId = string.Empty;

        public List<BTBlackboardEntryData> BlackboardEntries = new();

        public List<BTNodeData> Nodes = new();

        private Dictionary<string, BTNodeData> nodeLookup;

        private int nodeLookupCount = -1;

        public BTNodeData GetNode(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                return null;
            }

            if (this.nodeLookup == null || this.nodeLookupCount != this.Nodes.Count)
            {
                RebuildNodeLookup();
            }

            if (this.nodeLookup != null && this.nodeLookup.TryGetValue(nodeId, out BTNodeData cachedNode) && cachedNode?.NodeId == nodeId)
            {
                return cachedNode;
            }

            RebuildNodeLookup();
            return this.nodeLookup != null && this.nodeLookup.TryGetValue(nodeId, out BTNodeData rebuiltNode) ? rebuiltNode : null;
        }

        private void RebuildNodeLookup()
        {
            this.nodeLookup ??= new Dictionary<string, BTNodeData>(StringComparer.OrdinalIgnoreCase);
            this.nodeLookup.Clear();
            this.nodeLookupCount = this.Nodes.Count;
            foreach (BTNodeData node in this.Nodes)
            {
                if (node == null || string.IsNullOrWhiteSpace(node.NodeId))
                {
                    continue;
                }

                this.nodeLookup[node.NodeId] = node;
            }
        }

        public BTDefinition Clone()
        {
            BTDefinition definition = new()
            {
                TreeId = this.TreeId,
                TreeName = this.TreeName,
                Description = this.Description,
                RootNodeId = this.RootNodeId,
            };

            foreach (BTBlackboardEntryData blackboardEntry in this.BlackboardEntries)
            {
                definition.BlackboardEntries.Add(blackboardEntry.Clone());
            }

            foreach (BTNodeData node in this.Nodes)
            {
                definition.Nodes.Add(node.Clone());
            }

            return definition;
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BTPackage
    {
        public string PackageId = string.Empty;

        public string PackageName = string.Empty;

        public string EntryTreeId = string.Empty;

        public string EntryTreeName = string.Empty;

        public List<BTDefinition> Trees = new();

        public BTDefinition GetTree(string treeIdOrName)
        {
            if (string.IsNullOrWhiteSpace(treeIdOrName))
            {
                return null;
            }

            foreach (BTDefinition tree in this.Trees)
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

        public BTDefinition GetEntryTree()
        {
            BTDefinition definition = this.GetTree(this.EntryTreeId);
            if (definition != null)
            {
                return definition;
            }

            return this.GetTree(this.EntryTreeName);
        }

        public BTPackage Clone()
        {
            BTPackage package = new()
            {
                PackageId = this.PackageId,
                PackageName = this.PackageName,
                EntryTreeId = this.EntryTreeId,
                EntryTreeName = this.EntryTreeName,
            };

            foreach (BTDefinition tree in this.Trees)
            {
                package.Trees.Add(tree.Clone());
            }

            return package;
        }
    }
}
