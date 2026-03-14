using System;
using System.Collections.Generic;
using Nino.Core;
using Unity.Mathematics;

namespace ET.Client
{
    public static class BehaviorTreeDemoNodeTypes
    {
        public const string Patrol = "demo.client.behavior.patrol";
        public const string HasPatrolPath = "demo.client.condition.has_patrol_path";
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BehaviorTreePatrolPointDefinition
    {
        public float X;

        public float Y;

        public float Z;

        public BehaviorTreePatrolPointDefinition Clone()
        {
            return new BehaviorTreePatrolPointDefinition
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
    public sealed partial class BTDemoPatrolNodeData : BTNodeData, IBTHandlerNodeData
    {
        public List<BehaviorTreePatrolPointDefinition> PatrolPoints = new();

        public string NodeTypeId => BehaviorTreeDemoNodeTypes.Patrol;

        public string HandlerName => "DemoClientPatrol";

        public BTDemoPatrolNodeData()
        {
            this.NodeKind = BehaviorTreeNodeKind.Action;
        }

        public override BTNodeData Clone()
        {
            BTDemoPatrolNodeData definition = this.CloneBaseTo(new BTDemoPatrolNodeData());
            foreach (BehaviorTreePatrolPointDefinition patrolPoint in this.PatrolPoints)
            {
                definition.PatrolPoints.Add(patrolPoint?.Clone() ?? new BehaviorTreePatrolPointDefinition());
            }

            return definition;
        }
    }
}
