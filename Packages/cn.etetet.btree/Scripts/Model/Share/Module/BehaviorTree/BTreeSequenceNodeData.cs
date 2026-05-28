using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeSequenceNodeData : BTreeNodeData
    {
        public BTreeSequenceNodeData()
        {
            this.NodeKind = BTreeNodeKind.Sequence;
        }

        public override BTreeNodeData Clone() => this.CloneBaseTo(new BTreeSequenceNodeData());
    }
}
