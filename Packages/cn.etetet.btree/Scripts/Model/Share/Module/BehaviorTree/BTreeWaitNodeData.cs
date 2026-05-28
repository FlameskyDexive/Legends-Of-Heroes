using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeWaitNodeData : BTreeNodeData
    {
        public int WaitMilliseconds = 1000;

        public BTreeWaitNodeData()
        {
            this.NodeKind = BTreeNodeKind.Wait;
        }

        public override BTreeNodeData Clone()
        {
            return this.CloneBaseTo(new BTreeWaitNodeData
            {
                WaitMilliseconds = this.WaitMilliseconds,
            });
        }
    }
}
