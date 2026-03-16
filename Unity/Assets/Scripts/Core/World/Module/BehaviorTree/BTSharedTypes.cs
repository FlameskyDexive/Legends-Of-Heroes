using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    public enum BTNodeKind
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

    public enum BTNodeState
    {
        Inactive = 0,
        Running = 1,
        Success = 2,
        Failure = 3,
        Aborted = 4,
    }

    public enum BTCompareOperator
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
    public enum BTAbortMode
    {
        None = 0,
        Self = 1,
        LowerPriority = 2,
        Both = Self | LowerPriority,
    }

    public enum BTParallelPolicy
    {
        RequireOne = 0,
        RequireAll = 1,
    }

    public enum BTValueType
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
    public partial class BTSerializedValue
    {
        public BTValueType ValueType = BTValueType.None;

        public int IntValue;

        public long LongValue;

        public float FloatValue;

        public bool BoolValue;

        public string StringValue = string.Empty;

        public BTSerializedValue Clone()
        {
            return new BTSerializedValue()
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
    public partial class BTArgumentData
    {
        public string Name = string.Empty;

        public BTSerializedValue Value = new();

        public BTArgumentData Clone()
        {
            return new BTArgumentData()
            {
                Name = this.Name,
                Value = this.Value?.Clone() ?? new BTSerializedValue(),
            };
        }
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public partial class BTBlackboardEntryData
    {
        public string Key = string.Empty;

        public BTValueType ValueType = BTValueType.None;

        public BTSerializedValue DefaultValue = new();

        public string Description = string.Empty;

        public BTBlackboardEntryData Clone()
        {
            return new BTBlackboardEntryData()
            {
                Key = this.Key,
                ValueType = this.ValueType,
                DefaultValue = this.DefaultValue?.Clone() ?? new BTSerializedValue(),
                Description = this.Description,
            };
        }
    }
}
