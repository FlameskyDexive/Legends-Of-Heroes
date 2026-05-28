using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeSubTreeNodeData : BTreeNodeData
    {
        public string SubTreeId = string.Empty;

        public string SubTreeName = string.Empty;

        public BTreeSubTreeNodeData()
        {
            this.NodeKind = BTreeNodeKind.SubTree;
        }

        public override BTreeNodeData Clone()
        {
            return this.CloneBaseTo(new BTreeSubTreeNodeData
            {
                SubTreeId = this.SubTreeId,
                SubTreeName = this.SubTreeName,
            });
        }
    }
}
