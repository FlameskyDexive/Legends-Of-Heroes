using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using YooAsset;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ET
{
    [Invoke]
    public class GetOneBehaviorTreeBytesInvoker : AInvokeHandler<BTBytesLoader.GetOneBehaviorTreeBytes, ETTask<byte[]>>
    {
        public override async ETTask<byte[]> Handle(BTBytesLoader.GetOneBehaviorTreeBytes args)
        {
            if (string.IsNullOrWhiteSpace(args.TreeName))
            {
                Log.Error("behavior tree name is empty");
                return null;
            }

            if (Define.IsEditor && Define.PlayMode == EPlayMode.EditorSimulateMode)
            {
                await ETTask.CompletedTask;
                return TryLoadEditorBytes(args.TreeName);
            }

            TextAsset textAsset = await ResourcesComponent.Instance.LoadAssetAsync<TextAsset>(args.TreeName);
            if (textAsset == null)
            {
                Log.Error($"behavior tree bytes asset not found: {args.TreeName}");
                return null;
            }

            return textAsset.bytes;
        }

        private static byte[] TryLoadEditorBytes(string treeName)
        {
            string bytesFilePath = Path.Combine(BTBytesLoader.ClientBehaviorTreeBytesDir, $"{treeName}.bytes");
            if (File.Exists(bytesFilePath))
            {
                return File.ReadAllBytes(bytesFilePath);
            }

#if UNITY_EDITOR
            return TryBuildBytesFromAsset(treeName);
#else
            return null;
#endif
        }

#if UNITY_EDITOR
        private static byte[] TryBuildBytesFromAsset(string treeName)
        {
            string assetFilePath = Path.Combine(BTBytesLoader.BTAssetDir, $"{treeName}.asset").Replace("\\", "/");

            Assembly editorAssembly = GetAssembly("Unity.Editor");
            Assembly modelAssembly = GetAssembly("Unity.Model");
            if (editorAssembly == null || modelAssembly == null)
            {
                Log.Error("behavior tree required assemblies not found: Unity.Editor or Unity.Model");
                return null;
            }

            Type assetType = editorAssembly.GetType("ET.BTAsset");
            Type exporterType = editorAssembly.GetType("ET.BTExporter");
            Type serializerType = modelAssembly.GetType("ET.BTSerializer");
            if (assetType == null || exporterType == null || serializerType == null)
            {
                Log.Error("behavior tree required types not found");
                return null;
            }

            UnityEngine.Object assetObject = AssetDatabase.LoadAssetAtPath(assetFilePath, assetType);
            if (assetObject == null)
            {
                Log.Error($"behavior tree asset not found: {assetFilePath}");
                return null;
            }

            MethodInfo buildPackageMethod = exporterType.GetMethod("BuildPackage", BindingFlags.Public | BindingFlags.Static);
            MethodInfo serializeMethod = serializerType.GetMethod("Serialize", BindingFlags.Public | BindingFlags.Static);
            if (buildPackageMethod == null || serializeMethod == null)
            {
                Log.Error("behavior tree serializer methods not found");
                return null;
            }

            object packageObject = buildPackageMethod.Invoke(null, new object[] { assetObject });
            if (packageObject == null)
            {
                Log.Error($"behavior tree asset build package failed: {assetFilePath}");
                return null;
            }

            return serializeMethod.Invoke(null, new[] { packageObject }) as byte[];
        }

        private static Assembly GetAssembly(string assemblyName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == assemblyName)
                {
                    return assembly;
                }
            }

            return null;
        }
#endif
    }
}
