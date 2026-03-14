using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTParallelNodeData : BTNodeData
    {
        public BTParallelPolicy SuccessPolicy = BTParallelPolicy.RequireAll;

        public BTParallelPolicy FailurePolicy = BTParallelPolicy.RequireOne;

        public BTParallelNodeData()
        {
            this.NodeKind = BTNodeKind.Parallel;
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
