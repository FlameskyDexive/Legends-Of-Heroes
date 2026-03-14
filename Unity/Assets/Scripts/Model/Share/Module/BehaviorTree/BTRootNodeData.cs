using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTRootNodeData : BTNodeData
    {
        public BTRootNodeData()
        {
            this.NodeKind = BTNodeKind.Root;
        }

        public override BTNodeData Clone() => this.CloneBaseTo(new BTRootNodeData());
    }
}
