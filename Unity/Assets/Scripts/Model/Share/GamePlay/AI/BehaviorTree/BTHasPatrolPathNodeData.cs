using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTHasPatrolPathNodeData : BTNodeData, IBTHandlerNodeData
    {
        public string NodeTypeId => BTPatrolNodeTypes.HasPatrolPath;

        public string HandlerName => "BTHasPatrolPath";

        public BTHasPatrolPathNodeData()
        {
            this.NodeKind = BTNodeKind.Condition;
        }

        public override BTNodeData Clone()
        {
            return this.CloneBaseTo(new BTHasPatrolPathNodeData());
        }
    }
}
