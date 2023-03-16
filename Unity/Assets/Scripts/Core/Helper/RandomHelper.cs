using System;
using System.Collections;
using System.Collections.Generic;
using Random = System.Random;

namespace ET
{
    public static class RandomEx
    {
        public static ulong RandUInt64(this Random random)
        {
            byte[] byte8 = new byte[8];
            random.NextBytes(byte8);
            return BitConverter.ToUInt64(byte8, 0);
        }

        public static int RandInt32(this Random random)
        {
            return random.Next();
        }
        
        public static uint RandUInt32(this Random random)
        {
            return (uint)random.Next();
        }

        public static long RandInt64(this Random random)
        {
            byte[] byte8 = new byte[8];
            random.NextBytes(byte8);
            return BitConverter.ToInt64(byte8, 0);
        }
    }
    
}