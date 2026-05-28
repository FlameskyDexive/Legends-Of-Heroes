using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeSelectorNodeData : BTreeNodeData
    {
        public BTreeSelectorNodeData()
        {
            this.NodeKind = BTreeNodeKind.Selector;
        }

        public override BTreeNodeData Clone() => this.CloneBaseTo(new BTreeSelectorNodeData());
    }
}
