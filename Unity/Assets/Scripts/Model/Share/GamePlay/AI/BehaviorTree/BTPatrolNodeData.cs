using System;
using System.Collections.Generic;
using Nino.Core;

namespace ET
{
    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTPatrolNodeData : BTNodeData, IBTHandlerNodeData
    {
        public List<BTPatrolPointData> PatrolPoints = new();

        public string NodeTypeId => BTPatrolNodeTypes.Patrol;

        public string HandlerName => "BTPatrol";

        public BTPatrolNodeData()
        {
            this.NodeKind = BTNodeKind.Action;
        }

        public override BTNodeData Clone()
        {
            BTPatrolNodeData definition = this.CloneBaseTo(new BTPatrolNodeData());
            foreach (BTPatrolPointData patrolPoint in this.PatrolPoints)
            {
                definition.PatrolPoints.Add(patrolPoint?.Clone() ?? new BTPatrolPointData());
            }

            return definition;
        }
    }
}
