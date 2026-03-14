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
    public class GetOneBehaviorTreeBytesInvoker : AInvokeHandler<BTLoader.GetOneBehaviorTreeBytes, ETTask<byte[]>>
    {
        public override async ETTask<byte[]> Handle(BTLoader.GetOneBehaviorTreeBytes args)
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
            string bytesFilePath = Path.Combine(BTLoader.ClientBehaviorTreeBytesDir, $"{treeName}.bytes");
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
            string assetFilePath = Path.Combine(BTLoader.BTAssetDir, $"{treeName}.asset").Replace("\\", "/");

            Assembly editorAssembly = GetAssembly("Unity.Editor");
            if (editorAssembly == null)
            {
                Log.Error("behavior tree editor assembly not found: Unity.Editor");
                return null;
            }

            Type assetType = editorAssembly.GetType("ET.BTAsset");
            Type exporterType = editorAssembly.GetType("ET.BTExporter");
            if (assetType == null || exporterType == null)
            {
                Log.Error("behavior tree editor types not found");
                return null;
            }

            UnityEngine.Object assetObject = AssetDatabase.LoadAssetAtPath(assetFilePath, assetType);
            if (assetObject == null)
            {
                Log.Error($"behavior tree asset not found: {assetFilePath}");
                return null;
            }

            MethodInfo buildPackageMethod = exporterType.GetMethod("BuildPackage", BindingFlags.Public | BindingFlags.Static);
            if (buildPackageMethod == null)
            {
                Log.Error("behavior tree exporter BuildPackage not found");
                return null;
            }

            object packageObject = buildPackageMethod.Invoke(null, new object[] { assetObject });
            if (packageObject is not BTPackage package)
            {
                Log.Error($"behavior tree asset build package failed: {assetFilePath}");
                return null;
            }

            return BTSerializer.Serialize(package);
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
