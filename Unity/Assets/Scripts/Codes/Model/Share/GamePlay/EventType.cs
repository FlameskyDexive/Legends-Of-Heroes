using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;

namespace ET
{
    namespace EventType
    {
        public struct SceneChangeStart
        {
        }
        
        public struct SceneChangeFinish
        {
            public Unit myUnit;
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

        public struct UpdateRoomPlayers
        {
            public G2C_UpdateRoomPlayers roomPlayersProto;
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
}