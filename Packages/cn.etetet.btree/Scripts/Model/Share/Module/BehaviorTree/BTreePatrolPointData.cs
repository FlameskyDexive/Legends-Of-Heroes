using System;
using Nino.Core;
using Unity.Mathematics;

namespace ET
{
    public static class BTreePatrolNodeTypes
    {
        public const string Patrol = "demo.client.behavior.patrol";
        public const string HasPatrolPath = "demo.client.condition.has_patrol_path";
    }

    [EnableClass]
    [NinoType]
    [Serializable]
    public sealed partial class BTreePatrolPointData
    {
        public float X;

        public float Y;

        public float Z;

        public BTreePatrolPointData Clone()
        {
            return new BTreePatrolPointData
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
}
