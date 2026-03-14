using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    public enum BehaviorTreeNodeKind
    {
        Root = 0,
        Sequence = 1,
        Selector = 2,
        Parallel = 3,
        Inverter = 4,
        Succeeder = 5,
        Failer = 6,
        Repeater = 7,
        BlackboardCondition = 8,
        Service = 9,
        Action = 10,
        Condition = 11,
        Wait = 12,
        SubTree = 13,
    }

    public enum BehaviorTreeNodeState
    {
        Inactive = 0,
        Running = 1,
        Success = 2,
        Failure = 3,
        Aborted = 4,
    }

    public enum BehaviorTreeCompareOperator
    {
        IsSet = 0,
        IsNotSet = 1,
        IsTrue = 2,
        IsFalse = 3,
        Equal = 4,
        NotEqual = 5,
        Greater = 6,
        GreaterOrEqual = 7,
        Less = 8,
        LessOrEqual = 9,
    }

    [Flags]
    public enum BehaviorTreeAbortMode
    {
        None = 0,
        Self = 1,
        LowerPriority = 2,
        Both = Self | LowerPriority,
    }

    public enum BehaviorTreeParallelPolicy
    {
        RequireOne = 0,
        RequireAll = 1,
    }

    public enum BehaviorTreeValueType
    {
        None = 0,
        Integer = 1,
        Long = 2,
        Float = 3,
        Boolean = 4,
        String = 5,
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BehaviorTreeSerializedValue
    {
        public BehaviorTreeValueType ValueType = BehaviorTreeValueType.None;

        public int IntValue;

        public long LongValue;

        public float FloatValue;

        public bool BoolValue;

        public string StringValue = string.Empty;

        public BehaviorTreeSerializedValue Clone()
        {
            return new BehaviorTreeSerializedValue()
            {
                ValueType = this.ValueType,
                IntValue = this.IntValue,
                LongValue = this.LongValue,
                FloatValue = this.FloatValue,
                BoolValue = this.BoolValue,
                StringValue = this.StringValue,
            };
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BehaviorTreeArgumentDefinition
    {
        public string Name = string.Empty;

        public BehaviorTreeSerializedValue Value = new();

        public BehaviorTreeArgumentDefinition Clone()
        {
            return new BehaviorTreeArgumentDefinition()
            {
                Name = this.Name,
                Value = this.Value?.Clone() ?? new BehaviorTreeSerializedValue(),
            };
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BehaviorTreeBlackboardEntryDefinition
    {
        public string Key = string.Empty;

        public BehaviorTreeValueType ValueType = BehaviorTreeValueType.None;

        public BehaviorTreeSerializedValue DefaultValue = new();

        public string Description = string.Empty;

        public BehaviorTreeBlackboardEntryDefinition Clone()
        {
            return new BehaviorTreeBlackboardEntryDefinition()
            {
                Key = this.Key,
                ValueType = this.ValueType,
                DefaultValue = this.DefaultValue?.Clone() ?? new BehaviorTreeSerializedValue(),
                Description = this.Description,
            };
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public abstract partial class BTNodeData
    {
        public string NodeId = string.Empty;

        public string Title = string.Empty;

        public BehaviorTreeNodeKind NodeKind;

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
        List<BehaviorTreeArgumentDefinition> Arguments { get; }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BehaviorTreeDefinition
    {
        public string TreeId = string.Empty;

        public string TreeName = string.Empty;

        public string Description = string.Empty;

        public string RootNodeId = string.Empty;

        public List<BehaviorTreeBlackboardEntryDefinition> BlackboardEntries = new();

        public List<BTNodeData> Nodes = new();

        public BTNodeData GetNode(string nodeId)
        {
            foreach (BTNodeData node in this.Nodes)
            {
                if (node.NodeId == nodeId)
                {
                    return node;
                }
            }

            return null;
        }

        public BehaviorTreeDefinition Clone()
        {
            BehaviorTreeDefinition definition = new()
            {
                TreeId = this.TreeId,
                TreeName = this.TreeName,
                Description = this.Description,
                RootNodeId = this.RootNodeId,
            };

            foreach (BehaviorTreeBlackboardEntryDefinition blackboardEntry in this.BlackboardEntries)
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
    public partial class BehaviorTreePackage
    {
        public string PackageId = string.Empty;

        public string PackageName = string.Empty;

        public string EntryTreeId = string.Empty;

        public string EntryTreeName = string.Empty;

        public List<BehaviorTreeDefinition> Trees = new();

        public BehaviorTreeDefinition GetTree(string treeIdOrName)
        {
            if (string.IsNullOrWhiteSpace(treeIdOrName))
            {
                return null;
            }

            foreach (BehaviorTreeDefinition tree in this.Trees)
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

        public BehaviorTreeDefinition GetEntryTree()
        {
            BehaviorTreeDefinition definition = this.GetTree(this.EntryTreeId);
            if (definition != null)
            {
                return definition;
            }

            return this.GetTree(this.EntryTreeName);
        }

        public BehaviorTreePackage Clone()
        {
            BehaviorTreePackage package = new()
            {
                PackageId = this.PackageId,
                PackageName = this.PackageName,
                EntryTreeId = this.EntryTreeId,
                EntryTreeName = this.EntryTreeName,
            };

            foreach (BehaviorTreeDefinition tree in this.Trees)
            {
                package.Trees.Add(tree.Clone());
            }

            return package;
        }
    }
}
