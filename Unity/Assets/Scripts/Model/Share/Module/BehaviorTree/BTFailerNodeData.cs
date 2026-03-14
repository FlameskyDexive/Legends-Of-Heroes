using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTFailerNodeData : BTNodeData
    {
        public BTFailerNodeData()
        {
            this.NodeKind = BehaviorTreeNodeKind.Failer;
        }

        public override BTNodeData Clone() => this.CloneBaseTo(new BTFailerNodeData());
    }
}
