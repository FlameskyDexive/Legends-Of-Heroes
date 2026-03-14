using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTSubTreeNodeData : BTNodeData
    {
        public string SubTreeId = string.Empty;

        public string SubTreeName = string.Empty;

        public BTSubTreeNodeData()
        {
            this.NodeKind = BTNodeKind.SubTree;
        }

        public override BTNodeData Clone()
        {
            return this.CloneBaseTo(new BTSubTreeNodeData
            {
                SubTreeId = this.SubTreeId,
                SubTreeName = this.SubTreeName,
            });
        }
    }
}
