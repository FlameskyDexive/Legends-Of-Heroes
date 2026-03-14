using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTInverterNodeData : BTNodeData
    {
        public BTInverterNodeData()
        {
            this.NodeKind = BehaviorTreeNodeKind.Inverter;
        }

        public override BTNodeData Clone() => this.CloneBaseTo(new BTInverterNodeData());
    }
}
