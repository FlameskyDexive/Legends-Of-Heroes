using Box2DSharp.Dynamics.Contacts;

namespace ET
{
    public struct SceneChangeStart
    {
    }
    
    public struct SceneChangeFinish
    {
    }
    
    public struct AfterCreateClientScene
    {
    }
    
    public struct AfterCreateCurrentScene
    {
    }

    public struct AppStartInitFinish
    {
    }

    public struct LoginFinish
    {
    }

    public struct EnterMapFinish
    {
    }

    public struct AfterUnitCreate
    {
        public Unit Unit;
    }
    public struct AfterMyUnitCreate
    {
        public Unit unit;
    }

    public struct OnCollisionContact
    {
        public Contact contact;
        public bool isEnd;
    }

    public struct HitResult
    {
        public EHitResultType hitResultType;
        public int value;
    }
}