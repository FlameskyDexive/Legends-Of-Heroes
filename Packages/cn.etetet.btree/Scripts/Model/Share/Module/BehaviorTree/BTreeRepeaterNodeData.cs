using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeRepeaterNodeData : BTreeNodeData
    {
        public int MaxLoopCount;

        public BTreeRepeaterNodeData()
        {
            this.NodeKind = BTreeNodeKind.Repeater;
        }

        public override BTreeNodeData Clone()
        {
            return this.CloneBaseTo(new BTreeRepeaterNodeData
            {
                MaxLoopCount = this.MaxLoopCount,
            });
        }
    }
}
