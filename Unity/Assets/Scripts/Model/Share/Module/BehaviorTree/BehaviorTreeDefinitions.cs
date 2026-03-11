using System;
using System.Collections.Generic;
using MemoryPack;

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
    [MemoryPackable]
    [Serializable]
    public partial class BehaviorTreeSerializedValue
    {
        [MemoryPackOrder(0)]
        public BehaviorTreeValueType ValueType = BehaviorTreeValueType.None;

        [MemoryPackOrder(1)]
        public int IntValue;

        [MemoryPackOrder(2)]
        public long LongValue;

        [MemoryPackOrder(3)]
        public float FloatValue;

        [MemoryPackOrder(4)]
        public bool BoolValue;

        [MemoryPackOrder(5)]
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
    [MemoryPackable]
    [Serializable]
    public partial class BehaviorTreeArgumentDefinition
    {
        [MemoryPackOrder(0)]
        public string Name = string.Empty;

        [MemoryPackOrder(1)]
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
    [MemoryPackable]
    [Serializable]
    public partial class BehaviorTreeBlackboardEntryDefinition
    {
        [MemoryPackOrder(0)]
        public string Key = string.Empty;

        [MemoryPackOrder(1)]
        public BehaviorTreeValueType ValueType = BehaviorTreeValueType.None;

        [MemoryPackOrder(2)]
        public BehaviorTreeSerializedValue DefaultValue = new();

        [MemoryPackOrder(3)]
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
    [MemoryPackable]
    [Serializable]
    public partial class BehaviorTreeNodeDefinition
    {
        [MemoryPackOrder(0)]
        public string NodeId = string.Empty;

        [MemoryPackOrder(1)]
        public string Title = string.Empty;

        [MemoryPackOrder(2)]
        public BehaviorTreeNodeKind NodeKind;

        [MemoryPackOrder(3)]
        public List<string> ChildIds = new();

        [MemoryPackOrder(4)]
        public string HandlerName = string.Empty;

        [MemoryPackOrder(5)]
        public List<BehaviorTreeArgumentDefinition> Arguments = new();

        [MemoryPackOrder(6)]
        public string BlackboardKey = string.Empty;

        [MemoryPackOrder(7)]
        public BehaviorTreeCompareOperator CompareOperator = BehaviorTreeCompareOperator.IsSet;

        [MemoryPackOrder(8)]
        public BehaviorTreeSerializedValue CompareValue = new();

        [MemoryPackOrder(9)]
        public BehaviorTreeAbortMode AbortMode = BehaviorTreeAbortMode.Self;

        [MemoryPackOrder(10)]
        public int WaitMilliseconds = 1000;

        [MemoryPackOrder(11)]
        public int IntervalMilliseconds = 250;

        [MemoryPackOrder(12)]
        public int MaxLoopCount;

        [MemoryPackOrder(13)]
        public BehaviorTreeParallelPolicy SuccessPolicy = BehaviorTreeParallelPolicy.RequireAll;

        [MemoryPackOrder(14)]
        public BehaviorTreeParallelPolicy FailurePolicy = BehaviorTreeParallelPolicy.RequireOne;

        [MemoryPackOrder(15)]
        public string SubTreeId = string.Empty;

        [MemoryPackOrder(16)]
        public string SubTreeName = string.Empty;

        [MemoryPackOrder(17)]
        public string Comment = string.Empty;

        public BehaviorTreeNodeDefinition Clone()
        {
            BehaviorTreeNodeDefinition definition = new()
            {
                NodeId = this.NodeId,
                Title = this.Title,
                NodeKind = this.NodeKind,
                HandlerName = this.HandlerName,
                BlackboardKey = this.BlackboardKey,
                CompareOperator = this.CompareOperator,
                CompareValue = this.CompareValue?.Clone() ?? new BehaviorTreeSerializedValue(),
                AbortMode = this.AbortMode,
                WaitMilliseconds = this.WaitMilliseconds,
                IntervalMilliseconds = this.IntervalMilliseconds,
                MaxLoopCount = this.MaxLoopCount,
                SuccessPolicy = this.SuccessPolicy,
                FailurePolicy = this.FailurePolicy,
                SubTreeId = this.SubTreeId,
                SubTreeName = this.SubTreeName,
                Comment = this.Comment,
            };

            definition.ChildIds.AddRange(this.ChildIds);

            foreach (BehaviorTreeArgumentDefinition argument in this.Arguments)
            {
                definition.Arguments.Add(argument.Clone());
            }

            return definition;
        }
    }

    [EnableClass]
    [MemoryPackable]
    [Serializable]
    public partial class BehaviorTreeDefinition
    {
        [MemoryPackOrder(0)]
        public string TreeId = string.Empty;

        [MemoryPackOrder(1)]
        public string TreeName = string.Empty;

        [MemoryPackOrder(2)]
        public string Description = string.Empty;

        [MemoryPackOrder(3)]
        public string RootNodeId = string.Empty;

        [MemoryPackOrder(4)]
        public List<BehaviorTreeBlackboardEntryDefinition> BlackboardEntries = new();

        [MemoryPackOrder(5)]
        public List<BehaviorTreeNodeDefinition> Nodes = new();

        public BehaviorTreeNodeDefinition GetNode(string nodeId)
        {
            foreach (BehaviorTreeNodeDefinition node in this.Nodes)
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

            foreach (BehaviorTreeNodeDefinition node in this.Nodes)
            {
                definition.Nodes.Add(node.Clone());
            }

            return definition;
        }
    }

    [EnableClass]
    [MemoryPackable]
    [Serializable]
    public partial class BehaviorTreePackage
    {
        [MemoryPackOrder(0)]
        public string PackageId = string.Empty;

        [MemoryPackOrder(1)]
        public string PackageName = string.Empty;

        [MemoryPackOrder(2)]
        public string EntryTreeId = string.Empty;

        [MemoryPackOrder(3)]
        public string EntryTreeName = string.Empty;

        [MemoryPackOrder(4)]
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
