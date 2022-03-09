using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;


public static class MathHelper
{
    /// <summary>
    ///   <para>The well-known 3.14159265358979... value (Read Only).</para>
    /// </summary>
    public const float PI = 3.141593f;
    /// <summary>
    ///   <para>A representation of positive infinity (Read Only).</para>
    /// </summary>
    public const float Infinity = float.PositiveInfinity;
    /// <summary>
    ///   <para>A representation of negative infinity (Read Only).</para>
    /// </summary>
    public const float NegativeInfinity = float.NegativeInfinity;
    /// <summary>
    ///   <para>Degrees-to-radians conversion constant (Read Only).</para>
    /// </summary>
    public const float Deg2Rad = 0.01745329f;
    /// <summary>
    ///   <para>Radians-to-degrees conversion constant (Read Only).</para>
    /// </summary>
    public const float Rad2Deg = 57.29578f;

    public static float AbsDot3(Vector3 v1, Vector3 v2)
    {
        return (Math.Abs((v1).X * (v2).X) + Math.Abs((v1).Y * (v2).Y) + Math.Abs((v1).Z * (v2).Z));
    }

    public static Vector3 Abs(Vector3 v3)
    {
        return new Vector3(Math.Abs(v3.X), Math.Abs(v3.Y), Math.Abs(v3.Z));
    }

    public static float DistanceSq(Vector3 a, Vector3 rhs)
    {
        float dx = a.X - rhs.X;
        float dy = a.Y - rhs.Y;
        float dz = a.Z - rhs.Z;
        return dx * dx + dy * dy + dz * dz;
    }


    public static Vector3 ConvertVector3(UnityEngine.Vector3 v3)
    {
        return new Vector3(v3.x, v3.y, v3.z);
    }
    public static Quaternion ConvertQuaternion(UnityEngine.Quaternion qua)
    {
        return new Quaternion(qua.x, qua.y, qua.z, qua.w);
    }
    public static Vector3 TransformWithoutOverlap(Quaternion rotation, Vector3 point)
    {
        float num1 = rotation.X * 2f;
        float num2 = rotation.Y * 2f;
        float num3 = rotation.Z * 2f;
        float num4 = rotation.X * num1;
        float num5 = rotation.Y * num2;
        float num6 = rotation.Z * num3;
        float num7 = rotation.X * num2;
        float num8 = rotation.X * num3;
        float num9 = rotation.Y * num3;
        float num10 = rotation.W * num1;
        float num11 = rotation.W * num2;
        float num12 = rotation.W * num3;
        Vector3 vector3;
        vector3.X = (float)((1.0 - ((double)num5 + (double)num6)) * (double)point.X + ((double)num7 - (double)num12) * (double)point.Y + ((double)num8 + (double)num11) * (double)point.Z);
        vector3.Y = (float)(((double)num7 + (double)num12) * (double)point.X + (1.0 - ((double)num4 + (double)num6)) * (double)point.Y + ((double)num9 - (double)num10) * (double)point.Z);
        vector3.Z = (float)(((double)num8 - (double)num11) * (double)point.X + ((double)num9 + (double)num10) * (double)point.Y + (1.0 - ((double)num4 + (double)num5)) * (double)point.Z);
        return vector3;
    }


    public static float Sqrt(float f) => (float)Math.Sqrt((double)f);

    public static float[] v3Array(Vector3 vec3)
    {
        float[] array = new float[3];
        array[0] = vec3.X;  
        array[1] = vec3.Y;
        array[2] = vec3.Z;
        return array;
    }


    /*public static Vector3 Normalize(Vector3 v3)
    {
        float num = Magnitude(v3);
        if ((double)num > 9.99999974737875E-06)
            v3 = v3 / num;
        else
            v3 = Vector3.Zero;
        return v3;
    }*/

    public static float Clamp(float value, float min, float max)
    {
        if ((double)value < (double)min)
            value = min;
        else if ((double)value > (double)max)
            value = max;
        return value;
    }


    public static Vector3 Normalize(this Vector3 v3)
    {
        float num = Magnitude(v3);
        if ((double)num > 9.99999974737875E-06)
            v3 = v3 / num;
        else
            v3 = Vector3.Zero;
        return v3;
    }

    public static string ToStringF1(this Quaternion q)
    {
        return $"({q.X.ToString("f1")}, {q.Y.ToString("f1")}, {q.Z.ToString("f1")}, {q.W.ToString("f1")})";
    }

    public static float Magnitude(Vector3 vector) => (float)Math.Sqrt((double)vector.X * (double)vector.X + (double)vector.Y * (double)vector.Y + (double)vector.Z * (double)vector.Z);

    /// <summary>
    /// Convert Quaternion To Euler
    /// </summary>
    /// <param name="q">https://www.codegrepper.com/code-examples/csharp/c%23+quaternion+eular+calculator</param>
    /// <returns></returns>
    public static Vector3 QuaternionToEuler(Quaternion q)
    {
        Vector3 euler;

        // if the input quaternion is normalized, this is exactly one. Otherwise, this acts as a correction factor for the quaternion's not-normalizedness
        float unit = (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z) + (q.W * q.W);

        // this will have a magnitude of 0.5 or greater if and only if this is a singularity case
        float test = q.X * q.W - q.Y * q.Z;

        if (test > 0.4995f * unit) // singularity at north pole
        {
            euler.X = PI / 2;
            euler.Y = 2f * Atan2(q.Y, q.X);
            euler.Z = 0;
        }
        else if (test < -0.4995f * unit) // singularity at south pole
        {
            euler.X = -PI / 2;
            euler.Y = -2f * Atan2(q.Y, q.X);
            euler.Z = 0;
        }
        else // no singularity - this is the majority of cases
        {
            euler.X = Asin(2f * (q.W * q.X - q.Y * q.Z));
            euler.Y = Atan2(2f * q.W * q.Y + 2f * q.Z * q.X, 1 - 2f * (q.X * q.X + q.Y * q.Y));
            euler.Z = Atan2(2f * q.W * q.Z + 2f * q.X * q.Y, 1 - 2f * (q.Z * q.Z + q.X * q.X));
        }

        // all the math so far has been done in radians. Before returning, we convert to degrees...
        euler *= Rad2Deg;

        //...and then ensure the degree values are between 0 and 360
        euler.X %= 360;
        euler.Y %= 360;
        euler.Z %= 360;

        return euler;
    }

    /// <summary>
    /// Convert Euler To Quaternion
    /// </summary>
    /// <param name="euler">https://www.codegrepper.com/code-examples/csharp/c%23+quaternion+eular+calculator</param>
    /// <returns></returns>
    public static Quaternion EulerToQuaternion(Vector3 euler)
    {
        float xOver2 = euler.X * Deg2Rad * 0.5f;
        float yOver2 = euler.Y * Deg2Rad * 0.5f;
        float zOver2 = euler.Z * Deg2Rad * 0.5f;

        float sinXOver2 = Mathf.Sin(xOver2);
        float cosXOver2 = Mathf.Cos(xOver2);
        float sinYOver2 = Mathf.Sin(yOver2);
        float cosYOver2 = Mathf.Cos(yOver2);
        float sinZOver2 = Mathf.Sin(zOver2);
        float cosZOver2 = Mathf.Cos(zOver2);

        Quaternion result;
        result.X = cosYOver2 * sinXOver2 * cosZOver2 + sinYOver2 * cosXOver2 * sinZOver2;
        result.Y = sinYOver2 * cosXOver2 * cosZOver2 - cosYOver2 * sinXOver2 * sinZOver2;
        result.Z = cosYOver2 * cosXOver2 * sinZOver2 - sinYOver2 * sinXOver2 * cosZOver2;
        result.W = cosYOver2 * cosXOver2 * cosZOver2 + sinYOver2 * sinXOver2 * sinZOver2;
        return result;
    }

    public static Quaternion EulerToQuaternion(UnityEngine.Vector3 euler)
    {
        return EulerToQuaternion(new Vector3(euler.x, euler.y, euler.z));
    }

    public static Vector3 QuaternionToEuler(UnityEngine.Quaternion q)
    {
        return QuaternionToEuler(new Quaternion(q.x, q.y, q.z, q.w));
    }

    public static float Cos(float val)
    {
        return (float) Math.Cos(val);
    }
    public static float Sin(float val)
    {
        return (float)Math.Sin(val);
    }
    public static float Asin(float val)
    {
        return (float) Math.Asin(val);
    }
    public static float Atan2(float val1, float val2)
    {
        return (float) Math.Atan2(val1, val2);
    }

}