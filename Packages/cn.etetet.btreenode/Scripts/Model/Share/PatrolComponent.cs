using Unity.Mathematics;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class PatrolComponent: Entity, IAwake
    {
        public float3[] path = new float3[] { new float3(0, 0, 0), new float3(20, 0, 0), new float3(20, 0, 20), new float3(0, 0, 20), };
        public int Index;
    }
}
