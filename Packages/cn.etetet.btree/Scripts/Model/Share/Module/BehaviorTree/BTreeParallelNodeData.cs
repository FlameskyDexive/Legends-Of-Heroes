using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeParallelNodeData : BTreeNodeData
    {
        public BTreeParallelPolicy SuccessPolicy = BTreeParallelPolicy.RequireAll;

        public BTreeParallelPolicy FailurePolicy = BTreeParallelPolicy.RequireOne;

        public BTreeParallelNodeData()
        {
            this.NodeKind = BTreeNodeKind.Parallel;
        }

        public override BTreeNodeData Clone()
        {
            return this.CloneBaseTo(new BTreeParallelNodeData
            {
                SuccessPolicy = this.SuccessPolicy,
                FailurePolicy = this.FailurePolicy,
            });
        }
    }
}
