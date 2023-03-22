using System.Collections.Generic;
using HybridCLR;
using UnityEngine;

namespace ET
{
    public static class HybridCLRHelper
    {
        public static void Load()
        {
            // Dictionary<string, UnityEngine.Object> dictionary = AssetsBundleHelper.LoadBundle("aotdlls.unity3d");
            string json = Resources.Load<TextAsset>("AotDlls").text;
            string[] aotDlls = JsonUtility.FromJson<string[]>(json);
            foreach (var dll in aotDlls)
            {
                // byte[] bytes = (kv.Value as TextAsset).bytes;
                Log.Info($"load aot meta data dll: {dll}");
                byte[] bytes = MonoResComponent.Instance.LoadRawFile($"{dll}");
                RuntimeApi.LoadMetadataForAOTAssembly(bytes, HomologousImageMode.SuperSet);
            }
        }
    }
}