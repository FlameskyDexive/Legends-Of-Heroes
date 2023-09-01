using System.Collections.Generic;
using System.IO;
using System.Linq;
using HybridCLR.Editor;
using LitJson;
using UnityEditor;

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

            string aotDllsPath = "Assets/Resources/AotDlls.json";
            foreach (string aotDll in HybridCLRSettings.Instance.patchAOTAssemblies)
            {
                File.Copy(Path.Combine(fromDir, aotDll), Path.Combine(toDir, $"{aotDll}.bytes"), true);
            }

            string json = JsonMapper.ToJson(HybridCLRSettings.Instance.patchAOTAssemblies.ToList());
            File.WriteAllText(aotDllsPath, json);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}