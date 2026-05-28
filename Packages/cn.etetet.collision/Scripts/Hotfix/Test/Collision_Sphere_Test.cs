using Unity.Mathematics;

namespace ET.Test
{
    /// <summary>
    /// Sphere-Sphere、OBB-Sphere（带方向立方体与球体）、AABB-Sphere 相交测试。
    /// </summary>
    public class Collision_Sphere_Test : ATestHandler
    {
        public override async ETTask<int> Handle(TestContext context)
        {
            await using TestFiberScope scope = await TestFiberScope.Create(context.Fiber, SceneType.TestEmpty, nameof(Collision_Sphere_Test));

            // ---- Sphere-Sphere ----
            Sphere s = new Sphere(new float3(0, 0, 0), 1f);

            if (!Collision3DHelper.SphereIntersect(s, new Sphere(new float3(1.5f, 0, 0), 1f)))
            {
                Log.Console("sphere overlap should intersect");
                return 1;
            }
            if (Collision3DHelper.SphereIntersect(s, new Sphere(new float3(3f, 0, 0), 1f)))
            {
                Log.Console("sphere separated should not intersect");
                return 2;
            }
            // 边界接触：圆心距 2 = 半径和
            if (!Collision3DHelper.SphereIntersect(s, new Sphere(new float3(2f, 0, 0), 1f)))
            {
                Log.Console("sphere touching should intersect");
                return 3;
            }

            // ---- OBB-Sphere（关键：旋转判别）----
            // OBB 局部半长 (2,1,1)；绕 Z 转 90° 后世界 X 方向半宽变为 1。
            OBB rotated = new OBB(new float3(0, 0, 0), new float3(2, 1, 1), quaternion.RotateZ(math.radians(90f)));
            Sphere probe = new Sphere(new float3(1.5f, 0, 0), 0.4f);
            // 旋转后：最近点 x=1，距球心 0.5 > 0.4 → 不相交
            if (Collision3DHelper.OBBSphereIntersect(rotated, probe))
            {
                Log.Console("rotated obb-sphere should not intersect");
                return 4;
            }
            // 不旋转：世界 X 方向半宽 2，球心在盒内 → 相交
            OBB unrotated = new OBB(new float3(0, 0, 0), new float3(2, 1, 1), quaternion.identity);
            if (!Collision3DHelper.OBBSphereIntersect(unrotated, probe))
            {
                Log.Console("unrotated obb-sphere should intersect");
                return 5;
            }
            // 球心在 OBB 内部
            if (!Collision3DHelper.OBBSphereIntersect(rotated, new Sphere(new float3(0, 0, 0), 0.1f)))
            {
                Log.Console("obb-sphere center inside should intersect");
                return 6;
            }
            // 球远离 OBB
            if (Collision3DHelper.OBBSphereIntersect(rotated, new Sphere(new float3(10, 0, 0), 1f)))
            {
                Log.Console("obb-sphere far should not intersect");
                return 7;
            }

            // ---- AABB-Sphere ----
            AABB box = new AABB(new float3(0, 0, 0), new float3(1, 1, 1));
            // 最近点在角附近：球心 (1.5,1.5,0)，到角(1,1,0)距离 sqrt(0.5)~0.707 < 0.8 → 相交
            if (!Collision3DHelper.AABBSphereIntersect(box, new Sphere(new float3(1.5f, 1.5f, 0), 0.8f)))
            {
                Log.Console("aabb-sphere near corner should intersect");
                return 8;
            }
            // 球心 (2,2,0)，到角(1,1,0)距离 sqrt(2)~1.414 > 0.5 → 不相交
            if (Collision3DHelper.AABBSphereIntersect(box, new Sphere(new float3(2f, 2f, 0), 0.5f)))
            {
                Log.Console("aabb-sphere far corner should not intersect");
                return 9;
            }

            // ---- 点包含 ----
            if (!Collision3DHelper.ContainsPoint(s, new float3(0.5f, 0.5f, 0.5f)))
            {
                Log.Console("sphere should contain inner point");
                return 10;
            }
            if (Collision3DHelper.ContainsPoint(s, new float3(1.1f, 0, 0)))
            {
                Log.Console("sphere should not contain outer point");
                return 11;
            }

            Log.Debug("Collision_Sphere_Test passed");
            return ErrorCode.ERR_Success;
        }
    }
}
