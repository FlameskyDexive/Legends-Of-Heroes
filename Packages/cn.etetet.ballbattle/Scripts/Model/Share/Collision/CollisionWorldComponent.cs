using System.Collections.Generic;
using Box2DSharp.Dynamics;

namespace ET
{
    // 2D 碰撞世界(每地图一个),持有 Box2D World。挂 Scene。
    // 原 Ori IFixedUpdate,本项目无 IFixedUpdate,改 IUpdate + 固定 dt 步进。
    [ComponentOf(typeof(Scene))]
    public class CollisionWorldComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public Box2DSharp.Dynamics.World World;

        public List<Body> BodyToDestroy = new List<Body>();

        public int VelocityIteration = BallDefine.VelocityIteration;
        public int PositionIteration = BallDefine.PositionIteration;
    }
}
