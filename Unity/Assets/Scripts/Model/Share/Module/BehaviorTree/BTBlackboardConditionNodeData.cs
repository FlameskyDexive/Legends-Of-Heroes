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

        public BehaviorTreeCompareOperator CompareOperator = BehaviorTreeCompareOperator.IsSet;

        public BehaviorTreeSerializedValue CompareValue = new();

        public BehaviorTreeAbortMode AbortMode = BehaviorTreeAbortMode.Self;

        public BTBlackboardConditionNodeData()
        {
            this.NodeKind = BehaviorTreeNodeKind.BlackboardCondition;
        }

        public override BTNodeData Clone()
        {
            return this.CloneBaseTo(new BTBlackboardConditionNodeData
            {
                BlackboardKey = this.BlackboardKey,
                CompareOperator = this.CompareOperator,
                CompareValue = this.CompareValue?.Clone() ?? new BehaviorTreeSerializedValue(),
                AbortMode = this.AbortMode,
            });
        }
    }
}
