#if UNITY_EDITOR
using Type = System.Type;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using UnityEngine.Experimental.Rendering;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;


namespace VHierarchy.Libs
{
    public static class VUtils
    {

        #region Reflection


        public static object GetFieldValue(this object o, string fieldName)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(fieldName) is FieldInfo fieldInfo)
                return fieldInfo.GetValue(target);


            throw new System.Exception($"Field '{fieldName}' not found in type '{type.Name}' and its parent types");

        }
        public static object GetPropertyValue(this object o, string propertyName)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetPropertyInfo(propertyName) is PropertyInfo propertyInfo)
                return propertyInfo.GetValue(target);


            throw new System.Exception($"Property '{propertyName}' not found in type '{type.Name}' and its parent types");

        }
        public static object GetMemberValue(this object o, string memberName)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(memberName) is FieldInfo fieldInfo)
                return fieldInfo.GetValue(target);

            if (type.GetPropertyInfo(memberName) is PropertyInfo propertyInfo)
                return propertyInfo.GetValue(target);


            throw new System.Exception($"Member '{memberName}' not found in type '{type.Name}' and its parent types");

        }

        public static void SetFieldValue(this object o, string fieldName, object value)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(fieldName) is FieldInfo fieldInfo)
                fieldInfo.SetValue(target, value);


            else throw new System.Exception($"Field '{fieldName}' not found in type '{type.Name}' and its parent types");

        }
        public static void SetPropertyValue(this object o, string propertyName, object value)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetPropertyInfo(propertyName) is PropertyInfo propertyInfo)
                propertyInfo.SetValue(target, value);


            else throw new System.Exception($"Property '{propertyName}' not found in type '{type.Name}' and its parent types");

        }
        public static void SetMemberValue(this object o, string memberName, object value)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetFieldInfo(memberName) is FieldInfo fieldInfo)
                fieldInfo.SetValue(target, value);

            else if (type.GetPropertyInfo(memberName) is PropertyInfo propertyInfo)
                propertyInfo.SetValue(target, value);


            else throw new System.Exception($"Member '{memberName}' not found in type '{type.Name}' and its parent types");

        }

        public static object InvokeMethod(this object o, string methodName, params object[] parameters) // todo handle null params (can't get their type)
        {
            var type = o as Type ?? o.GetType();
            var target = o is Type ? null : o;


            if (type.GetMethodInfo(methodName, parameters.Select(r => r.GetType()).ToArray()) is MethodInfo methodInfo)
                return methodInfo.Invoke(target, parameters);


            throw new System.Exception($"Method '{methodName}' not found in type '{type.Name}', its parent types and interfaces");

        }


        public static T GetFieldValue<T>(this object o, string fieldName) => (T)o.GetFieldValue(fieldName);
        public static T GetPropertyValue<T>(this object o, string propertyName) => (T)o.GetPropertyValue(propertyName);
        public static T GetMemberValue<T>(this object o, string memberName) => (T)o.GetMemberValue(memberName);
        public static T InvokeMethod<T>(this object o, string methodName, params object[] parameters) => (T)o.InvokeMethod(methodName, parameters);




        public static FieldInfo GetFieldInfo(this Type type, string fieldName)
        {
            if (fieldInfoCache.TryGetValue(type, out var fieldInfosByNames))
                if (fieldInfosByNames.TryGetValue(fieldName, out var fieldInfo))
                    return fieldInfo;


            if (!fieldInfoCache.ContainsKey(type))
                fieldInfoCache[type] = new Dictionary<string, FieldInfo>();

            for (var curType = type; curType != null; curType = curType.BaseType)
                if (curType.GetField(fieldName, maxBindingFlags) is FieldInfo fieldInfo)
                    return fieldInfoCache[type][fieldName] = fieldInfo;


            return fieldInfoCache[type][fieldName] = null;

        }
        public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
            if (propertyInfoCache.TryGetValue(type, out var propertyInfosByNames))
                if (propertyInfosByNames.TryGetValue(propertyName, out var propertyInfo))
                    return propertyInfo;


            if (!propertyInfoCache.ContainsKey(type))
                propertyInfoCache[type] = new Dictionary<string, PropertyInfo>();

            for (var curType = type; curType != null; curType = curType.BaseType)
                if (curType.GetProperty(propertyName, maxBindingFlags) is PropertyInfo propertyInfo)
                    return propertyInfoCache[type][propertyName] = propertyInfo;


            return propertyInfoCache[type][propertyName] = null;

        }
        public static MethodInfo GetMethodInfo(this Type type, string methodName, params Type[] argumentTypes)
        {
            var methodHash = methodName.GetHashCode() ^ argumentTypes.Aggregate(0, (hash, r) => hash ^= r.GetHashCode());


            if (methodInfoCache.TryGetValue(type, out var methodInfosByHashes))
                if (methodInfosByHashes.TryGetValue(methodHash, out var methodInfo))
                    return methodInfo;



            if (!methodInfoCache.ContainsKey(type))
                methodInfoCache[type] = new Dictionary<int, MethodInfo>();

            for (var curType = type; curType != null; curType = curType.BaseType)
                if (curType.GetMethod(methodName, maxBindingFlags, null, argumentTypes, null) is MethodInfo methodInfo)
                    return methodInfoCache[type][methodHash] = methodInfo;

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType.GetMethod(methodName, maxBindingFlags, null, argumentTypes, null) is MethodInfo methodInfo)
                    return methodInfoCache[type][methodHash] = methodInfo;



            return methodInfoCache[type][methodHash] = null;

        }

        static Dictionary<Type, Dictionary<string, FieldInfo>> fieldInfoCache = new();
        static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyInfoCache = new();
        static Dictionary<Type, Dictionary<int, MethodInfo>> methodInfoCache = new();






        public static T GetCustomAttributeCached<T>(this MemberInfo memberInfo) where T : System.Attribute
        {
            if (!attributesCache.TryGetValue(memberInfo, out var attributes_byType))
                attributes_byType = attributesCache[memberInfo] = new();

            if (!attributes_byType.TryGetValue(typeof(T), out var attribute))
                attribute = attributes_byType[typeof(T)] = memberInfo.GetCustomAttribute<T>();

            return attribute as T;

        }

        static Dictionary<MemberInfo, Dictionary<Type, System.Attribute>> attributesCache = new();






        public static List<Type> GetSubclasses(this Type t) => t.Assembly.GetTypes().Where(type => type.IsSubclassOf(t)).ToList();

        public static object GetDefaultValue(this FieldInfo f, params object[] constructorVars) => f.GetValue(System.Activator.CreateInstance(((MemberInfo)f).ReflectedType, constructorVars));
        public static object GetDefaultValue(this FieldInfo f) => f.GetValue(System.Activator.CreateInstance(((MemberInfo)f).ReflectedType));

        public static IEnumerable<FieldInfo> GetFieldsWithoutBase(this Type t) => t.GetFields().Where(r => !t.BaseType.GetFields().Any(rr => rr.Name == r.Name));
        public static IEnumerable<PropertyInfo> GetPropertiesWithoutBase(this Type t) => t.GetProperties().Where(r => !t.BaseType.GetProperties().Any(rr => rr.Name == r.Name));


        public const BindingFlags maxBindingFlags = (BindingFlags)62;








        #endregion

        #region Collections


        public static class CollectionUtils
        {
            public static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dicts)
            {
                if (dicts.Count() == 0) return null;
                if (dicts.Count() == 1) return dicts.First();

                var mergedDict = new Dictionary<TKey, TValue>(dicts.First());

                foreach (var dict in dicts.Skip(1))
                    foreach (var r in dict)
                        if (!mergedDict.ContainsKey(r.Key))
                            mergedDict.Add(r.Key, r.Value);

                return mergedDict;
            }

        }


        public static T NextTo<T>(this IEnumerable<T> e, T to) => e.SkipWhile(r => !r.Equals(to)).Skip(1).FirstOrDefault();
        public static T PreviousTo<T>(this IEnumerable<T> e, T to) => e.Reverse().SkipWhile(r => !r.Equals(to)).Skip(1).FirstOrDefault();
        public static T NextToOrFirst<T>(this IEnumerable<T> e, T to) => e.NextTo(to) ?? e.First();
        public static T PreviousToOrLast<T>(this IEnumerable<T> e, T to) => e.PreviousTo(to) ?? e.Last();

        public static IEnumerable<T> InsertFirst<T>(this IEnumerable<T> ie, T t) => new[] { t }.Concat(ie);

        public static int IndexOfFirst<T>(this List<T> list, System.Func<T, bool> f) => list.FirstOrDefault(f) is T t ? list.IndexOf(t) : -1;
        public static int IndexOfLast<T>(this List<T> list, System.Func<T, bool> f) => list.LastOrDefault(f) is T t ? list.IndexOf(t) : -1;

        public static void SortBy<T, T2>(this List<T> list, System.Func<T, T2> keySelector) where T2 : System.IComparable => list.Sort((q, w) => keySelector(q).CompareTo(keySelector(w)));

        public static void RemoveValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            if (dictionary.FirstOrDefault(r => r.Value.Equals(value)) is var kvp)
                dictionary.Remove(kvp);
        }

        public static void ForEach<T>(this IEnumerable<T> sequence, System.Action<T> action) { foreach (T item in sequence) action(item); }



        public static T AddAt<T>(this List<T> l, T r, int i)
        {
            if (i < 0) i = 0;
            if (i >= l.Count)
                l.Add(r);
            else
                l.Insert(i, r);
            return r;
        }
        public static T RemoveLast<T>(this List<T> l)
        {
            if (!l.Any()) return default;

            var r = l.Last();

            l.RemoveAt(l.Count - 1);

            return r;
        }

        public static void Add<T>(this List<T> list, params T[] items)
        {
            foreach (var r in items)
                list.Add(r);
        }






        #endregion

        #region Math


        public static class MathUtil // MathUtils name is taken by UnityEditor.MathUtils 
        {

            public static float TriangleArea(Vector2 A, Vector2 B, Vector2 C) => Vector3.Cross(A - B, A - C).z.Abs() / 2;

            public static Vector2 LineIntersection(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
            {
                var a1 = B.y - A.y;
                var b1 = A.x - B.x;
                var c1 = a1 * A.x + b1 * A.y;

                var a2 = D.y - C.y;
                var b2 = C.x - D.x;
                var c2 = a2 * C.x + b2 * C.y;

                var d = a1 * b2 - a2 * b1;

                var x = (b2 * c1 - b1 * c2) / d;
                var y = (a1 * c2 - a2 * c1) / d;

                return new Vector2(x, y);

            }




            public static float Lerp(float f1, float f2, float t) => Mathf.LerpUnclamped(f1, f2, t);
            public static float Lerp(ref float f1, float f2, float t)
            {
                return f1 = Lerp(f1, f2, t);
            }

            public static Vector2 Lerp(Vector2 f1, Vector2 f2, float t) => Vector2.LerpUnclamped(f1, f2, t);
            public static Vector2 Lerp(ref Vector2 f1, Vector2 f2, float t)
            {
                return f1 = Lerp(f1, f2, t);
            }

            public static Vector3 Lerp(Vector3 f1, Vector3 f2, float t) => Vector3.LerpUnclamped(f1, f2, t);
            public static Vector3 Lerp(ref Vector3 f1, Vector3 f2, float t)
            {
                return f1 = Lerp(f1, f2, t);
            }

            public static Color Lerp(Color f1, Color f2, float t) => Color.LerpUnclamped(f1, f2, t);
            public static Color Lerp(ref Color f1, Color f2, float t)
            {
                return f1 = Lerp(f1, f2, t);
            }


            public static float Lerp(float current, float target, float speed, float deltaTime) => Mathf.Lerp(current, target, GetLerpT(speed, deltaTime));
            public static float Lerp(ref float current, float target, float speed, float deltaTime)
            {
                return current = Lerp(current, target, speed, deltaTime);
            }

            public static Vector2 Lerp(Vector2 current, Vector2 target, float speed, float deltaTime) => Vector2.Lerp(current, target, GetLerpT(speed, deltaTime));
            public static Vector2 Lerp(ref Vector2 current, Vector2 target, float speed, float deltaTime)
            {
                return current = Lerp(current, target, speed, deltaTime);
            }

            public static Vector3 Lerp(Vector3 current, Vector3 target, float speed, float deltaTime) => Vector3.Lerp(current, target, GetLerpT(speed, deltaTime));
            public static Vector3 Lerp(ref Vector3 current, Vector3 target, float speed, float deltaTime)
            {
                return current = Lerp(current, target, speed, deltaTime);
            }

            public static float SmoothDamp(float current, float target, float speed, ref float derivative, float deltaTime, float maxSpeed) => Mathf.SmoothDamp(current, target, ref derivative, .5f / speed, maxSpeed, deltaTime);
            public static float SmoothDamp(float current, float target, float speed, ref float derivative, float deltaTime)
            {
                return Mathf.SmoothDamp(current, target, ref derivative, .5f / speed, Mathf.Infinity, deltaTime);
            }
            public static float SmoothDamp(float current, float target, float speed, ref float derivative)
            {
                return SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }
            public static float SmoothDamp(ref float current, float target, float speed, ref float derivative, float deltaTime, float maxSpeed)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, deltaTime, maxSpeed);
            }
            public static float SmoothDamp(ref float current, float target, float speed, ref float derivative, float deltaTime)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, deltaTime);
            }
            public static float SmoothDamp(ref float current, float target, float speed, ref float derivative)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }

            public static Vector2 SmoothDamp(Vector2 current, Vector2 target, float speed, ref Vector2 derivative, float deltaTime) => Vector2.SmoothDamp(current, target, ref derivative, .5f / speed, Mathf.Infinity, deltaTime);
            public static Vector2 SmoothDamp(Vector2 current, Vector2 target, float speed, ref Vector2 derivative)
            {
                return SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }
            public static Vector2 SmoothDamp(ref Vector2 current, Vector2 target, float speed, ref Vector2 derivative, float deltaTime)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, deltaTime);
            }
            public static Vector2 SmoothDamp(ref Vector2 current, Vector2 target, float speed, ref Vector2 derivative)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }

            public static Vector3 SmoothDamp(Vector3 current, Vector3 target, float speed, ref Vector3 derivative, float deltaTime) => Vector3.SmoothDamp(current, target, ref derivative, .5f / speed, Mathf.Infinity, deltaTime);
            public static Vector3 SmoothDamp(Vector3 current, Vector3 target, float speed, ref Vector3 derivative)
            {
                return SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }
            public static Vector3 SmoothDamp(ref Vector3 current, Vector3 target, float speed, ref Vector3 derivative, float deltaTime)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, deltaTime);
            }
            public static Vector3 SmoothDamp(ref Vector3 current, Vector3 target, float speed, ref Vector3 derivative)
            {
                return current = SmoothDamp(current, target, speed, ref derivative, Time.deltaTime);
            }


            public static float GetLerpT(float lerpSpeed, float deltaTime) => 1 - Mathf.Exp(-lerpSpeed * 2f * deltaTime);
            public static float GetLerpT(float lerpSpeed)
            {
                return GetLerpT(lerpSpeed, Time.deltaTime);
            }



        }


        public static float DistanceTo(this float f1, float f2) => Mathf.Abs(f1 - f2);
        public static float DistanceTo(this Vector2 f1, Vector2 f2) => (f1 - f2).magnitude;
        public static float DistanceTo(this Vector3 f1, Vector3 f2) => (f1 - f2).magnitude;

        public static float Sign(this float f) => f == 0 ? 0 : Mathf.Sign(f);

        public static int Abs(this int f) => Mathf.Abs(f);
        public static float Abs(this float f) => Mathf.Abs(f);

        public static int Clamp(this int f, int f0, int f1) => Mathf.Clamp(f, f0, f1);
        public static float Clamp(this float f, float f0, float f1) => Mathf.Clamp(f, f0, f1);


        public static float Clamp01(this float f) => Mathf.Clamp(f, 0, 1);
        public static Vector2 Clamp01(this Vector2 f) => new(f.x.Clamp01(), f.y.Clamp01());
        public static Vector3 Clamp01(this Vector3 f) => new(f.x.Clamp01(), f.y.Clamp01(), f.z.Clamp01());


        public static int Pow(this int f, int pow) => (int)Mathf.Pow(f, pow);
        public static float Pow(this float f, float pow) => Mathf.Pow(f, pow);

        public static float Round(this float f) => Mathf.Round(f);
        public static float Ceil(this float f) => Mathf.Ceil(f);
        public static float Floor(this float f) => Mathf.Floor(f);

        public static int RoundToInt(this float f) => Mathf.RoundToInt(f);
        public static int CeilToInt(this float f) => Mathf.CeilToInt(f);
        public static int FloorToInt(this float f) => Mathf.FloorToInt(f);

        public static int ToInt(this float f) => (int)f;
        public static float ToFloat(this int f) => (float)f;
        public static float ToFloat(this double f) => (float)f;



        public static float Sqrt(this float f) => Mathf.Sqrt(f);

        public static int Max(this int f, int ff) => Mathf.Max(f, ff);
        public static int Min(this int f, int ff) => Mathf.Min(f, ff);
        public static float Max(this float f, float ff) => Mathf.Max(f, ff);
        public static float Min(this float f, float ff) => Mathf.Min(f, ff);

        public static float ClampMin(this float f, float limitMin) => Mathf.Max(f, limitMin);
        public static float ClampMax(this float f, float limitMax) => Mathf.Min(f, limitMax);

        public static Vector3 ClampMaxMagnitude(this Vector3 v, float maxMagnitude)
        {
            if (v.sqrMagnitude <= maxMagnitude * maxMagnitude)
                return v;
            else
                return v.normalized * maxMagnitude;
        }


        public static float Loop(this float f, float boundMin, float boundMax)
        {
            while (f < boundMin) f += boundMax - boundMin;
            while (f > boundMax) f -= boundMax - boundMin;
            return f;
        }
        public static float Loop(this float f, float boundMax) => f.Loop(0, boundMax);

        public static float PingPong(this float f, float boundMin, float boundMax) => boundMin + Mathf.PingPong(f - boundMin, boundMax - boundMin);
        public static float PingPong(this float f, float boundMax) => f.PingPong(0, boundMax);


        public static float Dot(this Vector3 v1, Vector3 v2) => Vector3.Dot(v1, v2);

        public static Vector3 Cross(this Vector3 v1, Vector3 v2) => Vector3.Cross(v1, v2);


        public static Vector2 ProjectOn(this Vector2 v, Vector2 on) => Vector3.Project(v, on);
        public static Vector3 ProjectOn(this Vector3 v, Vector3 on) => Vector3.Project(v, on);

        public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 normal) => Vector3.ProjectOnPlane(v, normal);


        public static float AngleTo(this Vector2 v, Vector2 to) => Vector2.Angle(v, to);

        public static Vector2 Rotate(this Vector2 v, float deg) => Quaternion.AngleAxis(deg, Vector3.forward) * v;

        public static float Smoothstep(this float f) { f = f.Clamp01(); return f * f * (3 - 2 * f); }

        public static float InverseLerp(this Vector2 v, Vector2 a, Vector2 b)
        {
            var ab = b - a;
            var av = v - a;
            return Vector2.Dot(av, ab) / Vector2.Dot(ab, ab);
        }


        public static bool IsOdd(this int i) => i % 2 == 1;
        public static bool IsEven(this int i) => i % 2 == 0;

        public static bool IsInRange(this int i, int a, int b) => i >= a && i <= b;
        public static bool IsInRange(this float i, float a, float b) => i >= a && i <= b;

        public static bool IsInRangeOf(this int i, IList list) => i.IsInRange(0, list.Count - 1);
        public static bool IsInRangeOf<T>(this int i, T[] array) => i.IsInRange(0, array.Length - 1);

        public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);



        [System.Serializable]
        public class GaussianKernel
        {
            public static float[,] GenerateArray(int size, float sharpness = .5f)
            {
                float[,] kr = new float[size, size];

                if (size == 1) { kr[0, 0] = 1; return kr; }


                var sigma = 1f - Mathf.Pow(sharpness, .1f) * .99999f;
                var radius = (size / 2f).FloorToInt();


                var a = -2f * radius * radius / Mathf.Log(sigma);
                var sum = 0f;

                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        var rX = size % 2 == 1 ? (x - radius) : (x - radius) + .5f;
                        var rY = size % 2 == 1 ? (y - radius) : (y - radius) + .5f;
                        var dist = Mathf.Sqrt(rX * rX + rY * rY);
                        kr[x, y] = Mathf.Exp(-dist * dist / a);
                        sum += kr[x, y];
                    }

                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                        kr[x, y] /= sum;

                return kr;
            }



            public GaussianKernel(bool isEvenSize = false, int radius = 7, float sharpness = .5f)
            {
                this.isEvenSize = isEvenSize;
                this.radius = radius;
                this.sharpness = sharpness;
            }

            public bool isEvenSize = false;
            public int radius = 7;
            public float sharpness = .5f;

            public int size => radius * 2 + (isEvenSize ? 0 : 1);
            public float sigma => 1 - Mathf.Pow(sharpness, .1f) * .99999f;

            public float[,] Array2d() // todo test and use GenerateArray
            {
                float[,] kr = new float[size, size];

                if (size == 1) { kr[0, 0] = 1; return kr; }

                var a = -2f * radius * radius / Mathf.Log(sigma);
                var sum = 0f;

                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                    {
                        var rX = size % 2 == 1 ? (x - radius) : (x - radius) + .5f;
                        var rY = size % 2 == 1 ? (y - radius) : (y - radius) + .5f;
                        var dist = Mathf.Sqrt(rX * rX + rY * rY);
                        kr[x, y] = Mathf.Exp(-dist * dist / a);
                        sum += kr[x, y];
                    }

                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                        kr[x, y] /= sum;

                return kr;
            }
            public float[] ArrayFlat()
            {
                var gk = Array2d();
                float[] flat = new float[size * size];

                for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                        flat[(i * size + j)] = gk[i, j];

                return flat;
            }

        }







        #endregion

        #region Rects


        public static Rect Resize(this Rect rect, float px) { rect.x += px; rect.y += px; rect.width -= px * 2; rect.height -= px * 2; return rect; }

        public static Rect SetPos(this Rect rect, Vector2 v) => rect.SetPos(v.x, v.y);
        public static Rect SetPos(this Rect rect, float x, float y) { rect.x = x; rect.y = y; return rect; }

        public static Rect SetX(this Rect rect, float x) => rect.SetPos(x, rect.y);
        public static Rect SetY(this Rect rect, float y) => rect.SetPos(rect.x, y);
        public static Rect SetXMax(this Rect rect, float xMax) { rect.xMax = xMax; return rect; }
        public static Rect SetYMax(this Rect rect, float yMax) { rect.yMax = yMax; return rect; }

        public static Rect SetMidPos(this Rect r, Vector2 v) => r.SetPos(v).MoveX(-r.width / 2).MoveY(-r.height / 2);
        public static Rect SetMidPos(this Rect r, float x, float y) => r.SetMidPos(new Vector2(x, y));

        public static Rect Move(this Rect rect, Vector2 v) { rect.position += v; return rect; }
        public static Rect Move(this Rect rect, float x, float y) { rect.x += x; rect.y += y; return rect; }
        public static Rect MoveX(this Rect rect, float px) { rect.x += px; return rect; }
        public static Rect MoveY(this Rect rect, float px) { rect.y += px; return rect; }

        public static Rect SetWidth(this Rect rect, float f) { rect.width = f; return rect; }
        public static Rect SetWidthFromMid(this Rect rect, float px) { rect.x += rect.width / 2; rect.width = px; rect.x -= rect.width / 2; return rect; }
        public static Rect SetWidthFromRight(this Rect rect, float px) { rect.x += rect.width; rect.width = px; rect.x -= rect.width; return rect; }

        public static Rect SetHeight(this Rect rect, float f) { rect.height = f; return rect; }
        public static Rect SetHeightFromMid(this Rect rect, float px) { rect.y += rect.height / 2; rect.height = px; rect.y -= rect.height / 2; return rect; }
        public static Rect SetHeightFromBottom(this Rect rect, float px) { rect.y += rect.height; rect.height = px; rect.y -= rect.height; return rect; }

        public static Rect AddWidth(this Rect rect, float f) => rect.SetWidth(rect.width + f);
        public static Rect AddWidthFromMid(this Rect rect, float f) => rect.SetWidthFromMid(rect.width + f);
        public static Rect AddWidthFromRight(this Rect rect, float f) => rect.SetWidthFromRight(rect.width + f);

        public static Rect AddHeight(this Rect rect, float f) => rect.SetHeight(rect.height + f);
        public static Rect AddHeightFromMid(this Rect rect, float f) => rect.SetHeightFromMid(rect.height + f);
        public static Rect AddHeightFromBottom(this Rect rect, float f) => rect.SetHeightFromBottom(rect.height + f);

        public static Rect SetSize(this Rect rect, Vector2 v) => rect.SetWidth(v.x).SetHeight(v.y);
        public static Rect SetSize(this Rect rect, float w, float h) => rect.SetWidth(w).SetHeight(h);
        public static Rect SetSize(this Rect rect, float f) { rect.height = rect.width = f; return rect; }

        public static Rect SetSizeFromMid(this Rect r, Vector2 v) => r.Move(r.size / 2).SetSize(v).Move(-v / 2);
        public static Rect SetSizeFromMid(this Rect r, float x, float y) => r.SetSizeFromMid(new Vector2(x, y));
        public static Rect SetSizeFromMid(this Rect r, float f) => r.SetSizeFromMid(new Vector2(f, f));

        public static Rect AlignToPixelGrid(this Rect r) => GUIUtility.AlignRectToDevice(r);





        #endregion

        #region Vectors


        public static Vector2 AddX(this Vector2 v, float f) => new(v.x + f, v.y + 0);
        public static Vector2 AddY(this Vector2 v, float f) => new(v.x + 0, v.y + f);

        public static Vector3 AddX(this Vector3 v, float f) => new(v.x + f, v.y + 0, v.z + 0);
        public static Vector3 AddY(this Vector3 v, float f) => new(v.x + 0, v.y + f, v.z + 0);
        public static Vector3 AddZ(this Vector3 v, float f) => new(v.x + 0, v.y + 0, v.z + f);

        public static Vector3 SetX(this Vector3 v, float f) => new(f, v.y, v.z);
        public static Vector3 SetY(this Vector3 v, float f) => new(v.x, f, v.z);
        public static Vector3 SetZ(this Vector3 v, float f) => new(v.x, v.y, f);

        public static Vector2 xx(this Vector3 v) { return new Vector2(v.x, v.x); }
        public static Vector2 xy(this Vector3 v) { return new Vector2(v.x, v.y); }
        public static Vector2 xz(this Vector3 v) { return new Vector2(v.x, v.z); }
        public static Vector2 yx(this Vector3 v) { return new Vector2(v.y, v.x); }
        public static Vector2 yy(this Vector3 v) { return new Vector2(v.y, v.y); }
        public static Vector2 yz(this Vector3 v) { return new Vector2(v.y, v.z); }
        public static Vector2 zx(this Vector3 v) { return new Vector2(v.z, v.x); }
        public static Vector2 zy(this Vector3 v) { return new Vector2(v.z, v.y); }
        public static Vector2 zz(this Vector3 v) { return new Vector2(v.z, v.z); }





        #endregion

        #region Colors


        public class ColorUtils
        {
            public static Color HSLToRGB(float h, float s, float l)
            {
                float hue2Rgb(float v1, float v2, float vH)
                {
                    if (vH < 0f)
                        vH += 1f;

                    if (vH > 1f)
                        vH -= 1f;

                    if (6f * vH < 1f)
                        return v1 + (v2 - v1) * 6f * vH;

                    if (2f * vH < 1f)
                        return v2;

                    if (3f * vH < 2f)
                        return v1 + (v2 - v1) * (2f / 3f - vH) * 6f;

                    return v1;
                }

                if (s.Approx(0)) return new Color(l, l, l);

                float k1;

                if (l < .5f)
                    k1 = l * (1f + s);
                else
                    k1 = l + s - s * l;


                var k2 = 2f * l - k1;

                float r, g, b;
                r = hue2Rgb(k2, k1, h + 1f / 3);
                g = hue2Rgb(k2, k1, h);
                b = hue2Rgb(k2, k1, h - 1f / 3);

                return new Color(r, g, b);
            }
            public static Color LCHtoRGB(float l, float c, float h)
            {
                l *= 100;
                c *= 100;
                h *= 360;

                double xw = 0.948110;
                double yw = 1.00000;
                double zw = 1.07304;

                float a = c * Mathf.Cos(Mathf.Deg2Rad * h);
                float b = c * Mathf.Sin(Mathf.Deg2Rad * h);

                float fy = (l + 16) / 116;
                float fx = fy + (a / 500);
                float fz = fy - (b / 200);

                float x = (float)System.Math.Round(xw * ((System.Math.Pow(fx, 3) > 0.008856) ? System.Math.Pow(fx, 3) : ((fx - 16 / 116) / 7.787)), 5);
                float y = (float)System.Math.Round(yw * ((System.Math.Pow(fy, 3) > 0.008856) ? System.Math.Pow(fy, 3) : ((fy - 16 / 116) / 7.787)), 5);
                float z = (float)System.Math.Round(zw * ((System.Math.Pow(fz, 3) > 0.008856) ? System.Math.Pow(fz, 3) : ((fz - 16 / 116) / 7.787)), 5);

                float r = x * 3.2406f - y * 1.5372f - z * 0.4986f;
                float g = -x * 0.9689f + y * 1.8758f + z * 0.0415f;
                float bValue = x * 0.0557f - y * 0.2040f + z * 1.0570f;

                r = r > 0.0031308f ? 1.055f * (float)System.Math.Pow(r, 1 / 2.4) - 0.055f : r * 12.92f;
                g = g > 0.0031308f ? 1.055f * (float)System.Math.Pow(g, 1 / 2.4) - 0.055f : g * 12.92f;
                bValue = bValue > 0.0031308f ? 1.055f * (float)System.Math.Pow(bValue, 1 / 2.4) - 0.055f : bValue * 12.92f;

                // r = (float)System.Math.Round(System.Math.Max(0, System.Math.Min(1, r)));
                // g = (float)System.Math.Round(System.Math.Max(0, System.Math.Min(1, g)));
                // bValue = (float)System.Math.Round(System.Math.Max(0, System.Math.Min(1, bValue)));

                return new Color(r, g, bValue);

            }

        }


        public static Color Greyscale(float brightness, float alpha = 1) => new(brightness, brightness, brightness, alpha);

        public static Color SetAlpha(this Color color, float alpha) { color.a = alpha; return color; }
        public static Color MultiplyAlpha(this Color color, float k) { color.a *= k; return color; }





        #endregion

        #region Objects


        public static Object[] FindObjects(Type type)
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType(type, FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType(type);
#endif
        }
        public static T[] FindObjects<T>() where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<T>();
#endif
        }

        public static void Destroy(this Object r)
        {
            if (Application.isPlaying)
                Object.Destroy(r);
            else
                Object.DestroyImmediate(r);

        }

        public static void DestroyImmediate(this Object o) => Object.DestroyImmediate(o);





        #endregion

        #region GameObjects


        public static bool IsPrefab(this GameObject go) => go.scene.name == null || go.scene.name == go.name;

        public static Bounds GetBounds(this GameObject go, bool local = false)
        {
            Bounds bounds = default;

            foreach (var r in go.GetComponentsInChildren<MeshRenderer>())
            {
                var b = local ? r.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds : r.bounds;

                if (bounds == default)
                    bounds = b;
                else
                    bounds.Encapsulate(b);
            }

            foreach (var r in go.GetComponentsInChildren<Terrain>())
            {
                var b = local ? new Bounds(r.terrainData.size / 2, r.terrainData.size) : new Bounds(r.transform.position + r.terrainData.size / 2, r.terrainData.size);

                if (bounds == default)
                    bounds = b;
                else
                    bounds.Encapsulate(new Bounds(r.transform.position + r.terrainData.size / 2, r.terrainData.size));

            }

            foreach (var r in go.GetComponentsInChildren<RectTransform>())
            {
                var localBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(r);
                var worldBounds = new Bounds(r.TransformPoint(localBounds.center), r.TransformVector(localBounds.size));

                if (bounds == default)
                    bounds = worldBounds;
                else
                    bounds.Encapsulate(worldBounds);

            }


            if (bounds == default)
                bounds.center = go.transform.position;

            return bounds;
        }





        #endregion

        #region Text


        public static bool IsEmpty(this string s) => s == "";
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        public static bool IsLower(this char c) => System.Char.IsLower(c);
        public static bool IsUpper(this char c) => System.Char.IsUpper(c);
        public static bool IsDigit(this char c) => System.Char.IsDigit(c);
        public static bool IsLetter(this char c) => System.Char.IsLetter(c);
        public static bool IsWhitespace(this char c) => System.Char.IsWhiteSpace(c);

        public static char ToLower(this char c) => System.Char.ToLower(c);
        public static char ToUpper(this char c) => System.Char.ToUpper(c);



        public static string Decamelcase(this string s)
        {
            return Regex.Replace(Regex.Replace(s, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }
        public static string FormatVariableName(this string s, bool lowercaseFollowingWords = true)
        {
            return string.Join(" ", s.Decamelcase()
                         .Split(' ')
                         .Select(r => new[] { "", "and", "or", "with", "without", "by", "from" }.Contains(r.ToLower()) || (lowercaseFollowingWords && !s.Trim().StartsWith(r)) ? r.ToLower()
                                                                                                                                                                                 : r.Substring(0, 1).ToUpper() + r.Substring(1))).Trim(' ');
        }

        public static string Remove(this string s, string toRemove)
        {
            if (toRemove == "") return s;
            return s.Replace(toRemove, "");
        }






        #endregion

        #region Paths


        public static bool HasParentPath(this string path) => path.LastIndexOf('/') > 0;
        public static string GetParentPath(this string path) => path.HasParentPath() ? path.Substring(0, path.LastIndexOf('/')) : "";

        public static string GetFilename(this string path, bool withExtension = false) => withExtension ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
        public static string GetExtension(this string path) => Path.GetExtension(path);


        public static string ToGlobalPath(this string localPath) => Application.dataPath + "/" + localPath.Substring(0, localPath.Length - 1);
        public static string ToLocalPath(this string globalPath) => "Assets" + globalPath.Replace(Application.dataPath, "");



        public static string CombinePath(this string p1, string p2, bool useBackslashOnWindows = false)
        {
            if (useBackslashOnWindows) // false by default because all paths in unity use forward slashes, even on Windows
                return Path.Combine(p1, p2);
            else
                return Path.Combine(p1, p2).Replace('\\', '/');

        }

        public static bool IsSubpathOf(this string path, string of) => path.StartsWith(of + "/") || of == "";

        public static string GetDirectory(this string pathOrDirectory)
        {
            var directory = pathOrDirectory.Contains('.') ? pathOrDirectory.Substring(0, pathOrDirectory.LastIndexOf('/')) : pathOrDirectory;

            if (directory.Contains('.'))
                directory = directory.Substring(0, directory.LastIndexOf('/'));

            return directory;

        }

        public static bool DirectoryExists(this string pathOrDirectory) => Directory.Exists(pathOrDirectory.GetDirectory());

        public static string EnsureDirExists(this string pathOrDirectory) // todo to EnsureDirectoryExists
        {
            var directory = pathOrDirectory.GetDirectory();

            if (directory.HasParentPath() && !Directory.Exists(directory.GetParentPath()))
                EnsureDirExists(directory.GetParentPath());

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return pathOrDirectory;

        }



        public static string ClearDir(this string dir)
        {
            if (!Directory.Exists(dir)) return dir;

            var diri = new DirectoryInfo(dir);
            foreach (var r in diri.EnumerateFiles()) r.Delete();
            foreach (var r in diri.EnumerateDirectories()) r.Delete(true);

            return dir;
        }






#if UNITY_EDITOR

        public static string EnsurePathIsUnique(this string path)
        {
            if (!path.DirectoryExists()) return path;

            var s = AssetDatabase.GenerateUniqueAssetPath(path); // returns empty if parent dir doesnt exist 

            return s == "" ? path : s;

        }

        public static void EnsureDirExistsAndRevealInFinder(string dir)
        {
            EnsureDirExists(dir);
            UnityEditor.EditorUtility.OpenWithDefaultApp(dir);
        }

#endif



        #endregion

        #region AssetDatabase

#if UNITY_EDITOR

        public static AssetImporter GetImporter(this Object t) => AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t));

        public static string ToPath(this string guid) => AssetDatabase.GUIDToAssetPath(guid); // returns empty string if not found
        public static List<string> ToPaths(this IEnumerable<string> guids) => guids.Select(r => r.ToPath()).ToList();


        public static string ToGuid(this string pathInProject) => AssetDatabase.AssetPathToGUID(pathInProject);
        public static List<string> ToGuids(this IEnumerable<string> pathsInProject) => pathsInProject.Select(r => r.ToGuid()).ToList();

        public static string GetPath(this Object o) => AssetDatabase.GetAssetPath(o);
        public static string GetGuid(this Object o) => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(o));

        public static string GetScriptPath(string scriptName) => AssetDatabase.FindAssets("t: script " + scriptName, null).FirstOrDefault()?.ToPath() ?? "scirpt not found"; // todonow to editorutils



#endif





        #endregion

        #region Serialization


        [System.Serializable]
        public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
        {
            [SerializeField] List<TKey> keys = new();
            [SerializeField] List<TValue> values = new();

            public void OnBeforeSerialize()
            {
                keys.Clear();
                values.Clear();

                foreach (KeyValuePair<TKey, TValue> kvp in this)
                {
                    keys.Add(kvp.Key);
                    values.Add(kvp.Value);
                }

            }
            public void OnAfterDeserialize()
            {
                this.Clear();

                for (int i = 0; i < keys.Count; i++)
                    this[keys[i]] = values[i];

            }

        }


#if UNITY_EDITOR

        public static object GetBoxedValue(this SerializedProperty p)
        {

#if UNITY_2022_1_OR_NEWER
            switch (p.propertyType)
            {
                case SerializedPropertyType.Integer:
                    switch (p.numericType)
                    {
                        case SerializedPropertyNumericType.Int8: return (sbyte)p.intValue;
                        case SerializedPropertyNumericType.UInt8: return (byte)p.uintValue;
                        case SerializedPropertyNumericType.Int16: return (short)p.intValue;
                        case SerializedPropertyNumericType.UInt16: return (ushort)p.uintValue;
                        case SerializedPropertyNumericType.Int32: return p.intValue;
                        case SerializedPropertyNumericType.UInt32: return p.uintValue;
                        case SerializedPropertyNumericType.Int64: return p.longValue;
                        case SerializedPropertyNumericType.UInt64: return p.ulongValue;
                        default: return p.intValue;

                    }

                case SerializedPropertyType.Float:
                    if (p.numericType == SerializedPropertyNumericType.Double)
                        return p.doubleValue;
                    else
                        return p.floatValue;

                case SerializedPropertyType.Hash128: return p.hash128Value;
                case SerializedPropertyType.Character: return (ushort)p.uintValue;
                case SerializedPropertyType.Gradient: return p.gradientValue;
                case SerializedPropertyType.ManagedReference: return p.managedReferenceValue;


            }
#endif

            switch (p.propertyType)
            {
                case SerializedPropertyType.Integer: return p.intValue;
                case SerializedPropertyType.Float: return p.floatValue;
                case SerializedPropertyType.Vector2: return p.vector2Value;
                case SerializedPropertyType.Vector3: return p.vector3Value;
                case SerializedPropertyType.Vector4: return p.vector4Value;
                case SerializedPropertyType.Vector2Int: return p.vector2IntValue;
                case SerializedPropertyType.Vector3Int: return p.vector3IntValue;
                case SerializedPropertyType.Quaternion: return p.quaternionValue;
                case SerializedPropertyType.Rect: return p.rectValue;
                case SerializedPropertyType.RectInt: return p.rectIntValue;
                case SerializedPropertyType.Bounds: return p.boundsValue;
                case SerializedPropertyType.BoundsInt: return p.boundsIntValue;
                case SerializedPropertyType.Enum: return p.enumValueIndex;
                case SerializedPropertyType.Boolean: return p.boolValue;
                case SerializedPropertyType.String: return p.stringValue;
                case SerializedPropertyType.Color: return p.colorValue;
                case SerializedPropertyType.ArraySize: return p.intValue;
                case SerializedPropertyType.Character: return (ushort)p.intValue;
                case SerializedPropertyType.AnimationCurve: return p.animationCurveValue;
                case SerializedPropertyType.ObjectReference: return p.objectReferenceValue;
                case SerializedPropertyType.ExposedReference: return p.exposedReferenceValue;
                case SerializedPropertyType.FixedBufferSize: return p.intValue;
                case SerializedPropertyType.LayerMask: return (LayerMask)p.intValue;

            }


            return _noValue;

        }
        public static void SetBoxedValue(this SerializedProperty p, object value)
        {
            if (value == _noValue) return;

            try
            {

#if UNITY_2022_1_OR_NEWER
                switch (p.propertyType)
                {
                    case SerializedPropertyType.ArraySize:
                    case SerializedPropertyType.Integer:
                        if (p.numericType == SerializedPropertyNumericType.UInt64)
                            p.ulongValue = System.Convert.ToUInt64(value);
                        else
                            p.longValue = System.Convert.ToInt64(value);
                        return;

                    case SerializedPropertyType.Float:
                        if (p.numericType == SerializedPropertyNumericType.Double)
                            p.doubleValue = System.Convert.ToDouble(value);
                        else
                            p.floatValue = System.Convert.ToSingle(value);
                        return;

                    case SerializedPropertyType.Character: p.uintValue = System.Convert.ToUInt16(value); return;
                    case SerializedPropertyType.Gradient: p.gradientValue = (Gradient)value; return;
                    case SerializedPropertyType.Hash128: p.hash128Value = (Hash128)value; return;

                }
#endif

                switch (p.propertyType)
                {
                    case SerializedPropertyType.ArraySize:
                    case SerializedPropertyType.Integer: p.intValue = System.Convert.ToInt32(value); return;
                    case SerializedPropertyType.Float: p.floatValue = System.Convert.ToSingle(value); return;
                    case SerializedPropertyType.Vector2: p.vector2Value = (Vector2)value; return;
                    case SerializedPropertyType.Vector3: p.vector3Value = (Vector3)value; return;
                    case SerializedPropertyType.Vector4: p.vector4Value = (Vector4)value; return;
                    case SerializedPropertyType.Vector2Int: p.vector2IntValue = (Vector2Int)value; return;
                    case SerializedPropertyType.Vector3Int: p.vector3IntValue = (Vector3Int)value; return;
                    case SerializedPropertyType.Quaternion: p.quaternionValue = (Quaternion)value; return;
                    case SerializedPropertyType.Rect: p.rectValue = (Rect)value; return;
                    case SerializedPropertyType.RectInt: p.rectIntValue = (RectInt)value; return;
                    case SerializedPropertyType.Bounds: p.boundsValue = (Bounds)value; return;
                    case SerializedPropertyType.BoundsInt: p.boundsIntValue = (BoundsInt)value; return;
                    case SerializedPropertyType.String: p.stringValue = (string)value; return;
                    case SerializedPropertyType.Boolean: p.boolValue = (bool)value; return;
                    case SerializedPropertyType.Enum: p.enumValueIndex = (int)value; return;
                    case SerializedPropertyType.Color: p.colorValue = (Color)value; return;
                    case SerializedPropertyType.AnimationCurve: p.animationCurveValue = (AnimationCurve)value; return;
                    case SerializedPropertyType.ObjectReference: p.objectReferenceValue = (UnityEngine.Object)value; return;
                    case SerializedPropertyType.ExposedReference: p.exposedReferenceValue = (UnityEngine.Object)value; return;
                    case SerializedPropertyType.ManagedReference: p.managedReferenceValue = value; return;

                    case SerializedPropertyType.LayerMask:
                        try
                        {
                            p.intValue = ((LayerMask)value).value; return;
                        }
                        catch (System.InvalidCastException)
                        {
                            p.intValue = System.Convert.ToInt32(value); return;
                        }

                }

            }
            catch { }

        }

        static object _noValue = new();

#endif




        #endregion

        #region GlobalID

#if UNITY_EDITOR

        [System.Serializable]
        public struct GlobalID : System.IEquatable<GlobalID>
        {
            public Object GetObject() => GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalObjectId);
            public int GetObjectInstanceId() => _GlobalObjectId_GlobalObjectIdentifierToInstanceIDSlow(globalObjectId);


            public int idType => globalObjectId.identifierType;
            public string guid => globalObjectId.assetGUID.ToString();
            public ulong fileId => globalObjectId.targetObjectId;
            public ulong prefabId => globalObjectId.targetPrefabId;

            public bool isNull => globalObjectId.identifierType == 0;
            public bool isAsset => globalObjectId.identifierType == 1;
            public bool isSceneObject => globalObjectId.identifierType == 2;

            public GlobalObjectId globalObjectId => _globalObjectId.Equals(default) && globalObjectIdString != null && GlobalObjectId.TryParse(globalObjectIdString, out var r) ? _globalObjectId = r : _globalObjectId;
            public GlobalObjectId _globalObjectId;

            public GlobalID(Object o) => globalObjectIdString = (_globalObjectId = GlobalObjectId.GetGlobalObjectIdSlow(o)).ToString();
            public GlobalID(string s) => globalObjectIdString = GlobalObjectId.TryParse(s, out _globalObjectId) ? s : s;

            public string globalObjectIdString;



            public bool Equals(GlobalID other) => this.globalObjectIdString.Equals(other.globalObjectIdString);

            public static bool operator ==(GlobalID a, GlobalID b) => a.Equals(b);
            public static bool operator !=(GlobalID a, GlobalID b) => !a.Equals(b);

            public override bool Equals(object other) => other is GlobalID otherglobalID && this.Equals(otherglobalID);
            public override int GetHashCode() => globalObjectIdString == null ? 0 : globalObjectIdString.GetHashCode();


            public override string ToString() => globalObjectIdString;




            public GlobalID UnpackForPrefab()
            {
                var unpackedFileId = (this.fileId ^ this.prefabId) & 0x7fffffffffffffff;

                var unpackedGId = new GlobalID($"GlobalObjectId_V1-{this.idType}-{this.guid}-{unpackedFileId}-0");

                return unpackedGId;

            }

        }

        public static GlobalID GetGlobalID(this Object o) => new(o);
        public static GlobalID[] GetGlobalIDs(this IEnumerable<int> instanceIds)
        {
            var unityGlobalIds = new GlobalObjectId[instanceIds.Count()];

            _GlobalObjectId_GetGlobalObjectIdsSlow(instanceIds.ToArray(), unityGlobalIds);

            var globalIds = unityGlobalIds.Select(r => new GlobalID(r.ToString()));

            return globalIds.ToArray();

        }

        public static Object[] GetObjects(this IEnumerable<GlobalID> globalIDs)
        {
            var goids = globalIDs.Select(r => r.globalObjectId).ToArray();

            var objects = new Object[goids.Length];

            GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(goids, objects);

            return objects;

        }
        public static int[] GetObjectInstanceIds(this IEnumerable<GlobalID> globalIDs)
        {
            var goids = globalIDs.Select(r => r.globalObjectId).ToArray();

            var iids = new int[goids.Length];

            _GlobalObjectId_GlobalObjectIdentifiersToInstanceIDsSlow(goids, iids);

            return iids;

        }


#endif




        #endregion

        #region Editor

#if UNITY_EDITOR


        public static class EditorUtils
        {

            public static void OpenFolder(string path)
            {
                var folder = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

                var t = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
                var w = (EditorWindow)t.GetField("s_LastInteractedProjectBrowser").GetValue(null);

                var m_ListAreaState = t.GetField("m_ListAreaState", maxBindingFlags).GetValue(w);

                m_ListAreaState.GetType().GetField("m_SelectedInstanceIDs").SetValue(m_ListAreaState, new List<int> { folder.GetInstanceID() });

                t.GetMethod("OpenSelectedFolders", maxBindingFlags).Invoke(null, null);

            }

            public static void PingObject(Object o, bool select = false, bool focusProjectWindow = true)
            {
                if (select)
                {
                    Selection.activeObject = null;
                    Selection.activeObject = o;
                }
                if (focusProjectWindow) EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(o);

            }
            public static void PingObject(string guid, bool select = false, bool focusProjectWindow = true) => PingObject(AssetDatabase.LoadAssetAtPath<Object>(guid.ToPath()));

            public static EditorWindow OpenObjectPicker<T>(Object obj = null, bool allowSceneObjects = false, string searchFilter = "", int controlID = 0) where T : Object
            {
                EditorGUIUtility.ShowObjectPicker<T>(obj, allowSceneObjects, searchFilter, controlID);

                return Resources.FindObjectsOfTypeAll(typeof(Editor).Assembly.GetType("UnityEditor.ObjectSelector")).FirstOrDefault() as EditorWindow;

            }
            public static EditorWindow OpenColorPicker(System.Action<Color> colorChangedCallback, Color color, bool showAlpha = true, bool hdr = false)
            {
#if UNITY_6000_3_OR_NEWER
                typeof(Editor).Assembly.GetType("UnityEditor.ColorPicker").InvokeMethod("Show", colorChangedCallback, color, showAlpha, hdr, false);
#else
                typeof(Editor).Assembly.GetType("UnityEditor.ColorPicker").InvokeMethod("Show", colorChangedCallback, color, showAlpha, hdr);
#endif

                return typeof(Editor).Assembly.GetType("UnityEditor.ColorPicker").GetPropertyValue<EditorWindow>("instance");

            }



            public static bool CheckUnityVersion(string versionQuery)
            {
                if (versionQueryCache.TryGetValue(versionQuery, out var cachedResult)) return cachedResult;

                if (versionQuery.Any(r => r.IsLetter() && !versionQuery.EndsWith(" or older") && !versionQuery.EndsWith(" or newer"))) throw new System.ArgumentException("Invalid unity version query");




                var curVersion = new string(Application.unityVersion.TakeWhile(r => !r.IsLetter()).ToArray());

                var curMajor = int.Parse(curVersion.Split('.')[0]);
                var curMinor = int.Parse(curVersion.Split('.')[1]);
                var curPatch = int.Parse(curVersion.Split('.')[2]);





                var givenVersion = new string(versionQuery.TakeWhile(r => !r.IsWhitespace()).ToArray());

                var isMinorGiven = givenVersion.Count(r => r == '.') >= 1;
                var isPatchGiven = givenVersion.Count(r => r == '.') >= 2;

                var givenMajor = int.Parse(givenVersion.Split('.')[0]);
                var givenMinor = isMinorGiven ? int.Parse(givenVersion.Split('.')[1]) : 0;
                var givenPatch = isPatchGiven ? int.Parse(givenVersion.Split('.')[2]) : 0;






                var curVersionCanBeNewer = versionQuery.Contains("or newer");
                var curVersionCanBeOlder = versionQuery.Contains("or older");


                if (curMajor > givenMajor) return versionQueryCache[versionQuery] = curVersionCanBeNewer;
                if (curMajor < givenMajor) return versionQueryCache[versionQuery] = curVersionCanBeOlder;

                if (!isMinorGiven) return versionQueryCache[versionQuery] = true;


                if (curMinor > givenMinor) return versionQueryCache[versionQuery] = curVersionCanBeNewer;
                if (curMinor < givenMinor) return versionQueryCache[versionQuery] = curVersionCanBeOlder;

                if (!isPatchGiven) return versionQueryCache[versionQuery] = true;


                if (curPatch > givenPatch) return versionQueryCache[versionQuery] = curVersionCanBeNewer;
                if (curPatch < givenPatch) return versionQueryCache[versionQuery] = curVersionCanBeOlder;

                return versionQueryCache[versionQuery] = true;




                // query examples:
                // 
                // "2022.3.5 or newer"
                // "2022.3.5 or older"
                // "2022.3 or older"
                // "2022.3"
                // "2022"

            }

            static Dictionary<string, bool> versionQueryCache = new();



            public static void SetSymbolDefinedInAsmdef(string asmdefName, string symbol, bool defined)
            {
                var isDefined = IsSymbolDefinedInAsmdef(asmdefName, symbol);
                var shouldBeDefined = defined;

                if (shouldBeDefined && !isDefined)
                    DefineSymbolInAsmdef(asmdefName, symbol);

                if (!shouldBeDefined && isDefined)
                    UndefineSymbolInAsmdef(asmdefName, symbol);

            }
            public static bool IsSymbolDefinedInAsmdef(string asmdefName, string symbol)
            {
                var path = AssetDatabase.FindAssets("t: asmdef " + asmdefName, null).First().ToPath();
                var importer = AssetImporter.GetAtPath(path);

                var editorType = typeof(Editor).Assembly.GetType("UnityEditor.AssemblyDefinitionImporterInspector");
                var editor = Editor.CreateEditor(importer, editorType);

                var state = editor.GetFieldValue<Object[]>("m_ExtraDataTargets").First();


                var definesList = state.GetFieldValue<IList>("versionDefines");
                var isSymbolDefined = Enumerable.Range(0, definesList.Count).Any(i => definesList[i].GetFieldValue<string>("define") == symbol);


                Object.DestroyImmediate(editor);

                return isSymbolDefined;

            }

            static void DefineSymbolInAsmdef(string asmdefName, string symbol)
            {
                var path = AssetDatabase.FindAssets("t: asmdef " + asmdefName, null).First().ToPath();
                var importer = AssetImporter.GetAtPath(path);

                var editorType = typeof(Editor).Assembly.GetType("UnityEditor.AssemblyDefinitionImporterInspector");
                var editor = Editor.CreateEditor(importer, editorType);

                var state = editor.GetFieldValue<Object[]>("m_ExtraDataTargets").First();


                var definesList = state.GetFieldValue<IList>("versionDefines");

                var defineType = definesList.GetType().GenericTypeArguments[0];
                var newDefine = System.Activator.CreateInstance(defineType);

                newDefine.SetFieldValue("name", "Unity");
                newDefine.SetFieldValue("define", symbol);

                definesList.Add(newDefine);


                editor.InvokeMethod("Apply");

                Object.DestroyImmediate(editor);

            }
            static void UndefineSymbolInAsmdef(string asmdefName, string symbol)
            {
                var path = AssetDatabase.FindAssets("t: asmdef " + asmdefName, null).First().ToPath();
                var importer = AssetImporter.GetAtPath(path);

                var editorType = typeof(Editor).Assembly.GetType("UnityEditor.AssemblyDefinitionImporterInspector");
                var editor = Editor.CreateEditor(importer, editorType);

                var state = editor.GetFieldValue<Object[]>("m_ExtraDataTargets").First();


                var definesList = state.GetFieldValue<IList>("versionDefines");

                var defineIndex = Enumerable.Range(0, definesList.Count).First(i => definesList[i].GetFieldValue<string>("define") == symbol);

                definesList.RemoveAt(defineIndex);


                editor.InvokeMethod("Apply");

                Object.DestroyImmediate(editor);

            }




            public static int GetCurrendUndoGroupIndex()
            {
                var args = new object[] { _dummyList, 0 };

                typeof(Undo).GetMethodInfo("GetRecords", typeof(List<string>), typeof(int).MakeByRefType())
                            .Invoke(null, args);


                return (int)args[1];

            }

            static List<string> _dummyList = new();





            public static void Hide(string path)
            {
                if (IsHidden(path)) return;

                if (File.Exists(path))
                    File.Move(path, path + "~");


                path += ".meta";
                if (File.Exists(path))
                    File.Move(path, path + "~");
            }
            public static void Unhide(string path)
            {
                if (!IsHidden(path)) return;
                if (path.EndsWith("~")) path = path.Substring(0, path.Length - 1);

                if (File.Exists(path + "~"))
                    File.Move(path + "~", path);

                path += ".meta";
                if (File.Exists(path + "~"))
                    File.Move(path + "~", path);
            }
            public static bool IsHidden(string path) => path.EndsWith("~") || File.Exists(path + "~");


            public static void CopyDirectoryDeep(string sourcePath, string destinationPath)
            {
                CopyDirectoryRecursively(sourcePath, destinationPath);

                var metas = GetFilesRecursively(destinationPath, (f) => f.EndsWith(".meta"));
                var guidTable = new List<(string originalGuid, string newGuid)>();

                foreach (string meta in metas)
                {
                    StreamReader file = new(meta);
                    file.ReadLine();
                    string guidLine = file.ReadLine();
                    file.Close();
                    string originalGuid = guidLine.Substring(6, guidLine.Length - 6);
                    string newGuid = GUID.Generate().ToString().Replace("-", "");
                    guidTable.Add((originalGuid, newGuid));
                }

                var allFiles = GetFilesRecursively(destinationPath);

                foreach (string fileToModify in allFiles)
                {
                    string content = File.ReadAllText(fileToModify);

                    foreach (var guidPair in guidTable)
                    {
                        content = content.Replace(guidPair.originalGuid, guidPair.newGuid);
                    }

                    File.WriteAllText(fileToModify, content);
                }

                AssetDatabase.Refresh();
            }

            private static void CopyDirectoryRecursively(string sourceDirName, string destDirName)
            {
                DirectoryInfo dir = new(sourceDirName);

                DirectoryInfo[] dirs = dir.GetDirectories();

                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, false);
                }

                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectoryRecursively(subdir.FullName, temppath);
                }
            }

            private static List<string> GetFilesRecursively(string path, System.Func<string, bool> criteria = null, List<string> files = null)
            {
                if (files == null)
                {
                    files = new List<string>();
                }

                files.AddRange(Directory.GetFiles(path).Where(f => criteria == null || criteria(f)));

                foreach (string directory in Directory.GetDirectories(path))
                {
                    GetFilesRecursively(directory, criteria, files);
                }

                return files;
            }





            // for non-extension methods

        }


        public static class EditorPrefsCached
        {
            public static int GetInt(string key, int defaultValue = 0)
            {
                if (ints_byKey.ContainsKey(key))
                    return ints_byKey[key];
                else
                    return ints_byKey[key] = EditorPrefs.GetInt(key, defaultValue);

            }
            public static bool GetBool(string key, bool defaultValue = false)
            {
                if (bools_byKey.ContainsKey(key))
                    return bools_byKey[key];
                else
                    return bools_byKey[key] = EditorPrefs.GetBool(key, defaultValue);

            }
            public static float GetFloat(string key, float defaultValue = 0)
            {
                if (floats_byKey.ContainsKey(key))
                    return floats_byKey[key];
                else
                    return floats_byKey[key] = EditorPrefs.GetFloat(key, defaultValue);

            }
            public static string GetString(string key, string defaultValue = "")
            {
                if (strings_byKey.ContainsKey(key))
                    return strings_byKey[key];
                else
                    return strings_byKey[key] = EditorPrefs.GetString(key, defaultValue);

            }

            public static void SetInt(string key, int value)
            {
                ints_byKey[key] = value;

                EditorPrefs.SetInt(key, value);

            }
            public static void SetBool(string key, bool value)
            {
                bools_byKey[key] = value;

                EditorPrefs.SetBool(key, value);

            }
            public static void SetFloat(string key, float value)
            {
                floats_byKey[key] = value;

                EditorPrefs.SetFloat(key, value);

            }
            public static void SetString(string key, string value)
            {
                strings_byKey[key] = value;

                EditorPrefs.SetString(key, value);

            }


            static Dictionary<string, int> ints_byKey = new();
            static Dictionary<string, bool> bools_byKey = new();
            static Dictionary<string, float> floats_byKey = new();
            static Dictionary<string, string> strings_byKey = new();

        }

        public static class ProjectPrefs
        {
            public static int GetInt(string key, int defaultValue = 0) => EditorPrefsCached.GetInt(key + projectId, defaultValue);
            public static bool GetBool(string key, bool defaultValue = false) => EditorPrefsCached.GetBool(key + projectId, defaultValue);
            public static float GetFloat(string key, float defaultValue = 0) => EditorPrefsCached.GetFloat(key + projectId, defaultValue);
            public static string GetString(string key, string defaultValue = "") => EditorPrefsCached.GetString(key + projectId, defaultValue);

            public static void SetInt(string key, int value) => EditorPrefsCached.SetInt(key + projectId, value);
            public static void SetBool(string key, bool value) => EditorPrefsCached.SetBool(key + projectId, value);
            public static void SetFloat(string key, float value) => EditorPrefsCached.SetFloat(key + projectId, value);
            public static void SetString(string key, string value) => EditorPrefsCached.SetString(key + projectId, value);



            public static bool HasKey(string key) => EditorPrefs.HasKey(key + projectId);
            public static void DeleteKey(string key) => EditorPrefs.DeleteKey(key + projectId);



            public static int projectId => PlayerSettings.productGUID.GetHashCode();

        }



        public static void RecordUndo(this Object o, string operationName = "") => Undo.RecordObject(o, operationName);
        public static void Dirty(this Object o) => UnityEditor.EditorUtility.SetDirty(o);
        public static void Save(this Object o) => AssetDatabase.SaveAssetIfDirty(o);



        public static void SelectInInspector(this Object[] objects, bool frameInHierarchy = false, bool frameInProject = false)
        {
            void setHierarchyLocked(bool isLocked) => allHierarchies.ForEach(r => r?.GetMemberValue("m_SceneHierarchy")?.SetMemberValue("m_RectSelectInProgress", isLocked));
            void setProjectLocked(bool isLocked) => allProjectBrowsers.ForEach(r => r?.SetMemberValue("m_InternalSelectionChange", isLocked));


            if (!frameInHierarchy) setHierarchyLocked(true);
            if (!frameInProject) setProjectLocked(true);

            Selection.objects = objects?.ToArray();

            if (!frameInHierarchy) EditorApplication.delayCall += () => setHierarchyLocked(false);
            if (!frameInProject) EditorApplication.delayCall += () => setProjectLocked(false);

        }
        public static void SelectInInspector(this Object obj, bool frameInHierarchy = false, bool frameInProject = false) => new[] { obj }.SelectInInspector(frameInHierarchy, frameInProject);

        static IEnumerable<EditorWindow> allHierarchies => _allHierarchies ??= typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow").GetFieldValue<IList>("s_SceneHierarchyWindows").Cast<EditorWindow>();
        static IEnumerable<EditorWindow> _allHierarchies;

        static IEnumerable<EditorWindow> allProjectBrowsers => _allProjectBrowsers ??= typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser").GetFieldValue<IList>("s_ProjectBrowsers").Cast<EditorWindow>();
        static IEnumerable<EditorWindow> _allProjectBrowsers;



        public static void MoveTo(this EditorWindow window, Vector2 position, bool ensureFitsOnScreen = true)
        {
            if (!ensureFitsOnScreen) { window.position = window.position.SetPos(position); return; }

            var windowRect = window.position;
            var unityWindowRect = EditorGUIUtility.GetMainWindowPosition();

            position.x = position.x.Max(unityWindowRect.position.x);
            position.y = position.y.Max(unityWindowRect.position.y);

            position.x = position.x.Min(unityWindowRect.xMax - windowRect.width);
            position.y = position.y.Min(unityWindowRect.yMax - windowRect.height);

            window.position = windowRect.SetPos(position);

        }



#endif

        #endregion

        #region Instance/Entity ID mess


        static int _GlobalObjectId_GlobalObjectIdentifierToInstanceIDSlow(GlobalObjectId id)
        {
#if UNITY_6000_3_OR_NEWER
            return GlobalObjectId.GlobalObjectIdentifierToEntityIdSlow(id);
#else
            return GlobalObjectId.GlobalObjectIdentifierToInstanceIDSlow(id);
#endif

        }

        static void _GlobalObjectId_GlobalObjectIdentifiersToInstanceIDsSlow(GlobalObjectId[] identifiers, int[] outputInstanceIDs)
        {
#if UNITY_6000_3_OR_NEWER

            var outputEntityIds = new EntityId[outputInstanceIDs.Length];

            GlobalObjectId.GlobalObjectIdentifiersToEntityIdsSlow(identifiers, outputEntityIds);

            for (int i = 0; i < outputEntityIds.Length; i++)
                outputInstanceIDs[i] = (int)outputEntityIds[i];

#else

            GlobalObjectId.GlobalObjectIdentifiersToInstanceIDsSlow(identifiers, outputInstanceIDs);

#endif

        }

        static void _GlobalObjectId_GetGlobalObjectIdsSlow(int[] ids, GlobalObjectId[] outputIdentifiers)
        {
#if UNITY_6000_3_OR_NEWER
            GlobalObjectId.GetGlobalObjectIdsSlow(ids.Select(r => (EntityId)r).ToArray(), outputIdentifiers);
#else
            GlobalObjectId.GetGlobalObjectIdsSlow(ids, outputIdentifiers);
#endif

        }



        public static Object _EditorUtility_InstanceIDToObject(int iid)
        {
#if UNITY_6000_3_OR_NEWER
            return EditorUtility.EntityIdToObject(iid);
#else
            return EditorUtility.InstanceIDToObject(iid);
#endif
        }

        public static string _AssetDatabase_GetAssetPath(int instanceID)
        {
#if UNITY_6000_3_OR_NEWER
            return AssetDatabase.GetAssetPath((EntityId)instanceID);
#else
            return AssetDatabase.GetAssetPath(instanceID);
#endif
        }

        public static int[] _Selection_instanceIDs
        {
            get
            {
#if UNITY_6000_3_OR_NEWER
                return Selection.entityIds.Select(r => (int)r).ToArray();
#else
                return Selection.instanceIDs;
#endif
            }
        }






        #endregion

    }

    public static class VGUI
    {

        #region Drawing


        public static Rect Draw(this Rect rect, Color color)
        {
            EditorGUI.DrawRect(rect, color);

            return rect;

        }
        public static Rect Draw(this Rect rect) => rect.Draw(Color.black);

        public static Rect DrawOutline(this Rect rect, Color color, float thickness = 1)
        {

            rect.SetWidth(thickness).Draw(color);
            rect.SetWidthFromRight(thickness).Draw(color);

            rect.SetHeight(thickness).Draw(color);
            rect.SetHeightFromBottom(thickness).Draw(color);


            return rect;

        }
        public static Rect DrawOutline(this Rect rect, float thickness = 1) => rect.DrawOutline(Color.black, thickness);




        public static Rect DrawRounded(this Rect rect, Color color, int cornerRadius)
        {
            if (!curEvent.isRepaint) return rect;

            cornerRadius = cornerRadius.Min((rect.height / 2).FloorToInt()).Min((rect.width / 2).FloorToInt());

            if (cornerRadius < 0) return rect;

            GUIStyle style;

            void getStyle()
            {
                if (_roundedStylesByCornerRadius.TryGetValue(cornerRadius, out style)) return;

                var pixelsPerPoint = 2;

                var res = cornerRadius * 2 * pixelsPerPoint;
                var pixels = new Color[res * res];

                var white = Greyscale(1, 1);
                var clear = Greyscale(1, 0);
                var halfRes = res / 2;

                for (int x = 0; x < res; x++)
                    for (int y = 0; y < res; y++)
                    {
                        var sqrMagnitude = (new Vector2(x - halfRes + .5f, y - halfRes + .5f)).sqrMagnitude;
                        pixels[x + y * res] = sqrMagnitude <= halfRes * halfRes ? white : clear;
                    }

                var texture = new Texture2D(res, res);
                texture.SetPropertyValue("pixelsPerPoint", pixelsPerPoint);
                texture.hideFlags = HideFlags.DontSave;
                texture.SetPixels(pixels);
                texture.Apply();



                style = new GUIStyle();
                style.normal.background = texture;
                style.alignment = TextAnchor.MiddleCenter;
                style.border = new RectOffset(cornerRadius, cornerRadius, cornerRadius, cornerRadius);


                _roundedStylesByCornerRadius[cornerRadius] = style;

            }
            void draw()
            {
                SetGUIColor(color);

                style.Draw(rect, false, false, false, false);

                ResetGUIColor();

            }

            getStyle();
            draw();

            return rect;

        }
        public static Rect DrawRounded(this Rect rect, Color color, float cornerRadius) => rect.DrawRounded(color, cornerRadius.RoundToInt());

        static Dictionary<int, GUIStyle> _roundedStylesByCornerRadius = new();




        public static Rect DrawBlurred(this Rect rect, Color color, int blurRadius)
        {
            if (!curEvent.isRepaint) return rect;

            var pixelsPerPoint = .5f;
            // var pixelsPerPoint = 1f;

            var blurRadiusScaled = (blurRadius * pixelsPerPoint).RoundToInt().Max(1).Min(123);

            var croppedRectWidth = (rect.width * pixelsPerPoint).RoundToInt().Min(blurRadiusScaled * 2);
            var croppedRectHeight = (rect.height * pixelsPerPoint).RoundToInt().Min(blurRadiusScaled * 2);

            var textureWidth = croppedRectWidth + blurRadiusScaled * 2;
            var textureHeight = croppedRectHeight + blurRadiusScaled * 2;

            if (textureWidth <= 0 || textureWidth > 1232) return rect;
            if (textureHeight <= 0 || textureHeight > 1232) return rect;


            GUIStyle style;

            void getStyle()
            {
                if (_blurredStylesByTextureSize.TryGetValue((textureWidth, textureHeight), out style)) return;

                // VDebug.LogStart(blurRadius + "");

                var pixels = new Color[textureWidth * textureHeight];
                var kernel = GaussianKernel.GenerateArray(blurRadiusScaled * 2 + 1);

                for (int x = 0; x < textureWidth; x++)
                    for (int y = 0; y < textureHeight; y++)
                    {
                        var sum = 0f;

                        for (int xSample = (x - blurRadiusScaled).Max(blurRadiusScaled); xSample <= (x + blurRadiusScaled).Min(textureWidth - 1 - blurRadiusScaled); xSample++)
                            for (int ySample = (y - blurRadiusScaled).Max(blurRadiusScaled); ySample <= (y + blurRadiusScaled).Min(textureHeight - 1 - blurRadiusScaled); ySample++)
                                sum += kernel[blurRadiusScaled + xSample - x, blurRadiusScaled + ySample - y];

                        pixels[x + y * textureWidth] = Greyscale(1, sum);

                    }

                var texture = new Texture2D(textureWidth, textureHeight);
                texture.SetPropertyValue("pixelsPerPoint", pixelsPerPoint);
                texture.hideFlags = HideFlags.DontSave;
                texture.SetPixels(pixels);
                texture.Apply();


                style = new GUIStyle();
                style.normal.background = texture;
                style.alignment = TextAnchor.MiddleCenter;

                var borderX = ((textureWidth / 2f - 1) / pixelsPerPoint).FloorToInt();
                var borderY = ((textureHeight / 2f - 1) / pixelsPerPoint).FloorToInt();
                style.border = new RectOffset(borderX, borderX, borderY, borderY);

                _blurredStylesByTextureSize[(textureWidth, textureHeight)] = style;

                // VDebug.LogFinish();

            }
            void draw()
            {
                SetGUIColor(color);

                style.Draw(rect.SetSizeFromMid(rect.width + blurRadius * 2, rect.height + blurRadius * 2), false, false, false, false);

                ResetGUIColor();

            }

            getStyle();
            draw();

            return rect;

        }
        public static Rect DrawBlurred(this Rect rect, Color color, float blurRadius) => rect.DrawBlurred(color, blurRadius.RoundToInt());

        static Dictionary<(int, int), GUIStyle> _blurredStylesByTextureSize = new();




        static void DrawCurtain(this Rect rect, Color color, int dir)
        {
            void genTextures()
            {
                if (_gradientTextures != null) return;

                _gradientTextures = new Texture2D[4];

                // var pixels = Enumerable.Range(0, 256).Select(r => Greyscale(1, r / 255f));
                var pixels = Enumerable.Range(0, 256).Select(r => Greyscale(1, (r / 255f).Smoothstep()));

                var up = new Texture2D(1, 256);
                up.SetPixels(pixels.Reverse().ToArray());
                up.Apply();
                up.hideFlags = HideFlags.DontSave;
                up.wrapMode = TextureWrapMode.Clamp;
                _gradientTextures[0] = up;

                var down = new Texture2D(1, 256);
                down.SetPixels(pixels.ToArray());
                down.Apply();
                down.hideFlags = HideFlags.DontSave;
                down.wrapMode = TextureWrapMode.Clamp;
                _gradientTextures[1] = down;

                var left = new Texture2D(256, 1);
                left.SetPixels(pixels.ToArray());
                left.Apply();
                left.hideFlags = HideFlags.DontSave;
                left.wrapMode = TextureWrapMode.Clamp;
                _gradientTextures[2] = left;

                var right = new Texture2D(256, 1);
                right.SetPixels(pixels.Reverse().ToArray());
                right.Apply();
                right.hideFlags = HideFlags.DontSave;
                right.wrapMode = TextureWrapMode.Clamp;
                _gradientTextures[3] = right;

            }
            void draw()
            {
                SetGUIColor(color);

                GUI.DrawTexture(rect, _gradientTextures[dir]);

                ResetGUIColor();

            }

            genTextures();
            draw();

        }

        static Texture2D[] _gradientTextures;

        public static void DrawCurtainUp(this Rect rect, Color color) => rect.DrawCurtain(color, 0);
        public static void DrawCurtainDown(this Rect rect, Color color) => rect.DrawCurtain(color, 1);
        public static void DrawCurtainLeft(this Rect rect, Color color) => rect.DrawCurtain(color, 2);
        public static void DrawCurtainRight(this Rect rect, Color color) => rect.DrawCurtain(color, 3);






        #endregion

        #region Events


        public class WrappedEvent
        {
            public Event e;

            public bool isRepaint => e.type == EventType.Repaint;
            public bool isLayout => e.type == EventType.Layout;
            public bool isUsed => e.type == EventType.Used;
            public bool isMouseLeaveWindow => e.type == EventType.MouseLeaveWindow;
            public bool isMouseEnterWindow => e.type == EventType.MouseEnterWindow;
            public bool isContextClick => e.type == EventType.ContextClick;
            public bool isIgnore => e.type == EventType.Ignore;

            public bool isKeyDown => e.type == EventType.KeyDown;
            public bool isKeyUp => e.type == EventType.KeyUp;
            public KeyCode keyCode => e.keyCode;
            public char characted => e.character;

            public bool isExecuteCommand => e.type == EventType.ExecuteCommand;
            public string commandName => e.commandName;

            public bool isMouse => e.isMouse;
            public bool isMouseDown => e.type == EventType.MouseDown;
            public bool isMouseUp => e.type == EventType.MouseUp;
            public bool isMouseDrag => e.type == EventType.MouseDrag;
            public bool isMouseMove => e.type == EventType.MouseMove;
            public bool isScroll => e.type == EventType.ScrollWheel;
            public int mouseButton => e.button;
            public int clickCount => e.clickCount;
            public Vector2 mousePosition => e.mousePosition;
            public Vector2 mousePosition_screenSpace => GUIUtility.GUIToScreenPoint(e.mousePosition);
            public Vector2 mouseDelta => e.delta;

            public bool isDragUpdate => e.type == EventType.DragUpdated;
            public bool isDragPerform => e.type == EventType.DragPerform;
            public bool isDragExit => e.type == EventType.DragExited;

            public EventModifiers modifiers => e.modifiers;
            public bool holdingAnyModifierKey => modifiers != EventModifiers.None;

            public bool holdingAlt => e.alt;
            public bool holdingShift => e.shift;
            public bool holdingCtrl => e.control;
            public bool holdingCmd => e.command;
            public bool holdingCmdOrCtrl => e.command || e.control;

            public bool holdingAltOnly => e.modifiers == EventModifiers.Alt;        // in some sessions FunctionKey is always pressed?
            public bool holdingShiftOnly => e.modifiers == EventModifiers.Shift;        // in some sessions FunctionKey is always pressed?
            public bool holdingCtrlOnly => e.modifiers == EventModifiers.Control;
            public bool holdingCmdOnly => e.modifiers == EventModifiers.Command;
            public bool holdingCmdOrCtrlOnly => (e.modifiers == EventModifiers.Command || e.modifiers == EventModifiers.Control);

            public EventType type => e.type;

            public void Use() => e?.Use();


            public WrappedEvent(Event e) => this.e = e;

            public override string ToString() => e.ToString();

        }

        public static WrappedEvent Wrap(this Event e) => new(e);

        public static WrappedEvent curEvent => _curEvent ??= typeof(Event).GetFieldValue<Event>("s_Current").Wrap();
        static WrappedEvent _curEvent;





        #endregion

        #region Shortcuts


        public static Rect lastRect => GUILayoutUtility.GetLastRect();

        public static bool isDarkTheme => EditorGUIUtility.isProSkin;

        public static bool IsHovered(this Rect r) => r.Contains(curEvent.mousePosition);

        public static float GetLabelWidth(this string s) => GUI.skin.label.CalcSize(new GUIContent(s)).x;
        public static float GetLabelWidth(this string s, int fontSize)
        {
            SetLabelFontSize(fontSize);

            var r = s.GetLabelWidth();

            ResetLabelStyle();

            return r;

        }
        public static float GetLabelWidth(this string s, bool isBold)
        {
            if (isBold)
                SetLabelBold();

            var r = s.GetLabelWidth();

            if (isBold)
                ResetLabelStyle();

            return r;

        }
        public static float GetLabelWidth(this string s, int fontSize, bool isBold)
        {
            if (isBold)
                SetLabelBold();

            SetLabelFontSize(fontSize);

            var r = s.GetLabelWidth();

            ResetLabelStyle();

            return r;

        }

        public static void SetGUIEnabled(bool enabled) { _prevGuiEnabled = GUI.enabled; GUI.enabled = enabled; }
        public static void ResetGUIEnabled() => GUI.enabled = _prevGuiEnabled;
        static bool _prevGuiEnabled = true;

        public static void SetLabelFontSize(int size) => GUI.skin.label.fontSize = size;
        public static void SetLabelBold() => GUI.skin.label.fontStyle = FontStyle.Bold;
        public static void SetLabelAlignmentCenter() => GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        public static void ResetLabelStyle()
        {
            GUI.skin.label.fontSize = 0;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.skin.label.wordWrap = false;
        }


        public static void SetGUIColor(Color c)
        {
            _guiColorStack.Push(GUI.color);

            GUI.color *= c;

        }
        public static void ResetGUIColor()
        {
            GUI.color = _guiColorStack.Pop();
        }

        static Stack<Color> _guiColorStack = new();



        public static float editorDeltaTime = .0166f;

        static void EditorDeltaTime_Update()
        {
            editorDeltaTime = (float)(EditorApplication.timeSinceStartup - _lastUpdateTime);

            _lastUpdateTime = EditorApplication.timeSinceStartup;

        }
        static double _lastUpdateTime;

        [InitializeOnLoadMethod]
        static void EditorDeltaTime_Subscribe()
        {
            EditorApplication.update -= EditorDeltaTime_Update;
            EditorApplication.update += EditorDeltaTime_Update;
        }




        #endregion

        #region Controls


        public static bool IconButton(Rect rect, string iconName, float iconSize = default, Color color = default, Color colorHovered = default, Color colorPressed = default)
        {
            var id = EditorGUIUtility.GUIToScreenRect(rect).GetHashCode();// GUIUtility.GetControlID(FocusType.Passive, rect);
            var isPressed = id == _pressedIconButtonId;

            var wasActivated = false;

            void icon()
            {
                if (!curEvent.isRepaint) return;


                if (color == default)
                    color = Color.white;

                if (colorHovered == default)
                    colorHovered = Color.white;

                if (colorPressed == default)
                    colorPressed = Color.white.SetAlpha(.6f);


                if (rect.IsHovered())
                    color = colorHovered;

                if (isPressed)
                    color = colorPressed;


                if (iconSize == default)
                    iconSize = rect.width.Min(rect.height);

                var iconRect = rect.SetSizeFromMid(iconSize);



                SetGUIColor(color);

                GUI.DrawTexture(iconRect, EditorIcons.GetIcon(iconName));

                ResetGUIColor();


            }
            void mouseDown()
            {
                if (!curEvent.isMouseDown) return;
                if (!rect.IsHovered()) return;

                _pressedIconButtonId = id;

                curEvent.Use();

            }
            void mouseUp()
            {
                if (!curEvent.isMouseUp) return;
                if (!isPressed) return;

                _pressedIconButtonId = 0;

                if (rect.IsHovered())
                    wasActivated = true;

                curEvent.Use();

            }
            void mouseDrag()
            {
                if (!curEvent.isMouseDrag) return;
                if (!isPressed) return;

                curEvent.Use();

            }

            rect.MarkInteractive();

            icon();
            mouseDown();
            mouseUp();
            mouseDrag();

            return wasActivated;

        }

        static int _pressedIconButtonId;





        #endregion

        #region Layout


        public static void Space(float px = 6) => GUILayout.Space(px);

        public static Rect ExpandWidthLabelRect() { GUILayout.Label(""/* , GUILayout.Height(0) */, GUILayout.ExpandWidth(true)); return lastRect; }
        public static Rect ExpandWidthLabelRect(float height) { GUILayout.Label("", GUILayout.Height(height), GUILayout.ExpandWidth(true)); return lastRect; }




        public static void BeginIndent(float f)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(f);
            GUILayout.BeginVertical();

            _indentLabelWidthStack.Push(EditorGUIUtility.labelWidth);

            EditorGUIUtility.labelWidth -= f;
        }

        public static void EndIndent(float f = 0)
        {
            GUILayout.EndVertical();
            GUILayout.Space(f);
            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = _indentLabelWidthStack.Pop();
        }
        static Stack<float> _indentLabelWidthStack = new();





        #endregion

        #region GUIColors


        public static class GUIColors
        {
            public static Color windowBackground => isDarkTheme ? Greyscale(.22f) : Greyscale(.78f); // prev backgroundCol
            public static Color pressedButtonBackground => isDarkTheme ? new Color(.48f, .76f, 1f, 1f) * 1.4f : new Color(.48f, .7f, 1f, 1f) * 1.2f; // prev pressedButtonCol
            public static Color greyedOutTint => Greyscale(.7f);
            public static Color selectedBackground => isDarkTheme ? new Color(.17f, .365f, .535f) : new Color(.2f, .375f, .555f) * 1.2f;
        }




        #endregion

        #region EditorIcons


        public static partial class EditorIcons
        {
            public static Texture2D GetIcon(string iconNameOrPath, bool returnNullIfNotFound = false)
            {
                iconNameOrPath ??= "";

                if (icons_byName.TryGetValue(iconNameOrPath, out var cachedResult) && cachedResult) return cachedResult;


                Texture2D icon = null;

                void getCustom()
                {
                    if (icon) return;
                    if (!customIcons.ContainsKey(iconNameOrPath)) return;

                    var pngBytesString = customIcons[iconNameOrPath];
                    var pngBytes = pngBytesString.Split("-").Select(r => System.Convert.ToByte(r, 16)).ToArray();

                    icon = new Texture2D(1, 1);

                    icon.LoadImage(pngBytes);

                }
                void getBuiltin()
                {
                    if (icon) return;

                    icon = typeof(EditorGUIUtility).InvokeMethod<Texture2D>("LoadIcon", iconNameOrPath) as Texture2D;

                }

                getCustom();
                getBuiltin();

                icons_byName[iconNameOrPath] = icon;

                if (icon == null && !returnNullIfNotFound) return Texture2D.grayTexture;
                else return icon;

            }

            static Dictionary<string, Texture2D> icons_byName = new();

            static Dictionary<string, string> customIcons = new()
            {
                ["Search_"] = "89-50-4E-47-0D-0A-1A-0A-00-00-00-0D-49-48-44-52-00-00-00-20-00-00-00-20-08-06-00-00-00-73-7A-7A-F4-00-00-00-09-70-48-59-73-00-00-0B-13-00-00-0B-13-01-00-9A-9C-18-00-00-00-01-73-52-47-42-00-AE-CE-1C-E9-00-00-00-04-67-41-4D-41-00-00-B1-8F-0B-FC-61-05-00-00-02-00-49-44-41-54-78-01-ED-56-3D-4F-02-41-10-1D-3E-22-C6-C4-CA-D0-9A-58-6B-87-8D-12-6B-0B-1B-7F-07-85-89-95-F4-34-D6-F2-07-2C-FD-23-D0-41-A2-8D-FF-00-8A-0B-0D-7A-70-7C-1C-EB-7B-B8-47-16-C4-70-7B-77-1C-0D-2F-B9-DC-DE-DD-CE-CE-DB-99-D9-37-27-B2-C7-8E-91-B1-99-EC-38-CE-71-A1-50-38-E0-78-34-1A-8D-8B-C5-E2-97-6C-1B-4A-A9-43-38-AB-8E-C7-E3-CF-D9-6C-E6-29-0D-DF-F7-5D-BE-C3-B7-1A-E7-C8-36-E0-BA-EE-1D-1C-39-6A-03-40-CC-E5-5C-49-12-9E-E7-55-B0-B0-AF-1D-F8-93-C9-A4-81-77-0F-B8-97-79-E9-71-DB-20-E1-83-44-45-92-00-77-13-38-47-04-BA-83-C1-E0-FA-BF-B9-20-71-85-39-3D-83-44-BC-48-30-9F-0C-A9-5E-B0-D7-E9-74-8E-36-D9-70-0E-89-6A-C2-4E-AC-9A-D0-45-35-07-43-1D-D6-0E-51-2A-07-51-63-D1-4A-54-B0-B2-B5-F3-B6-58-02-36-4D-DA-72-8D-B0-36-59-F3-81-E7-3C-9B-CD-9E-72-8C-75-5E-C5-12-B0-79-E3-3D-9F-CF-9F-71-AD-30-36-4B-04-28-32-B9-5C-6E-9E-F3-4C-26-D3-12-4B-4C-A7-D3-96-B6-2D-04-82-65-45-60-17-58-22-40-79-65-0D-71-8C-7B-49-2C-81-D0-97-B4-AD-CB-B5-24-0A-8C-22-6C-8A-25-70-04-DB-B6-45-F8-07-3C-42-86-FA-59-1D-43-E3-F8-D6-24-2A-28-22-81-FE-53-5C-C2-0A-11-45-2B-E8-0B-2A-6E-73-32-A5-98-0B-87-90-E2-6E-62-52-6C-90-58-34-A3-40-98-90-DB-D5-66-D4-80-F3-45-C3-62-03-A3-ED-70-38-BC-C1-AB-0B-89-0B-1D-09-57-6D-00-53-16-EC-1C-24-6E-F1-3C-82-DD-37-08-5F-4A-5C-A8-DF-1F-92-1A-2B-1B-42-E3-1A-ED-D7-D3-3F-24-D5-20-E7-DC-39-9D-1B-73-26-F8-7E-2F-49-81-F2-DA-EF-F7-4F-78-AD-93-5A-38-3B-E7-CE-D5-F2-CF-CA-14-A9-8A-DE-9C-6C-C1-B0-73-E7-AB-69-42-6A-5E-24-2D-30-EC-DC-F9-1A-12-8F-92-16-50-0B-4F-6B-8A-F5-5D-D2-04-76-5C-37-09-20-3D-75-49-1B-28-C0-67-5C-1F-74-0E-0E-79-D9-63-8F-15-FC-00-17-02-EB-AB-1A-4B-B3-E7-00-00-00-00-49-45-4E-44-AE-42-60-82",
                ["Cross"] = "89-50-4E-47-0D-0A-1A-0A-00-00-00-0D-49-48-44-52-00-00-00-20-00-00-00-20-08-06-00-00-00-73-7A-7A-F4-00-00-00-09-70-48-59-73-00-00-0B-13-00-00-0B-13-01-00-9A-9C-18-00-00-00-01-73-52-47-42-00-AE-CE-1C-E9-00-00-00-04-67-41-4D-41-00-00-B1-8F-0B-FC-61-05-00-00-00-C5-49-44-41-54-78-01-ED-96-D1-0D-83-30-0C-44-9D-4E-D0-51-BA-02-13-B5-23-A4-1B-A4-13-31-42-3B-4A-37-70-8D-6A-04-42-E0-D8-88-E0-1F-3F-29-8A-50-1C-DF-05-48-62-80-20-08-9C-49-D2-20-22-5E-A9-BB-53-1B-FA-67-4A-E9-0B-0A-66-F3-06-5E-DA-79-6B-89-32-4E-BC-39-71-55-9C-63-47-B2-14-7F-01-3D-37-6A-BD-64-82-C7-7A-8E-1D-A9-9A-06-29-E1-62-35-9B-6F-C2-12-7B-B8-89-66-E2-1A-81-E6-E2-0A-13-ED-C5-2B-26-CE-11-57-98-D8-25-6E-D9-86-FE-B8-7E-02-D7-9F-10-3D-B7-21-7A-1E-44-96-C4-4D-4C-D0-E4-62-49-B8-61-22-4B-1A-B5-6D-38-BF-C7-3F-D4-3A-E9-6E-E7-B1-8E-63-55-68-0A-92-07-3F-16-63-41-92-E1-BF-80-B2-BB-20-09-82-E0-0C-7E-54-36-6A-69-F6-3F-13-EF-00-00-00-00-49-45-4E-44-AE-42-60-82",
                ["Plus"] = "89-50-4E-47-0D-0A-1A-0A-00-00-00-0D-49-48-44-52-00-00-00-20-00-00-00-20-08-06-00-00-00-73-7A-7A-F4-00-00-00-09-70-48-59-73-00-00-0B-13-00-00-0B-13-01-00-9A-9C-18-00-00-00-01-73-52-47-42-00-AE-CE-1C-E9-00-00-00-04-67-41-4D-41-00-00-B1-8F-0B-FC-61-05-00-00-00-A7-49-44-41-54-78-01-ED-D5-01-09-84-30-14-06-E0-7F-C7-05-B8-06-77-0D-CE-08-46-31-C2-1A-68-04-4D-A0-51-8C-A0-0D-B4-81-0D-E6-13-14-C6-10-C5-4D-11-E1-FF-60-F8-78-0C-F7-B3-E1-04-88-02-18-63-B4-8C-04-77-98-17-5F-64-F0-F4-82-BF-C8-AA-BF-F0-14-12-E0-14-0C-C0-00-0C-A0-D6-9A-72-B1-A4-F2-F8-61-5B-6C-CD-E9-64-D4-3B-F3-1B-A5-54-81-3D-D3-D5-6A-AE-A3-DD-F5-D6-8E-E0-83-EB-0C-6E-E3-ED-36-64-9B-72-49-3A-95-7F-6C-8B-71-EC-08-7A-79-77-85-B3-48-C8-CA-DA-DA-12-9E-F8-19-32-00-03-3C-3A-40-67-D5-2D-EE-30-FF-37-34-88-02-8C-19-03-9B-84-46-97-A8-ED-00-00-00-00-49-45-4E-44-AE-42-60-82",
                ["Star"] = "89-50-4E-47-0D-0A-1A-0A-00-00-00-0D-49-48-44-52-00-00-00-20-00-00-00-20-08-06-00-00-00-73-7A-7A-F4-00-00-00-09-70-48-59-73-00-00-0B-13-00-00-0B-13-01-00-9A-9C-18-00-00-00-01-73-52-47-42-00-AE-CE-1C-E9-00-00-00-04-67-41-4D-41-00-00-B1-8F-0B-FC-61-05-00-00-01-16-49-44-41-54-78-01-ED-94-6D-0D-C2-30-10-86-DF-11-04-20-61-12-70-C0-1C-50-07-AB-03-90-80-04-50-00-28-19-0E-90-00-0E-C0-C1-71-CD-BA-B0-B1-86-B5-BD-0E-FE-EC-49-2E-6D-2E-D7-CB-F5-BE-80-89-09-01-44-94-1B-81-80-19-64-28-16-0D-01-73-C8-28-59-9E-F8-07-36-FD-0D-39-22-91-94-40-B5-EE-1A-91-48-02-58-B7-EE-2B-FC-92-8F-F4-37-2C-10-41-6C-06-0A-87-4E-23-82-DE-14-F0-4F-0A-3E-F2-81-77-1B-87-AE-E4-B7-43-13-71-CF-B2-EC-F2-D5-C2-A4-92-E5-44-E9-39-05-95-89-8D-B7-2C-0F-92-63-7C-6C-11-03-D5-CD-76-A3-78-AE-24-5C-D5-4D-20-3B-0A-67-8F-94-B0-43-E5-99-0D-63-53-60-0C-D8-71-E5-11-40-85-31-A0-7A-3A-7C-30-4D-E7-DD-ED-21-8B-48-79-DA-2D-02-6C-83-02-58-3B-74-07-96-B3-43-5F-22-25-8E-F4-77-66-9B-EF-9A-BA-3B-23-A8-0C-3E-01-E8-96-73-E7-6C-53-7F-67-68-A4-82-9D-1D-AD-D3-C1-D9-A6-F7-CE-38-22-15-F6-D7-45-80-BD-B2-6F-E4-65-60-27-4B-8A-58-A7-B6-24-E9-FA-60-62-62-2C-5E-30-1D-6B-34-83-5B-F0-2B-00-00-00-00-49-45-4E-44-AE-42-60-82",
                ["Star Hollow"] = "89-50-4E-47-0D-0A-1A-0A-00-00-00-0D-49-48-44-52-00-00-00-20-00-00-00-20-08-06-00-00-00-73-7A-7A-F4-00-00-00-09-70-48-59-73-00-00-0B-13-00-00-0B-13-01-00-9A-9C-18-00-00-00-01-73-52-47-42-00-AE-CE-1C-E9-00-00-00-04-67-41-4D-41-00-00-B1-8F-0B-FC-61-05-00-00-01-5A-49-44-41-54-78-01-ED-96-FD-6D-C2-30-10-C5-2F-15-03-B0-41-33-42-47-48-37-C8-06-64-03-BA-41-D9-80-6E-00-1B-B4-9D-20-DD-20-EA-04-C9-06-65-83-EB-3B-F1-2C-8C-04-F9-B0-AD-F0-4F-7E-D2-29-16-3A-5F-EC-77-1F-41-64-61-21-02-55-CD-CD-24-82-27-89-A3-84-55-12-C1-4A-E2-D8-C0-4E-F2-08-28-BF-23-97-40-62-52-60-F2-77-72-56-A0-92-40-32-09-04-B7-AE-79-00-8B-F1-9C-65-D9-AB-CC-85-27-7F-09-2B-B8-5E-CB-5C-E0-65-15-EC-8F-EB-B5-AD-61-6F-12-C0-EA-46-F0-02-8F-7C-60-DF-16-F6-65-0B-48-7F-C2-9E-6F-2C-37-78-0E-75-44-07-FF-9F-5E-0F-DE-E8-A8-C3-94-FE-A1-47-F8-1F-27-A5-C9-24-A5-B4-6D-48-9B-B1-4E-9A-98-F4-B8-20-ED-D4-20-F0-DD-72-4F-A3-91-A3-DA-05-DC-51-C6-43-5F-40-A6-EF-40-DF-0F-49-09-5B-CE-D4-68-7B-7C-1A-FA-14-32-92-D1-93-10-D5-6B-55-DF-C1-7E-7B-DC-AC-0B-86-2B-3D-04-CA-6B-54-DE-6F-57-9F-63-AF-70-D3-0F-25-3D-0F-1F-75-2F-64-EB-B9-2E-A9-EE-1D-32-E5-01-3E-61-35-5F-B2-77-85-A6-97-99-F1-4E-3F-F3-A9-25-25-DE-CD-76-B7-7A-9B-EA-38-35-F6-C9-D3-E0-C9-AF-F7-7A-DB-9B-19-9A-3C-0D-53-7A-5B-BD-99-21-A9-E0-AD-8B-09-FE-25-F7-C4-A7-01-41-5E-34-FC-5B-30-DF-7F-84-85-85-50-FE-01-12-E7-01-A3-5F-51-F9-4C-00-00-00-00-49-45-4E-44-AE-42-60-82",
                ["Collapse"] = "89-50-4E-47-0D-0A-1A-0A-00-00-00-0D-49-48-44-52-00-00-00-20-00-00-00-20-08-06-00-00-00-73-7A-7A-F4-00-00-00-09-70-48-59-73-00-00-0B-13-00-00-0B-13-01-00-9A-9C-18-00-00-00-01-73-52-47-42-00-AE-CE-1C-E9-00-00-00-04-67-41-4D-41-00-00-B1-8F-0B-FC-61-05-00-00-00-CE-49-44-41-54-78-01-ED-95-01-0D-C2-30-10-45-7F-A7-60-12-2A-01-09-93-80-04-1C-80-03-EA-60-16-70-30-1C-20-01-09-93-C0-1C-1C-BF-61-24-CD-18-0C-9A-DE-1A-92-BE-A4-49-D7-25-F7-7F-72-FF-5A-A0-90-19-33-3D-10-91-1D-F4-18-8C-31-E7-B7-7F-29-7E-10-7D-5C-A8-59-4D-3C-D4-D0-67-08-3F-E6-5A-B0-D5-34-C2-16-9C-50-48-05-DB-B5-F7-C1-45-0E-28-BC-19-53-7D-F3-7B-AC-09-05-2D-57-1F-8C-96-DF-5B-AC-05-C5-AE-33-F3-ED-CF-F4-C7-98-22-ED-87-4B-A6-85-26-14-38-CA-32-3A-A1-64-E1-46-BE-A7-41-4A-E4-35-74-4B-F8-C9-B0-48-01-0B-D5-3F-8A-3F-E9-25-45-28-59-A4-93-78-3A-68-C1-E2-2E-10-72-88-A4-42-66-8A-81-62-A0-18-C8-6E-20-1A-79-BC-0F-97-71-59-14-FE-95-3B-1B-9A-F9-A3-61-43-3F-A9-00-00-00-00-49-45-4E-44-AE-42-60-82",
                ["Plus Thicker"] = "89-50-4E-47-0D-0A-1A-0A-00-00-00-0D-49-48-44-52-00-00-00-20-00-00-00-20-08-06-00-00-00-73-7A-7A-F4-00-00-00-09-70-48-59-73-00-00-0B-13-00-00-0B-13-01-00-9A-9C-18-00-00-00-01-73-52-47-42-00-AE-CE-1C-E9-00-00-00-04-67-41-4D-41-00-00-B1-8F-0B-FC-61-05-00-00-00-A1-49-44-41-54-78-01-ED-96-D1-09-84-30-10-44-27-C7-15-70-D7-81-25-D9-C1-9D-95-A8-1D-D9-89-25-C4-0E-4C-07-71-85-80-12-22-91-B8-E8-CF-3C-98-8F-2C-21-79-10-D8-0D-40-48-21-DE-FB-5A-32-4B-AC-E4-87-BB-91-4B-47-BF-61-51-C8-0B-E5-7C-A0-C0-15-01-15-28-40-01-0A-98-B8-10-BA-5A-87-3C-55-B4-9E-32-FB-9D-A4-37-C6-0C-39-01-9B-38-5C-0B-27-02-DF-7D-21-F5-04-2A-1D-EE-48-20-2E-BC-13-9B-1A-49-7B-42-A4-8A-D6-13-F2-F4-D0-22-4C-C1-47-87-91-0A-14-A0-00-05-B4-04-1C-EE-46-9A-CF-3F-34-A3-F5-6B-5E-83-90-42-16-B4-42-4C-CD-3F-8F-0E-C4-00-00-00-00-49-45-4E-44-AE-42-60-82",
                ["Dropdown"] = "89-50-4E-47-0D-0A-1A-0A-00-00-00-0D-49-48-44-52-00-00-00-20-00-00-00-20-08-06-00-00-00-73-7A-7A-F4-00-00-00-09-70-48-59-73-00-00-0B-13-00-00-0B-13-01-00-9A-9C-18-00-00-00-01-73-52-47-42-00-AE-CE-1C-E9-00-00-00-04-67-41-4D-41-00-00-B1-8F-0B-FC-61-05-00-00-00-78-49-44-41-54-78-01-ED-D0-B1-0D-80-20-14-84-E1-D3-D2-15-98-82-41-18-C6-51-5C-85-41-98-C2-15-AC-F1-E8-2C-50-91-04-8C-F1-BE-E4-35-0A-FC-04-40-44-44-FE-6E-28-59-14-09-15-06-BA-5B-33-A2-CC-82-E7-6A-F6-9C-E3-23-F8-58-CE-A3-05-1E-1C-0A-E2-01-AD-F0-F0-89-B3-5E-C4-D3-BF-09-2D-31-60-38-5B-26-9E-BE-19-F4-C0-90-CD-5C-C0-A2-27-06-DD-21-EE-F0-06-86-E7-34-10-11-91-2F-DB-01-40-06-CB-42-92-53-E4-4B-00-00-00-00-49-45-4E-44-AE-42-60-82",
            };
        }



        #endregion

        #region Other


        public static void MarkInteractive(this Rect rect)
        {
            if (!curEvent.isRepaint) return;

            var unclippedRect = (Rect)_mi_GUIClip_UnclipToWindow.Invoke(null, new object[] { rect });

            var curGuiView = _pi_GUIView_current.GetValue(null);

            _mi_GUIView_MarkHotRegion.Invoke(curGuiView, new object[] { unclippedRect });

        }

        static PropertyInfo _pi_GUIView_current = typeof(Editor).Assembly.GetType("UnityEditor.GUIView").GetProperty("current", maxBindingFlags);
        static MethodInfo _mi_GUIView_MarkHotRegion = typeof(Editor).Assembly.GetType("UnityEditor.GUIView").GetMethod("MarkHotRegion", maxBindingFlags);
        static MethodInfo _mi_GUIClip_UnclipToWindow = typeof(GUI).Assembly.GetType("UnityEngine.GUIClip").GetMethod("UnclipToWindow", maxBindingFlags, null, new[] { typeof(Rect) }, null);









        #endregion



        public class ReorderableRow<Item> where Item : class
        {

            public void OnGUI(Rect rect)
            {
                void normalItem(int i)
                {
                    if (items[i] == droppedItem && animatingDroppedItem) return;

                    var centerX = GetItemCenterX_withGaps(i);
                    var centerY = itemsRect.height / 2;


                    var minX = centerX - GetItemWidth(items[i]) / 2;

                    if (minX < itemsRect.x) return;

                    lastItemX = minX;


                    var item = items[i];
                    var itemRect = Rect.zero.SetSize(GetItemWidth(item), itemsRect.height).SetMidPos(centerX, centerY);

                    ItemGUI(itemRect, items[i]);

                }
                void normalItems()
                {
                    for (int i = 0; i < items.Count; i++)
                        normalItem(i);

                }
                void draggedItem_()
                {
                    if (!draggingItem) return;


                    var centerX = curEvent.mousePosition.x + draggedItemHoldOffset.x;
                    var centerY = itemsRect.IsHovered() ? itemsRect.height / 2 : curEvent.mousePosition.y;

                    var itemRect = Rect.zero.SetSize(GetItemWidth(draggedItem), itemsRect.height).SetMidPos(centerX, centerY);


                    ItemGUI(itemRect, draggedItem);

                }
                void droppedItem_()
                {
                    if (!animatingDroppedItem) return;


                    var centerX = droppedItemX;
                    var centerY = itemsRect.height / 2;

                    var itemRect = Rect.zero.SetSize(GetItemWidth(droppedItem), itemsRect.height).SetMidPos(centerX, centerY);


                    ItemGUI(itemRect, droppedItem);

                }



                this.itemsRect = rect;

                MouseState();
                Dragging();
                Animations();

                normalItems();
                draggedItem_();
                droppedItem_();

            }

            Rect itemsRect;

            public bool repaintNeededAfterUndoRedo;

            public float lastItemX;










            void MouseState()
            {
                void down()
                {
                    if (!curEvent.isMouseDown) return;
                    if (!itemsRect.IsHovered()) return;

                    mousePressed = true;

                    mouseDownPosiion = curEvent.mousePosition;

                    var pressedItemIndex = GetItemIndex(mouseDownPosiion.x);

                    if (pressedItemIndex.IsInRangeOf(items))
                        pressedItem = items[pressedItemIndex];

                    doubleclickUnhandled = curEvent.clickCount == 2;

                    curEvent.Use();

                }
                void up()
                {
                    if (!curEvent.isMouseUp) return;

                    mousePressed = false;
                    pressedItem = null;

                }
                void hover()
                {
                    var hoveredItemIndex = GetItemIndex(curEvent.mousePosition.x);

                    mouseHoversItem = itemsRect.IsHovered() && hoveredItemIndex.IsInRangeOf(items);

                    if (mouseHoversItem)
                        lastHoveredItem = items[hoveredItemIndex];


                }

                down();
                up();
                hover();

            }

            public bool mouseHoversItem;
            public bool mousePressed;
            public bool doubleclickUnhandled;

            public Vector2 mouseDownPosiion;

            public Item pressedItem;
            public Item lastHoveredItem;





            void Dragging()
            {
                void initFromOutside()
                {
                    if (draggingItem) return;
                    if (!itemsRect.IsHovered()) return;
                    if (!curEvent.isDragUpdate) return;
                    if (DragAndDrop.objectReferences.FirstOrDefault() is not Object draggedObject) return;

                    if (CanCreateItemFrom == null) return;
                    if (!CanCreateItemFrom(draggedObject)) return;


                    draggedItem = CreateItem(draggedObject);

                    draggedItemHoldOffset = Vector2.zero;

                    draggingItem = true;
                    draggingItemFromInside = false;

                    animatingDroppedItem = false;


                }
                void initFromInside()
                {
                    if (draggingItem) return;
                    if (!mousePressed) return;
                    if ((curEvent.mousePosition - mouseDownPosiion).magnitude <= 2) return;
                    if (pressedItem == null) return;

                    var i = GetItemIndex(mouseDownPosiion.x);

                    if (i >= items.Count) return;
                    if (i < 0) return;


                    animatingDroppedItem = false;

                    draggingItem = true;
                    draggingItemFromInside = true;

                    draggedItem = items[i];
                    draggedItemHoldOffset = new Vector2(GetItemCenterX_withGaps(i) - mouseDownPosiion.x, itemsRect.center.y - mouseDownPosiion.y);

                    gaps[i] = GetItemWidth(draggedItem);


                    itemsHolderObject.RecordUndo();

                    items.Remove(draggedItem);

                }

                void acceptFromOutside()
                {
                    if (!draggingItem) return;
                    if (!curEvent.isDragPerform) return;
                    if (!itemsRect.IsHovered()) return;

                    DragAndDrop.AcceptDrag();
                    curEvent.Use();

                    itemsHolderObject.RecordUndo();

                    accept();

                    itemsHolderObject.Dirty();

                }
                void acceptFromInside()
                {
                    if (!draggingItem) return;
                    if (!curEvent.isMouseUp) return;
                    if (!itemsRect.IsHovered()) return;

                    curEvent.Use();
                    EditorGUIUtility.hotControl = 0;

                    DragAndDrop.PrepareStartDrag(); // fixes phantom dragged component indicator after reordering items

                    itemsHolderObject.RecordUndo();
                    itemsHolderObject.Dirty();

                    accept();

                }
                void accept()
                {
                    draggingItem = false;
                    draggingItemFromInside = false;
                    mousePressed = false;

                    items.AddAt(draggedItem, insertDraggedItemAtIndex);

                    gaps[insertDraggedItemAtIndex] -= GetItemWidth(draggedItem);
                    gaps.AddAt(0, insertDraggedItemAtIndex);

                    droppedItem = draggedItem;

                    droppedItemX = curEvent.mousePosition.x + draggedItemHoldOffset.x;
                    droppedItemXDerivative = 0;
                    animatingDroppedItem = true;

                    draggedItem = null;

                    EditorGUIUtility.hotControl = 0;

                    repaintNeededAfterUndoRedo = true;

                }

                void cancelFromOutside()
                {
                    if (!draggingItem) return;
                    if (draggingItemFromInside) return;
                    if (itemsRect.IsHovered()) return;

                    draggingItem = false;
                    mousePressed = false;

                }
                void cancelFromInsideAndDelete()
                {
                    if (!draggingItem) return;
                    if (!curEvent.isMouseUp) return;
                    if (itemsRect.IsHovered()) return;

                    draggingItem = false;

                    DragAndDrop.PrepareStartDrag(); // fixes phantom dragged component indicator after reordering items

                    itemsHolderObject.Dirty();

                    repaintNeededAfterUndoRedo = true;

                }

                void update()
                {
                    if (!draggingItem) return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (draggingItemFromInside) // otherwise it breaks vTabs dragndrop
                        EditorGUIUtility.hotControl = EditorGUIUtility.GetControlID(FocusType.Passive);



                    insertDraggedItemAtIndex = GetItemIndex(curEvent.mousePosition.x + draggedItemHoldOffset.x);

                }

                // void dropIntoItem()
                // {
                //     if (draggingItem) return;
                //     if (!itemsRect.IsHovered()) return;
                //     if (!DragAndDrop.objectReferences.Any()) return;
                //     if (!DragAndDrop.objectReferences.Any(r => r is not DefaultAsset)) return;
                //     if (!DragAndDrop.objectReferences.All(r => AssetDatabase.Contains(r))) return;

                //     if (!mouseHoversItem) return;
                //     if (lastHoveredItem.isDeleted) return;


                //     if (curEvent.isDragUpdate)
                //         DragAndDrop.visualMode = DragAndDropVisualMode.Move;


                //     if (!curEvent.isDragPerform) return;

                //     DragAndDrop.AcceptDrag();

                //     curEvent.Use();


                //     var oldPaths = DragAndDrop.objectReferences.Select(r => r.GetPath()).ToList();

                //     var newPaths = oldPaths.Select(r => lastHoveredItem.guid.ToPath().CombinePath(r.GetFilename(withExtension: true)))
                //                            .Select(r => AssetDatabase.GenerateUniqueAssetPath(r)).ToList();


                //     typeof(Undo).GetMethod("RegisterAssetsMoveUndo", maxBindingFlags).Invoke(null, new object[] { oldPaths.ToArray() });

                //     for (int i = 0; i < newPaths.Count; i++)
                //         AssetDatabase.MoveAsset(oldPaths[i], newPaths[i]);

                // }


                initFromOutside();
                initFromInside();

                acceptFromOutside();
                acceptFromInside();

                cancelFromOutside();
                cancelFromInsideAndDelete();

                update();

                // dropIntoItem();

            }

            public bool draggingItem;
            public bool draggingItemFromInside;

            int insertDraggedItemAtIndex;

            Vector2 draggedItemHoldOffset;

            public Item draggedItem;
            public Item droppedItem;





            void Animations()
            {
                if (!curEvent.isLayout) return;

                void gaps_()
                {
                    var makeSpaceForDraggedItem = draggingItem && itemsRect.IsHovered();

                    var lerpSpeed = 12;

                    for (int i = 0; i < gaps.Count; i++)
                        if (makeSpaceForDraggedItem && i == insertDraggedItemAtIndex)
                            gaps[i] = MathUtil.Lerp(gaps[i], GetItemWidth(draggedItem), lerpSpeed, editorDeltaTime);
                        else
                            gaps[i] = MathUtil.Lerp(gaps[i], 0, lerpSpeed, editorDeltaTime);



                    for (int i = 0; i < gaps.Count; i++)
                        if (gaps[i].Approx(0))
                            gaps[i] = 0;



                    animatingGaps = gaps.Any(r => r > .1f);


                }
                void droppedItem_()
                {
                    if (!animatingDroppedItem) return;

                    var lerpSpeed = 8;

                    var targX = GetItemCenterX_withoutGaps(items.IndexOf(droppedItem));

                    MathUtil.SmoothDamp(ref droppedItemX, targX, lerpSpeed, ref droppedItemXDerivative, editorDeltaTime);

                    if ((droppedItemX - targX).Abs() < .5f)
                        animatingDroppedItem = false;

                }
                void tooltip()
                {
                    if (!curEvent.isLayout) return;

                    if (!mouseHoversItem || lastHoveredItem != lastClickedItem)
                        hideTooltip = false;


                    var lerpSpeed = UnityEditorInternal.InternalEditorUtility.isApplicationActive ? 15 : 12321;

                    if (mouseHoversItem && !draggingItem && !hideTooltip)
                        MathUtil.SmoothDamp(ref tooltipOpacity, 1, lerpSpeed, ref tooltipOpacityDerivative, editorDeltaTime);
                    else
                        MathUtil.SmoothDamp(ref tooltipOpacity, 0, lerpSpeed, ref tooltipOpacityDerivative, editorDeltaTime);


                    if (tooltipOpacity > .99f)
                        tooltipOpacity = 1;

                    if (tooltipOpacity < .01f)
                        tooltipOpacity = 0;


                    animatingTooltip = tooltipOpacity != 0 && tooltipOpacity != 1;



                    // this animation needs to happen after MouseState() (because it relies on MouseHoversItem) but before gui

                }

                gaps_();
                droppedItem_();
                tooltip();

            }

            float droppedItemX;
            float droppedItemXDerivative;

            public bool animatingDroppedItem;
            public bool animatingGaps;
            public bool animatingItemMovement => draggingItem || animatingDroppedItem || animatingGaps;

            public bool animatingTooltip;
            public bool hideTooltip;
            public float tooltipOpacity;
            public float tooltipOpacityDerivative;
            public Item lastClickedItem;

            public List<float> gaps
            {
                get
                {
                    while (_gaps.Count < items.Count + 1) _gaps.Add(0);
                    while (_gaps.Count > items.Count + 1) _gaps.RemoveLast();

                    return _gaps;

                }
            }
            public List<float> _gaps = new();














            public List<Item> items;

            public ScriptableObject itemsHolderObject; // to dirty and record undo


            public System.Action<Rect, Item> ItemGUI;

            public System.Func<Object, Item> CreateItem;
            public System.Func<Object, bool> CanCreateItemFrom;

            public System.Func<float, int> GetItemIndex;
            public System.Func<Item, float> GetItemWidth;
            public System.Func<int, float> GetItemCenterX_withGaps;
            public System.Func<int, float> GetItemCenterX_withoutGaps;

        }


    }

}
#endif