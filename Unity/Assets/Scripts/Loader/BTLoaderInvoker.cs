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
            if (editorAssembly == null)
            {
                Log.Error("behavior tree required assembly not found: Unity.Editor");
                return null;
            }

            Type assetType = editorAssembly.GetType("ET.BTAsset");
            Type exporterType = editorAssembly.GetType("ET.BTExporter");
            if (assetType == null || exporterType == null)
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

            MethodInfo buildBytesMethod = exporterType.GetMethod("BuildBytes", BindingFlags.Public | BindingFlags.Static);
            if (buildBytesMethod == null)
            {
                Log.Error("behavior tree exporter BuildBytes method not found");
                return null;
            }

            object bytesObject = buildBytesMethod.Invoke(null, new object[] { assetObject });
            if (bytesObject == null)
            {
                Log.Error($"behavior tree asset build bytes failed: {assetFilePath}");
                return null;
            }

            return bytesObject as byte[];
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
