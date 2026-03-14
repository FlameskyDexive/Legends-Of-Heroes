using Unity.Mathematics;

namespace ET
{
    [FriendOf(typeof(PatrolComponent))]
    public static partial class PatrolComponentSystem
    {
        public static float3 GetCurrent(this PatrolComponent self)
        {
            return self.path[self.Index];
        }
        
        public static void MoveNext(this PatrolComponent self)
        {
            self.Index = ++self.Index % self.path.Length;
        }
    }
}
