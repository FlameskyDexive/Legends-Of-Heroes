//#define RF_DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ReferenceFinder
{
    [InitializeOnLoad]
    internal class RF_CacheHelper : AssetPostprocessor
    {
        [NonSerialized] private static HashSet<string> scenes;
        [NonSerialized] private static HashSet<string> guidsIgnore;
        [NonSerialized] internal static bool inited = false;
        
        // [NonSerialized] private static bool _isInited = false;
        // internal static bool inited
        // {
        //     get => _isInited;
        //     set
        //     {
        //         _isInited = value;
        //         Debug.Log("Set Inited: " + value);
        //     }
        // }

        static RF_CacheHelper()
        {
            try
            {
                EditorApplication.update -= InitHelper;
                EditorApplication.update += InitHelper;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {

            RF_Cache.DelayCheck4Changes();

            //Debug.Log("OnPostProcessAllAssets : " + ":" + importedAssets.Length + ":" + deletedAssets.Length + ":" + movedAssets.Length + ":" + movedFromAssetPaths.Length);

            if (!RF_Cache.isReady)
            {
#if RF_DEBUG
			Debug.Log("Not ready, will refresh anyway !");
#endif
                return;
            }

            // RF not yet ready
            if (RF_Cache.Api.AssetMap == null) return;

            for (var i = 0; i < importedAssets.Length; i++)
            {
                if (importedAssets[i] == RF_Cache.CachePath) continue;

                string guid = AssetDatabase.AssetPathToGUID(importedAssets[i]);
                if (!RF_Asset.IsValidGUID(guid)) continue;

                if (RF_Cache.Api.AssetMap.ContainsKey(guid))
                {
                    RF_Cache.Api.RefreshAsset(guid, true);

#if RF_DEBUG
				Debug.Log("Changed : " + importedAssets[i]);
#endif

                    continue;
                }

                RF_Cache.Api.AddAsset(guid);
#if RF_DEBUG
			Debug.Log("New : " + importedAssets[i]);
#endif
            }

            for (var i = 0; i < deletedAssets.Length; i++)
            {
                string guid = AssetDatabase.AssetPathToGUID(deletedAssets[i]);
                RF_Cache.Api.RemoveAsset(guid);

#if RF_DEBUG
			Debug.Log("Deleted : " + deletedAssets[i]);
#endif
            }

            for (var i = 0; i < movedAssets.Length; i++)
            {
                string guid = AssetDatabase.AssetPathToGUID(movedAssets[i]);
                RF_Asset asset = RF_Cache.Api.Get(guid);
                if (asset != null) asset.MarkAsDirty();
            }

#if RF_DEBUG
		Debug.Log("Changes :: " + importedAssets.Length + ":" + RF_Cache.Api.workCount);
#endif

            RF_Cache.Api.Check4Work();
        }
        
        internal static void InitHelper()
        {
            if (RF_Unity.isEditorCompiling || RF_Unity.isEditorUpdating) return;
            if (!RF_Cache.isReady) return;
            EditorApplication.update -= InitHelper;
            
            inited = true;
            InitListScene();
            InitIgnore();

#if UNITY_2018_1_OR_NEWER
            EditorBuildSettings.sceneListChanged -= InitListScene;
            EditorBuildSettings.sceneListChanged += InitListScene;
#endif

            #if UNITY_2022_1_OR_NEWER
            EditorApplication.projectWindowItemInstanceOnGUI -= OnGUIProjectInstance;
            EditorApplication.projectWindowItemInstanceOnGUI += OnGUIProjectInstance;
            #else
            EditorApplication.projectWindowItemOnGUI -= OnGUIProjectItem;
            EditorApplication.projectWindowItemOnGUI += OnGUIProjectItem;
            #endif

            InitIgnore();
            // force repaint all project panels
            EditorApplication.RepaintProjectWindow();
        }
        
        public static void InitIgnore()
        {
            guidsIgnore = new HashSet<string>();
            foreach (string item in RF_Setting.IgnoreAsset)
            {
                string guid = AssetDatabase.AssetPathToGUID(item);
                guidsIgnore.Add(guid);
            }
            
            // Debug.Log($"Init Ignore: {guidsIgnore.Count} items");
        }

        private static void InitListScene()
        {
            scenes = new HashSet<string>();

            // string[] scenes = new string[sceneCount];
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                string sce = AssetDatabase.AssetPathToGUID(scene.path);
                scenes.Add(sce);
            }
        }

        private static string lastGUID;
        private static void OnGUIProjectInstance(int instanceID, Rect selectionRect)
        {
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(instanceID, out string guid, out long localId)) return;

            bool isMainAsset = guid != lastGUID;
            lastGUID = guid;

            if (isMainAsset)
            {
                DrawProjectItem(guid, selectionRect);
                return;
            }
            
            if (!RF_Cache.Api.setting.showSubAssetFileId) return;
            var rect2 = selectionRect;
            var label = new GUIContent(localId.ToString());
            rect2.xMin = rect2.xMax - EditorStyles.miniLabel.CalcSize(label).x;

            var c = GUI.color;
            GUI.color = new Color(.5f, .5f, .5f, 0.5f);
            GUI.Label(rect2, label, EditorStyles.miniLabel);
            GUI.color = c;
        }

        private static void OnGUIProjectItem(string guid, Rect rect)
        {
            bool isMainAsset = guid != lastGUID;
            lastGUID = guid;
            if (isMainAsset) DrawProjectItem(guid, rect);
        }

        private static void DrawProjectItem(string guid, Rect rect)
        {
            var r = new Rect(rect.x, rect.y, 1f, 16f);
            if (scenes.Contains(guid))
                EditorGUI.DrawRect(r, GUI2.Theme(new Color32(72, 150, 191, 255), Color.blue));
            else if (guidsIgnore.Contains(guid))
            {
                var ignoreRect = new Rect(rect.x + 3f, rect.y + 6f, 2f, 2f);
                EditorGUI.DrawRect(ignoreRect, GUI2.darkRed);
            }

            if (!RF_Cache.isReady) return; // not ready
            if (!RF_Setting.ShowReferenceCount) return;

            RF_Cache api = RF_Cache.Api;
            if (RF_Cache.Api.AssetMap == null) RF_Cache.Api.Check4Changes(false);
            if (!api.AssetMap.TryGetValue(guid, out RF_Asset item)) return;

            if (item == null || item.UsedByMap == null) return;

            if (item.UsedByMap.Count > 0)
            {
                var content = RF_GUIContent.FromString(item.UsedByMap.Count.ToString());
                r.width = 0f;
                r.xMin -= 100f;
                GUI.Label(r, content, GUI2.miniLabelAlignRight);
            } else if (item.forcedIncludedInBuild)
            {
                var c = GUI.color;
                GUI.color = c.Alpha(0.2f);
                var content = RF_GUIContent.FromString("+");
                r.width = 0f;
                r.xMin -= 100f;
                GUI.Label(r, content, GUI2.miniLabelAlignRight);
                GUI.color = c;
            }
        }
    }

    [Serializable] internal class RF_SettingExt
    {
        public static bool disable
        {
            get => inst.internalDisabled;

            set => inst.internalDisabled = value;
        }
        
        private const string path = "Library/RF/fr2.cfg";
        private static RF_SettingExt inst;
        
        static RF_SettingExt()
        {
            
            inst = new RF_SettingExt();
            if (!File.Exists(path)) return;

            try
            {
                string content = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(content, inst);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                //nothing
            }
        }

        static void DelaySave()
        {
            EditorApplication.update -= DelaySave;
            
            try
            {
                Directory.CreateDirectory("Library/RF/");
                File.WriteAllText(path, JsonUtility.ToJson(inst));
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                //nothing
            }
        }
        
        [SerializeField] private bool _disableInPlayMode = true;
        [SerializeField] private bool _disabled;
        
        private bool internalDisabled
        {
            get => _disabled || (_disableInPlayMode && EditorApplication.isPlayingOrWillChangePlaymode);
            set
            {
                ref bool disableRef = ref _disabled;
                if (EditorApplication.isPlayingOrWillChangePlaymode) disableRef = ref _disableInPlayMode;
                
                if (disableRef == value) return;
                disableRef = value;
                
                if (value) // disable at runtime: only disable `disableInPlayMode`
                {	
                    // ready = false;
                    // EditorApplication.update -= AsyncProcess;
                }
                else // enable at runtime: enable all
                {
                    _disabled = false;
                    // Api.Check4Changes(false);
                }
                
                EditorApplication.update -= DelaySave;
                EditorApplication.update += DelaySave;
            }
        }
    }
    
    
    

    [Serializable]
    internal class RF_Setting
    {
        private static RF_Setting d;

        [NonSerialized] private static HashSet<string> _hashIgnore;

        //		private static Dictionary<string, List<string>> _IgnoreFiltered;
        public static Action OnIgnoreChange;

        public bool alternateColor = true;
        public int excludeTypes; //32-bit type Mask

        public List<string> listIgnore = new List<string>();
        public bool pingRow = true;
        public bool referenceCount = true;
        public bool showPackageAsset = true;
        public bool showSubAssetFileId = false;

        public bool showFileSize;
        public bool displayFileSize = true;
        public bool displayAtlasName;
        public bool displayAssetBundleName;

        public bool showUsedByClassed = true;
        public int treeIndent = 10;

        public Color32 rowColor = new Color32(0, 0, 0, 12);

        // public Color32 ScanColor = new Color32(0, 204, 102, 255);
        public Color SelectedColor = new Color(0, 0f, 1f, 0.25f);


        //public bool scanScripts		= false;



        /*
		Doesn't have a settings option - I will include one in next update
		
		2. Hide the reference number - Should be in the setting above so will be coming next
		3. Cache file path should be configurable - coming next in the setting
		4. Disable / Selectable color in alternative rows - coming next in the setting panel
		5. Applied filters aren't saved - Should be fixed in next update too
		6. Hide Selection part - should be com as an option so you can quickly toggle it on or off
		7. Click whole line to ping - coming next by default and can adjustable in the setting panel
		
		*/

        internal static RF_Setting s => RF_Cache.Api ? RF_Cache.Api.setting : d ?? (d = new RF_Setting());

        public static bool ShowUsedByClassed => s.showUsedByClassed;

        public static bool ShowFileSize => s.showFileSize;

        public static int TreeIndent
        {
            get => s.treeIndent;
            set
            {
                if (s.treeIndent == value) return;

                s.treeIndent = value;
                setDirty();
            }
        }

        public static bool ShowReferenceCount
        {
            get => s.referenceCount;
            set
            {
                if (s.referenceCount == value) return;

                s.referenceCount = value;
                setDirty();
            }
        }
        public static bool AlternateRowColor
        {
            get => s.alternateColor;
            set
            {
                if (s.alternateColor == value) return;

                s.alternateColor = value;
                setDirty();
            }
        }

        public static Color32 RowColor
        {
            get => s.rowColor;
            set
            {
                if (s.rowColor.Equals(value)) return;

                s.rowColor = value;
                setDirty();
            }
        }

        public static bool PingRow
        {
            get => s.pingRow;
            set
            {
                if (s.pingRow == value) return;

                s.pingRow = value;
                setDirty();
            }
        }

        public static HashSet<string> IgnoreAsset
        {
            get
            {
                if (_hashIgnore != null) return _hashIgnore;
                _hashIgnore = new HashSet<string>();
                if (s?.listIgnore == null) return _hashIgnore;
                
                for (var i = 0; i < s.listIgnore.Count; i++)
                {
                    _hashIgnore.Add(s.listIgnore[i]);
                }

                return _hashIgnore;
            }
        }

        //		public static Dictionary<string, List<string>> IgnoreFiltered
        //		{
        //			get
        //			{
        //				if (_IgnoreFiltered == null)
        //				{
        //					initIgnoreFiltered();
        //				}
        //
        //				return _IgnoreFiltered;
        //			}
        //		}

        //static public bool ScanScripts
        //{
        //	get  { return s.scanScripts; }
        //	set  {
        //		if (s.scanScripts == value) return;
        //		s.scanScripts = value; setDirty();
        //	}
        //}

        // public static RF_RefDrawer.Mode GroupMode
        // {
        //     get => s.groupMode;
        //     set
        //     {
        //         if (s.groupMode.Equals(value)) return;
        //
        //         s.groupMode = value;
        //         setDirty();
        //     }
        // }
        //
        // public static RF_RefDrawer.Sort SortMode
        // {
        //     get => s.sortMode;
        //     set
        //     {
        //         if (s.sortMode.Equals(value)) return;
        //
        //         s.sortMode = value;
        //         setDirty();
        //     }
        // }

        public static bool HasTypeExcluded => s.excludeTypes != 0;

        private static void setDirty()
        {
            if (RF_Cache.Api != null) EditorUtility.SetDirty(RF_Cache.Api);
        }

        //		private static void initIgnoreFiltered()
        //		{
        //			RF_Asset.ignoreTS = Time.realtimeSinceStartup;
        //
        //			_IgnoreFiltered = new Dictionary<string, List<string>>();
        //			var lst = new List<string>(s.listIgnore);
        //			lst = lst.OrderBy(x => x.Length).ToList();
        //			int count = lst.Count;
        //			for (var i = 0; i < count; i++)
        //			{
        //				string str = lst[i];
        //				_IgnoreFiltered.Add(str, new List<string> {str});
        //				for (int j = count - 1; j > i; j--)
        //				{
        //					if (lst[j].StartsWith(str))
        //					{
        //						_IgnoreFiltered[str].Add(lst[j]);
        //						lst.RemoveAt(j);
        //						count--;
        //					}
        //				}
        //			}
        //		}

        public static void AddIgnore(string path)
        {
            if (string.IsNullOrEmpty(path) || IgnoreAsset.Contains(path) || path == "Assets") return;

            s.listIgnore.Add(path);
            _hashIgnore.Add(path);
            AssetType.SetDirtyIgnore();
            RF_CacheHelper.InitIgnore();

            //initIgnoreFiltered();

            RF_Asset.ignoreTS = Time.realtimeSinceStartup;
            if (OnIgnoreChange != null) OnIgnoreChange();
        }


        public static void RemoveIgnore(string path)
        {
            if (!IgnoreAsset.Contains(path)) return;

            _hashIgnore.Remove(path);
            s.listIgnore.Remove(path);
            AssetType.SetDirtyIgnore();
            RF_CacheHelper.InitIgnore();

            //initIgnoreFiltered();

            RF_Asset.ignoreTS = Time.realtimeSinceStartup;
            if (OnIgnoreChange != null) OnIgnoreChange();
        }

        public static bool IsTypeExcluded(int type)
        {
            return ((s.excludeTypes >> type) & 1) != 0;
        }

        public static void ToggleTypeExclude(int type)
        {
            bool v = ((s.excludeTypes >> type) & 1) != 0;
            if (v)
            {
                s.excludeTypes &= ~(1 << type);
            } else
            {
                s.excludeTypes |= 1 << type;
            }

            setDirty();
        }

        public static int GetExcludeType()
        {
            return s.excludeTypes;
        }

        public static bool IsIncludeAllType()
        {
            // Debug.Log ((AssetType.FILTERS.Length & s.excludeTypes) + "  " + Mathf.Pow(2, AssetType.FILTERS.Length) ); 
            return s.excludeTypes == 0 || Mathf.Abs(s.excludeTypes) == Mathf.Pow(2, AssetType.FILTERS.Length);
        }

        public static void ExcludeAllType()
        {
            s.excludeTypes = -1;
        }

        public static void IncludeAllType()
        {
            s.excludeTypes = 0;
        }

        public void DrawSettings()
        {
            // if (RF_Unity.DrawToggle(ref pingRow, "Full Row click to Ping")) setDirty();

            GUILayout.BeginHorizontal();
            {
                if (RF_Unity.DrawToggle(ref alternateColor, "Alternate Odd & Even Row Color"))
                {
                    setDirty();
                    RF_Unity.RepaintRFWindows();
                }

                EditorGUI.BeginDisabledGroup(!alternateColor);
                {
                    Color c = EditorGUILayout.ColorField(rowColor);
                    if (!c.Equals(rowColor))
                    {
                        rowColor = c;
                        setDirty();
                        RF_Unity.RepaintRFWindows();
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();

            if (RF_Unity.DrawToggle(ref referenceCount, "Show Usage Count in Project panel"))
            {
                setDirty();
                RF_Unity.RepaintProjectWindows();
            }
            
            if (RF_Unity.DrawToggle(ref showSubAssetFileId, "Show SubAsset FileId"))
            {
                setDirty();
                RF_Unity.RepaintRFWindows();
            }

            if (RF_Unity.DrawToggle(ref showUsedByClassed, "Show Asset Type in use"))
            {
                setDirty();
                RF_Unity.RepaintRFWindows();
            }
            
            if (RF_Unity.DrawToggle(ref showPackageAsset, "Show Asset in Packages"))
            {
                setDirty();
                RF_Unity.RepaintRFWindows();
            }
        }
    }

    internal class RF_Cache : ScriptableObject
    {
        internal const string DEFAULT_CACHE_PATH = "Assets/Scripts/Editor/ReferenceFinder/RF_Cache.asset";
        internal const string CACHE_VERSION = "2.5";

        internal static int cacheStamp;
        internal static Action onReady;

        internal static bool _triedToLoadCache;
        internal static RF_Cache _cache;

        internal static string _cacheGUID;
        internal static string _cachePath;
        public static int priority = 5;
        
        //internal List<RF_DuplicateInfo> ScanDuplication(){
        //	if (AssetMap == null) Check4Changes(false);

        //	var dict = new Dictionary<string, RF_DuplicateInfo>();
        //	foreach (var item in AssetMap){
        //		if (item.Value.IsMissing || item.Value.IsFolder) continue;
        //		var hash = item.Value.GetFileInfoHash();
        //		RF_DuplicateInfo info;

        //		if (!dict.TryGetValue(hash, out info)){
        //			info = new RF_DuplicateInfo(hash, item.Value.fileSize);
        //			dict.Add(hash, info);
        //		}

        //		info.assets.Add(item.Value);
        //	}

        //	var result = new List<RF_DuplicateInfo>();
        //	foreach (var item in dict){
        //		if (item.Value.assets.Count > 1){
        //			result.Add(item.Value);
        //		}
        //	}

        //	result.Sort((item1, item2)=>{
        //		return item2.fileSize.CompareTo(item1.fileSize);
        //	});

        //	return result;
        //}

        private static readonly HashSet<string> SPECIAL_USE_ASSETS = new HashSet<string>
        {
            "Assets/link.xml", // this file used to control build/link process do not remove
            "Assets/csc.rsp",
            "Assets/mcs.rsp",
            "Assets/GoogleService-Info.plist",
            "Assets/google-services.json"
        };

        private static readonly HashSet<string> SPECIAL_EXTENSIONS = new HashSet<string>
        {
            ".asmdef",
            ".cginc",
            ".cs",
            ".dll",
            ".mdb",
            ".pdb",
            ".rsp",
            ".md",
            ".winmd",
            ".xml",
            ".XML",
            ".tsv",
            ".csv",
            ".json",
            ".pdf",
            ".txt",
            ".giparams",
            ".wlt",
            ".preset",
            ".exr",
            ".aar",
            ".srcaar",
            ".pom",
            ".bin",
            ".html",
            ".chm",
            ".data",
            ".jsp",
            ".unitypackage",
        };

        [SerializeField] private bool _autoRefresh;
        [SerializeField] private string _curCacheVersion;
        
        [SerializeField] public List<RF_Asset> AssetList;
        [SerializeField] internal RF_Setting setting = new RF_Setting();

        // ----------------------------------- INSTANCE -------------------------------------

        [SerializeField] public int timeStamp;
        [NonSerialized] internal Dictionary<string, RF_Asset> AssetMap;


        private int frameSkipped;
        [NonSerialized] internal List<RF_Asset> queueLoadContent;


        internal bool ready;
        [NonSerialized] internal int workCount;

        internal static string CacheGUID
        {
            get
            {
                if (!string.IsNullOrEmpty(_cacheGUID)) return _cacheGUID;

                if (_cache != null)
                {
                    _cachePath = AssetDatabase.GetAssetPath(_cache);
                    _cacheGUID = AssetDatabase.AssetPathToGUID(_cachePath);
                    return _cacheGUID;
                }

                return null;
            }
        }

        internal static string CachePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_cachePath)) return _cachePath;

                if (_cache != null)
                {
                    _cachePath = AssetDatabase.GetAssetPath(_cache);
                    return _cachePath;
                }

                return null;
            }
        }

        public bool Dirty { get; private set; }
        internal static RF_Cache Api
        {
            get
            {
                if (_cache != null) return _cache;
                if (!_triedToLoadCache) TryLoadCache();
                return _cache;
            }
        }
        
        internal static bool isReady
        {
            get
            {
                if (RF_SettingExt.disable) return false;
                if (!_triedToLoadCache) TryLoadCache();
                return (_cache != null) && _cache.ready;
            }
        }

        internal static bool hasCache
        {
            get
            {
                if (!_triedToLoadCache) TryLoadCache();

                return _cache != null;
            }
        }

        internal float progress
        {
            get
            {
                int n = workCount - queueLoadContent.Count;
                return workCount == 0 ? 1 : n / (float)workCount;
            }
        }

        private void OnEnable()
        {
#if RF_DEBUG
		Debug.Log("OnEnabled : " + _cache);
#endif
            if (_cache == null) _cache = this;

            Check4Changes(false);
        }


        // public static void DrawPriorityGUI()
        // {
        //     float w = EditorGUIUtility.labelWidth;
        //     EditorGUIUtility.labelWidth = 120f;
        //     priority = EditorGUILayout.IntSlider("  Scan Priority", priority, 0, 5);
        //     EditorGUIUtility.labelWidth = w;
        // }

        public static bool CheckSameVersion()
        {
            // Debug.Log((_cache == null) + " " + _cache._curCacheVersion );
            if (_cache == null) return false;

            return _cache._curCacheVersion == CACHE_VERSION;
        }

        public void makeDirty()
        {
            Dirty = true;
        }

        private static void FoundCache(bool savePrefs, bool writeFile)
        {
            //Debug.LogWarning("Found Cache!");

            _cachePath = AssetDatabase.GetAssetPath(_cache);
            _cache.ReadFromCache();
            _cache.Check4Changes(false);
            _cacheGUID = AssetDatabase.AssetPathToGUID(_cachePath);

            if (savePrefs) EditorPrefs.SetString("fr2_cache.guid", _cacheGUID);

            if (writeFile) File.WriteAllText("Library/fr2_cache.guid", _cacheGUID);
        }

        private static bool RestoreCacheFromGUID(string guid, bool savePrefs, bool writeFile)
        {
            if (string.IsNullOrEmpty(guid)) return false;

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) return false;

            return RestoreCacheFromPath(path, savePrefs, writeFile);
        }

        private static bool RestoreCacheFromPath(string path, bool savePrefs, bool writeFile)
        {
            if (string.IsNullOrEmpty(path)) return false;

            _cache = RF_Unity.LoadAssetAtPath<RF_Cache>(path);
            if (_cache != null) FoundCache(savePrefs, writeFile);

            return _cache != null;
        }

        private static void TryLoadCache()
        {
            _triedToLoadCache = true;

            if (RestoreCacheFromPath(DEFAULT_CACHE_PATH, false, false)) return;

            // Check EditorPrefs
            string pref = EditorPrefs.GetString("fr2_cache.guid", string.Empty);
            if (RestoreCacheFromGUID(pref, false, false)) return;

            // Read GUID from File
            if (File.Exists("Library/fr2_cache.guid"))
            {
                if (RestoreCacheFromGUID(File.ReadAllText("Library/fr2_cache.guid"), true, false))
                {
                    return;
                }
            }

            // Search whole project
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            for (var i = 0; i < allAssets.Length; i++)
            {
                if (allAssets[i].EndsWith("/RF_Cache.asset", StringComparison.Ordinal))
                {
                    RestoreCacheFromPath(allAssets[i], true, true);
                    break;
                }
            }
        }

        internal static void DeleteCache()
        {
            if (_cache == null) return;

            try
            {
                if (!string.IsNullOrEmpty(_cachePath)) AssetDatabase.DeleteAsset(_cachePath);
            } catch
            { // ignored
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static void CreateCache()
        {
            _cache = CreateInstance<RF_Cache>();
            _cache._curCacheVersion = CACHE_VERSION;
            string path = Application.dataPath + DEFAULT_CACHE_PATH
                .Substring(0, DEFAULT_CACHE_PATH.LastIndexOf('/') + 1).Replace("Assets", string.Empty);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            AssetDatabase.CreateAsset(_cache, DEFAULT_CACHE_PATH);
            EditorUtility.SetDirty(_cache);

            FoundCache(true, true);
            DelayCheck4Changes();
        }

        internal static List<string> FindUsage(string[] listGUIDs)
        {
            if (!isReady) return null;

            List<RF_Asset> refs = Api.FindAssets(listGUIDs, true);

            for (var i = 0; i < refs.Count; i++)
            {
                List<RF_Asset> tmp = RF_Asset.FindUsage(refs[i]);

                for (var j = 0; j < tmp.Count; j++)
                {
                    RF_Asset itm = tmp[j];
                    if (refs.Contains(itm)) continue;

                    refs.Add(itm);
                }
            }

            return refs.Select(item => item.guid).ToList();
        }

        internal void ReadFromCache()
        {
            if (RF_SettingExt.disable)
            {
                Debug.LogWarning("Something wrong??? RF is disabled!");
            }
            
            if (AssetList == null) AssetList = new List<RF_Asset>();

            RF_Unity.Clear(ref queueLoadContent);
            RF_Unity.Clear(ref AssetMap);

            for (var i = 0; i < AssetList.Count; i++)
            {
                RF_Asset item = AssetList[i];
                item.state = RF_AssetState.CACHE;

                string path = AssetDatabase.GUIDToAssetPath(item.guid);
                if (string.IsNullOrEmpty(path))
                {
                    item.type = RF_AssetType.UNKNOWN; // to make sure if GUIDs being reused for a different kind of asset
                    item.state = RF_AssetState.MISSING;
                    AssetMap.Add(item.guid, item);
                    continue;
                }

                if (AssetMap.ContainsKey(item.guid))
                {
#if RF_DEBUG
					Debug.LogWarning("Something wrong, cache found twice <" + item.guid + ">");
#endif
                    continue;
                }

                AssetMap.Add(item.guid, item);
            }
        }

        internal void ReadFromProject(bool force)
        {
            if (RF_SettingExt.disable)
            {
                Debug.LogWarning("Something wrong??? RF is disabled!");
            }
            
            if (AssetMap == null || AssetMap.Count == 0) ReadFromCache();
            foreach (var b in RF_Asset.BUILT_IN_ASSETS)
            {
                if (AssetMap.ContainsKey(b)) continue;
                var asset = new RF_Asset(b);
                AssetMap.Add(b, asset);
                AssetList.Add(asset);
            }
            
            string[] paths = AssetDatabase.GetAllAssetPaths();
            cacheStamp++;
            workCount = 0;
            if (queueLoadContent != null) queueLoadContent.Clear();

            // Check for new assets
            foreach (string p in paths)
            {
                bool isValid = RF_Unity.StringStartsWith(p, "Assets/", "Packages/", "Library/", "ProjectSettings/");

                if (!isValid)
                {
#if RF_DEBUG
					Debug.LogWarning("Ignore asset: " + p);
#endif
                    continue;
                }

                string guid = AssetDatabase.AssetPathToGUID(p);
                if (!RF_Asset.IsValidGUID(guid)) continue;

                RF_Asset asset;
                if (!AssetMap.TryGetValue(guid, out asset))
                    AddAsset(guid);
                else
                {
                    asset.refreshStamp = cacheStamp; // mark this asset so it won't be deleted
                    if (!asset.isDirty && !force) continue;

                    if (force) asset.MarkAsDirty(true, true);

                    workCount++;
                    queueLoadContent.Add(asset);
                }
            }

            // Check for deleted assets
            for (int i = AssetList.Count - 1; i >= 0; i--)
            {
                if (AssetList[i].refreshStamp != cacheStamp) RemoveAsset(AssetList[i]);
            }
        }

        [NonSerialized] internal static int delayCounter;
        internal static void DelayCheck4Changes()
        {
            EditorApplication.update -= Check;
            EditorApplication.update += Check;
        }

        private static void Check()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || RF_SettingExt.disable)
            {
                delayCounter = 100;
                return;
            }
            
            if (Api == null) return;
            if (delayCounter-- > 0) return;
            EditorApplication.update -= Check;
            Api.Check4Changes(false);
        }
        
        internal void Check4Changes(bool force)
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || RF_SettingExt.disable)
            {
                DelayCheck4Changes();
                return;
            }
            
            ready = false;
            ReadFromProject(force);

#if RF_DEBUG
		Debug.Log("After checking :: WorkCount :: " + workCount + ":" + AssetMap.Count + ":" + AssetList.Count);
#endif
            Check4Work();
        }

        internal void RefreshAsset(string guid, bool force)
        {

            if (!AssetMap.TryGetValue(guid, out RF_Asset asset)) return;
            RefreshAsset(asset, force);
        }

        internal void RefreshSelection()
        {
            string[] list = RF_Unity.Selection_AssetGUIDs;
            for (var i = 0; i < list.Length; i++)
            {
                RefreshAsset(list[i], true);
            }

            Check4Work();
        }

        internal void RefreshAsset(RF_Asset asset, bool force)
        {
            asset.MarkAsDirty(true, force);
            DelayCheck4Changes();

            //#if RF_DEBUG
            //		    Debug.Log("RefreshAsset: " + asset.guid + ":" + workCount);
            //#endif
            //			
            //			workCount++;
            //
            //			if (force)
            //			{
            //				asset.MarkAsDirty(true, true);
            //				
            //				if (asset.type == RF_AssetType.FOLDER && !asset.IsMissing)
            //				{
            //					string[] dirs = Directory.GetDirectories(asset.assetPath, "*", SearchOption.AllDirectories);
            //					//refresh children directories as well
            //
            //					for (var i = 0; i < dirs.Length; i++)
            //					{
            //						string guid = AssetDatabase.AssetPathToGUID(dirs[i]);
            //						RF_Asset child = Api.Get(guid);
            //						if (child == null)
            //						{
            //							continue;
            //						}
            //
            //						workCount++;
            //						child.MarkAsDirty();
            //						queueLoadContent.Add(child);
            //					}
            //				}
            //			}
            //			
            //			queueLoadContent.Add(asset);
        }

        internal void AddAsset(string guid)
        {
            if (AssetMap.ContainsKey(guid))
            {
                Debug.LogWarning("guid already exist <" + guid + ">");
                return;
            }

            var asset = new RF_Asset(guid);
            asset.LoadPathInfo();
            asset.refreshStamp = cacheStamp;

            AssetList.Add(asset);
            AssetMap.Add(guid, asset);

            //Debug.LogWarning("Add - AssetList: " + AssetList.Count);

            // Do not load content for RF_Cache asset
            if (guid == CacheGUID) return;

            workCount++;
            queueLoadContent.Add(asset);
        }

        internal void RemoveAsset(string guid)
        {
            if (!AssetMap.ContainsKey(guid)) return;

            RemoveAsset(AssetMap[guid]);
        }

        internal void RemoveAsset(RF_Asset asset)
        {
            AssetList.Remove(asset);

            // Deleted Asset : still in the map but not in the AssetList
            asset.state = RF_AssetState.MISSING;
        }

        internal void Check4Usage()
        {
#if RF_DEBUG
			Debug.Log("Check 4 Usage");
#endif

            foreach (RF_Asset item in AssetList)
            {
                if (item.IsMissing) continue;
                RF_Unity.Clear(ref item.UsedByMap);
            }

            foreach (RF_Asset item in AssetList)
            {
                if (item.IsMissing) continue;
                AsyncUsedBy(item);
            }

            workCount = 0;
            ready = true;
        }

        internal void Check4Work()
        {
            if (workCount == 0)
            {
                Check4Usage();
                return;
            }

            ready = false;
            EditorApplication.update -= AsyncProcess;
            EditorApplication.update += AsyncProcess;
        }
        
        internal void AsyncProcess()
        {
            if (this == null) return;
            if (RF_SettingExt.disable) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (frameSkipped++ < 10 - 2 * priority) return;

            frameSkipped = 0;
            float t = Time.realtimeSinceStartup;

#if RF_DEBUG
			Debug.Log(Mathf.Round(t) + " : " + progress*workCount + "/" + workCount + ":" + isReady + " ::: " + queueLoadContent.Count);
#endif

            if (!AsyncWork(queueLoadContent, AsyncLoadContent, t)) return;

            EditorApplication.update -= AsyncProcess;
            EditorUtility.SetDirty(this);

            Check4Usage();
        }

        internal bool AsyncWork<T>(List<T> arr, Action<int, T> action, float t)
        {
            float FRAME_DURATION = 1 / 1000f * (priority * 5 + 1); //prevent zero

            int c = arr.Count;

            while (c-- > 0)
            {
                T last = arr[c];
                arr.RemoveAt(c);
                action(c, last);

                //workCount--;

                float dt = Time.realtimeSinceStartup - t - FRAME_DURATION;
                if (dt >= 0) return false;

            }

            return true;
        }

        internal void AsyncLoadContent(int idx, RF_Asset asset)
        {
            //Debug.Log("Async: " + idx);
            if (asset.fileInfoDirty) asset.LoadFileInfo();
            if (asset.fileContentDirty) asset.LoadContent();
        }

        internal void AsyncUsedBy(RF_Asset asset)
        {
            if (AssetMap == null) Check4Changes(false);

            if (asset.IsFolder) return;

#if RF_DEBUG
			Debug.Log("Async UsedBy: " + asset.assetPath);
#endif

            foreach (KeyValuePair<string, HashSet<long>> item in asset.UseGUIDs)
            {
                if (!AssetMap.TryGetValue(item.Key, out RF_Asset tAsset)) continue;
                if (tAsset == null || tAsset.UsedByMap == null) continue;

                if (!tAsset.UsedByMap.ContainsKey(asset.guid)) tAsset.AddUsedBy(asset.guid, asset);
            }
        }


        //---------------------------- Dependencies -----------------------------

        internal RF_Asset Get(string guid, bool isForce = false)
        {
            return AssetMap.ContainsKey(guid) ? AssetMap[guid] : null;
        }

        internal List<RF_Asset> FindAssetsOfType(RF_AssetType type)
        {
            var result = new List<RF_Asset>();
            foreach (KeyValuePair<string, RF_Asset> item in AssetMap)
            {
                if (item.Value.type != type) continue;

                result.Add(item.Value);
            }

            return result;
        }
        internal RF_Asset FindAsset(string guid, string fileId)
        {
            if (AssetMap == null) Check4Changes(false);
            if (!isReady)
            {
#if RF_DEBUG
			Debug.LogWarning("Cache not ready !");
#endif
                return null;
            }

            if (string.IsNullOrEmpty(guid)) return null;

            //for (var i = 0; i < guids.Length; i++)
            {
                //string guid = guids[i];
                if (!AssetMap.TryGetValue(guid, out RF_Asset asset)) return null;

                if (asset.IsMissing) return null;

                if (asset.IsFolder) return null;
                return asset;
            }
        }
        internal List<RF_Asset> FindAssets(string[] guids, bool scanFolder)
        {
            if (AssetMap == null) Check4Changes(false);

            var result = new List<RF_Asset>();

            if (!isReady)
            {
#if RF_DEBUG
			Debug.LogWarning("Cache not ready !");
#endif
                return result;
            }

            var folderList = new List<RF_Asset>();

            if (guids.Length == 0) return result;

            for (var i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                RF_Asset asset;
                if (!AssetMap.TryGetValue(guid, out asset)) continue;

                if (asset.IsMissing) continue;

                if (asset.IsFolder)
                {
                    if (!folderList.Contains(asset)) folderList.Add(asset);
                } else
                    result.Add(asset);
            }

            if (!scanFolder || folderList.Count == 0) return result;

            int count = folderList.Count;
            for (var i = 0; i < count; i++)
            {
                RF_Asset item = folderList[i];

                // for (var j = 0; j < item.UseGUIDs.Count; j++)
                // {
                //     RF_Asset a;
                //     if (!AssetMap.TryGetValue(item.UseGUIDs[j], out a)) continue;
                foreach (KeyValuePair<string, HashSet<long>> useM in item.UseGUIDs)
                {
                    RF_Asset a;
                    if (!AssetMap.TryGetValue(useM.Key, out a)) continue;

                    if (a.IsMissing) continue;

                    if (a.IsFolder)
                    {
                        if (!folderList.Contains(a))
                        {
                            folderList.Add(a);
                            count++;
                        }
                    } else
                        result.Add(a);
                }
            }

            return result;
        }

        //---------------------------- Dependencies -----------------------------

        internal List<List<string>> ScanSimilar(Action IgnoreWhenScan, Action IgnoreFolderWhenScan)
        {
            if (AssetMap == null) Check4Changes(true);

            var dict = new Dictionary<string, List<RF_Asset>>();
            foreach (KeyValuePair<string, RF_Asset> item in AssetMap)
            {
                if (item.Value == null) continue;
                if (item.Value.IsMissing || item.Value.IsFolder) continue;
                if (item.Value.inPlugins) continue;
                if (item.Value.inEditor) continue;
                if (item.Value.IsExcluded) continue;
                if (!item.Value.assetPath.StartsWith("Assets/")) continue;
                if (RF_Setting.IsTypeExcluded(AssetType.GetIndex(item.Value.extension)))
                {
                    if (IgnoreWhenScan != null) IgnoreWhenScan();
                    continue;
                }

                string hash = item.Value.fileInfoHash;
                if (string.IsNullOrEmpty(hash))
                {
#if RF_DEBUG
                    Debug.LogWarning("Hash can not be null! ");
#endif
                    continue;
                }

                if (!dict.TryGetValue(hash, out List<RF_Asset> list))
                {
                    list = new List<RF_Asset>();
                    dict.Add(hash, list);
                }

                list.Add(item.Value);
            }

            return dict.Values
                .Where(item => item.Count > 1)
                .OrderByDescending(item => item[0].fileSize)
                .Select(item => item.Select(asset => asset.assetPath).ToList())
                .ToList();
        }

        internal List<RF_Asset> ScanUnused()
        {
            if (AssetMap == null) Check4Changes(false);
            
            // Get Addressable assets
            var addressable = RF_Addressable.isOk ? RF_Addressable.GetAddresses()
                .SelectMany(item => item.Value.assetGUIDs.Union(item.Value.childGUIDs))
                .ToHashSet() : new HashSet<string>();
            
            var result = new List<RF_Asset>();
            foreach (KeyValuePair<string, RF_Asset> item in AssetMap)
            {
                RF_Asset v = item.Value;
                if (v.IsMissing || v.inEditor || v.IsScript || v.inResources || v.inPlugins || v.inStreamingAsset || v.IsFolder) continue;

                if (!v.assetPath.StartsWith("Assets/")) continue; // ignore built-in / packages assets
                if (v.forcedIncludedInBuild) continue; // ignore assets that are forced to be included in build
                if (v.assetName == "LICENSE") continue; // ignore license files
                
                if (SPECIAL_USE_ASSETS.Contains(v.assetPath)) continue; // ignore assets with special use (can not remove)
                if (SPECIAL_EXTENSIONS.Contains(v.extension)) continue;

                if (v.type == RF_AssetType.DLL) continue;
                if (v.type == RF_AssetType.SCRIPT) continue;
                if (v.type == RF_AssetType.UNKNOWN) continue;
                if (addressable.Contains(v.guid))continue;
                
                // special handler for .spriteatlas
                if (v.extension == ".spriteatlas")
                {
                    var isInUsed = false;
                    List<string> allSprites = v.UseGUIDs.Keys.ToList();
                    foreach (string spriteGUID in allSprites)
                    {
                        var asset = Api.Get(spriteGUID);
                        if (asset.UsedByMap.Count <= 1) continue; // only use by this atlas
                        
                        isInUsed = true;
                        break; // this one is used by other assets
                    }
                    
                    if (isInUsed) continue;
                }
                
                if (v.IsExcluded)
                {
                    // Debug.Log($"Excluded: {v.assetPath}");
                    continue;
                }

                if (!string.IsNullOrEmpty(v.AtlasName)) continue;
                if (!string.IsNullOrEmpty(v.AssetBundleName)) continue;
                if (!string.IsNullOrEmpty(v.AddressableName)) continue;

                if (v.UsedByMap.Count == 0) //&& !RF_Asset.IGNORE_UNUSED_GUIDS.Contains(v.guid)
                    result.Add(v);
            }

            result.Sort((item1, item2) => item1.extension == item2.extension
                ? string.Compare(item1.assetPath, item2.assetPath, StringComparison.Ordinal)
                : string.Compare(item1.extension, item2.extension, StringComparison.Ordinal));

            return result;
        }
    }
}
