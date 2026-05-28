using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeRootNodeData : BTreeNodeData
    {
        public BTreeRootNodeData()
        {
            this.NodeKind = BTreeNodeKind.Root;
        }

        public override BTreeNodeData Clone() => this.CloneBaseTo(new BTreeRootNodeData());
    }
}
