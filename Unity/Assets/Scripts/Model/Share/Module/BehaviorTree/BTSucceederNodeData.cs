using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTSucceederNodeData : BTNodeData
    {
        public BTSucceederNodeData()
        {
            this.NodeKind = BTNodeKind.Succeeder;
        }

        public override BTNodeData Clone() => this.CloneBaseTo(new BTSucceederNodeData());
    }
}
