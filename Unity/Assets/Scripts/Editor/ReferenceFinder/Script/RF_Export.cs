//#define RF_DEV

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReferenceFinder
{
    internal class RF_Export
    {
        // private static int processIndex;
        private const int maxThread = 5;

        //[MenuItem("Assets/RF/Tools/Merge Duplicates")]

        private static Dictionary<string, ProcessReplaceData> listReplace;
        private static HashSet<string> cacheSelection;


        // private static List<Thread> lstThreads;
        public static bool IsMergeProcessing { get; private set; }


        public static void ExportCSV(RF_Ref[] csvSource)
        {
            string result = RF_CSV.GetCSVRows(csvSource);
            if (result.Length > 0)
            {
                EditorGUIUtility.systemCopyBuffer = result;
                Debug.Log("[RF] CSV file content (" + csvSource.Length + " assets) copied to clipboard!");
            } else
                Debug.LogWarning("[RF] Nothing to export!");
        }

        #if RF_DEV
        [MenuItem("Assets/RF/Debug details", false, 19)]
        private static void DebugDetails()
        {
            var s = Selection.activeObject;
            if (s == null) return;

            var path = AssetDatabase.GetAssetPath(s);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Path: " + path);
                return;
            }
            
            var guid = AssetDatabase.AssetPathToGUID(path);
            var asset = RF_Cache.Api.Get(guid);
            if (asset == null)
            {
                Debug.LogWarning($"Asset is null??? {guid}");
                return;
            }
            
            Debug.Log(asset.DebugUseGUID());
        }
        #endif

        [MenuItem("Assets/RF/Toggle Ignore", false, 19)]
        private static void Ignore()
        {
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("RF cache not yet ready, please open Window > RF_Window and hit scan project!");
                return;
            }

            Object[] actives = Selection.objects;
            for (var i = 0; i < actives.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(actives[i]);
                if (path.Equals(RF_Cache.DEFAULT_CACHE_PATH)) continue;

                if (RF_Setting.IgnoreAsset.Contains(path))
                {
                    RF_Setting.RemoveIgnore(path);
                } else
                {
                    RF_Setting.AddIgnore(path);
                }
            }
        }

        [MenuItem("Assets/RF/Copy GUID", false, 20)]
        private static void CopyGUID()
        {
            EditorGUIUtility.systemCopyBuffer = AssetDatabase.AssetPathToGUID(
                AssetDatabase.GetAssetPath(Selection.activeObject)
            );
        }

        [MenuItem("Assets/RF/Export Selection", false, 21)]
        private static void ExportSelection()
        {
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("RF cache not yet ready, please open Window > RF_Window and hit scan project!");
                return;
            }

            RF_Unity.ExportSelection();
        }

        [MenuItem("Assets/RF/Select Dependencies (assets I use)", false, 22)]
        private static void SelectDependencies_wtme()
        {
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("RF cache not yet ready, please open Window > RF_Window and hit scan project!");
                return;
            }

            SelectDependencies(false);
        }

        [MenuItem("Assets/RF/Refresh")]
        public static void ForceRefreshSelection()
        {
            string[] guids = Selection.assetGUIDs;
            if (!RF_Cache.isReady) return; // cache not ready!

            for (var i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                if (guid == RF_Cache.CachePath) continue;

                if (!RF_Asset.IsValidGUID(guid)) continue;

                if (RF_Cache.Api.AssetMap.ContainsKey(guid))
                {
                    RF_Cache.Api.RefreshAsset(guid, true);
#if RF_DEBUG
				UnityEngine.Debug.Log("Changed : " + guids[i]);
#endif

                    continue;
                }

                RF_Cache.Api.AddAsset(guid);
            }

            RF_Cache.Api.Check4Work();
        }

        [MenuItem("Assets/RF/Select Dependencies included me", false, 23)]
        private static void SelectDependencies_wme()
        {
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("RF cache not yet ready, please open Window > RF_Window and hit scan project!");
                return;
            }

            SelectDependencies(true);
        }

        //[MenuItem("Assets/RF/Select")] 
        [MenuItem("Assets/RF/Select Used (assets used me)", false, 24)]
        private static void SelectUsed_wtme()
        {
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("RF cache not yet ready, please open Window > RF_Window and hit scan project!");
                return;
            }

            SelectUsed(false);
        }

        [MenuItem("Assets/RF/Select Used included me", false, 25)]
        private static void SelectUsed_wme()
        {
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("RF cache not yet ready, please open Window > RF_Window and hit scan project!");
                return;
            }

            SelectUsed(true);
        }

        [MenuItem("Assets/RF/Export Dependencies", false, 40)]
        private static void ExportDependencies()
        {
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("RF cache not yet ready, please open Window > RF_Window and hit scan project!");
                return;
            }

            List<Object> deps = GetSelectionDependencies();
            if (deps == null) return;

            Selection.objects = deps.ToArray();
            RF_Unity.ExportSelection();
        }

        [MenuItem("Assets/RF/Export Assets (no scripts)", false, 41)]
        private static void ExportAsset()
        {
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("RF cache not yet ready, please open Window > RF_Window and hit scan project!");
                return;
            }

            List<Object> list = GetSelectionDependencies();

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] is MonoScript) list.RemoveAt(i);
            }

            //Debug.Log(i + ":" + list[i] + ":" + list[i].GetType());
            Selection.objects = list.ToArray();
            RF_Unity.ExportSelection();
        }

        public static void MergeDuplicate(string guid_file)
        {
            // for (int i = 0; i < Selection.objects.Length; i++)
            // {
            //     Object item = Selection.objects[i];
            //     Debug.Log(item.name);
            // }
            //string guid_file = EditorGUIUtility.systemCopyBuffer;
            long toFileId = 0;
            string[] string_arr = guid_file.Split('/');
            if (string_arr.Length > 1) toFileId = long.Parse(string_arr[1]);
            string guid = string_arr[0];

            // var wat = new System.Diagnostics.Stopwatch();
            // wat.Start();
            //validate clipboard guid

            string gPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(gPath) || !gPath.StartsWith("Assets/"))
            {
                Debug.LogWarning("Invalid guid <" + guid + "> in clipboard, can not replace !");
                return;
            }

            string[] temp = RF_Unity.Selection_AssetGUIDs; //cheat refresh selection, DO NOT delete
            HashSet<string> guids_files = RF_Unity._Selection_AssetGUIDs;

            var realKey = "";
            foreach (string item in guids_files)
            {
                if (item.StartsWith(guid_file, StringComparison.Ordinal)) realKey = item;
            }
            if (string.IsNullOrEmpty(realKey))
            {
                Debug.LogWarning("Clipboard guid <" + guid +
                    "> not found in Selection, you may not intentionally replace selection assets by clipboard guid");

                //				foreach (var item in guids_files) {
                //					Debug.Log ("item: " + item);
                //				}
                return;
            }
            guids_files.Remove(realKey);
            cacheSelection = new HashSet<string>();
            foreach (string item in cacheSelection)
            {
                cacheSelection.Add(item);
            }
            if (guids_files.Count == 0)
            {
                Debug.LogWarning("No new asset selected to replace, must select all duplications to replace");
                return;
            }


            //check asset type, only replace same type
#if REPLACE_SAME_TYPE
            var type1 = AssetDatabase.GetMainAssetTypeAtPath(gPath);
            var importType1 = AssetImporter.GetAtPath(gPath);
#endif


            var assetList = new List<RF_Asset>();
            var lstFind = new List<string>();

            foreach (string item in guids_files)
            {
                string[] arr = item.Split('/');
                string g = arr[0];

#if REPLACE_SAME_TYPE
                var p2 = AssetDatabase.GUIDToAssetPath(g);
                var type2 = AssetDatabase.GetMainAssetTypeAtPath(p2);

                if(type1 != type2)
                {
                    Debug.LogWarning("Cannot replace asset: " + p2 + " becase difference type");
                    continue;
                }
                if(type1 == typeof(UnityEngine.Texture2D))
                {
                    var importType2 = AssetImporter.GetAtPath(p2) as TextureImporter;
                    var textureImportType1 = importType1 as TextureImporter;
                    if (importType2 == null || textureImportType1 == null)
                    {
                        Debug.LogWarning("Cannot replace asset: " + p2 + " becase difference type");
                        continue;
                    }
                    if(textureImportType1.textureType != importType2.textureType)
                    {
                        Debug.LogWarning("Cannot replace asset: " + p2 + " becase difference type");
                        continue;
                    }
                    if (textureImportType1.textureType == TextureImporterType.Sprite)
                    {
                        if (textureImportType1.spriteImportMode != importType2.spriteImportMode)
                        {
                            Debug.LogWarning("Cannot replace asset: " + p2 + " becase difference type");
                            continue;
                        }
                    }
                    //Debug.Log("import type " + mainImportType);
                }
                //Debug.Log("type: " + mainType);
#endif
                lstFind.Add(g);
            }

            if (lstFind.Count == 0)
            {
                Debug.LogWarning("No new asset selected to replace, must select all duplications to replace");
                return;
            }

            assetList = RF_Cache.Api.FindAssets(lstFind.ToArray(), false);

            //replace one by one
            listReplace = new Dictionary<string, ProcessReplaceData>();
            for (int i = assetList.Count - 1; i >= 0; i--)
            {
                Debug.Log("RF Replace GUID : " + assetList[i].guid + " ---> " + guid + " : " + assetList[i].UsedByMap.Count + " assets updated");

                string fromId = assetList[i].guid;

                List<RF_Asset> arr = assetList[i].UsedByMap.Values.ToList();
                for (var j = 0; j < arr.Count; j++)
                {
                    RF_Asset a = arr[j];
                    if (!listReplace.ContainsKey(a.assetPath)) listReplace.Add(a.assetPath, new ProcessReplaceData());

                    listReplace[a.assetPath].datas.Add(new ReplaceData
                    {
                        from = fromId,
                        to = guid,
                        asset = a,
                        toFileId = toFileId
                    });
                }
            }

            // foreach (KeyValuePair<string, ProcessReplaceData> item in listReplace)
            // {
            //     item.Value.processIndex = item.Value.datas.Count - 1;
            // }

            IsMergeProcessing = true;
            EditorApplication.update -= ApplicationUpdate;
            EditorApplication.update += ApplicationUpdate;

            // for (var i = assetList.Count - 1; i >= 0; i--)
            // {
            //     // Debug.Log("RF Replace GUID : " + assetList[i].guid + " ---> " + guid + " : " + assetList[i].UsedByMap.Count + " assets updated");
            //     var from = assetList[i].guid;

            //     var arr = assetList[i].UsedByMap.Values.ToList();
            //     for (var j = 0; j < arr.Count; j ++)
            //     {
            //         var a = arr[j];
            //         var result = a.ReplaceReference(from, guid);

            //         if (result && !dictAsset.ContainsKey(a.guid))
            //         {
            //             dictAsset.Add(a.guid, 1);
            //         }
            //     }
            // }
            // Debug.Log("Time replace guid " + wat.ElapsedMilliseconds);
            // wat = new System.Diagnostics.Stopwatch();
            // wat.Start();
            // var listRefresh = dictAsset.Keys.ToList();
            // for (var i = 0; i < listRefresh.Count; i++)
            // {
            //     RF_Cache.Api.RefreshAsset(listRefresh[i], true);
            // }

            // RF_Cache.Api.RefreshSelection();
            // RF_Cache.Api.Check4Usage();
            // AssetDatabase.Refresh();
            // Debug.Log("Time replace guid " + wat.ElapsedMilliseconds);
        }

        private static void ApplicationUpdate()
        {
            var isCompleted = true;
            foreach (KeyValuePair<string, ProcessReplaceData> item in listReplace)
            {
                if (item.Value.processed) continue;
                item.Value.processed = true;

                for (var i = 0; i < item.Value.datas.Count; i++)
                {
                    ReplaceData a = item.Value.datas[i];
                    a.isTerrian = a.asset.type == RF_AssetType.TERRAIN;
                    if (a.isTerrian)
                    {
                        a.terrainData = AssetDatabase.LoadAssetAtPath(a.asset.assetPath, typeof(Object)) as TerrainData;
                    }
                    a.isSucess = a.asset.ReplaceReference(a.from, a.to, a.toFileId, a.terrainData);

                    if (a.isTerrian)
                    {
                        a.terrainData = null;
                        RF_Unity.UnloadUnusedAssets();
                    }
                }

                isCompleted = false;
                break;
            }

            if (!isCompleted) return;
            foreach (KeyValuePair<string, ProcessReplaceData> item in listReplace)
            {
                List<ReplaceData> lst = item.Value.datas;
                for (var i = 0; i < lst.Count; i++)
                {
                    ReplaceData data = lst[i];
                    if (!data.isUpdated && data.isSucess)
                    {
                        data.isUpdated = true;
                        if (data.isTerrian)
                        {
                            EditorUtility.SetDirty(data.terrainData);
                            AssetDatabase.SaveAssets();
                            data.terrainData = null;
                            RF_Unity.UnloadUnusedAssets();
                        } else
                            try
                            {
                                AssetDatabase.ImportAsset(data.asset.assetPath, ImportAssetOptions.Default);
                            } catch (Exception e)
                            {
                                Debug.LogWarning(data.asset.assetPath + "\n" + e);
                            }
                    }
                }
            }
            var guidsRefreshed = new HashSet<string>();
            EditorApplication.update -= ApplicationUpdate;
            foreach (KeyValuePair<string, ProcessReplaceData> item in listReplace)
            {
                List<ReplaceData> lst = item.Value.datas;
                for (var i = 0; i < lst.Count; i++)
                {
                    ReplaceData data = lst[i];
                    if (data.isSucess && !guidsRefreshed.Contains(data.asset.guid))
                    {
                        guidsRefreshed.Add(data.asset.guid);
                        RF_Cache.Api.RefreshAsset(data.asset.guid, true);
                    }
                }
            }

            // lstThreads = null;
            listReplace = null;
            RF_Cache.Api.RefreshSelection();
            RF_Cache.Api.Check4Work();

            AssetDatabase.Refresh();
            IsMergeProcessing = false;
        }


        //[MenuItem("Assets/RF/Tools/Fix Model Import Material")]
        //public static void FixModelImportMaterial(){
        //	if (Selection.activeObject == null) return;
        //	CreatePrefabReplaceModel((GameObject)Selection.activeObject);
        //}

        //[MenuItem("GameObject/RF/Paste Materials", false, 10)]
        //public static void PasteMaterials(){
        //	if (Selection.activeObject == null) return;

        //	var r = Selection.activeGameObject.GetComponent<Renderer>();
        //	Undo.RecordObject(r, "Replace Materials");
        //	r.materials = model_materials;
        //	EditorUtility.SetDirty(r);
        //}

        //[MenuItem("GameObject/RF/Copy Materials", false, 10)]
        //public static void CopyMaterials(){
        //	if (Selection.activeObject == null) return;
        //	var r = Selection.activeGameObject.GetComponent<Renderer>();
        //	if (r == null) return;
        //	model_materials = r.sharedMaterials;
        //}

        //-------------------------- APIs ----------------------

        private static void SelectDependencies(bool includeMe)
        {
            List<RF_Asset> list = RF_Cache.Api.FindAssets(RF_Unity.Selection_AssetGUIDs, false);
            var dict = new Dictionary<string, Object>();

            if (includeMe) AddToDict(dict, list.ToArray());

            for (var i = 0; i < list.Count; i++)
            {
                AddToDict(dict, RF_Asset.FindUsage(list[i]).ToArray());
            }

            Selection.objects = dict.Values.ToArray();
        }

        private static void SelectUsed(bool includeMe)
        {
            List<RF_Asset> list = RF_Cache.Api.FindAssets(RF_Unity.Selection_AssetGUIDs, false);
            var dict = new Dictionary<string, Object>();

            if (includeMe) AddToDict(dict, list.ToArray());

            for (var i = 0; i < list.Count; i++)
            {
                AddToDict(dict, list[i].UsedByMap.Values.ToArray());
            }

            Selection.objects = dict.Values.ToArray();
        }


        //-------------------------- UTILS ---------------------

        internal static void AddToDict(Dictionary<string, Object> dict, params RF_Asset[] list)
        {
            for (var j = 0; j < list.Length; j++)
            {
                string guid = list[j].guid;
                if (!dict.ContainsKey(guid))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    dict.Add(guid, RF_Unity.LoadAssetAtPath<Object>(assetPath));
                }
            }
        }

        private static List<Object> GetSelectionDependencies()
        {
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("RF cache not yet ready, please open Window > RF_Window and hit scan project!");
                return null;
            }

            return RF_Cache.FindUsage(RF_Unity.Selection_AssetGUIDs).Select(
                guid =>
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    return RF_Unity.LoadAssetAtPath<Object>(assetPath);
                }
            ).ToList();
        }

        private class ProcessReplaceData
        {
            public readonly List<ReplaceData> datas = new List<ReplaceData>();
            public bool processed;
        }

        private class ReplaceData
        {
            public RF_Asset asset;
            public string from;
            public bool isSucess;
            public bool isTerrian;
            public bool isUpdated;
            public TerrainData terrainData;
            public string to;

            public long toFileId;
        }

        //	AssetDatabase.ImportAsset(oAssetPath, ImportAssetOptions.Default);
        //	importer.importMaterials = false;
        //	var importer = AssetImporter.GetAtPath(oAssetPath) as ModelImporter;
        //	var nModel = AssetDatabase.LoadAssetAtPath<GameObject>(oAssetPath);

        //	// Reimport model with importMaterial = false
        //	var extension = Path.GetExtension(oAssetPath);

        //	model_materials = model.GetComponent<Renderer>().sharedMaterials;
        //	var oGUID = AssetDatabase.AssetPathToGUID(oAssetPath);

        //	var oAssetPath = AssetDatabase.GetAssetPath(model);
        //	if (model == null) return;
        //{
        //static void CreatePrefabReplaceModel(GameObject model)

        //static Material[] model_materials;

        //	//create prefab from new model
        //	var prefabPath = oAssetPath.Replace(extension, ".prefab");
        //	var clone = (GameObject)Object.Instantiate(nModel);
        //	clone.GetComponent<Renderer>().sharedMaterials = model_materials;
        //	PrefabUtility.CreatePrefab(prefabPath, clone, ReplacePrefabOptions.ReplaceNameBased);
        //	AssetDatabase.SaveAssets();
        //	GameObject.DestroyImmediate(clone);
        //}
    }
}
