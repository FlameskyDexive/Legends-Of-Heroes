using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTRepeaterNodeData : BTNodeData
    {
        public int MaxLoopCount;

        public BTRepeaterNodeData()
        {
            this.NodeKind = BTNodeKind.Repeater;
        }

        public override BTNodeData Clone()
        {
            return this.CloneBaseTo(new BTRepeaterNodeData
            {
                MaxLoopCount = this.MaxLoopCount,
            });
        }
    }
}
