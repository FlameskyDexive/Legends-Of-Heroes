using System;
using System.Collections.Generic;
using Nino.Core;
using Unity.Mathematics;

namespace ET
{
    public static class BTPatrolNodeTypes
    {
        public const string Patrol = "demo.client.behavior.patrol";
        public const string HasPatrolPath = "demo.client.condition.has_patrol_path";
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTPatrolPointData
    {
        public float X;

        public float Y;

        public float Z;

        public BTPatrolPointData Clone()
        {
            return new BTPatrolPointData
            {
                X = this.X,
                Y = this.Y,
                Z = this.Z,
            };
        }

        public float3 ToFloat3()
        {
            return new float3(this.X, this.Y, this.Z);
        }
    }

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
