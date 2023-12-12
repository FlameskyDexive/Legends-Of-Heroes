using UnityEngine;

namespace NaughtyAttributes.Test
{
    public enum TestEnum
    {
        [Label("全不选择")]
        None = 0,
        [Label("定义B")]
        B = 1 << 0,
        [Label("定义C")]
        C = 1 << 1,
        [Label("定义D")]
        D = 1 << 2,
        [Label("定义E")]
        E = 1 << 3,
        [Label("定义F")]
        F = 1 << 4,
        [Label("全部选择")]
        All = ~0
    }

    public enum TestEnumSingle
    {
        [Label("无定义")]
        None = 0,
        [Label("定义B")]
        B = 1,
        [Label("定义C")]
        C = 1,


    }

    public class EnumFlagsTest : MonoBehaviour
    {
        [EnumFlags]
        public TestEnum flags0;

        [Label("测试枚举")]
        public TestEnumSingle flags1;

        public EnumFlagsNest1 nest1;
    }

    [System.Serializable]
    public class EnumFlagsNest1
    {
        [EnumFlags]
        public TestEnum flags1;

        public EnumFlagsNest2 nest2;
    }

    [System.Serializable]
    public class EnumFlagsNest2
    {
        [EnumFlags]
        public TestEnum flags2;
    }
}
