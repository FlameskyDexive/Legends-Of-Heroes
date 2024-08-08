using System;

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
        public static int LogicFrame = 30;
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
                //如果想省一个除法计算，直接用预算值也可以
                /*if (LogicFrame == 20)
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
                }*/

                return (int)(TimeSpan.TicksPerSecond / LogicFrame);
                // return 500000;
            }
        }
        
    }
}
