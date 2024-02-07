using System;
using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor.Settings;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class HybridCLREditor
    {
        [MenuItem("HybridCLR/CopyAotDlls")]
        public static void CopyAotDll()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            string fromDir = Path.Combine(HybridCLRSettings.Instance.strippedAOTDllOutputRootDir, target.ToString());
            string toDir = "Assets/Bundles/AotDlls";
            if (Directory.Exists(toDir))
            {
                Directory.Delete(toDir, true);
            }
            Directory.CreateDirectory(toDir);
            AssetDatabase.Refresh();
            
            foreach (string aotDll in HybridCLRSettings.Instance.patchAOTAssemblies)
            {
                try
                {
                    Debug.Log($"copy aot dll:{aotDll}");
                    File.Copy(Path.Combine(fromDir, aotDll), Path.Combine(toDir, $"{aotDll}.bytes"), true);
                }
                catch (Exception e)
                {
                    Debug.LogError($"copy aotDll {aotDll} error:\n{e}");
                }
            }

            string aotDllsPath = "Assets/Bundles/Config/AotDllConfigs.json";
            File.WriteAllText(aotDllsPath, JsonMapper.ToJson(HybridCLRSettings.Instance.patchAOTAssemblies));

            // 设置ab包
            // AssetImporter assetImporter = AssetImporter.GetAtPath(toDir);
            // assetImporter.assetBundleName = "AotDlls.unity3d";
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}