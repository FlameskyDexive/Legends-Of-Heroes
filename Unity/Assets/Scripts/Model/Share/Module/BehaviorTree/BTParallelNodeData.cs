using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTParallelNodeData : BTNodeData
    {
        public BehaviorTreeParallelPolicy SuccessPolicy = BehaviorTreeParallelPolicy.RequireAll;

        public BehaviorTreeParallelPolicy FailurePolicy = BehaviorTreeParallelPolicy.RequireOne;

        public BTParallelNodeData()
        {
            this.NodeKind = BehaviorTreeNodeKind.Parallel;
        }

        public override BTNodeData Clone()
        {
            return this.CloneBaseTo(new BTParallelNodeData
            {
                SuccessPolicy = this.SuccessPolicy,
                FailurePolicy = this.FailurePolicy,
            });
        }
    }
}
