using Unity.Mathematics;

namespace ET.Test
{
    /// <summary>
    /// OBB-OBB（带方向立方体之间）相交测试，重点验证旋转被正确纳入分离轴判定。
    /// </summary>
    public class Collision_OBB_Test : ATestHandler
    {
        public override async ETTask<int> Handle(TestContext context)
        {
            await using TestFiberScope scope = await TestFiberScope.Create(context.Fiber, SceneType.TestEmpty, nameof(Collision_OBB_Test));

            float3 he = new float3(1, 1, 1);

            // 同一个盒子必相交
            OBB origin = new OBB(new float3(0, 0, 0), he, quaternion.identity);
            if (!Collision3DHelper.OBBIntersect(origin, origin))
            {
                Log.Console("obb identical should intersect");
                return 1;
            }

            // 轴对齐、距离 3 > 2，分离
            OBB axisFar = new OBB(new float3(3, 0, 0), he, quaternion.identity);
            if (Collision3DHelper.OBBIntersect(origin, axisFar))
            {
                Log.Console("obb axis-aligned far should not intersect");
                return 2;
            }

            // 关键判别：中心相距 2.5。
            // 不旋转时各自在 X 方向触及 1，合计 2 < 2.5 → 分离。
            OBB unrotatedB = new OBB(new float3(2.5f, 0, 0), he, quaternion.identity);
            if (Collision3DHelper.OBBIntersect(origin, unrotatedB))
            {
                Log.Console("obb unrotated at 2.5 should not intersect");
                return 3;
            }

            // 同样相距 2.5，但两者都绕 Z 轴转 45°，X 方向触及约 1.414，合计约 2.83 > 2.5 → 相交。
            quaternion rot45 = quaternion.RotateZ(math.radians(45f));
            OBB rotatedA = new OBB(new float3(0, 0, 0), he, rot45);
            OBB rotatedB = new OBB(new float3(2.5f, 0, 0), he, rot45);
            if (!Collision3DHelper.OBBIntersect(rotatedA, rotatedB))
            {
                Log.Console("obb rotated 45 at 2.5 should intersect");
                return 4;
            }

            // 绕任意轴旋转后重叠
            quaternion rotArb = quaternion.AxisAngle(math.normalize(new float3(1, 1, 0)), math.radians(30f));
            OBB arbA = new OBB(new float3(0, 0, 0), new float3(1, 2, 1), rotArb);
            OBB arbB = new OBB(new float3(1, 0.5f, 0), new float3(1, 1, 1), quaternion.identity);
            if (!Collision3DHelper.OBBIntersect(arbA, arbB))
            {
                Log.Console("obb arbitrary rotation overlap should intersect");
                return 5;
            }

            // AABB 视为单位旋转 OBB，与轴对齐 OBB 结果一致
            AABB aabb = new AABB(new float3(0, 0, 0), he);
            OBB aabbAsObb = new OBB(new float3(0, 0, 0), he, quaternion.identity);
            OBB probe = new OBB(new float3(1.5f, 0, 0), he, quaternion.identity);
            if (Collision3DHelper.AABBOBBIntersect(aabb, probe) != Collision3DHelper.OBBIntersect(aabbAsObb, probe))
            {
                Log.Console("aabb-obb should match obb-obb for identity rotation");
                return 6;
            }

            // 点包含：旋转后局部坐标判定
            if (!Collision3DHelper.ContainsPoint(rotatedA, new float3(0.5f, 0.5f, 0)))
            {
                Log.Console("rotated obb should contain inner point");
                return 7;
            }

            Log.Debug("Collision_OBB_Test passed");
            return ErrorCode.ERR_Success;
        }
    }
}
