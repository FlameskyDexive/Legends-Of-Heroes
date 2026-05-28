using System;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreeHasPatrolPathNodeData : BTreeNodeData, IBTreeHandlerNodeData
    {
        public string NodeTypeId => BTreePatrolNodeTypes.HasPatrolPath;

        public string HandlerName => "BTreeHasPatrolPath";

        public BTreeHasPatrolPathNodeData()
        {
            this.NodeKind = BTreeNodeKind.Condition;
        }

        public override BTreeNodeData Clone()
        {
            return this.CloneBaseTo(new BTreeHasPatrolPathNodeData());
        }
    }
}
