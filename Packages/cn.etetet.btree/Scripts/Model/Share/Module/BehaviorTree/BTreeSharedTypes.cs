using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    public enum BTreeNodeKind
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

    public enum BTreeNodeState
    {
        Inactive = 0,
        Running = 1,
        Success = 2,
        Failure = 3,
        Aborted = 4,
    }

    public enum BTreeCompareOperator
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
    public enum BTreeAbortMode
    {
        None = 0,
        Self = 1,
        LowerPriority = 2,
        Both = Self | LowerPriority,
    }

    public enum BTreeParallelPolicy
    {
        RequireOne = 0,
        RequireAll = 1,
    }

    public enum BTreeValueType
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
    public partial class BTreeSerializedValue
    {
        public BTreeValueType ValueType = BTreeValueType.None;

        public int IntValue;

        public long LongValue;

        public float FloatValue;

        public bool BoolValue;

        public string StringValue = string.Empty;

        public BTreeSerializedValue Clone()
        {
            return new BTreeSerializedValue()
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
    public partial class BTreeArgumentData
    {
        public string Name = string.Empty;

        public BTreeSerializedValue Value = new();

        public BTreeArgumentData Clone()
        {
            return new BTreeArgumentData()
            {
                Name = this.Name,
                Value = this.Value?.Clone() ?? new BTreeSerializedValue(),
            };
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BTreeBlackboardEntryData
    {
        public string Key = string.Empty;

        public BTreeValueType ValueType = BTreeValueType.None;

        public BTreeSerializedValue DefaultValue = new();

        public string Description = string.Empty;

        public BTreeBlackboardEntryData Clone()
        {
            return new BTreeBlackboardEntryData()
            {
                Key = this.Key,
                ValueType = this.ValueType,
                DefaultValue = this.DefaultValue?.Clone() ?? new BTreeSerializedValue(),
                Description = this.Description,
            };
        }
    }
}
