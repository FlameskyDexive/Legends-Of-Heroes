using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreePatrolNodeData : BTreeNodeData, IBTreeHandlerNodeData
    {
        public List<BTreePatrolPointData> PatrolPoints = new();

        public string NodeTypeId => BTreePatrolNodeTypes.Patrol;

        public string HandlerName => "BTreePatrol";

        public BTreePatrolNodeData()
        {
            this.NodeKind = BTreeNodeKind.Action;
        }

        public override BTreeNodeData Clone()
        {
            BTreePatrolNodeData definition = this.CloneBaseTo(new BTreePatrolNodeData());
            foreach (BTreePatrolPointData patrolPoint in this.PatrolPoints)
            {
                definition.PatrolPoints.Add(patrolPoint?.Clone() ?? new BTreePatrolPointData());
            }

            return definition;
        }
    }
}
