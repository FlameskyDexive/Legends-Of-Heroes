using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if UNITY_5_3_OR_NEWER
#endif

namespace ReferenceFinder
{
    // filter, ignore anh huong ket qua thi hien mau do
    // optimize lag duplicate khi use
    [Serializable] internal class SelectHistory
    {
        public bool isSceneAssets;
        public UnityObject[] selection;

        public bool IsTheSame(UnityObject[] objects)
        {
            if (objects.Length != selection.Length) return false;
            var j = 0;
            for (; j < objects.Length; j++)
            {
                if (selection[j] != objects[j]) break;
            }
            return j == objects.Length;
        }
    }
    
    [Serializable] internal class PanelSettings
    {
        public bool selection = false;
        public bool horzLayout = false;
        public bool scene = true;
        public bool asset = true;
        public bool details = false;
        public bool bookmark = false;
        public bool toolMode = false;
        
        public bool showFullPath = true;
        public bool showFileSize = false;
        public bool showFileExtension = false;
        public bool showUsageType = true;
        
        public RF_RefDrawer.Mode toolGroupMode = RF_RefDrawer.Mode.Type;
        public RF_RefDrawer.Mode groupMode = RF_RefDrawer.Mode.Dependency;
        public RF_RefDrawer.Sort sortMode = RF_RefDrawer.Sort.Path;

        public int historyIndex;
        public List<SelectHistory> history = new List<SelectHistory>();
    } 
    
    internal class RfWindowAll : RF_WindowBase, IHasCustomMenu
    {
        [SerializeField] internal PanelSettings settings = new PanelSettings();
        
        [MenuItem("Tools/ReferenceFinder")]
        private static void ShowWindow()
        {
            var _window = CreateInstance<RfWindowAll>();
            _window.InitIfNeeded();
            RF_Unity.SetWindowTitle(_window, "ReferenceFinder");
            _window.Show();
        }
        
        [NonSerialized] internal RF_Bookmark bookmark;
        [NonSerialized] internal RF_Selection selection;
        [NonSerialized] internal RF_UsedInBuild UsedInBuild;
        [NonSerialized] internal RF_DuplicateTree2 Duplicated;
        [NonSerialized] internal RF_RefDrawer RefUnUse;

        [NonSerialized] internal RF_RefDrawer UsesDrawer; // [Selected Assets] are [USING] (depends on / contains reference to) ---> those assets
        [NonSerialized] internal RF_RefDrawer UsedByDrawer; // [Selected Assets] are [USED BY] <---- those assets 
        [NonSerialized] internal RF_RefDrawer SceneToAssetDrawer; // [Selected GameObjects in current Scene] are [USING] ---> those assets
        [NonSerialized] internal RF_AddressableDrawer AddressableDrawer;
        
        
        [NonSerialized] internal RF_RefDrawer RefInScene; // [Selected Assets] are [USED BY] <---- those components in current Scene 
        [NonSerialized] internal RF_RefDrawer SceneUsesDrawer; // [Selected GameObjects] are [USING] ---> those components / GameObjects in current scene
        [NonSerialized] internal RF_RefDrawer RefSceneInScene; // [Selected GameObjects] are [USED BY] <---- those components / GameObjects in current scene


        internal int level;
        private Vector2 scrollPos;
        private string tempGUID;
        private string tempFileID;
        private UnityObject tempObject;

        protected bool lockSelection => (selection != null) && selection.isLock;

        private void OnEnable()
        {
            Repaint();
        }
        
        protected void InitIfNeeded()
        {
            if (UsesDrawer != null) return;

            UsesDrawer = new RF_RefDrawer(this, ()=> settings.sortMode, ()=> settings.groupMode)
            {
                messageEmpty = "[Selected Assets] are not [USING] (depends on / contains reference to) any other assets!"
            };

            UsedByDrawer = new RF_RefDrawer(this, ()=> settings.sortMode, ()=> settings.groupMode)
            {
                messageEmpty = "[Selected Assets] are not [USED BY] any other assets!"
            };

            AddressableDrawer = new RF_AddressableDrawer(this, () => settings.sortMode, () => settings.groupMode);
            
            Duplicated = new RF_DuplicateTree2(this, ()=> settings.sortMode, ()=> settings.toolGroupMode);
            SceneToAssetDrawer = new RF_RefDrawer(this, ()=> settings.sortMode, ()=> settings.groupMode)
            {
                messageEmpty = "[Selected GameObjects] (in current open scenes) are not [USING] any assets!"
            };

            RefUnUse = new RF_RefDrawer(this, ()=> settings.sortMode, ()=> settings.toolGroupMode)
            {
                groupDrawer =
                {
                    hideGroupIfPossible = true
                }
            };

            UsedInBuild = new RF_UsedInBuild(this, ()=> settings.sortMode, ()=> settings.toolGroupMode);
            bookmark = new RF_Bookmark(this, ()=> settings.sortMode, ()=> settings.groupMode);
            selection = new RF_Selection(this,()=> settings.sortMode, ()=> settings.groupMode);

            SceneUsesDrawer = new RF_RefDrawer(this, ()=> settings.sortMode, ()=> settings.groupMode)
            {
                messageEmpty = "[Selected GameObjects] are not [USING] any other GameObjects in scenes"
            };

            RefInScene = new RF_RefDrawer(this, ()=> settings.sortMode, ()=> settings.groupMode)
            {
                messageEmpty = "[Selected Assets] are not [USED BY] any GameObjects in opening scenes!"
            };

            RefSceneInScene = new RF_RefDrawer(this, ()=> settings.sortMode, ()=> settings.groupMode)
            {
                messageEmpty = "[Selected GameObjects] are not [USED BY] by any GameObjects in opening scenes!"
            };

#if UNITY_2018_OR_NEWER
            UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
            UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
#elif UNITY_2017_OR_NEWER
            UnityEditor.SceneManagement.EditorSceneManager.activeSceneChanged -= OnSceneChanged;
            UnityEditor.SceneManagement.EditorSceneManager.activeSceneChanged += OnSceneChanged;
#endif

            RF_Cache.onReady -= OnReady;
            RF_Cache.onReady += OnReady;

            RF_Setting.OnIgnoreChange -= OnIgnoreChanged;
            RF_Setting.OnIgnoreChange += OnIgnoreChanged;

            int idx = settings.historyIndex;
            if (idx != -1 && settings.history.Count > idx)
            {
                SelectHistory h = settings.history[idx];
                Selection.objects = h.selection;
                settings.historyIndex = idx;
                RefreshOnSelectionChange();
                Repaint();
            }

            RefreshShowFullPath();
            RefreshShowFileSize();
            RefreshShowFileExtension();
            RefreshShowUsageType();
            Repaint();
        }

#if UNITY_2018_OR_NEWER
        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            if (IsFocusingFindInScene || IsFocusingSceneToAsset || IsFocusingSceneInScene)
            {
                OnSelectionChange();
            }
        }
#endif
        protected void OnIgnoreChanged()
        {
            RefUnUse.ResetUnusedAsset();
            UsedInBuild.SetDirty();
            OnSelectionChange();
        }
        protected void OnCSVClick()
        {
            RF_Ref[] csvSource = null;
            RF_RefDrawer drawer = GetAssetDrawer();

            if (drawer != null) csvSource = drawer.source;

            if (isFocusingUnused && (csvSource == null)) csvSource = RefUnUse.source;

            //if (csvSource != null) Debug.Log("d : " + csvSource.Length);
            if (isFocusingUsedInBuild && (csvSource == null)) csvSource = RF_Ref.FromDict(UsedInBuild.refs);

            //if (csvSource != null) Debug.Log("e : " + csvSource.Length);
            if (isFocusingDuplicate && (csvSource == null)) csvSource = RF_Ref.FromList(Duplicated.list);

            //if (csvSource != null) Debug.Log("f : " + csvSource.Length);
            RF_Export.ExportCSV(csvSource);
        }

        protected void OnReady()
        {
            OnSelectionChange();
        }
        
        void AddHistory()
        {
            UnityObject[] objects = Selection.objects;
            
            // Check if the same set of selection has already existed
            RefreshHistoryIndex(objects);
            if (settings.historyIndex != -1) return;
            
            // Add newly selected objects to the selection
            const int MAX_HISTORY_LENGTH = 10;
            settings.history.Add(new SelectHistory { selection =  Selection.objects});
            settings.historyIndex = settings.history.Count - 1;
            if (settings.history.Count > MAX_HISTORY_LENGTH)
            {
                settings.history.RemoveRange(0, settings.history.Count-MAX_HISTORY_LENGTH);
            }
            EditorUtility.SetDirty(this);
        }

        void RefreshHistoryIndex(UnityObject[] objects)
        {
            if (this == null) return;
            
            settings.historyIndex = -1;
            if (objects == null || objects.Length == 0) return;
            List<SelectHistory> history = settings.history;
            for (var i = 0; i < history.Count; i++)
            {
                SelectHistory h = history[i];
                if (!h.IsTheSame(objects)) continue;
                settings.historyIndex = i;
            }
            
            EditorUtility.SetDirty(this);
        }

        bool isScenePanelVisible {
            get
            {
                if (isFocusingAddressable) return false;
                
                if (selection.isSelectingAsset && isFocusingUses) // Override
                {
                    return false;
                }
                
                if (!selection.isSelectingAsset && isFocusingUsedBy)
                {
                    return true;
                }

                return settings.scene;
            }
        }
        
        bool isAssetPanelVisible
        {
            get
            {
                if (isFocusingAddressable) return false;
                
                if (selection.isSelectingAsset && isFocusingUses) // Override
                {
                    return true;
                }
                
                if (!selection.isSelectingAsset && isFocusingUsedBy)
                {
                    return false;
                }
                
                return settings.asset;
            }
        }

        void RefreshPanelVisible()
        {
            if (sp1 == null || sp2 == null) return;
            sp2.splits[0].visible = isScenePanelVisible;
            sp2.splits[1].visible = isAssetPanelVisible;
            sp2.splits[2].visible = isFocusingAddressable;
            sp2.CalculateWeight();
        }
        
        void RefreshOnSelectionChange()
        {
            ids = RF_Unity.Selection_AssetGUIDs;
            selection.Clear();

            var gameObjects = Selection.gameObjects;
            
            //ignore selection on asset when selected any object in scene
            if ((gameObjects.Length > 0) && !RF_Unity.IsInAsset(gameObjects[0]))
            {
                ids = Array.Empty<string>();
                selection.AddRange(gameObjects);
            } else
            {
                selection.AddRange(ids);
            }
            
            level = 0;
            RefreshPanelVisible();
            
            if (selection.isSelectingAsset)
            {
                UsesDrawer.Reset(ids, true);
                UsedByDrawer.Reset(ids, false);
                RefInScene.Reset(ids, this as IWindow);
                AddressableDrawer.RefreshView();

            } else
            {
                RefSceneInScene.ResetSceneInScene(gameObjects);
                SceneToAssetDrawer.Reset(gameObjects, true, true);
                SceneUsesDrawer.ResetSceneUseSceneObjects(gameObjects);
            }
        }

        public override void OnSelectionChange()
        {
            Repaint();

            isNoticeIgnore = false;
            if (!RF_Cache.isReady) return;

            if (focusedWindow == null) return;
            if (SceneUsesDrawer == null) InitIfNeeded();
            if (UsesDrawer == null) InitIfNeeded();

            if (!lockSelection)
            {
                RefreshOnSelectionChange();
                RefreshHistoryIndex(Selection.objects);
            }

            if (isFocusingGUIDs)
            {
                //guidObjs = new Object[ids.Length];
                guidObjs = new Dictionary<string, UnityObject>();
                UnityObject[] objects = Selection.objects;
                for (var i = 0; i < objects.Length; i++)
                {
                    UnityObject item = objects[i];

#if UNITY_2018_1_OR_NEWER
                    {
                        var guid = "";
                        long fileid = -1;
                        try
                        {
                            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(item, out guid, out fileid))
                            {
                                guidObjs.Add(guid + "/" + fileid, objects[i]);
                            }

                            //Debug.Log("guid: " + guid + "  fileID: " + fileid);
                        } catch { }
                    }
#else
					{
						var path = AssetDatabase.GetAssetPath(item);
                        if (string.IsNullOrEmpty(path)) continue;
                        var guid = AssetDatabase.AssetPathToGUID(path);
                        System.Reflection.PropertyInfo inspectorModeInfo =
                        typeof(SerializedObject).GetProperty("inspectorMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                        SerializedObject serializedObject = new SerializedObject(item);
                        inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);

                        SerializedProperty localIdProp =
                            serializedObject.FindProperty("m_LocalIdentfierInFile");   //note the misspelling!

                        var localId = localIdProp.longValue;
                        if (localId <= 0)
                        {
                            localId = localIdProp.intValue;
                        }
                        if (localId <= 0)
                        {
                            continue;
                        }
                        if (!string.IsNullOrEmpty(guid)) guidObjs.Add(guid + "/" + localId, objects[i]);
					}
#endif
                }
            }

            if (isFocusingUnused)
            {
                RefUnUse.ResetUnusedAsset();
            }

            if (RF_SceneCache.Api.Dirty && !Application.isPlaying)
            {
                RF_SceneCache.Api.refreshCache(this);
            }

            EditorApplication.delayCall -= Repaint;
            EditorApplication.delayCall += Repaint;
        }


        [NonSerialized] public RF_SplitView sp1; // container : Selection / sp2 / Bookmark 
        [NonSerialized] public RF_SplitView sp2; // Scene / Assets
        [NonSerialized] public RF_SplitView sp3; // Addressable
        
        private void DrawHistory(Rect rect)
        {
            Color c = GUI.backgroundColor;
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            
            for (var i = 0; i < settings.history.Count; i++)
            {
                SelectHistory h = settings.history[i];
                int idx = i;
                GUI.backgroundColor = i == settings.historyIndex ? GUI2.darkBlue : c;

                var content = new GUIContent($"{i + 1}", "RightClick to delete!");
                if (GUILayout.Button(content, EditorStyles.miniButton, GUI2.GLW_24))
                {
                    // Debug.Log($"Button: {Event.current.button}");
                    
                    if (Event.current.button == 0) // left click
                    {
                        Selection.objects = h.selection;
                        settings.historyIndex = idx;
                        RefreshOnSelectionChange();
                        Repaint();    
                    }

                    if (Event.current.button == 1) // right click
                    {
                        bool isActive = i == settings.historyIndex;
                        settings.history.RemoveAt(idx);
                        
                        if (isActive && settings.history.Count > 0)
                        {
                            int idx2 = settings.history.Count - 1;
                            Selection.objects = settings.history[idx2].selection;
                            settings.historyIndex = idx2;
                            RefreshOnSelectionChange();
                            Repaint();
                        }
                    }
                }
                
                
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUI.backgroundColor = c;
        }

        private void InitPanes()
        {
            sp2 = new RF_SplitView(this)
            {
                isHorz = false,
                splits = new List<RF_SplitView.Info>
                {
                    new RF_SplitView.Info
                        { title = new GUIContent("Scene", RF_Icon.Scene.image), draw = DrawScene, visible = settings.scene},
                    new RF_SplitView.Info
                        { title = new GUIContent("Assets", RF_Icon.Asset.image), draw = DrawAsset, visible = settings.asset },
                    new RF_SplitView.Info
                        {title = null, draw = rect => AddressableDrawer.Draw(rect), visible = false }
                }
            };

            sp2.CalculateWeight();
            
            sp1 = new RF_SplitView(this)
            {
                isHorz = true,
                splits = new List<RF_SplitView.Info>
                {
                    new RF_SplitView.Info
                        { 
                            title = new GUIContent("Selection", RF_Icon.Selection.image),
                            weight = 0.4f,
                            visible = settings.selection,
                            draw = rect =>
                            {
                                Rect historyRect = rect;
                                historyRect.yMin = historyRect.yMax - 16f;
                                
                                rect.yMax -= 16f;
                                selection.Draw(rect);
                                DrawHistory(historyRect);
                            }
                        },
                    new RF_SplitView.Info
                    {
                        draw = r =>
                        {
                            sp2.Draw(r);
                        }
                    },
                    new RF_SplitView.Info
                    {
                        title = new GUIContent("Asset Detail", RF_Icon.Details.image), weight = 0.4f, visible = settings.details, draw = rect =>
                        {
                            RF_RefDrawer assetDrawer = GetAssetDrawer();
                            if (assetDrawer != null) assetDrawer.DrawDetails(rect);
                        }
                    },
                    new RF_SplitView.Info
                        { title = new GUIContent("Bookmark", RF_Icon.Favorite.image), weight = 0.4f, visible = settings.bookmark, draw = rect => bookmark.Draw(rect) }
                }
            };

            sp1.CalculateWeight();
        }

        private RF_TabView tabs;
        private RF_TabView toolTabs;
        private RF_TabView bottomTabs;
        private RF_SearchView search;
        
        private void DrawScene(Rect rect)
        {
            RF_RefDrawer drawer = isFocusingUses
                ? selection.isSelectingAsset ? null : SceneUsesDrawer
                : selection.isSelectingAsset
                    ? RefInScene
                    : RefSceneInScene;
            if (drawer == null) return;

            if (!RF_SceneCache.ready)
            {
                Rect rr = rect;
                rr.height = 16f;

                int cur = RF_SceneCache.Api.current, total = RF_SceneCache.Api.total;
                EditorGUI.ProgressBar(rr, cur * 1f / total, $"{cur} / {total}");
                WillRepaint = true;
                return;
            }

            drawer.Draw(rect);

            var refreshRect = new Rect(rect.xMax - 16f, rect.yMin - 14f, 18f, 18f);
            if (GUI2.ColorIconButton(refreshRect, RF_Icon.Refresh.image,
                RF_SceneCache.Api.Dirty ? GUI2.lightRed : (Color?)null))
            {
                RF_SceneCache.Api.refreshCache(drawer.window);
            }
        }



        private RF_RefDrawer GetAssetDrawer()
        {
            if (isFocusingUses) return selection.isSelectingAsset ? UsesDrawer : SceneToAssetDrawer;
            if (isFocusingUsedBy) return selection.isSelectingAsset ? UsedByDrawer : null;
            if (isFocusingAddressable) return AddressableDrawer.drawer;
            return null;
        }

        private void DrawAsset(Rect rect)
        {
            RF_RefDrawer drawer = GetAssetDrawer();
            if (drawer == null) return;
            drawer.Draw(rect);

            if (!drawer.showDetail) return;
            
            settings.details = true;
            drawer.showDetail = false;
            sp1.splits[2].visible = settings.details;
            sp1.CalculateWeight();
            Repaint();
        }

        private void DrawSearch()
        {
            if (search == null) search = new RF_SearchView();
            search.DrawLayout();
        }

        protected override void OnGUI()
        {
			// UnityEngine.Profiling.Profiler.BeginSample("RF-OnGUI");
            // {
                OnGUI2();
            // }
			// UnityEngine.Profiling.Profiler.EndSample();
        }
        
        protected bool CheckDrawImport()
        {
            if (RF_Unity.isEditorCompiling)
            {
                EditorGUILayout.HelpBox("Compiling scripts, please wait!", MessageType.Warning);
                Repaint();
                return false;
            }

            if (RF_Unity.isEditorUpdating)
            {
                EditorGUILayout.HelpBox("Importing assets, please wait!", MessageType.Warning);
                Repaint();
                return false;
            }

            InitIfNeeded();

            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                EditorGUILayout.HelpBox("RF requires serialization mode set to FORCE TEXT!", MessageType.Warning);
                if (GUILayout.Button("FORCE TEXT")) EditorSettings.serializationMode = SerializationMode.ForceText;

                return false;
            }

            if (RF_Cache.hasCache && !RF_Cache.CheckSameVersion())
            {
                EditorGUILayout.HelpBox("Incompatible cache version found!!!\nRF will need a full refresh and according to your project's size this process may take several minutes to complete finish!",
                    MessageType.Warning);
                // RF_Cache.DrawPriorityGUI();
                if (GUILayout.Button("Scan project"))
                {
                    RF_Cache.DeleteCache();
                    RF_Cache.CreateCache();
                }

                return false;
            }

            if (RF_Cache.isReady) return DrawEnable();
            
            if (!RF_Cache.hasCache)
            {
                EditorGUILayout.HelpBox(
                    "RF cache not found!\nA first scan is needed to build the cache for all asset references.\nDepending on the size of your project, this process may take a few minutes to complete but once finished, searching for asset references will be incredibly fast!",
                    MessageType.Warning);

                // RF_Cache.DrawPriorityGUI();

                if (GUILayout.Button("Scan project"))
                {
                    RF_Cache.CreateCache();
                    Repaint();
                }

                return false;
            }
            // RF_Cache.DrawPriorityGUI();
            if (!DrawEnable()) return false;
            
            RF_Cache api = RF_Cache.Api;
            if (api.workCount > 0)
            {
                string text = "Refreshing ... " + (int)(api.progress * api.workCount) + " / " + api.workCount;
                Rect rect = GUILayoutUtility.GetRect(1f, Screen.width, 18f, 18f);
                EditorGUI.ProgressBar(rect, api.progress, text);
                Repaint();    
            } else
            {
                // Debug.LogWarning("DONE????");
                api.workCount = 0;
                api.ready = true;
            }
            
            return false;
        }

        protected bool isFocusingUses => (tabs != null) && (tabs.current == 0);
        protected bool isFocusingUsedBy => (tabs != null) && (tabs.current == 1);
        protected bool isFocusingAddressable => (tabs != null) && (tabs.current == 2);
        
        // 
        protected bool isFocusingDuplicate => (toolTabs != null) && (toolTabs.current == 0);
        protected bool isFocusingGUIDs => (toolTabs != null) && (toolTabs.current == 1);
        protected bool isFocusingUnused => (toolTabs != null) && (toolTabs.current == 2);
        protected bool isFocusingUsedInBuild => (toolTabs != null) && (toolTabs.current == 3);
        
        private static readonly HashSet<RF_RefDrawer.Mode> allowedModes = new HashSet<RF_RefDrawer.Mode>()
        {
            RF_RefDrawer.Mode.Type,
            RF_RefDrawer.Mode.Extension,
            RF_RefDrawer.Mode.Folder
        };
        
        private void OnTabChange()
        {
            if (isFocusingUnused || isFocusingUsedInBuild)
            {
                if (!allowedModes.Contains(settings.groupMode))
                {
                    settings.groupMode = RF_RefDrawer.Mode.Type;
                }
            }
            
            if (deleteUnused != null) deleteUnused.hasConfirm = false;
            if (UsedInBuild != null) UsedInBuild.SetDirty();
        }

        private void InitTabs()
        {
            bottomTabs = RF_TabView.Create(this, true,
                new GUIContent(RF_Icon.Setting.image, "Settings"),
                new GUIContent(RF_Icon.Ignore.image, "Ignore"),
                new GUIContent(RF_Icon.Filter.image, "Filter by Type")
            );
            bottomTabs.current = -1;
            bottomTabs.flexibleWidth = false;
            
            toolTabs = RF_TabView.Create(this, false, "Duplicate", "GUID", "Not Referenced", "In Build");

            
            if (RF_Addressable.asmStatus == RF_Addressable.ASMStatus.AsmNotFound)
            { // No Addressable
                tabs = RF_TabView.Create(this, false, // , "Tools"
                    "Uses", "Used By"
                );
            }
            else
            {
                tabs = RF_TabView.Create(this, false, // , "Tools"
                    "Uses", "Used By", "Addressables"
                );
            }
            
            
            tabs.onTabChange = OnTabChange;
            
            const float IconW = 24f;
            tabs.offsetFirst = IconW;
            tabs.offsetLast = IconW * 5;
            
            tabs.callback = new DrawCallback
            {
                BeforeDraw = (rect) =>
                {
                    rect.width = IconW;
                    if (GUI2.ToolbarToggle(ref selection.isLock,
                        selection.isLock ? RF_Icon.Lock.image : RF_Icon.Unlock.image,
                        Vector2.zero, "Lock Selection", rect))
                    {
                        WillRepaint = true;
                        OnSelectionChange();
                        if (selection.isLock) AddHistory();
                    }
                },
                
                AfterDraw = (rect) =>
                {
                    rect.xMin = rect.xMax - IconW * 5;
                    rect.width = IconW;
                    
                    if (GUI2.ToolbarToggle(ref settings.selection,
                        RF_Icon.Selection.image,
                        Vector2.zero, "Show / Hide Selection", rect))
                    {
                        sp1.splits[0].visible = settings.selection;
                        sp1.CalculateWeight();
                        Repaint();
                    }
                    
                    rect.x += IconW;
                    if (GUI2.ToolbarToggle(ref settings.scene, RF_Icon.Scene.image, Vector2.zero, "Show / Hide Scene References", rect))
                    {
                        if (settings.asset == false && settings.scene == false)
                        {
                            settings.asset = true;
                            sp2.splits[1].visible = settings.asset;
                        }

                        RefreshPanelVisible();
                        Repaint();
                    }
                    
                    rect.x += IconW;
                    if (GUI2.ToolbarToggle(ref settings.asset, RF_Icon.Asset.image, Vector2.zero, "Show / Hide Asset References", rect))
                    {
                        if (settings.asset == false && settings.scene == false)
                        {
                            settings.scene = true;
                            sp2.splits[0].visible = settings.scene;
                        }
                        
                        RefreshPanelVisible();
                        Repaint();
                    }
                    
                    rect.x += IconW;
                    if (GUI2.ToolbarToggle(ref settings.details, RF_Icon.Details.image, Vector2.zero, "Show / Hide Details", rect))
                    {
                        sp1.splits[2].visible = settings.details;
                        sp1.CalculateWeight();
                        Repaint();
                    }
                    
                    rect.x += IconW;
                    if (GUI2.ToolbarToggle(ref settings.bookmark, RF_Icon.Favorite.image, Vector2.zero, "Show / Hide Bookmarks", rect))
                    {
                        sp1.splits[3].visible = settings.bookmark;
                        sp1.CalculateWeight();
                        Repaint();
                    }
                }
            };
        }
        
        protected bool DrawFooter()
        {
            bottomTabs.DrawLayout();
            var bottomBar = GUILayoutUtility.GetLastRect();
            bottomBar.xMin += 100f; // offset for left buttons
            
            var (fullPathRect, flex1) = bottomBar.ExtractLeft(24f);
            var (fileSizeRect, flex2) = flex1.ExtractLeft(24f);
            var (extensionRect, flex3) = flex2.ExtractLeft(24f);
            
            var (buttonRect, _) = flex3.ExtractRight(24f);
            bottomBar = flex3;
            
            var viewModeRect = bottomBar;
            viewModeRect.xMax -= 24f;
            viewModeRect.xMin = viewModeRect.xMax - 200f;
            
            DrawViewModes(viewModeRect);
            DrawButton(buttonRect, ref settings.toolMode, RF_Icon.CustomTool);
            if (DrawButton(fullPathRect, ref settings.showFullPath, RF_Icon.FullPath))
            {
                RefreshShowFullPath();
            }
            if (DrawButton(fileSizeRect, ref settings.showFileSize, RF_Icon.Filesize))
            {
                RefreshShowFileSize();
            }
            if (DrawButton(extensionRect, ref settings.showFileExtension, RF_Icon.FileExtension))
            {
                RefreshShowFileExtension();
            }
            
            return false;
        }


        private bool DrawButton(Rect rect, ref bool show, GUIContent icon)
        {
            var changed = false;
            Color oColor = GUI.color;
            if (show) GUI.color = new Color(0.7f, 1f, 0.7f, 1f);
            {
                if (GUI.Button(rect, icon, EditorStyles.toolbarButton))
                {
                    show = !show;
                    EditorUtility.SetDirty(this);
                    WillRepaint = true;
                    changed = true;
                }    
            }
            GUI.color = oColor;
            return changed;
        }

        private void DrawAssetViewSettings()
        {
            bool isDisable = !sp2.splits[1].visible;
            EditorGUI.BeginDisabledGroup(isDisable);
            {
                GUI2.ToolbarToggle(ref RF_Setting.s.displayAssetBundleName, RF_Icon.AssetBundle.image, Vector2.zero, "Show / Hide Assetbundle Names");
#if UNITY_2017_1_OR_NEWER
                GUI2.ToolbarToggle(ref RF_Setting.s.displayAtlasName, RF_Icon.Atlas.image, Vector2.zero, "Show / Hide Atlas packing tags");
#endif
                GUI2.ToolbarToggle(ref RF_Setting.s.showUsedByClassed, RF_Icon.Material.image, Vector2.zero, "Show / Hide usage icons");
                // GUI2.ToolbarToggle(ref RF_Setting.s.displayFileSize, RF_Icon.Filesize.image, Vector2.zero, "Show / Hide file size");

                if (GUILayout.Button("CSV", EditorStyles.toolbarButton)) OnCSVClick();
            }
            EditorGUI.EndDisabledGroup();
        }

		RF_EnumDrawer groupModeED;
        RF_EnumDrawer toolModeED;
		RF_EnumDrawer sortModeED;
        
        private void DrawViewModes(Rect rect)
        {
            var rect1 = rect;
            rect1.width = rect.width / 2f;
            
            var rect2 = rect1;
            rect2.x += rect1.width;
            
            if (toolModeED == null) toolModeED = new RF_EnumDrawer()
            {
                fr2_enum = new RF_EnumDrawer.EnumInfo(
                    RF_RefDrawer.Mode.Type,
                    RF_RefDrawer.Mode.Folder,
                    RF_RefDrawer.Mode.Extension
                )
            };
            if (groupModeED == null) groupModeED = new RF_EnumDrawer(){tooltip = "Group By"};
			if (sortModeED == null) sortModeED = new RF_EnumDrawer(){tooltip = "Sort By"};

            if (settings.toolMode)
            {
                var tMode = settings.toolGroupMode;
                    if (toolModeED.Draw(rect1, ref tMode))
                {
                    settings.toolGroupMode = tMode;
                    markDirty();
                    RefreshSort();
                }
            } else
            {
                var gMode = settings.groupMode;
                    if (groupModeED.Draw(rect1, ref gMode))
                {
                    // Debug.Log($"GroupMode: {gMode}");
                    settings.groupMode = gMode;
                    markDirty();
                    RefreshSort();
                }
            }
            
            // GUILayout.Space(16f);
            var sMode = settings.sortMode;
                if (sortModeED.Draw(rect2, ref sMode))
            {
                // Debug.Log($"sortMode: {sMode}");
                settings.sortMode = sMode;
                RefreshSort();
            }
            
            
        }
        
        // Save status to temp variable so the result will be consistent between Layout & Repaint
        internal static int delayRepaint;
        internal static bool checkDrawImportResult;
        
        
        protected void OnGUI2()
        {
            if (Event.current.type == EventType.Layout)
            {
                RF_Unity.RefreshEditorStatus();
            }

            if (RF_SettingExt.disable)
            {
                DrawEnable();
                return;
            }
            
            // GUILayout.Label($"OnGUI2: \ndisable={RF_SettingExt.disable} | \nInited={RF_CacheHelper.inited} | \nisReady={RF_Cache.isReady}");
            if (!RF_CacheHelper.inited)
            {
                RF_CacheHelper.InitHelper();
            }
            
            if (tabs == null) InitTabs();
            if (sp1 == null) InitPanes();
            
            bool result = CheckDrawImport();
            if (Event.current.type == EventType.Layout)
            {
                checkDrawImportResult = result;
            }
            
            if (!checkDrawImportResult)
            {
                return;
            }
            
            if (settings.toolMode)
            {
                EditorGUILayout.HelpBox(RF_GUIContent.From("Tools are POWERFUL & DANGEROUS! Only use if you know what you are doing!!!", RF_Icon.Warning.image));
                toolTabs.DrawLayout();
                DrawTools();
            }
            else
            {   
                tabs.DrawLayout();
                sp1.DrawLayout();
            }
            
            DrawSettings();
            DrawFooter();
            if (!WillRepaint) return;
            WillRepaint = false;
            Repaint();
        }


        private RF_DeleteButton deleteUnused;
        
        private void DrawTools()
        {
            if (isFocusingDuplicate)
            {
                Duplicated.DrawLayout();
                GUILayout.FlexibleSpace();
                return;
            }

            if (isFocusingUnused)
            {
                if ((RefUnUse.refs != null) && (RefUnUse.refs.Count == 0))
                {
                    EditorGUILayout.HelpBox("Wow! So clean!?", MessageType.Info);
                    EditorGUILayout.HelpBox("Your project does not has have any unused assets, or have you just hit DELETE ALL?", MessageType.Info);
                    EditorGUILayout.HelpBox("Your backups are placed at Library/RF/ just in case you want your assets back!", MessageType.Info);
                } else
                {
                    RefUnUse.DrawLayout();
                    
                    if (deleteUnused == null) deleteUnused = new RF_DeleteButton
                    {
                        warningMessage = "A backup (.unitypackage) will be created so you can reimport the deleted assets later!",
                        deleteLabel = RF_GUIContent.From("DELETE ASSETS", RF_Icon.Delete.image),
                        confirmMessage = "Create backup at Library/RF/"
                    };
                    
                    GUILayout.BeginHorizontal();
                    {
                        deleteUnused.Draw(() => { RF_Unity.BackupAndDeleteAssets(RefUnUse.source); });
                    }
                    GUILayout.EndHorizontal();
                    
                    // Rect toolRect = GUILayoutUtility.GetRect(0, Screen.width, 40, 40f);
                    // toolRect.yMin = toolRect.yMax;
                    //
                    // Rect lineRect = toolRect;
                    // lineRect.height = 1f;
                    //
                    // GUI2.Rect(lineRect, Color.black, 0.5f);
                    //
                    //
                    // GUILayout.BeginArea(toolRect);
                    // deleteUnused.Draw(() => { RF_Unity.BackupAndDeleteAssets(RefUnUse.source); });
                    // GUILayout.EndArea();
                }
                return;
            }

            if (isFocusingUsedInBuild)
            {
                UsedInBuild.DrawLayout();
                return;
            }
            
            if (isFocusingGUIDs)
            {
                DrawGUIDs();
            }
        }

        private void DrawSettings()
        {
            if (bottomTabs.current == -1) return;

            GUILayout.BeginVertical(GUILayout.Height(100f));
            {
                GUILayout.Space(2f);
                switch (bottomTabs.current)
                {
                case 0:
                    {
                        RF_Setting.s.DrawSettings();
                        break;
                    }

                case 1:
                    {
                        if (AssetType.DrawIgnoreFolder()) markDirty();
                        break;
                    }

                case 2:
                    {
                        if (AssetType.DrawSearchFilter()) markDirty();
                        break;
                    }
                }
            }
            GUILayout.EndVertical();

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.height = 1f;
            GUI2.Rect(rect, Color.black, 0.4f);
        }

        protected void markDirty()
        {
            UsedByDrawer.SetDirty();
            UsesDrawer.SetDirty();
            Duplicated.SetDirty();
            AddressableDrawer.RefreshSort();
            SceneToAssetDrawer.SetDirty();
            RefUnUse.SetDirty();

            RefInScene.SetDirty();
            RefSceneInScene.SetDirty();
            SceneUsesDrawer.SetDirty();
            UsedInBuild.SetDirty();
            WillRepaint = true;
        }

        protected void RefreshShowFileExtension()
        {
            RefUnUse.drawExtension = settings.showFileExtension;
            UsesDrawer.drawExtension = settings.showFileExtension;
            UsedByDrawer.drawExtension = settings.showFileExtension;
            SceneToAssetDrawer.drawExtension = settings.showFileExtension;
            RefInScene.drawExtension = settings.showFileExtension;
            SceneUsesDrawer.drawExtension = settings.showFileExtension;
            RefSceneInScene.drawExtension = settings.showFileExtension;
        }

        protected void RefreshShowFullPath()
        {
            RefUnUse.drawFullPath = settings.showFullPath;
            UsesDrawer.drawFullPath = settings.showFullPath;
            UsedByDrawer.drawFullPath = settings.showFullPath;
            SceneToAssetDrawer.drawFullPath = settings.showFullPath;
            RefInScene.drawFullPath = settings.showFullPath;
            SceneUsesDrawer.drawFullPath = settings.showFullPath;
            RefSceneInScene.drawFullPath = settings.showFullPath;
        }

        protected void RefreshShowFileSize()
        {
            RefUnUse.drawFileSize = settings.showFileSize;
            UsesDrawer.drawFileSize = settings.showFileSize;
            UsedByDrawer.drawFileSize = settings.showFileSize;
            SceneToAssetDrawer.drawFileSize = settings.showFileSize;
            RefInScene.drawFileSize = settings.showFileSize;
            SceneUsesDrawer.drawFileSize = settings.showFileSize;
            RefSceneInScene.drawFileSize = settings.showFileSize;
        }
        
        protected void RefreshShowUsageType()
        {
            RefUnUse.drawUsageType = settings.showUsageType;
            UsesDrawer.drawUsageType = settings.showUsageType;
            UsedByDrawer.drawUsageType = settings.showUsageType;
            SceneToAssetDrawer.drawUsageType = settings.showUsageType;
            RefInScene.drawUsageType = settings.showUsageType;
            SceneUsesDrawer.drawUsageType = settings.showUsageType;
            RefSceneInScene.drawUsageType = settings.showUsageType;
        }
        
        

        protected void RefreshSort()
        {
            UsedByDrawer.RefreshSort();
            UsesDrawer.RefreshSort();  
            AddressableDrawer.RefreshSort();
                
            Duplicated.RefreshSort();
            SceneToAssetDrawer.RefreshSort();
            RefUnUse.RefreshSort();
            UsedInBuild.RefreshSort();
        }

        // public bool isExcludeByFilter;

        // protected bool checkNoticeFilter()
        // {
        //     var rsl = false;
        //
        //     if (IsFocusingUsedBy && !rsl) rsl = UsedByDrawer.isExclueAnyItem();
        //
        //     if (IsFocusingDuplicate) return Duplicated.isExclueAnyItem();
        //
        //     if (IsFocusingUses && (rsl == false)) rsl = UsesDrawer.isExclueAnyItem();
        //
        //     //tab use by
        //     return rsl;
        // }
        //
        // protected bool checkNoticeIgnore()
        // {
        //     bool rsl = isNoticeIgnore;
        //     return rsl;
        // }


        private Dictionary<string, UnityObject> guidObjs;
        private string[] ids;

        private void DrawGUIDs()
        {
            GUILayout.Label("GUID to Object", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            {
                string guid = EditorGUILayout.TextField(tempGUID ?? string.Empty);
                string fileId = EditorGUILayout.TextField(tempFileID ?? string.Empty);
                EditorGUILayout.ObjectField(tempObject, typeof(UnityObject), false, GUI2.GLW_160);

                if (GUILayout.Button("Paste", EditorStyles.miniButton, GUI2.GLW_70))
                {
                    string[] split = EditorGUIUtility.systemCopyBuffer.Split('/');
                    guid = split[0];
                    fileId = split.Length == 2 ? split[1] : string.Empty;
                }

                if ((guid != tempGUID || fileId != tempFileID) && !string.IsNullOrEmpty(guid))
                {
                    tempGUID = guid;
                    tempFileID = fileId;
                    string fullId = string.IsNullOrEmpty(fileId) ? tempGUID : tempGUID + "/" + tempFileID;

                    tempObject = RF_Unity.LoadAssetAtPath<UnityObject>
                    (
                        AssetDatabase.GUIDToAssetPath(fullId)
                    );
                }

                if (GUILayout.Button("Set FileID"))
                {
                    var newDict = new Dictionary<string, UnityObject>();
                    foreach (KeyValuePair<string, UnityObject> kvp in guidObjs)
                    {
                        string key = kvp.Key.Split('/')[0];
                        if (!string.IsNullOrEmpty(fileId)) key = key + "/" + fileId;

                        var value = RF_Unity.LoadAssetAtPath<UnityObject>
                        (
                            AssetDatabase.GUIDToAssetPath(key)
                        );
                        newDict.Add(key, value);
                    }

                    guidObjs = newDict;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            if (guidObjs == null) // || ids == null)
                return;

            //GUILayout.Label("Selection", EditorStyles.boldLabel);
            //if (ids.Length == guidObjs.Count)
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                {
                    //for (var i = 0; i < ids.Length; i++)
                    foreach (KeyValuePair<string, UnityObject> item in guidObjs)
                    {
                        //if (!guidObjs.ContainsKey(ids[i])) continue;

                        GUILayout.BeginHorizontal();
                        {
                            //var obj = guidObjs[ids[i]];
                            UnityObject obj = item.Value;

                            EditorGUILayout.ObjectField(obj, typeof(UnityObject), false, GUI2.GLW_150);
                            string idi = item.Key;
                            GUILayout.TextField(idi, GUI2.GLW_320);
                            if (GUILayout.Button(RF_GUIContent.FromString("Copy"), EditorStyles.miniButton, GUI2.GLW_50))
                            {
                                tempObject = obj;

                                //EditorGUIUtility.systemCopyBuffer = tempGUID = item.Key;
                                string[] arr = item.Key.Split('/');
                                tempGUID = arr[0];
                                tempFileID = arr[1];

                                //string guid = "";
                                //long file = -1;
                                //if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out file))
                                //{
                                //    EditorGUIUtility.systemCopyBuffer = tempGUID = idi + "/" + file;

                                //    if (!string.IsNullOrEmpty(tempGUID))
                                //    {
                                //        tempObject = obj;
                                //    }
                                //}  
                            }

                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Merge Selection To"))
            {
                string fullId = string.IsNullOrEmpty(tempFileID) ? tempGUID : tempGUID + "/" + tempFileID;
                RF_Export.MergeDuplicate(fullId);
            }

            EditorGUILayout.ObjectField(tempObject, typeof(UnityObject), false, GUI2.GLW_120);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
    }
}
