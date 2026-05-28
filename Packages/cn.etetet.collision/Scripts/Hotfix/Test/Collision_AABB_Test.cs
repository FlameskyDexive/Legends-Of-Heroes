using Unity.Mathematics;

namespace ET.Test
{
    /// <summary>
    /// AABB-AABB 相交与点包含测试。
    /// </summary>
    public class Collision_AABB_Test : ATestHandler
    {
        public override async ETTask<int> Handle(TestContext context)
        {
            await using TestFiberScope scope = await TestFiberScope.Create(context.Fiber, SceneType.TestEmpty, nameof(Collision_AABB_Test));

            AABB a = new AABB(new float3(0, 0, 0), new float3(1, 1, 1));

            // 重叠
            AABB overlap = new AABB(new float3(1, 0, 0), new float3(1, 1, 1));
            if (!Collision3DHelper.AABBIntersect(a, overlap))
            {
                Log.Console("aabb overlap should intersect");
                return 1;
            }

            // 完全分离
            AABB far = new AABB(new float3(3, 0, 0), new float3(1, 1, 1));
            if (Collision3DHelper.AABBIntersect(a, far))
            {
                Log.Console("aabb separated should not intersect");
                return 2;
            }

            // 边界接触（距离恰等于半长之和）算相交
            AABB touch = new AABB(new float3(2, 0, 0), new float3(1, 1, 1));
            if (!Collision3DHelper.AABBIntersect(a, touch))
            {
                Log.Console("aabb touching should intersect");
                return 3;
            }

            // 一个完全包含另一个
            AABB inner = new AABB(new float3(0, 0, 0), new float3(0.2f, 0.2f, 0.2f));
            if (!Collision3DHelper.AABBIntersect(a, inner))
            {
                Log.Console("aabb contained should intersect");
                return 4;
            }

            // 仅单轴分离即不相交
            AABB axisSep = new AABB(new float3(0, 5, 0), new float3(1, 1, 1));
            if (Collision3DHelper.AABBIntersect(a, axisSep))
            {
                Log.Console("aabb single-axis separated should not intersect");
                return 5;
            }

            // FromMinMax 构造一致性
            AABB byMinMax = AABB.FromMinMax(new float3(-1, -1, -1), new float3(1, 1, 1));
            if (!byMinMax.Center.Equals(a.Center) || !byMinMax.HalfExtents.Equals(a.HalfExtents))
            {
                Log.Console("aabb FromMinMax mismatch");
                return 6;
            }

            // 点包含
            if (!Collision3DHelper.ContainsPoint(a, new float3(0.5f, -0.5f, 0.9f)))
            {
                Log.Console("aabb should contain inner point");
                return 7;
            }
            if (Collision3DHelper.ContainsPoint(a, new float3(1.5f, 0, 0)))
            {
                Log.Console("aabb should not contain outer point");
                return 8;
            }

            Log.Debug("Collision_AABB_Test passed");
            return ErrorCode.ERR_Success;
        }
    }
}
