using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTWaitNodeData : BTNodeData
    {
        public int WaitMilliseconds = 1000;

        public BTWaitNodeData()
        {
            this.NodeKind = BTNodeKind.Wait;
        }

        public override BTNodeData Clone()
        {
            return this.CloneBaseTo(new BTWaitNodeData
            {
                WaitMilliseconds = this.WaitMilliseconds,
            });
        }
    }
}
