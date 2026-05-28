using Unity.Mathematics;

namespace ET.Test
{
    /// <summary>
    /// Collider3D 统一多态派发测试：验证各形状组合派发到正确算法，且对称（a-b 与 b-a 结果一致）。
    /// </summary>
    public class Collision_Unified_Test : ATestHandler
    {
        public override async ETTask<int> Handle(TestContext context)
        {
            await using TestFiberScope scope = await TestFiberScope.Create(context.Fiber, SceneType.TestEmpty, nameof(Collision_Unified_Test));

            Collider3D aabb = Collider3D.Create(new AABB(new float3(0, 0, 0), new float3(1, 1, 1)));
            Collider3D obb = Collider3D.Create(new OBB(new float3(1.5f, 0, 0), new float3(1, 1, 1), quaternion.RotateZ(math.radians(45f))));
            Collider3D sphere = Collider3D.Create(new Sphere(new float3(0.5f, 0, 0), 0.6f));
            Collider3D farSphere = Collider3D.Create(new Sphere(new float3(10, 0, 0), 0.5f));

            // AABB vs OBB 相交
            if (!Collision3DHelper.Intersect(aabb, obb))
            {
                Log.Console("unified aabb-obb should intersect");
                return 1;
            }
            // AABB vs Sphere 相交
            if (!Collision3DHelper.Intersect(aabb, sphere))
            {
                Log.Console("unified aabb-sphere should intersect");
                return 2;
            }
            // OBB vs Sphere 相交
            if (!Collision3DHelper.Intersect(obb, sphere))
            {
                Log.Console("unified obb-sphere should intersect");
                return 3;
            }
            // 远处球与任何形状都不相交
            if (Collision3DHelper.Intersect(aabb, farSphere))
            {
                Log.Console("unified aabb-far sphere should not intersect");
                return 4;
            }

            // 对称性：交换参数顺序结果一致（覆盖派发表两侧分支）
            Collider3D[] all = { aabb, obb, sphere, farSphere };
            for (int i = 0; i < all.Length; i++)
            {
                for (int j = 0; j < all.Length; j++)
                {
                    if (Collision3DHelper.Intersect(all[i], all[j]) != Collision3DHelper.Intersect(all[j], all[i]))
                    {
                        Log.Console($"unified dispatch asymmetric at {i},{j}");
                        return 5;
                    }
                }
            }

            Log.Debug("Collision_Unified_Test passed");
            return ErrorCode.ERR_Success;
        }
    }
}
