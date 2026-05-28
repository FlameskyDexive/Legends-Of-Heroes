using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeFailerNodeData : BTreeNodeData
    {
        public BTreeFailerNodeData()
        {
            this.NodeKind = BTreeNodeKind.Failer;
        }

        public override BTreeNodeData Clone() => this.CloneBaseTo(new BTreeFailerNodeData());
    }
}
