using System.Numerics;
using System;
using Unity.Mathematics;

namespace ET
{
    public static class MathHelper
    {
        //
        // Summary:
        //     The well-known 3.14159265358979... value (Read Only).
        public const float PI = (float)Math.PI;

        //
        // Summary:
        //     A representation of positive infinity (Read Only).
        public const float Infinity = float.PositiveInfinity;

        //
        // Summary:
        //     A representation of negative infinity (Read Only).
        public const float NegativeInfinity = float.NegativeInfinity;

        //
        // Summary:
        //     Degrees-to-radians conversion constant (Read Only).
        public const float Deg2Rad = (float)Math.PI / 180f;

        //
        // Summary:
        //     Radians-to-degrees conversion constant (Read Only).
        public const float Rad2Deg = 57.29578f;
        
        public static float RadToDeg(float radians)
        {
            return (float)(radians * 180 / System.Math.PI);
        }

        public static float DegToRad(float degrees)
        {
            return (float)(degrees * System.Math.PI / 180);
        }

        public static float Angle(float3 a, float3 b)
        {
            float angle = math.acos(math.dot(a, b)) * Rad2Deg;
            return angle;
        }

        public static float Dot(float3 vector1, float3 vector2)
        {
            return (float)(vector1.x * (double)vector2.x + vector1.y * (double)vector2.y +
                vector1.z * (double)vector2.z);
        }
    }
}