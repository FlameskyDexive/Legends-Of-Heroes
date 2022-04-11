using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        return (Math.Abs((v1).x * (v2).x) + Math.Abs((v1).y * (v2).y) + Math.Abs((v1).z * (v2).z));
    }

    public static Vector3 Abs(Vector3 v3)
    {
        return new Vector3(Math.Abs(v3.x), Math.Abs(v3.y), Math.Abs(v3.z));
    }

    public static float DistanceSq(Vector3 a, Vector3 rhs)
    {
        float dx = a.x - rhs.x;
        float dy = a.y - rhs.y;
        float dz = a.z - rhs.z;
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
        float num1 = rotation.x * 2f;
        float num2 = rotation.y * 2f;
        float num3 = rotation.z * 2f;
        float num4 = rotation.x * num1;
        float num5 = rotation.y * num2;
        float num6 = rotation.z * num3;
        float num7 = rotation.x * num2;
        float num8 = rotation.x * num3;
        float num9 = rotation.y * num3;
        float num10 = rotation.w * num1;
        float num11 = rotation.w * num2;
        float num12 = rotation.w * num3;
        Vector3 vector3;
        vector3.x = (float)((1.0 - ((double)num5 + (double)num6)) * (double)point.x + ((double)num7 - (double)num12) * (double)point.y + ((double)num8 + (double)num11) * (double)point.z);
        vector3.y = (float)(((double)num7 + (double)num12) * (double)point.x + (1.0 - ((double)num4 + (double)num6)) * (double)point.y + ((double)num9 - (double)num10) * (double)point.z);
        vector3.z = (float)(((double)num8 - (double)num11) * (double)point.x + ((double)num9 + (double)num10) * (double)point.y + (1.0 - ((double)num4 + (double)num5)) * (double)point.z);
        return vector3;
    }


    public static float Sqrt(float f) => (float)Math.Sqrt((double)f);

    public static float[] v3Array(Vector3 vec3)
    {
        float[] array = new float[3];
        array[0] = vec3.x;  
        array[1] = vec3.y;
        array[2] = vec3.z;
        return array;
    }


    /*public static Vector3 Normalize(Vector3 v3)
    {
        float num = Magnitude(v3);
        if ((double)num > 9.99999974737875E-06)
            v3 = v3 / num;
        else
            v3 = Vector3.zero;
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
            v3 = Vector3.zero;
        return v3;
    }

    public static string ToStringF1(this Quaternion q)
    {
        return $"({q.x.ToString("f1")}, {q.y.ToString("f1")}, {q.z.ToString("f1")}, {q.w.ToString("f1")})";
    }

    public static float Magnitude(Vector3 vector) => (float)Math.Sqrt((double)vector.x * (double)vector.x + (double)vector.y * (double)vector.y + (double)vector.z * (double)vector.z);

    /// <summary>
    /// Convert Quaternion To Euler
    /// </summary>
    /// <param name="q">https://www.codegrepper.com/code-examples/csharp/c%23+quaternion+eular+calculator</param>
    /// <returns></returns>
    public static Vector3 QuaternionToEuler(Quaternion q)
    {
        Vector3 euler;

        // if the input quaternion is normalized, this is exactly one. Otherwise, this acts as a correction factor for the quaternion's not-normalizedness
        float unit = (q.x * q.x) + (q.y * q.y) + (q.z * q.z) + (q.w * q.w);

        // this will have a magnitude of 0.5 or greater if and only if this is a singularity case
        float test = q.x * q.w - q.y * q.z;

        if (test > 0.4995f * unit) // singularity at north pole
        {
            euler.x = PI / 2;
            euler.y = 2f * Atan2(q.y, q.x);
            euler.z = 0;
        }
        else if (test < -0.4995f * unit) // singularity at south pole
        {
            euler.x = -PI / 2;
            euler.y = -2f * Atan2(q.y, q.x);
            euler.z = 0;
        }
        else // no singularity - this is the majority of cases
        {
            euler.x = Asin(2f * (q.w * q.x - q.y * q.z));
            euler.y = Atan2(2f * q.w * q.y + 2f * q.z * q.x, 1 - 2f * (q.x * q.x + q.y * q.y));
            euler.z = Atan2(2f * q.w * q.z + 2f * q.x * q.y, 1 - 2f * (q.z * q.z + q.x * q.x));
        }

        // all the math so far has been done in radians. Before returning, we convert to degrees...
        euler *= Rad2Deg;

        //...and then ensure the degree values are between 0 and 360
        euler.x %= 360;
        euler.y %= 360;
        euler.z %= 360;

        return euler;
    }

    /// <summary>
    /// Convert Euler To Quaternion
    /// </summary>
    /// <param name="euler">https://www.codegrepper.com/code-examples/csharp/c%23+quaternion+eular+calculator</param>
    /// <returns></returns>
    public static Quaternion EulerToQuaternion(Vector3 euler)
    {
        float xOver2 = euler.x * Deg2Rad * 0.5f;
        float yOver2 = euler.y * Deg2Rad * 0.5f;
        float zOver2 = euler.z * Deg2Rad * 0.5f;

        float sinXOver2 = Mathf.Sin(xOver2);
        float cosXOver2 = Mathf.Cos(xOver2);
        float sinYOver2 = Mathf.Sin(yOver2);
        float cosYOver2 = Mathf.Cos(yOver2);
        float sinZOver2 = Mathf.Sin(zOver2);
        float cosZOver2 = Mathf.Cos(zOver2);

        Quaternion result;
        result.x = cosYOver2 * sinXOver2 * cosZOver2 + sinYOver2 * cosXOver2 * sinZOver2;
        result.y = sinYOver2 * cosXOver2 * cosZOver2 - cosYOver2 * sinXOver2 * sinZOver2;
        result.z = cosYOver2 * cosXOver2 * sinZOver2 - sinYOver2 * sinXOver2 * cosZOver2;
        result.w = cosYOver2 * cosXOver2 * cosZOver2 + sinYOver2 * sinXOver2 * sinZOver2;
        return result;
    }

    /*public static Quaternion EulerToQuaternion(UnityEngine.Vector3 euler)
    {
        return EulerToQuaternion(new Vector3(euler.x, euler.y, euler.z));
    }

    public static Vector3 QuaternionToEuler(UnityEngine.Quaternion q)
    {
        return QuaternionToEuler(new Quaternion(q.x, q.y, q.z, q.w));
    }*/

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