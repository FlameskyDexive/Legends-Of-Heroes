using System.Collections.Generic;
using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;

namespace ET
{
    // Box2D 接触监听器(每地图一个),把 Box2D 的 BeginContact 收集起来。挂 Scene。
    // 🔑 BeginContact 在 World.Step **进行中**被回调,此时**绝不能**移除/销毁单位
    //   (Box2D 禁止 Step 中改世界;ET 在 AOI 迭代中销毁被观察单位会让 LeaveSight 对已销毁单位建 EntityRef 而崩)。
    //   故这里只**记录**接触的单位对(id),由 CollisionWorldComponentSystem.Update 在 Step **之后**再逐对发 OnCollisionContact 裁决。
    [EnableMethod]
    [ComponentOf(typeof(Scene))]
    public class CollisionListenerComponent : Entity, IAwake, IDestroy, IContactListener
    {
        // 本帧 World.Step 期间收集到的接触单位对(id);Update 在 Step 后排干并清空。
        public List<(long, long)> PendingContacts = new();

        public void BeginContact(Contact contact)
        {
            Unit a = contact.FixtureA.UserData as Unit;
            Unit b = contact.FixtureB.UserData as Unit;
            if (a == null || b == null || a.IsDisposed || b.IsDisposed)
            {
                return;
            }
            this.PendingContacts.Add((a.Id, b.Id));
        }

        public void EndContact(Contact contact)
        {
        }

        public void PreSolve(Contact contact, in Manifold oldManifold)
        {
        }

        public void PostSolve(Contact contact, in ContactImpulse impulse)
        {
        }
    }
}
