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
            this.NodeKind = BehaviorTreeNodeKind.Root;
        }

        public override BTNodeData Clone() => this.CloneBaseTo(new BTRootNodeData());
    }
}
