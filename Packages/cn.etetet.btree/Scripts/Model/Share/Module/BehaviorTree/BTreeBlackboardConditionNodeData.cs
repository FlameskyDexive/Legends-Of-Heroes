using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeBlackboardConditionNodeData : BTreeNodeData
    {
        public string BlackboardKey = string.Empty;

        public BTreeCompareOperator CompareOperator = BTreeCompareOperator.IsSet;

        public BTreeSerializedValue CompareValue = new();

        public BTreeAbortMode AbortMode = BTreeAbortMode.Self;

        public BTreeBlackboardConditionNodeData()
        {
            this.NodeKind = BTreeNodeKind.BlackboardCondition;
        }

        public override BTreeNodeData Clone()
        {
            return this.CloneBaseTo(new BTreeBlackboardConditionNodeData
            {
                BlackboardKey = this.BlackboardKey,
                CompareOperator = this.CompareOperator,
                CompareValue = this.CompareValue?.Clone() ?? new BTreeSerializedValue(),
                AbortMode = this.AbortMode,
            });
        }
    }
}
