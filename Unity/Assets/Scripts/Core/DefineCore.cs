

namespace ET
{
    public static partial class DefineCore
    {

#if UNITY_EDITOR
        [StaticField]
        public static int LogicFrame = 30;
#else
        [StaticField]
		public static int LogicFrame = 30;
#endif
    }
}
