using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
namespace ReferenceFinder
{
    internal class RF_SceneRef : RF_Ref
    {
        internal static readonly Dictionary<string, Type> CacheType = new Dictionary<string, Type>();
        

        // ------------------------- Ref in scene
        private static Action<Dictionary<string, RF_Ref>> onFindRefInSceneComplete;
        private static Dictionary<string, RF_Ref> refs = new Dictionary<string, RF_Ref>();
        private static string[] cacheAssetGuids;
        public string sceneFullPath = "";
        public string scenePath = "";
        public string targetType;
        public HashSet<string> usingType = new HashSet<string>();
        
        public RF_SceneRef(int index, int depth, RF_Asset asset, RF_Asset by) : base(index, depth, asset, by)
        {
            isSceneRef = false;
        }

        //		public override string ToString()
        //		{
        //			return "SceneRef: " + sceneFullPath;
        //		}

        public RF_SceneRef(int depth, Object target) : base(0, depth, null, null)
        {
            component = target;
            this.depth = depth;
            isSceneRef = true;
            var obj = target as GameObject;
            if (obj == null)
            {
                var com = target as Component;
                if (com != null) obj = com.gameObject;
            }

            scenePath = RF_Unity.GetGameObjectPath(obj, false);
            if (component == null) return;

			string cName = component.name ?? "(empty)"; // some components are hidden
            
            sceneFullPath = scenePath + cName;
            targetType = component.GetType().Name;
			assetNameGC = RF_GUIContent.FromString(cName);
        }

        public Func<bool> drawFullPath;
        
        public static IWindow window { get; set; }

        public override bool isSelected()
        {
            return component != null && RF_Bookmark.Contains(component);
        }
        readonly GUIContent assetNameGC;

        public void Draw(Rect r, IWindow window, RF_RefDrawer.Mode groupMode, bool showDetails)
        {
            bool selected = isSelected();
            DrawToogleSelect(r);

            var margin = 2;
            var left = new Rect(r);
            left.width = r.width / 3f;

            var right = new Rect(r);
            right.xMin += left.width + margin;

            //Debug.Log("draw scene "+ selected);
            if ( /* RF_Setting.PingRow && */ (Event.current.type == EventType.MouseDown) && (Event.current.button == 0))
            {
                Rect pingRect = RF_Setting.PingRow
                    ? new Rect(0, r.y, r.x + r.width, r.height)
                    : left;

                if (pingRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.control || Event.current.command)
                    {
                        if (selected)
                            RF_Bookmark.Remove(this);
                        else
                            RF_Bookmark.Add(this);
                        if (window != null) window.Repaint();
                    } else
                        EditorGUIUtility.PingObject(component);

                    Event.current.Use();
                }
            }

            EditorGUI.ObjectField(showDetails ? left : r, GUIContent.none, component, typeof(GameObject), true);
            if (!showDetails) return;

            bool drawPath = groupMode != RF_RefDrawer.Mode.Folder;
            float pathW = drawPath && !string.IsNullOrEmpty(scenePath) ? EditorStyles.miniLabel.CalcSize(RF_GUIContent.FromString(scenePath)).x : 0;
            // string assetName = component.name;

            // if(usingType!= null && usingType.Count > 0)
            // {
            // 	assetName += " -> ";
            // 	foreach(var item in usingType)
            // 	{
            // 		assetName += item + " - ";
            // 	}
            // 	assetName = assetName.Substring(0, assetName.Length - 3);
            // }
            Color cc = RF_Cache.Api.setting.SelectedColor;

            var lableRect = new Rect(
                right.x,
                right.y,
                pathW + EditorStyles.boldLabel.CalcSize(assetNameGC).x,
                right.height);

            if (selected)
            {
                Color c = GUI.color;
                GUI.color = cc;
                GUI.DrawTexture(lableRect, EditorGUIUtility.whiteTexture);
                GUI.color = c;
            }

            if (drawPath)
            {
                GUI.Label(LeftRect(pathW, ref right), scenePath, EditorStyles.miniLabel);
                right.xMin -= 4f;
                GUI.Label(right, assetNameGC, EditorStyles.boldLabel);
            } else
            {
                GUI.Label(right, assetNameGC);
            }


            if (!RF_Setting.ShowUsedByClassed || usingType == null) return;

            float sub = 10;
            var re = new Rect(r.x + r.width - sub, r.y, 20, r.height);
            Type t = null;
            foreach (string item in usingType)
            {
                string name = item;
                if (!CacheType.TryGetValue(item, out t))
                {
                    t = RF_Unity.GetType(name);

                    // if (t == null)
                    // {
                    // 	continue;
                    // } 
                    CacheType.Add(item, t);
                }

                GUIContent content;
                var width = 0.0f;
                if (!RF_Asset.cacheImage.TryGetValue(name, out content))
                {
                    if (t == null)
                        content = RF_GUIContent.FromString(name);
                    else
                    {
                        Texture text = EditorGUIUtility.ObjectContent(null, t).image;
                        if (text == null)
                            content = RF_GUIContent.FromString(name);
                        else
                            content = RF_GUIContent.FromTexture(text, name);
                    }


                    RF_Asset.cacheImage.Add(name, content);
                }

                if (content.image == null)
                    width = EditorStyles.label.CalcSize(content).x;
                else
                    width = 20;

                re.x -= width;
                re.width = width;

                GUI.Label(re, content);
                re.x -= margin; // margin;
            }


            // var nameW = EditorStyles.boldLabel.CalcSize(new GUIContent(assetName)).x;
        }

        private Rect LeftRect(float w, ref Rect rect)
        {
            rect.x += w;
            rect.width -= w;
            return new Rect(rect.x - w, rect.y, w, rect.height);
        }

        // ------------------------- Scene use scene objects
        public static Dictionary<string, RF_Ref> FindSceneUseSceneObjects(GameObject[] targets)
        {
            var results = new Dictionary<string, RF_Ref>();
            GameObject[] objs = Selection.gameObjects;
            for (var i = 0; i < objs.Length; i++)
            {
                if (RF_Unity.IsInAsset(objs[i])) continue;

                var key = objs[i].GetInstanceID().ToString();
                if (!results.ContainsKey(key)) results.Add(key, new RF_SceneRef(0, objs[i]));

                Component[] coms = objs[i].GetComponents<Component>();
                Dictionary<Component, HashSet<RF_SceneCache.HashValue>> SceneCache = RF_SceneCache.Api.cache;
                for (var j = 0; j < coms.Length; j++)
                {
                    HashSet<RF_SceneCache.HashValue> hash = null;
                    if (coms[j] == null) continue; // missing component

                    if (SceneCache.TryGetValue(coms[j], out hash))
                        foreach (RF_SceneCache.HashValue item in hash)
                        {
                            if (item.isSceneObject)
                            {
                                Object obj = item.target;
                                var key1 = obj.GetInstanceID().ToString();
                                if (!results.ContainsKey(key1)) results.Add(key1, new RF_SceneRef(1, obj));
                            }
                        }
                }
            }

            return results;
        }

        // ------------------------- Scene in scene
        public static Dictionary<string, RF_Ref> FindSceneInScene(GameObject[] targets)
        {
            var results = new Dictionary<string, RF_Ref>();
            GameObject[] objs = Selection.gameObjects;
            for (var i = 0; i < objs.Length; i++)
            {
                if (RF_Unity.IsInAsset(objs[i])) continue;

                var key = objs[i].GetInstanceID().ToString();
                if (!results.ContainsKey(key)) results.Add(key, new RF_SceneRef(0, objs[i]));


                foreach (KeyValuePair<Component, HashSet<RF_SceneCache.HashValue>> item in RF_SceneCache.Api.cache)
                foreach (RF_SceneCache.HashValue item1 in item.Value)
                {
                    // if(item.Key.gameObject.name == "ScenesManager")
                    // Debug.Log(item1.objectReferenceValue);
                    GameObject ob = null;
                    if (item1.target is GameObject)
                        ob = item1.target as GameObject;
                    else
                    {
                        var com = item1.target as Component;
                        if (com == null) continue;

                        ob = com.gameObject;
                    }

                    if (ob == null) continue;

                    if (ob != objs[i]) continue;

                    key = item.Key.GetInstanceID().ToString();
                    if (!results.ContainsKey(key)) results.Add(key, new RF_SceneRef(1, item.Key));

                    (results[key] as RF_SceneRef).usingType.Add(item1.target.GetType().FullName);
                }
            }

            return results;
        }

        public static Dictionary<string, RF_Ref> FindRefInScene(
            string[] assetGUIDs, bool depth,
            Action<Dictionary<string, RF_Ref>> onComplete, IWindow win)
        {
            // var watch = new System.Diagnostics.Stopwatch();
            // watch.Start();
            window = win;
            cacheAssetGuids = assetGUIDs;
            onFindRefInSceneComplete = onComplete;
            if (RF_SceneCache.ready)
                FindRefInScene();
            else
            {
                RF_SceneCache.onReady -= FindRefInScene;
                RF_SceneCache.onReady += FindRefInScene;
            }

            return refs;
        }

        private static void FindRefInScene()
        {
            refs = new Dictionary<string, RF_Ref>();
            for (var i = 0; i < cacheAssetGuids.Length; i++)
            {
                RF_Asset asset = RF_Cache.Api.Get(cacheAssetGuids[i]);
                if (asset == null) continue;

                Add(refs, asset, 0);

                ApplyFilter(refs, asset);
            }

            if (onFindRefInSceneComplete != null) onFindRefInSceneComplete(refs);

            RF_SceneCache.onReady -= FindRefInScene;

            //    UnityEngine.Debug.Log("Time find ref in scene " + watch.ElapsedMilliseconds);
        }

        private static void FilterAll(Dictionary<string, RF_Ref> refs, Object obj, string targetPath)
        {
            // ApplyFilter(refs, obj, targetPath);
        }

        private static void ApplyFilter(Dictionary<string, RF_Ref> refs, RF_Asset asset)
        {
            string targetPath = AssetDatabase.GUIDToAssetPath(asset.guid);
            if (string.IsNullOrEmpty(targetPath)) return; // asset not found - might be deleted!

            //asset being moved!
            if (targetPath != asset.assetPath) asset.MarkAsDirty();

            Object target = AssetDatabase.LoadAssetAtPath(targetPath, typeof(Object));
            if (target == null)

                //Debug.LogWarning("target is null");
                return;

            bool targetIsGameobject = target is GameObject;

            if (targetIsGameobject)
                foreach (GameObject item in RF_Unity.getAllObjsInCurScene())
                {
                    if (RF_Unity.CheckIsPrefab(item))
                    {
                        string itemGUID = RF_Unity.GetPrefabParent(item);

                        // Debug.Log(item.name + " itemGUID: " + itemGUID);
                        // Debug.Log(target.name + " asset.guid: " + asset.guid);
                        if (itemGUID == asset.guid) Add(refs, item, 1);
                    }
                }

            string dir = Path.GetDirectoryName(targetPath);
            if (RF_SceneCache.Api.folderCache.ContainsKey(dir))
                foreach (Component item in RF_SceneCache.Api.folderCache[dir])
                {
                    if (RF_SceneCache.Api.cache.ContainsKey(item))
                        foreach (RF_SceneCache.HashValue item1 in RF_SceneCache.Api.cache[item])
                        {
                            if (targetPath == AssetDatabase.GetAssetPath(item1.target)) Add(refs, item, 1);
                        }
                }
        }

        private static void Add(Dictionary<string, RF_Ref> refs, RF_Asset asset, int depth)
        {
            string targetId = asset.guid;
            if (!refs.ContainsKey(targetId)) refs.Add(targetId, new RF_Ref(0, depth, asset, null));
        }

        private static void Add(Dictionary<string, RF_Ref> refs, Object target, int depth)
        {
            var targetId = target.GetInstanceID().ToString();
            if (!refs.ContainsKey(targetId)) refs.Add(targetId, new RF_SceneRef(depth, target));
        }
    }

    internal class RF_Ref
    {

        public RF_Asset addBy;
        public RF_Asset asset;
        public Object component;
        public int depth;
        public string group;
        public int index;

        public bool isSceneRef;
        public int matchingScore;
        public int type;

        public RF_Ref(){}

        public RF_Ref(int index, int depth, RF_Asset asset, RF_Asset by)
        {
            this.index = index;
            this.depth = depth;

            this.asset = asset;
            if (asset != null) type = AssetType.GetIndex(asset.extension);

            addBy = by;

            // isSceneRef = false;
        }

        public RF_Ref(int index, int depth, RF_Asset asset, RF_Asset by, string group) : this(index, depth, asset,
            by)
        {
            this.group = group;

            // isSceneRef = false;
        }
        private static int CSVSorter(RF_Ref item1, RF_Ref item2)
        {
            int r = item1.depth.CompareTo(item2.depth);
            if (r != 0) return r;

            int t = item1.type.CompareTo(item2.type);
            if (t != 0) return t;

            return item1.index.CompareTo(item2.index);
        }


        public static RF_Ref[] FromDict(Dictionary<string, RF_Ref> dict)
        {
            if (dict == null || dict.Count == 0) return null;

            var result = new List<RF_Ref>();

            foreach (KeyValuePair<string, RF_Ref> kvp in dict)
            {
                if (kvp.Value == null) continue;
                if (kvp.Value.asset == null) continue;

                result.Add(kvp.Value);
            }

            result.Sort(CSVSorter);


            return result.ToArray();
        }

        public static RF_Ref[] FromList(List<RF_Ref> list)
        {
            if (list == null || list.Count == 0) return null;

            list.Sort(CSVSorter);
            var result = new List<RF_Ref>();
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].asset == null) continue;
                result.Add(list[i]);
            }
            return result.ToArray();
        }

        public override string ToString()
        {
            if (isSceneRef)
            {
                var sr = (RF_SceneRef)this;
                return sr.scenePath;
            }

            return asset.assetPath;
        }

        public string GetSceneObjId()
        {
            if (component == null) return string.Empty;

            return component.GetInstanceID().ToString();
        }

        public virtual bool isSelected()
        {
            return RF_Bookmark.Contains(asset.guid);
        }
        public virtual void DrawToogleSelect(Rect r)
        {
            bool s = isSelected();
            r.width = 16f;
            if (!GUI2.Toggle(r, ref s)) return;

            if (s)
                RF_Bookmark.Add(this);
            else
                RF_Bookmark.Remove(this);
        }

        // public RF_Ref(int depth, UnityEngine.Object target)
        // {
        // 	this.component = target;
        // 	this.depth = depth;
        // 	// isSceneRef = true;
        // }
        internal List<RF_Ref> Append(Dictionary<string, RF_Ref> dict, params string[] guidList)
        {
            var result = new List<RF_Ref>();
            if (!RF_Cache.isReady)
            {
                Debug.LogWarning("Cache not yet ready! Please wait!");
                return result;
            }

            var excludePackage = !RF_Cache.Api.setting.showPackageAsset;

            //filter to remove items that already in dictionary
            for (var i = 0; i < guidList.Length; i++)
            {
                string guid = guidList[i];
                if (dict.ContainsKey(guid)) continue;

                RF_Asset child = RF_Cache.Api.Get(guid);
                if (child == null) continue;
                if (excludePackage && child.inPackages) continue;

                var r = new RF_Ref(dict.Count, depth + 1, child, asset);
                if (!asset.IsFolder) dict.Add(guid, r);

                result.Add(r);
            }

            return result;
        }

        internal void AppendUsedBy(Dictionary<string, RF_Ref> result, bool deep)
        {
            // var list = Append(result, RF_Asset.FindUsedByGUIDs(asset).ToArray());
            // if (!deep) return;

            // // Add next-level
            // for (var i = 0;i < list.Count;i ++)
            // {
            // 	list[i].AppendUsedBy(result, true);
            // }

            Dictionary<string, RF_Asset> h = asset.UsedByMap;
            List<RF_Ref> list = deep ? new List<RF_Ref>() : null;

            if (asset.UsedByMap == null) return;
            var excludePackage = !RF_Cache.Api.setting.showPackageAsset;
            
            foreach (KeyValuePair<string, RF_Asset> kvp in h)
            {
                string guid = kvp.Key;
                if (result.ContainsKey(guid)) continue;

                RF_Asset child = RF_Cache.Api.Get(guid);
                if (child == null) continue;
                if (child.IsMissing) continue;
                if (excludePackage && child.inPackages) continue;

                var r = new RF_Ref(result.Count, depth + 1, child, asset);
                if (!asset.IsFolder) result.Add(guid, r);

                if (deep) list.Add(r);
            }

            if (!deep) return;

            foreach (RF_Ref item in list)
            {
                item.AppendUsedBy(result, true);
            }
        }

        internal void AppendUsage(Dictionary<string, RF_Ref> result, bool deep)
        {
            Dictionary<string, HashSet<long>> h = asset.UseGUIDs;
            List<RF_Ref> list = deep ? new List<RF_Ref>() : null;
            var excludePackage = !RF_Cache.Api.setting.showPackageAsset;
            foreach (KeyValuePair<string, HashSet<long>> kvp in h)
            {
                string guid = kvp.Key;
                if (result.ContainsKey(guid)) continue;

                RF_Asset child = RF_Cache.Api.Get(guid);
                if (child == null) continue;
                if (child.IsMissing) continue;
                if (excludePackage && child.inPackages) continue;

                var r = new RF_Ref(result.Count, depth + 1, child, asset);
                if (!asset.IsFolder) result.Add(guid, r);

                if (deep) list.Add(r);
            }

            if (!deep) return;

            foreach (RF_Ref item in list)
            {
                item.AppendUsage(result, true);
            }
        }

        // --------------------- STATIC UTILS -----------------------
        
        
        internal static Dictionary<string, RF_Ref> FindRefs(string[] guids, bool usageOrUsedBy, bool addFolder)
        {
            var dict = new Dictionary<string, RF_Ref>();
            var list = new List<RF_Ref>();
            var excludePackage = !RF_Cache.Api.setting.showPackageAsset;

            for (var i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                if (dict.ContainsKey(guid)) continue;

                RF_Asset asset = RF_Cache.Api.Get(guid);
                if (asset == null) continue;
                if (excludePackage && asset.inPackages) continue;
                
                var r = new RF_Ref(i, 0, asset, null);
                if (!asset.IsFolder || addFolder) dict.Add(guid, r);

                list.Add(r);
            }

            for (var i = 0; i < list.Count; i++)
            {
                if (usageOrUsedBy)
                    list[i].AppendUsage(dict, true);
                else
                    list[i].AppendUsedBy(dict, true);
            }

            //var result = dict.Values.ToList();
            //result.Sort((item1, item2)=>{
            //	return item1.index.CompareTo(item2.index);
            //});

            return dict;
        }


        public static Dictionary<string, RF_Ref> FindUsage(string[] guids)
        {
            return FindRefs(guids, true, true);
        }

        public static Dictionary<string, RF_Ref> FindUsedBy(string[] guids)
        {
            return FindRefs(guids, false, true);
        }

        public static Dictionary<string, RF_Ref> FindUsageScene(GameObject[] objs, bool depth)
        {
            var dict = new Dictionary<string, RF_Ref>();

            // var list = new List<RF_Ref>();

            for (var i = 0; i < objs.Length; i++)
            {
                if (RF_Unity.IsInAsset(objs[i])) continue; //only get in scene 

                //add selection
                if (!dict.ContainsKey(objs[i].GetInstanceID().ToString())) dict.Add(objs[i].GetInstanceID().ToString(), new RF_SceneRef(0, objs[i]));

                foreach (Object item in RF_Unity.GetAllRefObjects(objs[i]))
                {
                    AppendUsageScene(dict, item);
                }

                if (!depth) continue;
                foreach (GameObject child in RF_Unity.getAllChild(objs[i]))
                {
                    foreach (Object item2 in RF_Unity.GetAllRefObjects(child))
                    {
                        AppendUsageScene(dict, item2);
                    }
                }
            }

            return dict;
        }

        private static void AppendUsageScene(Dictionary<string, RF_Ref> dict, Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) return;

            string guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid)) return;

            if (dict.ContainsKey(guid)) return;

            RF_Asset asset = RF_Cache.Api.Get(guid);
            if (asset == null) return;

            if (!RF_Cache.Api.setting.showPackageAsset && asset.inPackages) return;

            var r = new RF_Ref(0, 1, asset, null);
            dict.Add(guid, r);
        }
    }


    internal class RF_RefDrawer : IRefDraw
    {
        public enum Mode
        {
            Dependency,
            Depth,
            Type,
            Extension,
            Folder,
            Atlas,
            AssetBundle,
            None
        }

        public enum Sort
        {
            Type,
            Path,
            Size
        }

        public static GUIStyle toolbarSearchField;
        public static GUIStyle toolbarSearchFieldCancelButton;
        public static GUIStyle toolbarSearchFieldCancelButtonEmpty;
        private readonly Dictionary<string, BookmarkInfo> gBookmarkCache = new Dictionary<string, BookmarkInfo>();

        internal readonly RF_TreeUI2.GroupDrawer groupDrawer;

        // FILTERING
        private readonly string searchTerm = string.Empty;
        private readonly bool showSearch = true;
        public bool caseSensitive = false;

        private readonly Func<Sort> getSortMode;
        private readonly Func<Mode> getGroupMode;

        // STATUS
        private bool dirty;
        private int excludeCount;
        public bool forceHideDetails;

        public readonly List<RF_Asset> highlight = new List<RF_Asset>();
        
        public string level0Group;
        public bool showDetail;
        internal List<RF_Ref> list;
        public string messageEmpty = "It's empty!";

        public string messageNoRefs = "Do select something!";
        internal Dictionary<string, RF_Ref> refs;
        private bool selectFilter;
        private bool showIgnore;
        
        public Func<RF_Ref, string> customGetGroup;

        public Action<Rect, string, int> customDrawGroupLabel;
        public Action<Rect, RF_Ref> beforeItemDraw;
        public Action<Rect, RF_Ref> afterItemDraw;

        public bool drawFullPath;
        public bool drawExtension;
        public bool drawFileSize;
        public bool drawUsageType;
        
        
        public RF_RefDrawer(IWindow window, Func<Sort> getSortMode, Func<Mode> getGroupMode)
        {
            this.window = window;
            this.getSortMode = getSortMode;
            this.getGroupMode = getGroupMode;
            groupDrawer = new RF_TreeUI2.GroupDrawer(DrawGroup, DrawAsset);
        }



        // ORIGINAL
        internal RF_Ref[] source => RF_Ref.FromList(list);

        public IWindow window { get; set; }
        public bool Draw(Rect rect)
        {   
            if (refs == null || refs.Count == 0)
            {
                DrawEmpty(rect, messageNoRefs);
                return false;
            }

            if (dirty || list == null) ApplyFilter();

            if (!groupDrawer.hasChildren)
                DrawEmpty(rect, messageEmpty);
            else
                groupDrawer.Draw(rect);
            return false;
        }

        public bool DrawLayout()
        {
            if (refs == null || refs.Count == 0) return false;

            if (dirty || list == null) ApplyFilter();

            groupDrawer.DrawLayout();
            return false;
        }

        public int ElementCount()
        {
            if (refs == null) return 0;

            return refs.Count;

            // return refs.Where(x => x.Value.depth != 0).Count();
        }

        private void DrawEmpty(Rect rect, string text)
        {
            rect = GUI2.Padding(rect, 2f, 2f);
            rect.height = 45f;

            EditorGUI.HelpBox(rect, text, MessageType.Info);
        }
        public void SetRefs(Dictionary<string, RF_Ref> dictRefs)
        {
            refs = dictRefs;
            dirty = true;
        }

        private void SetBookmarkGroup(string groupLabel, bool willbookmark)
        {
            string[] ids = groupDrawer.GetChildren(groupLabel);
            BookmarkInfo info = GetBMInfo(groupLabel);

            for (var i = 0; i < ids.Length; i++)
            {
                RF_Ref rf;
                if (!refs.TryGetValue(ids[i], out rf)) continue;

                if (willbookmark)
                    RF_Bookmark.Add(rf);
                else
                    RF_Bookmark.Remove(rf);
            }

            info.count = willbookmark ? info.total : 0;
        }

        private BookmarkInfo GetBMInfo(string groupLabel)
        {
            BookmarkInfo info = null;
            if (!gBookmarkCache.TryGetValue(groupLabel, out info))
            {
                string[] ids = groupDrawer.GetChildren(groupLabel);

                info = new BookmarkInfo();
                for (var i = 0; i < ids.Length; i++)
                {
                    RF_Ref rf;
                    if (!refs.TryGetValue(ids[i], out rf)) continue;
                    info.total++;

                    bool isBM = RF_Bookmark.Contains(rf);
                    if (isBM) info.count++;
                }

                gBookmarkCache.Add(groupLabel, info);
            }

            return info;
        }

        private void DrawToggleGroup(Rect r, string groupLabel)
        {
            BookmarkInfo info = GetBMInfo(groupLabel);
            bool selectAll = info.count == info.total;
            r.width = 16f;
            if (GUI2.Toggle(r, ref selectAll)) SetBookmarkGroup(groupLabel, selectAll);

            if (!selectAll && (info.count > 0))
            {
                //GUI.DrawTexture(r, EditorStyles.
            }
        }

        private void DrawGroup(Rect r, string label, int childCount)
        {
            if (string.IsNullOrEmpty(label)) label = "(none)";
            DrawToggleGroup(r, label);
            r.xMin += 18f;
            
            var groupMode = getGroupMode();
            if (groupMode == Mode.Folder)
            {
                Texture tex = AssetDatabase.GetCachedIcon("Assets");
                GUI.DrawTexture(new Rect(r.x, r.y, 16f, 16f), tex);
                r.xMin += 16f;
            }

            if (customDrawGroupLabel != null)
            {
                customDrawGroupLabel.Invoke(r, label, childCount);
            }
			else
			{
	            GUIContent lbContent = RF_GUIContent.FromString(label);
	            GUI.Label(r, lbContent, EditorStyles.boldLabel);

	            var cRect = r;
	            cRect.x += EditorStyles.boldLabel.CalcSize(lbContent).x;
	            cRect.y += 1f;
	            GUI.Label(cRect, RF_GUIContent.FromString($"({childCount})"), EditorStyles.miniLabel);
            }

            bool hasMouse = (Event.current.type == EventType.MouseUp) && r.Contains(Event.current.mousePosition);
            if (hasMouse && (Event.current.button == 1))
            {
                var menu = new GenericMenu();
                menu.AddItem(RF_GUIContent.FromString("Add Bookmark"), false, () => { SetBookmarkGroup(label, true); });
                menu.AddItem(RF_GUIContent.FromString("Remove Bookmark"), false, () =>
                {
                    SetBookmarkGroup(label, false);
                });

                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        public void DrawDetails(Rect rect)
        {
            Rect r = rect;
            r.xMin += 18f;
            r.height = 18f;

            for (var i = 0; i < highlight.Count; i++)
            {
                highlight[i].Draw(r, 
                    false, 
                    false, 
                    false, 
                    false, 
                    false,
                    false, window, false);
                r.y += 18f;
                r.xMin += 18f;
            }
        }

        private void DrawAsset(Rect r, string guid)
        {
            if (!refs.TryGetValue(guid, out RF_Ref rf)) return;

            if (rf.isSceneRef)
            {
                if (rf.component == null) return;
                if (!(rf is RF_SceneRef re)) return;
                beforeItemDraw?.Invoke(r, rf);
                // r.x -= 16f;
                rf.DrawToogleSelect(r);
                r.xMin += 32f;
                re.Draw(r, window, getGroupMode(), !forceHideDetails);
            } else
            {
                beforeItemDraw?.Invoke(r, rf);
                // r.xMin -= 16f;
                rf.DrawToogleSelect(r);
                r.xMin += 32f;

                float w2 = (r.x + r.width) / 2f;
                var rRect = new Rect(w2, r.y, w2, r.height);
                bool isClick = (Event.current.type == EventType.MouseDown) && (Event.current.button == 0);

                if (isClick && rRect.Contains(Event.current.mousePosition))
                {
                    showDetail = true;
                    highlight.Clear();
                    highlight.Add(rf.asset);

                    RF_Asset p = rf.addBy;
                    var cnt = 0;

                    while ((p != null) && refs.ContainsKey(p.guid))
                    {
                        highlight.Add(p);

                        RF_Ref rfRef = refs[p.guid];
                        if (rfRef != null) p = rfRef.addBy;

                        if (++cnt > 100)
                        {
                            Debug.LogWarning("Break on depth 1000????");
                            break;
                        }
                    }

                    highlight.Sort((item1, item2) =>
                    {
                        int d1 = refs[item1.guid].depth;
                        int d2 = refs[item2.guid].depth;
                        return d1.CompareTo(d2);
                    });

                    // Debug.Log("Highlight: " + highlight.Count + "\n" + string.Join("\n", highlight.ToArray()));
                    Event.current.Use();
                }

                bool isHighlight = highlight.Contains(rf.asset);
                // if (isHighlight)
                // {
                //     var hlRect = new Rect(-20, r.y, 15f, r.height);
                //     GUI2.Rect(hlRect, GUI2.darkGreen);
                // }
                
                rf.asset.Draw(r,
                    isHighlight,
                    drawFullPath,
                    !forceHideDetails && drawFileSize,
                    !forceHideDetails && RF_Setting.s.displayAssetBundleName,
                    !forceHideDetails && RF_Setting.s.displayAtlasName,
                    !forceHideDetails && drawUsageType,
                    window,
                    !forceHideDetails && drawExtension
                );
            }
            
            afterItemDraw?.Invoke(r, rf);
        }
        
        private string GetGroup(RF_Ref rf)
        {
            if (customGetGroup != null) return customGetGroup(rf);
            
            if (rf.depth == 0) return level0Group;

            if (getGroupMode() == Mode.None) return "(no group)";

            RF_SceneRef sr = null;
            if (rf.isSceneRef)
            {
                sr = rf as RF_SceneRef;
                if (sr == null) return null;
            }

            if (!rf.isSceneRef)
                if (rf.asset.IsExcluded)
                    return null; // "(ignored)"

            switch (getGroupMode())
            {
            case Mode.Extension:
                {
                    // if (!rf.isSceneRef) Debug.Log($"Extension: {rf.asset.assetPath} | {rf.asset.extension}");
                    return rf.isSceneRef ? sr.targetType 
                        : string.IsNullOrEmpty(rf.asset.extension) ? "(no extension)" : rf.asset.extension;
                }
                case Mode.Type:
                {
                    return rf.isSceneRef ? sr.targetType : AssetType.FILTERS[rf.type].name;
                }

                case Mode.Folder: return rf.isSceneRef ? sr.scenePath : rf.asset.assetFolder;

                case Mode.Dependency:
                {
                    return rf.depth == 1 ? "Direct Usage" : "Indirect Usage";
                }

                case Mode.Depth:
                {
                    return "Level " + rf.depth;
                }

                case Mode.Atlas: return rf.isSceneRef ? "(not in atlas)" : string.IsNullOrEmpty(rf.asset.AtlasName) ? "(not in atlas)" : rf.asset.AtlasName;
                case Mode.AssetBundle: return rf.isSceneRef ? "(not in assetbundle)" : string.IsNullOrEmpty(rf.asset.AssetBundleName) ? "(not in assetbundle)" : rf.asset.AssetBundleName;
            }

            return "(others)";
        }

        private void SortGroup(List<string> groups)
        {
            groups.Sort((item1, item2) =>
            {
                if (item1.Contains("(")) return 1;
                if (item2.Contains("(")) return -1;

                return String.Compare(item1, item2, StringComparison.Ordinal);
            });
        }

        public RF_RefDrawer Reset(string[] assetGUIDs, bool isUsage)
        {
            gBookmarkCache.Clear();

            refs = isUsage ? RF_Ref.FindUsage(assetGUIDs) : RF_Ref.FindUsedBy(assetGUIDs);
            dirty = true;
            if (list != null) list.Clear();
            return this;
        }

        public void Reset(Dictionary<string, RF_Ref> newRefs)
        {
            if (refs == null) refs = new Dictionary<string, RF_Ref>();
            refs.Clear();
            foreach (var kvp in newRefs)
            {
                refs.Add(kvp.Key, kvp.Value);
            }
            dirty = true;
            if (list != null) list.Clear();
        }

        public RF_RefDrawer Reset(GameObject[] objs, bool findDept, bool findPrefabInAsset)
        {
            refs = RF_Ref.FindUsageScene(objs, findDept);

            var guidss = new List<string>();
            Dictionary<GameObject, HashSet<string>> dependent = RF_SceneCache.Api.prefabDependencies;
            foreach (GameObject gameObject in objs)
            {
                if (!dependent.TryGetValue(gameObject, out HashSet<string> hash)) continue;
                foreach (string guid in hash)
                {
                    guidss.Add(guid);
                }
            }

            Dictionary<string, RF_Ref> usageRefs1 = RF_Ref.FindUsage(guidss.ToArray());
            foreach (KeyValuePair<string, RF_Ref> kvp in usageRefs1)
            {
                if (refs.ContainsKey(kvp.Key)) continue;

                if (guidss.Contains(kvp.Key)) kvp.Value.depth = 1;

                refs.Add(kvp.Key, kvp.Value);
            }


            if (findPrefabInAsset)
            {
                var guids = new List<string>();
                for (var i = 0; i < objs.Length; i++)
                {
                    string guid = RF_Unity.GetPrefabParent(objs[i]);
                    if (string.IsNullOrEmpty(guid)) continue;

                    guids.Add(guid);
                }

                Dictionary<string, RF_Ref> usageRefs = RF_Ref.FindUsage(guids.ToArray());
                foreach (KeyValuePair<string, RF_Ref> kvp in usageRefs)
                {
                    if (refs.ContainsKey(kvp.Key)) continue;

                    if (guids.Contains(kvp.Key)) kvp.Value.depth = 1;

                    refs.Add(kvp.Key, kvp.Value);
                }
            }

            dirty = true;
            if (list != null) list.Clear();

            return this;
        }

        //ref in scene
        public RF_RefDrawer Reset(string[] assetGUIDs, IWindow window)
        {
            refs = RF_SceneRef.FindRefInScene(assetGUIDs, true, SetRefInScene, window);
            dirty = true;
            if (list != null) list.Clear();

            return this;
        }

        private void SetRefInScene(Dictionary<string, RF_Ref> data)
        {
            refs = data;
            dirty = true;
            if (list != null) list.Clear();
        }

        //scene in scene
        public RF_RefDrawer ResetSceneInScene(GameObject[] objs)
        {
            refs = RF_SceneRef.FindSceneInScene(objs);
            dirty = true;
            if (list != null) list.Clear();

            return this;
        }

        public RF_RefDrawer ResetSceneUseSceneObjects(GameObject[] objs)
        {
            refs = RF_SceneRef.FindSceneUseSceneObjects(objs);
            dirty = true;
            if (list != null) list.Clear();

            return this;
        }

        public RF_RefDrawer ResetUnusedAsset()
        {
            List<RF_Asset> lst = RF_Cache.Api.ScanUnused();

            refs = lst.ToDictionary(x => x.guid, x => new RF_Ref(0, 1, x, null));
            dirty = true;
            if (list != null) list.Clear();

            return this;
        }

        public void RefreshSort()
        {
            if (list == null) return;

            if ((list.Count > 0) && (list[0].isSceneRef == false) && (getSortMode() == Sort.Size))
                list = list.OrderByDescending(x => x.asset?.fileSize ?? 0).ToList();
            else
                list.Sort((r1, r2) =>
                {
                    bool isMixed = r1.isSceneRef ^ r2.isSceneRef;
                    if (isMixed)
                    {
#if RF_DEBUG
						var sb = new StringBuilder();
						sb.Append("r1: " + r1.ToString());
						sb.AppendLine();
						sb.Append("r2: " +r2.ToString());
						Debug.LogWarning("Mixed compared!\n" + sb.ToString());
#endif

                        int v1 = r1.isSceneRef ? 1 : 0;
                        int v2 = r2.isSceneRef ? 1 : 0;
                        return v2.CompareTo(v1);
                    }

                    if (r1.isSceneRef)
                    {
                        var rs1 = (RF_SceneRef)r1;
                        var rs2 = (RF_SceneRef)r2;

                        return SortAsset(rs1.sceneFullPath, rs2.sceneFullPath,
                            rs1.targetType, rs2.targetType,
                            getSortMode() == Sort.Path);
                    }

                    if (r1.asset == null) return -1;
                    if (r2.asset == null) return 1;
                    
                    return SortAsset(
                        r1.asset.assetPath, r2.asset.assetPath,
                        r1.asset.extension, r2.asset.extension,
                        false
                    );
                });

            // clean up list
            for (int i = list.Count - 1; i >= 0; i--)
            {
                RF_Ref item = list[i];

                if (item.isSceneRef)
                {
                    if (string.IsNullOrEmpty(item.GetSceneObjId())) list.RemoveAt(i);

                    continue;
                }

                if (item.asset == null) list.RemoveAt(i);
            }

            groupDrawer.Reset(list,
                rf =>
                {
                    if (rf == null) return null;
                    return rf.isSceneRef ? rf.GetSceneObjId() : rf.asset?.guid;
                }, GetGroup, SortGroup);
        }

        public bool isExclueAnyItem()
        {
            return excludeCount > 0;
        }

        private void ApplyFilter()
        {
            dirty = false;

            if (refs == null) return;

            if (list == null)
                list = new List<RF_Ref>();
            else
                list.Clear();

            int minScore = searchTerm.Length;

            string term1 = searchTerm;
            if (!caseSensitive) term1 = term1.ToLower();

            string term2 = term1.Replace(" ", string.Empty);

            excludeCount = 0;

            foreach (KeyValuePair<string, RF_Ref> item in refs)
            {
                RF_Ref r = item.Value;

                if (RF_Setting.IsTypeExcluded(r.type))
                {
                    excludeCount++;
                    continue; //skip this one
                }

                if (!showSearch || string.IsNullOrEmpty(searchTerm))
                {
                    r.matchingScore = 0;
                    list.Add(r);
                    continue;
                }

                //calculate matching score
                string name1 = r.isSceneRef ? (r as RF_SceneRef)?.sceneFullPath : r.asset.assetName;
                if (!caseSensitive) name1 = name1?.ToLower();

                string name2 = name1?.Replace(" ", string.Empty);

                int score1 = RF_Unity.StringMatch(term1, name1);
                int score2 = RF_Unity.StringMatch(term2, name2);

                r.matchingScore = Mathf.Max(score1, score2);
                if (r.matchingScore > minScore) list.Add(r);
            }

            RefreshSort();
        }

        public void SetDirty()
        {
            dirty = true;
        }

        private int SortAsset(string term11, string term12, string term21, string term22, bool swap)
        {
            //			if (term11 == null) term11 = string.Empty;
            //			if (term12 == null) term12 = string.Empty;
            //			if (term21 == null) term21 = string.Empty;
            //			if (term22 == null) term22 = string.Empty;
            int v1 = string.Compare(term11, term12, StringComparison.Ordinal);
            int v2 = string.Compare(term21, term22, StringComparison.Ordinal);
            return swap ? v1 == 0 ? v2 : v1 : v2 == 0 ? v1 : v2;
        }

        public Dictionary<string, RF_Ref> getRefs()
        {
            return refs;
        }

        internal class BookmarkInfo
        {
            public int count;
            public int total;
        }
    }
}
