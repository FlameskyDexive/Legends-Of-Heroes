using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;

namespace ET
{
    // Box2D 接触监听器(每地图一个),把 Box2D 的 BeginContact 转成 ET 事件。挂 Scene。
    // 当前只处理开始碰撞(吞噬/子弹命中是瞬时判定);持续/结束碰撞(如激光)按需扩展。
    [EnableMethod]
    [ComponentOf(typeof(Scene))]
    public class CollisionListenerComponent : Entity, IAwake, IDestroy, IContactListener
    {
        public void BeginContact(Contact contact)
        {
            EventSystem.Instance.Publish(this.Root(), new OnCollisionContact() { contact = contact });
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
