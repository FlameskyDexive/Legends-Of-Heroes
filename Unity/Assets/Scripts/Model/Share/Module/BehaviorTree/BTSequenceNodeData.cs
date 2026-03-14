using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTSequenceNodeData : BTNodeData
    {
        public BTSequenceNodeData()
        {
            this.NodeKind = BTNodeKind.Sequence;
        }

        public override BTNodeData Clone() => this.CloneBaseTo(new BTSequenceNodeData());
    }
}
