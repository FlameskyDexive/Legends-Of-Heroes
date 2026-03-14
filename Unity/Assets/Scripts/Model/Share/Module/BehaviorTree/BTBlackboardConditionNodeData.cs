using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTBlackboardConditionNodeData : BTNodeData
    {
        public string BlackboardKey = string.Empty;

        public BTCompareOperator CompareOperator = BTCompareOperator.IsSet;

        public BTSerializedValue CompareValue = new();

        public BTAbortMode AbortMode = BTAbortMode.Self;

        public BTBlackboardConditionNodeData()
        {
            this.NodeKind = BTNodeKind.BlackboardCondition;
        }

        public override BTNodeData Clone()
        {
            return this.CloneBaseTo(new BTBlackboardConditionNodeData
            {
                BlackboardKey = this.BlackboardKey,
                CompareOperator = this.CompareOperator,
                CompareValue = this.CompareValue?.Clone() ?? new BTSerializedValue(),
                AbortMode = this.AbortMode,
            });
        }
    }
}
