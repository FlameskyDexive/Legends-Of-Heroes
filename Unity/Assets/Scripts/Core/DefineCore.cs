

namespace ET
{
    public static partial class DefineCore
    {
        [StaticField]
        public static float FixedDeltaTime = 1f / LogicFrame;
        [StaticField]
        public static bool DevelopMode = true;

#if UNITY_EDITOR
        // [StaticField]
        public const int LogicFrame = 30;
#else
        // [StaticField]
        public const int LogicFrame = 30;
#endif
    }
}
