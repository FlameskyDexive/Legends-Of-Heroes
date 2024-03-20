using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using System.IO;
using ET;
using UnityEngine;

namespace ClientEditor
{
    class UIEditorController
    {
        [MenuItem("GameObject/SpawnEUICode", false, -2)]
        static public void CreateNewCode()
        {
            GameObject go = Selection.activeObject as GameObject;
            UICodeSpawner.SpawnEUICode(go);
        }

        
        [MenuItem("ET/EUICodeSpawn", false, 1007)]
        static public void CreateAllEUICode()
        {
            string folderPath = "Assets/Bundles/UI/Dlg/";
            CreateUICodeByPath(folderPath);
            folderPath = "Assets/Bundles/UI/Item/";
            CreateUICodeByPath(folderPath);
            folderPath = "Assets/Bundles/UI/Common/";
            CreateUICodeByPath(folderPath);
        }

        private static void CreateUICodeByPath(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                if (prefab != null)
                {
                    UICodeSpawner.SpawnEUICode(prefab);
                    Log.Info($"生成{prefab.name}的UI代码");
                }
            }
        }

        [MenuItem("Assets/AssetBundle/NameUIPrefab")]
        public static void NameAllUIPrefab()
        {
            string suffix = ".unity3d";
            UnityEngine.Object[] selectAsset = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
            for (int i = 0; i < selectAsset.Length; i++)
            {
                string prefabName = AssetDatabase.GetAssetPath(selectAsset[i]);
                //MARKER：判断是否是.prefab
                if (prefabName.EndsWith(".prefab"))
                {
                    Debug.Log(prefabName);
                    AssetImporter importer = AssetImporter.GetAtPath(prefabName);
                    importer.assetBundleName = selectAsset[i].name.ToLower() + suffix;
                }

            }
            AssetDatabase.Refresh();
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        [MenuItem("Assets/AssetBundle/ClearABName")]
        public static void ClearABName()
        {
            UnityEngine.Object[] selectAsset = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
            for (int i = 0; i < selectAsset.Length; i++)
            {
                string prefabName = AssetDatabase.GetAssetPath(selectAsset[i]);
                AssetImporter importer = AssetImporter.GetAtPath(prefabName);
                importer.assetBundleName = string.Empty;
                Debug.Log(prefabName);
            }
            AssetDatabase.Refresh();
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }
    }
}
