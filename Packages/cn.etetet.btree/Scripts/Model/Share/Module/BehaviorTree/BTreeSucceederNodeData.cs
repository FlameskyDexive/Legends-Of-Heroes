using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeSucceederNodeData : BTreeNodeData
    {
        public BTreeSucceederNodeData()
        {
            this.NodeKind = BTreeNodeKind.Succeeder;
        }

        public override BTreeNodeData Clone() => this.CloneBaseTo(new BTreeSucceederNodeData());
    }
}
