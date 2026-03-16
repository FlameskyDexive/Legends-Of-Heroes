using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ET
{
    internal static class BTEditorRuntimeBridge
    {
        private const string ModelAssemblyName = "Unity.Model";

        private static Assembly modelAssembly;
        private static Type serializerType;
        private static bool ninoInitialized;

        public static object CreateInstance(string fullTypeName)
        {
            return Activator.CreateInstance(ResolveRuntimeType(fullTypeName));
        }

        public static Type ResolveRuntimeType(string fullTypeName)
        {
            Assembly assembly = GetModelAssembly();
            Type type = assembly?.GetType(fullTypeName);
            if (type == null)
            {
                throw new InvalidOperationException($"BehaviorTree runtime type not found: {fullTypeName}");
            }

            return type;
        }

        public static byte[] SerializePackage(object package)
        {
            EnsureNinoInitialized();
            serializerType ??= ResolveRuntimeType("ET.BTSerializer");
            MethodInfo method = serializerType.GetMethod("Serialize", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                throw new InvalidOperationException("BehaviorTree serializer Serialize method not found.");
            }

            try
            {
                return method.Invoke(null, new[] { package }) as byte[];
            }
            catch (TargetInvocationException exception) when (exception.InnerException != null)
            {
                throw exception.InnerException;
            }
        }

        public static object DeserializePackage(byte[] bytes)
        {
            EnsureNinoInitialized();
            serializerType ??= ResolveRuntimeType("ET.BTSerializer");
            MethodInfo method = serializerType.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                throw new InvalidOperationException("BehaviorTree serializer Deserialize method not found.");
            }

            try
            {
                return method.Invoke(null, new object[] { bytes });
            }
            catch (TargetInvocationException exception) when (exception.InnerException != null)
            {
                throw exception.InnerException;
            }
        }

        public static IList GetList(object target, string memberName)
        {
            object value = GetMemberValue(target, memberName);
            if (value is IList list)
            {
                return list;
            }

            throw new InvalidOperationException($"BehaviorTree list member not found: {target?.GetType().FullName}.{memberName}");
        }

        public static T GetValue<T>(object target, string memberName, T defaultValue = default)
        {
            object value = GetMemberValue(target, memberName);
            if (value == null)
            {
                return defaultValue;
            }

            return value is T matched ? matched : defaultValue;
        }

        public static void SetValue(object target, string memberName, object value)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Type type = target.GetType();
            FieldInfo field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }

            PropertyInfo property = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null && property.CanWrite)
            {
                property.SetValue(target, value);
                return;
            }

            throw new InvalidOperationException($"BehaviorTree member not found: {type.FullName}.{memberName}");
        }

        public static bool IsRuntimeNodeData(object node, string runtimeTypeName)
        {
            return node != null && string.Equals(node.GetType().Name, runtimeTypeName, StringComparison.Ordinal);
        }

        private static object GetMemberValue(object target, string memberName)
        {
            if (target == null)
            {
                return null;
            }

            Type type = target.GetType();
            FieldInfo field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                return field.GetValue(target);
            }

            PropertyInfo property = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null && property.CanRead)
            {
                return property.GetValue(target);
            }

            return null;
        }

        private static Assembly GetModelAssembly()
        {
            if (modelAssembly != null)
            {
                return modelAssembly;
            }

            modelAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == ModelAssemblyName);
            if (modelAssembly != null)
            {
                return modelAssembly;
            }

            string dllPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath) ?? string.Empty, "Library", "ScriptAssemblies", "Unity.Model.dll");
            if (System.IO.File.Exists(dllPath))
            {
                modelAssembly = Assembly.LoadFrom(dllPath);
            }

            return modelAssembly;
        }

        private static void EnsureNinoInitialized()
        {
            if (ninoInitialized)
            {
                return;
            }

            Assembly assembly = GetModelAssembly();
            if (assembly == null)
            {
                throw new InvalidOperationException($"BehaviorTree runtime assembly not found: {ModelAssemblyName}");
            }

            InvokeStaticInit(assembly, "Unity.Model.NinoGen.NinoBuiltInTypesRegistration");
            InvokeStaticInit(assembly, "Unity.Model.NinoGen.Serializer");
            InvokeStaticInit(assembly, "Unity.Model.NinoGen.Deserializer");
            InvokeStaticInit(assembly, "ET.EntitySerializeRegister");
            ninoInitialized = true;
        }

        private static void InvokeStaticInit(Assembly assembly, string fullTypeName)
        {
            Type type = assembly.GetType(fullTypeName);
            if (type == null)
            {
                throw new InvalidOperationException($"BehaviorTree init type not found: {fullTypeName}");
            }

            MethodInfo method = type.GetMethod("Init", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
            {
                throw new InvalidOperationException($"BehaviorTree init method not found: {fullTypeName}.Init");
            }

            try
            {
                method.Invoke(null, Array.Empty<object>());
            }
            catch (TargetInvocationException exception) when (exception.InnerException != null)
            {
                throw exception.InnerException;
            }
        }
    }
}
