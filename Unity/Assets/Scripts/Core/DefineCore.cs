

namespace ET
{
    public static partial class DefineCore
    {
        [StaticField]
        public static float FixedDeltaTime = 1f / LogicFrame;

#if UNITY_EDITOR
        [StaticField]
        public static int LogicFrame = 50;
#else
        [StaticField]
        public static int LogicFrame = 20;
#endif
        /// <summary>
        /// you can alse use dynamic calculate below
        /// TimeSpan.TicksPerSecond / DefineCore.LogicFrame
        /// </summary>
        [StaticField]
        public static int FixedDeltaTicks
        {
            get
            {
                if (LogicFrame == 20)
                {
                    return 500000;
                }
                else if (LogicFrame == 25)
                {
                    return 400000;
                }
                else if (LogicFrame == 30)
                {
                    return 333333;
                }
                else if (LogicFrame == 50)
                {
                    return 200000;
                }
                else if (LogicFrame == 60)
                {
                    return 166666;
                }

                return 500000;
            }
        }
        
    }
}
