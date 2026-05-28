using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeInverterNodeData : BTreeNodeData
    {
        public BTreeInverterNodeData()
        {
            this.NodeKind = BTreeNodeKind.Inverter;
        }

        public override BTreeNodeData Clone() => this.CloneBaseTo(new BTreeInverterNodeData());
    }
}
