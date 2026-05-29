using Unity.Mathematics;

namespace ET
{
    // 球的数值公式:以 HP 作为"质量",驱动半径与速度。双端共用(放 Share)。
    // 任何改变 HP 的地方(吞噬、扣血、分裂、吃食物)调用一次 RecalcFromHp,
    // 内部对 RadiusBase/SpeedBase 的 Set 会经数值系统重算 Radius/Speed 并自动广播(M2C_NumericChange)。
    public static class BallNumericHelper
    {
        public static void RecalcFromHp(this Unit unit)
        {
            NumericComponent nc = unit.GetComponent<NumericComponent>();
            if (nc == null)
            {
                return;
            }

            float hp = nc.GetAsInt(NumericType.HP);
            if (hp < BallDefine.MinHp)
            {
                hp = BallDefine.MinHp;
            }

            // 面积 ∝ HP → 半径 ∝ sqrt(HP)
            float radius = BallDefine.RadiusCoef * math.sqrt(hp);
            nc.Set(NumericType.RadiusBase, radius); // 重算并广播 Radius

            // 越大越慢(但有下限)
            float speed = math.clamp(BallDefine.BaseSpeed * BallDefine.SpeedCoef / math.sqrt(hp), BallDefine.MinSpeed, BallDefine.BaseSpeed);
            nc.Set(NumericType.SpeedBase, speed); // 重算并广播 Speed
        }

        // 读当前半径(米)
        public static float GetRadius(this Unit unit)
        {
            NumericComponent nc = unit.GetComponent<NumericComponent>();
            return nc == null ? 0f : nc.GetAsFloat(NumericType.Radius);
        }
    }
}
