#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.IMGUI.Controls;
using Type = System.Type;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;
// using static VTools.VDebug;
using static VHierarchy.VHierarchyData;
using static VHierarchy.VHierarchyCache;

#if UNITY_6000_3_OR_NEWER
using TreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem<UnityEngine.EntityId>;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<UnityEngine.EntityId>;
#elif UNITY_6000_2_OR_NEWER
using TreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem<int>;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<int>;
#endif




namespace VHierarchy
{
    public static class VHierarchy
    {

        static void WrappedGUI(EditorWindow window)
        {
            var navbarHeight = 26;

            void navbarGui()
            {
                if (!navbars_byWindow.ContainsKey(window))
                    navbars_byWindow[window] = new VHierarchyNavbar(window);

                var navbarRect = window.position.SetPos(0, 0).SetHeight(navbarHeight);


                navbars_byWindow[window].OnGUI(navbarRect);

            }
            void defaultGuiWithOffset()
            {
                var defaultTopBarHeight = 20;
                var topOffset = navbarHeight - defaultTopBarHeight;

                var m_Pos_original = window.GetFieldValue<Rect>("m_Pos");




                GUI.BeginGroup(m_Pos_original.SetPos(0, 0).AddHeightFromBottom(-topOffset));

                window.SetFieldValue("m_Pos", m_Pos_original.AddHeightFromBottom(-topOffset));


                try
                {
                    if (curEvent.isMouseDown && m_Pos_original.IsHovered())
                        t_SceneHierarchyWindow.SetMemberValue("s_LastInteractedHierarchy", window);

                    window.InvokeMethod("DoSceneHierarchy");
                    window.InvokeMethod("ExecuteCommands");

                    // same as SceneHierarchyWindow.OnGUI() but without DoToolbarLayout():

                }
                catch (System.Exception exception)
                {
                    if (exception.InnerException is ExitGUIException)
                        throw exception.InnerException;
                    else
                        throw exception;

                    // GUIUtility.ExitGUI() works by throwing ExitGUIException, which just exits imgui loop and doesn't appear in console
                    // but if ExitGUI is called from a reflected method (DoSceneHierarchy in this case), the exception becomes TargetInvokationException
                    // which gets logged to console (only if debugger is attached, for some reason)
                    // so here in such cases we rethrow the original ExitGUIException

                }


                window.SetFieldValue("m_Pos", m_Pos_original);

                GUI.EndGroup();

            }
            void shadow()
            {
                if (!curEvent.isRepaint) return;

                var shadowLength = 30;
                var shadowPos = 21;
                var shadowGreyscale = isDarkTheme ? .1f : .28f;
                var shadowAlpha = isDarkTheme ? .35f : .15f;

                var minScrollPos = 10;
                var maxScrollPos = 20;

                if (StageUtility.GetCurrentStage() is PrefabStage)
                    shadowPos += 30;
                else if (EditorSceneManager.loadedRootSceneCount > 1)
                    shadowPos += 16;



                var scrollPos = window.GetMemberValue("m_SceneHierarchy").GetMemberValue<TreeViewState>("m_TreeViewState").scrollPos.y;

                if (scrollPos <= minScrollPos) return;

                var opacity = ((scrollPos - minScrollPos) / (maxScrollPos - minScrollPos)).Clamp01();


                var rectWidth = window.position.width;// - 12;

                var rect = window.position.SetPos(0, 0).MoveY(shadowPos).SetHeight(shadowLength).SetWidth(rectWidth);



                var clipAtY = navbarHeight + 1;

                if (EditorSceneManager.loadedRootSceneCount > 1)
                    clipAtY += 16;


                GUI.BeginClip(window.position.SetPos(0, clipAtY));

                rect.MoveY(-clipAtY).DrawCurtainDown(Greyscale(shadowGreyscale, shadowAlpha * opacity));

                GUI.EndClip();

            }



            var doNavbarFirst = navbars_byWindow.ContainsKey(window) && navbars_byWindow[window].isSearchActive;

            if (doNavbarFirst)
                navbarGui();

            GUILayout.Space(0); // to fix GameAnalytics accessing lastRect for some reason

            defaultGuiWithOffset();
            shadow();

            if (!doNavbarFirst)
                navbarGui();

        }

        static Dictionary<EditorWindow, VHierarchyNavbar> navbars_byWindow = new();



        static void UpdateGUIWrapping(EditorWindow window)
        {
            if (!window.hasFocus) return;

            var curOnGUIMethod = window.GetMemberValue("m_Parent").GetMemberValue<System.Delegate>("m_OnGUI").Method;

            var isWrapped = curOnGUIMethod == mi_WrappedGUI;
            var shouldBeWrapped = VHierarchyMenu.navigationBarEnabled;

            void wrap()
            {
                var hostView = window.GetMemberValue("m_Parent");

                var newDelegate = typeof(VHierarchy).GetMethod(nameof(WrappedGUI), maxBindingFlags).CreateDelegate(t_EditorWindowDelegate, window);

                hostView.SetMemberValue("m_OnGUI", newDelegate);

                window.Repaint();

            }
            void unwrap()
            {
                var hostView = window.GetMemberValue("m_Parent");

                var originalDelegate = hostView.InvokeMethod("CreateDelegate", "OnGUI");

                hostView.SetMemberValue("m_OnGUI", originalDelegate);

                window.Repaint();

            }


            if (shouldBeWrapped && !isWrapped)
                wrap();

            if (!shouldBeWrapped && isWrapped)
                unwrap();

        }
        static void UpdateGUIWrappingForAllHierarchies() => allHierarchies.ForEach(r => UpdateGUIWrapping(r));

        static void OnDomainReloaded() => toCallInGUI += UpdateGUIWrappingForAllHierarchies;

        static void OnWindowUnmaximized() => UpdateGUIWrappingForAllHierarchies();

        static void OnHierarchyFocused() => UpdateGUIWrapping(EditorWindow.focusedWindow);

        static void OnDelayCall() => UpdateGUIWrappingForAllHierarchies();





        static void CheckIfFocusedWindowChanged()
        {
            if (prevFocusedWindow != EditorWindow.focusedWindow)
                if (EditorWindow.focusedWindow?.GetType() == t_SceneHierarchyWindow)
                    OnHierarchyFocused();

            prevFocusedWindow = EditorWindow.focusedWindow;

        }

        static EditorWindow prevFocusedWindow;



        static void CheckIfWindowWasUnmaximized()
        {
            var isMaximized = EditorWindow.focusedWindow?.maximized == true;

            if (!isMaximized && wasMaximized)
                OnWindowUnmaximized();

            wasMaximized = isMaximized;

        }

        static bool wasMaximized;



        static void OnSomeGUI()
        {
            toCallInGUI?.Invoke();
            toCallInGUI = null;

            CheckIfFocusedWindowChanged();

        }

        static void ProjectWindowItemOnGUI(string _, Rect __) => OnSomeGUI();
        static void HierarchyWindowItemOnGUI(int _, Rect __) => OnSomeGUI();

        static System.Action toCallInGUI;



        static void DelayCallLoop()
        {
            OnDelayCall();

            EditorApplication.delayCall -= DelayCallLoop;
            EditorApplication.delayCall += DelayCallLoop;

        }














        static void RowGUI(int instanceId, Rect rowRect)
        {
            EditorWindow window;

            void findWindow()
            {
                if (allHierarchies.Count() == 1) { window = allHierarchies.First(); return; }


                var pointInsideWindow = EditorGUIUtility.GUIToScreenPoint(rowRect.center);

                window = allHierarchies.FirstOrDefault(r => r.position.AddHeight(30).Contains(pointInsideWindow) && r.hasFocus);

            }
            void updateWindow()
            {
                if (!window) return; // happens on half-visible rows during expand animation

                if (curEvent.isLayout && !lastEventWasLayout)
                    UpdateWindow(window);

                lastEventWasLayout = curEvent.isLayout;

            }
            void catchScrollInputForController()
            {
                if (!window) return;
                if (!controllers_byWindow.ContainsKey(window)) return;

                if (curEvent.isScroll)
                    controllers_byWindow[window].animatingScroll = false;

            }
            void callGUI()
            {
                if (!window) return;
                if (!guis_byWindow.ContainsKey(window)) return;


#if UNITY_6000_3_OR_NEWER
                Object getObject(int id) => EditorUtility.EntityIdToObject(instanceId);
                int getSceneId(Scene scene) => scene.handle.GetMemberValue<EntityId>("m_Value");
#else
                Object getObject(int id) => EditorUtility.InstanceIDToObject(instanceId);
                int getSceneId(Scene scene) => scene.GetHashCode();
#endif


                var gui = guis_byWindow[window];

                if (getObject(instanceId) is GameObject go)
                    gui.RowGUI_GameObject(rowRect, go);
                else
                    for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                        if (getSceneId(EditorSceneManager.GetSceneAt(i)) == instanceId)
                            gui.RowGUI_Scene(rowRect, EditorSceneManager.GetSceneAt(i));

            }

            findWindow();
            updateWindow();
            catchScrollInputForController();
            callGUI();

        }

        static bool lastEventWasLayout;



        static void UpdateWindow(EditorWindow window)
        {
            if (!guis_byWindow.TryGetValue(window, out var gui))
                gui = guis_byWindow[window] = new(window);

            if (!controllers_byWindow.TryGetValue(window, out var controller))
                controller = controllers_byWindow[window] = new(window);


            gui.UpdateState();

            controller.UpdateState();
            controller.UpdateExpandQueue();
            controller.UpdateScrollAnimation();
            controller.UpdateHighlightAnimation();

        }

        public static Dictionary<EditorWindow, VHierarchyGUI> guis_byWindow = new();
        public static Dictionary<EditorWindow, VHierarchyController> controllers_byWindow = new();














        public static Texture GetComponentIcon(Component component)
        {
            if (!component) return null;

            if (!componentIcons_byType.ContainsKey(component.GetType()))
                componentIcons_byType[component.GetType()] = EditorGUIUtility.ObjectContent(component, component.GetType()).image;

            return componentIcons_byType[component.GetType()];

        }

        static Dictionary<System.Type, Texture> componentIcons_byType = new();



        static Texture2D GetIcon_forVTabs(GameObject gameObject)
        {
            var goData = GetGameObjectData(gameObject, false);

            if (goData == null) return null;

            var iconNameOrPath = goData.iconNameOrGuid.Length == 32 ? goData.iconNameOrGuid.ToPath() : goData.iconNameOrGuid;

            if (!iconNameOrPath.IsNullOrEmpty())
                return EditorIcons.GetIcon(iconNameOrPath);

            return null;

        }

        static string GetIconName_forVFavorites(GameObject gameObject)
        {
            var goData = GetGameObjectData(gameObject, false);

            if (goData == null) return "";

            var iconNameOrPath = goData.iconNameOrGuid.Length == 32 ? goData.iconNameOrGuid.ToPath() : goData.iconNameOrGuid;

            return iconNameOrPath;

        }
        static string GetIconName_forVInspector(GameObject gameObject)
        {
            return GetIconName_forVFavorites(gameObject);
        }

        public static void SetIcon(GameObject gameObject, string iconName, bool recursive = false)
        {
            if (!data)
                data = AssetDatabase.LoadAssetAtPath<VHierarchyData>(ProjectPrefs.GetString("vHierarchy-lastKnownDataPath"));

            if (!data)
                data = AssetDatabase.FindAssets("t:VHierarchyData").Select(guid => AssetDatabase.LoadAssetAtPath<VHierarchyData>(guid.ToPath())).FirstOrDefault();

            if (!data)
            {
                data = ScriptableObject.CreateInstance<VHierarchyData>();

                AssetDatabase.CreateAsset(data, GetScriptPath("VHierarchy").GetParentPath().CombinePath("vHierarchy Data.asset"));
            }



            goDataCache.Clear();

            var goData = GetGameObjectData(gameObject, createDataIfDoesntExist: true);

            goData.iconNameOrGuid = iconName ?? "";
            goData.isIconRecursive = recursive;


            goInfoCache.Clear();

            EditorApplication.RepaintHierarchyWindow();



            data.Dirty();

        }
        public static void SetColor(GameObject gameObject, int colorIndex, bool recursive = false)
        {
            if (!data)
                data = AssetDatabase.LoadAssetAtPath<VHierarchyData>(ProjectPrefs.GetString("vHierarchy-lastKnownDataPath"));

            if (!data)
                data = AssetDatabase.FindAssets("t:VHierarchyData").Select(guid => AssetDatabase.LoadAssetAtPath<VHierarchyData>(guid.ToPath())).FirstOrDefault();

            if (!data)
            {
                data = ScriptableObject.CreateInstance<VHierarchyData>();

                AssetDatabase.CreateAsset(data, GetScriptPath("VHierarchy").GetParentPath().CombinePath("vHierarchy Data.asset"));
            }




            goDataCache.Clear();

            var goData = GetGameObjectData(gameObject, createDataIfDoesntExist: true);

            goData.colorIndex = colorIndex;
            goData.isColorRecursive = recursive;


            goInfoCache.Clear();

            EditorApplication.RepaintHierarchyWindow();



            data.Dirty();

        }














        static void Shortcuts()
        {
            if (!curEvent.isKeyDown) return;
            if (curEvent.keyCode == KeyCode.None) return;
            if (EditorWindow.mouseOverWindow is not EditorWindow hoveredWindow) return;
            if (hoveredWindow?.GetType() != t_SceneHierarchyWindow) return;


            void toggleExpanded()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.keyCode != KeyCode.E) return;
                if (curEvent.holdingAnyModifierKey) return;
                if (!VHierarchyMenu.toggleExpandedEnabled) return;

                if (Tools.viewTool == ViewTool.FPS) return;
                if (hoveredGo == null && hoveredScene == default) return;


                curEvent.Use();


                if (transformToolNeedsReset = Application.unityVersion.Contains("2022"))
                    previousTransformTool = Tools.current;

                if (hoveredScene == default)
                    if (hoveredGo.transform.childCount == 0) return;


                if (hoveredScene != default)
                    controllers_byWindow[hoveredWindow].ToggleExpanded(hoveredScene.handle);
                else
                    controllers_byWindow[hoveredWindow].ToggleExpanded(hoveredGo.GetInstanceID());

            }
            void collapseAll()
            {
                if (curEvent.modifiers != (EventModifiers.Shift | EventModifiers.Command) && curEvent.modifiers != (EventModifiers.Shift | EventModifiers.Control)) return;
                if (!curEvent.isKeyDown || curEvent.keyCode != KeyCode.E) return;
                if (!VHierarchyMenu.collapseEverythingEnabled) return;

                curEvent.Use();


                controllers_byWindow[hoveredWindow].CollapseAll();

            }
            void isolate()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.keyCode != KeyCode.E) return;
                if (curEvent.modifiers != EventModifiers.Shift) return;
                if (!VHierarchyMenu.isolateEnabled) return;

                if (hoveredGo == null && hoveredScene == default) return;

                curEvent.Use();

                if (hoveredGo && hoveredGo.transform.childCount == 0) return;
                if (!hoveredGo && hoveredScene.rootCount == 0) return;


                if (hoveredScene != default)
                    controllers_byWindow[hoveredWindow].Isolate(hoveredScene.handle);
                else
                    controllers_byWindow[hoveredWindow].Isolate(hoveredGo.GetInstanceID());

            }
            void toggleActive()
            {
                if (!hoveredGo) return;
                if (curEvent.holdingAnyModifierKey) return;
                if (!curEvent.isKeyDown || curEvent.keyCode != KeyCode.A) return;
                if (Tools.viewTool == ViewTool.FPS) return;
                if (!VHierarchyMenu.toggleActiveEnabled) return;

                curEvent.Use();


                var gos = Selection.gameObjects.Contains(hoveredGo) ? Selection.gameObjects : new[] { hoveredGo };
                var active = !gos.Any(r => r.activeSelf);

                foreach (var r in gos)
                {
                    r.RecordUndo();
                    r.SetActive(active);
                }

            }
            void delete()
            {
                if (!hoveredGo) return;
                if (curEvent.holdingAnyModifierKey) return;
                if (!curEvent.isKeyDown || curEvent.keyCode != KeyCode.X) return;
                if (!VHierarchyMenu.deleteEnabled) return;

                var gos = Selection.gameObjects.Contains(hoveredGo) ? Selection.gameObjects : new[] { hoveredGo };

                foreach (var r in gos)
                    Undo.DestroyObjectImmediate(r);

                curEvent.Use();
            }
            void focus()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.modifiers != EventModifiers.None) return;
                if (curEvent.keyCode != KeyCode.F) return;
                if (SceneView.sceneViews.Count == 0) return;
                if (!hoveredGo) return;
                if (!VHierarchyMenu.focusEnabled) return;

                var sv = SceneView.lastActiveSceneView;

                if (!sv || !sv.hasFocus)
                    sv = SceneView.sceneViews.ToArray().FirstOrDefault(r => (r as SceneView).hasFocus) as SceneView;

                if (!sv)
                    (sv = SceneView.lastActiveSceneView ?? SceneView.sceneViews[0] as SceneView).Focus();

                sv.Frame(hoveredGo.GetBounds(), false);

            }
            void setDefaultParent()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.modifiers != EventModifiers.None) return;
                if (curEvent.keyCode != KeyCode.D) return;
                if (!hoveredGo) return;
                if (!VHierarchyMenu.setDefaultParentEnabled) return;


                var isDefaultParentHovered = hoveredGo == typeof(SceneView).InvokeMethod<Transform>("GetDefaultParentObjectIfSet")?.gameObject;

                if (isDefaultParentHovered)
                    EditorUtility.ClearDefaultParentObject();
                else
                    EditorUtility.SetDefaultParentObject(hoveredGo);


                hoveredWindow.Repaint();

                curEvent.Use();

            }
            void resetDefaultParent()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.modifiers != EventModifiers.Shift) return;
                if (curEvent.keyCode != KeyCode.D) return;
                if (!VHierarchyMenu.setDefaultParentEnabled) return;


                EditorUtility.ClearDefaultParentObject();


                hoveredWindow.Repaint();

                curEvent.Use();

            }


            toggleExpanded();
            toggleActive();
            delete();
            collapseAll();
            isolate();
            focus();
            setDefaultParent();
            resetDefaultParent();

        }

        public static GameObject hoveredGo;
        public static Scene hoveredScene;















        public static GameObjectInfo GetGameObjectInfo(GameObject go)
        {
            if (goInfoCache.TryGetValue(go, out var cachedGoInfo)) return cachedGoInfo;


            var goInfo = new GameObjectInfo();

            var goData = goInfo.goData = GetGameObjectData(go, createDataIfDoesntExist: false);


            var recursiveIconNameOrGuid = "";
            var recursiveColorIndex = 0;

            var ruledIconNameOrGuid = "";
            var ruledColorIndex = 0;

            void checkRules()
            {
                if (rules == null)
                    rules = TypeCache.GetMethodsWithAttribute<RuleAttribute>()
                                     .Where(r => r.IsStatic
                                              && r.GetParameters().Count() == 1
                                              && r.GetParameters().First().ParameterType == typeof(ObjectInfo)).ToList();

                if (!rules.Any()) return;



                var objectInfo = new ObjectInfo(go);

                foreach (var rule in rules)
                    rule.Invoke(null, new[] { objectInfo });


                ruledIconNameOrGuid = objectInfo.icon;
                ruledColorIndex = objectInfo.color;


            }
            void checkRecursion(Transform transform, int depth)
            {
                if (!transform.parent) return;

                var parentGoData = GetGameObjectData(transform.parent.gameObject, createDataIfDoesntExist: false);

                if (parentGoData != null)
                {

                    if (parentGoData.isIconRecursive && parentGoData.iconNameOrGuid != "")
                        if (recursiveIconNameOrGuid == "")
                            recursiveIconNameOrGuid = parentGoData.iconNameOrGuid;

                    if (parentGoData.isColorRecursive && parentGoData.colorIndex != 0)
                        if (recursiveColorIndex == 0)
                            recursiveColorIndex = parentGoData.colorIndex;


                    if (parentGoData.isColorRecursive && parentGoData.colorIndex != 0)
                        goInfo.maxColorRecursionDepth = depth + 1;

                }



                checkRecursion(transform.parent, depth + 1);

            }
            void setIcon()
            {
                var iconNameOrGuid = "";

                if (goData != null && goData.iconNameOrGuid != "")
                    iconNameOrGuid = goData.iconNameOrGuid;

                else if (recursiveIconNameOrGuid != "")
                    iconNameOrGuid = recursiveIconNameOrGuid;

                else if (ruledIconNameOrGuid != "")
                    iconNameOrGuid = ruledIconNameOrGuid;



                if (iconNameOrGuid == "") { goInfo.hasIcon = false; return; }

                goInfo.hasIcon = true;
                goInfo.hasIconByRecursion = recursiveIconNameOrGuid != "";

                goInfo.iconNameOrPath = iconNameOrGuid.Length == 32 ? iconNameOrGuid.ToPath()
                                                                    : iconNameOrGuid;

            }
            void setColor()
            {
                var colorIndex = 0;

                if (goData != null && goData.colorIndex > 0)
                    colorIndex = goData.colorIndex;

                else if (recursiveColorIndex != 0)
                    colorIndex = recursiveColorIndex;

                else if (ruledColorIndex != 0)
                    colorIndex = ruledColorIndex;



                if (colorIndex == 0) { goInfo.hasColor = false; return; }

                goInfo.hasColor = true;
                goInfo.hasColorByRecursion = recursiveColorIndex != 0;




                var brightness = palette?.colorBrightness ?? 1;
                var saturation = palette?.colorSaturation ?? 1;

                if (colorIndex <= VHierarchyPalette.greyColorsCount)
                    saturation = brightness = 1;


                var rawColor = palette ? palette.colors[colorIndex - 1] : VHierarchyPalette.GetDefaultColor(colorIndex - 1);

                var brightenedColor = MathUtil.Lerp(Greyscale(.2f), rawColor, brightness);

                Color.RGBToHSV(brightenedColor, out float h, out float s, out float v);
                var saturatedColor = Color.HSVToRGB(h, s * saturation, v);


                goInfo.color = saturatedColor;

                goInfo.isGreyColor = colorIndex <= VHierarchyPalette.greyColorsCount;

            }

            checkRules();
            checkRecursion(go.transform, 0);
            setIcon();
            setColor();


            return goInfoCache[go] = goInfo;

        }

        public static Dictionary<GameObject, GameObjectInfo> goInfoCache = new();

        public static List<MethodInfo> rules = null;

        public class GameObjectInfo
        {
            public string iconNameOrPath = "";
            public bool hasIcon;
            public bool hasIconByRecursion;

            public Color color;
            public bool hasColor;
            public bool hasColorByRecursion;
            public int maxColorRecursionDepth;
            public bool isGreyColor;


            public GameObjectData goData;

        }



        public static GameObjectData GetGameObjectData(GameObject go, bool createDataIfDoesntExist)
        {
            if (!data) return null;
            if (goDataCache.TryGetValue(go, out var cachedResult)) return cachedResult;

            GameObjectData goData = null;
            SceneData sceneData = null;

            void sceneObject()
            {
                if (StageUtility.GetCurrentStage() is PrefabStage) return;


                SceneIdMap sceneIdMap = null;

                var currentSceneGuid = go.scene.path.ToGuid();
                var originalSceneGuid = cache.originalSceneGuids_byInstanceId.GetValueOrDefault(go.GetInstanceID()) ?? currentSceneGuid;


                void getSceneDataFromComponents()
                {
                    if (!VHierarchyData.teamModeEnabled) return;

                    if (!dataComponents_byScene.ContainsKey(go.scene))
                        dataComponents_byScene[go.scene] = Resources.FindObjectsOfTypeAll<VHierarchyDataComponent>().FirstOrDefault(r => r.gameObject?.scene == go.scene);

                    if (dataComponents_byScene[go.scene])
                        sceneData = dataComponents_byScene[go.scene].sceneData;

                }
                void getSceneDataFromScriptableObject()
                {
                    if (sceneData != null) return;

                    data.sceneDatas_byGuid.TryGetValue(originalSceneGuid, out sceneData);

                }
                void createSceneData()
                {
                    if (sceneData != null) return;
                    if (!createDataIfDoesntExist) return;

                    sceneData = new SceneData();

                    data.sceneDatas_byGuid[originalSceneGuid] = sceneData;

                }

                void getSceneIdMap()
                {
                    if (sceneData == null) return;

                    cache.sceneIdMaps_bySceneGuid.TryGetValue(originalSceneGuid, out sceneIdMap);

                }
                void createSceneIdMap()
                {
                    if (sceneIdMap != null) return;
                    if (sceneData == null) return;
                    if (currentSceneGuid != originalSceneGuid) return;

                    sceneIdMap = new SceneIdMap();

                    cache.sceneIdMaps_bySceneGuid[currentSceneGuid] = sceneIdMap;

                }
                void updateSceneIdMapAndOriginalSceneGuids()
                {
                    if (sceneIdMap == null) return;
                    if (currentSceneGuid != originalSceneGuid) return;
                    if (!go.scene.isLoaded) return; // can happen when setting icons via api


                    var curInstanceIdsHash = go.scene.GetRootGameObjects().FirstOrDefault()?.GetInstanceID() ?? 0;
                    var curGlobalIdsHash = sceneData.goDatas_byGlobalId.Keys.Aggregate(0, (hash, r) => hash ^= r.GetHashCode());

                    if (sceneIdMap.instanceIdsHash == curInstanceIdsHash && sceneIdMap.globalIdsHash == curGlobalIdsHash) return;


                    var globalIds = sceneData.goDatas_byGlobalId.Keys.ToList();
                    var instanceIds = globalIds.Select(r => Application.isPlaying ? r.UnpackForPrefab() : r)
                                               .GetObjectInstanceIds();
                    void clearIdMap()
                    {
                        if (Application.isPlaying) return; // not clearing in playmode fixes data loss on first root object when it's moved to DontDestroyOnLoad (sice it causes map update)

                        sceneIdMap.globalIds_byInstanceId = new SerializableDictionary<int, GlobalID>();

                    }
                    void clearSceneGuids()
                    {
                        if (Application.isPlaying) return; // not clearing in playmode fixes data loss on first root object when it's moved to DontDestroyOnLoad (sice it causes map update)

                        foreach (var instanceId in sceneIdMap.globalIds_byInstanceId.Keys)
                            cache.originalSceneGuids_byInstanceId.Remove(instanceId);

                    }
                    void fillIdMap()
                    {
                        for (int i = 0; i < instanceIds.Length; i++)
                            if (instanceIds[i] != 0)
                                sceneIdMap.globalIds_byInstanceId[instanceIds[i]] = globalIds[i];

                    }
                    void fillSceneGuids()
                    {
                        for (int i = 0; i < instanceIds.Length; i++)
                            cache.originalSceneGuids_byInstanceId[instanceIds[i]] = currentSceneGuid;

                    }


                    clearIdMap();
                    clearSceneGuids();
                    fillIdMap();
                    fillSceneGuids();

                    sceneIdMap.instanceIdsHash = curInstanceIdsHash;
                    sceneIdMap.globalIdsHash = curGlobalIdsHash;

                }

                void getGoData()
                {
                    if (sceneData == null) return;
                    if (sceneIdMap == null) return;
                    if (!sceneIdMap.globalIds_byInstanceId.TryGetValue(go.GetInstanceID(), out var globalId)) return;

                    sceneData.goDatas_byGlobalId.TryGetValue(globalId, out goData);

                }
                void moveGoDataToCurrentSceneGuid()
                {
                    if (goData == null) return;
                    if (currentSceneGuid == originalSceneGuid) return;
                    if (Application.isPlaying) return;

                    var originalSceneData = sceneData;
                    var currentSceneData = dataComponents_byScene.GetValueOrDefault(go.scene)?.sceneData ?? data.sceneDatas_byGuid.GetValueOrDefault(currentSceneGuid);

                    if (originalSceneData == null) return;
                    if (currentSceneData == null) return;

                    var globalId = go.GetGlobalID();

                    originalSceneData.goDatas_byGlobalId.Remove(originalSceneData.goDatas_byGlobalId.First(r => r.Value == goData).Key);
                    currentSceneData.goDatas_byGlobalId[go.GetGlobalID()] = goData;

                }
                void createGoData()
                {
                    if (goData != null) return;
                    if (!createDataIfDoesntExist) return;

                    goData = new GameObjectData();

                    sceneData.goDatas_byGlobalId[go.GetGlobalID()] = goData;

                }


                getSceneDataFromComponents();
                getSceneDataFromScriptableObject();
                createSceneData();

                getSceneIdMap();
                createSceneIdMap();
                updateSceneIdMapAndOriginalSceneGuids();

                getGoData();
                moveGoDataToCurrentSceneGuid();
                createGoData();

            }
            void prefabObject()
            {
                if (StageUtility.GetCurrentStage() is not PrefabStage prefabStage) return;


                var prefabGuid = prefabStage.assetPath.ToGuid();

                GlobalID sourceGlobalId;

                void calcGlobalId()
                {

                    var rawGlobalId = go.GetGlobalID();


#if UNITY_2023_2_OR_NEWER

                    var so = new SerializedObject(go);

                    so.SetPropertyValue("inspectorMode", UnityEditor.InspectorMode.Debug);

                    var rawFileId = so.FindProperty("m_LocalIdentfierInFile").longValue;

                    if (rawFileId == 0) // happens for prefab variants in unity 6
                        rawFileId = (long)t_Unsupported.InvokeMethod<ulong>("GetOrGenerateFileIDHint", go);

#else
                    var rawFileId = rawGlobalId.fileId;
#endif

                    // fixes fileId for prefab variants
                    // also works for getting prefab's unpacked fileId
                    var fileId = ((long)rawFileId ^ (long)rawGlobalId.globalObjectId.targetPrefabId) & 0x7fffffffffffffff;


                    sourceGlobalId = new GlobalID($"GlobalObjectId_V1-1-{prefabGuid}-{fileId}-0");

                }


                void getSceneDataFromScriptableObject()
                {
                    data.sceneDatas_byGuid.TryGetValue(prefabGuid, out sceneData);
                }
                void createSceneData()
                {
                    if (sceneData != null) return;
                    if (!createDataIfDoesntExist) return;

                    sceneData = new SceneData();

                    data.sceneDatas_byGuid[prefabGuid] = sceneData;

                }

                void getGoData()
                {
                    if (sceneData == null) return;

                    sceneData.goDatas_byGlobalId.TryGetValue(sourceGlobalId, out goData);

                }
                void createGoData()
                {
                    if (goData != null) return;
                    if (!createDataIfDoesntExist) return;

                    goData = new GameObjectData();

                    sceneData.goDatas_byGlobalId[sourceGlobalId] = goData;

                }


                calcGlobalId();

                getSceneDataFromScriptableObject();
                createSceneData();

                getGoData();
                createGoData();

            }
            void prefabInstance_editMode()
            {
                if (!PrefabUtility.IsPartOfPrefabInstance(go)) return;
                if (goData != null) return;

                void tryGetForSourceGo(GameObject sourceGo)
                {
                    var sourceGoGlobalId = sourceGo.GetGlobalID();
                    var sourcePrefabGuid = sourceGoGlobalId.guid;

                    cache.prefabInstanceGlobalIds_byInstanceIds[go.GetInstanceID()] = sourceGoGlobalId;


                    data.sceneDatas_byGuid.TryGetValue(sourcePrefabGuid, out sceneData);

                    sceneData?.goDatas_byGlobalId.TryGetValue(sourceGoGlobalId, out goData);


                    if (goData == null)
                        try
                        {
                            if (PrefabUtility.GetCorrespondingObjectFromSource(sourceGo) is GameObject previousSourceGo)
                                tryGetForSourceGo(previousSourceGo);

                            // wrapped in try-catch because GetCorrespondingObjectFromSource throws exceptions on broken prefabs
                        }
                        catch { }

                }

                tryGetForSourceGo(PrefabUtility.GetCorrespondingObjectFromSource(go));

            }
            void prefabInstance_playmode()
            {
                if (!Application.isPlaying) return;
                if (goData != null) return;


                if (!cache.prefabInstanceGlobalIds_byInstanceIds.TryGetValue(go.GetInstanceID(), out var globalId)) return;

                var prefabGuid = globalId.guid;


                data.sceneDatas_byGuid.TryGetValue(prefabGuid, out sceneData);

                sceneData?.goDatas_byGlobalId.TryGetValue(globalId, out goData);

            }

            sceneObject();
            prefabObject();
            prefabInstance_editMode();
            prefabInstance_playmode();

            if (goData != null)
                goData.sceneData = sceneData;

            return goDataCache[go] = goData;

        }

        public static Dictionary<GameObject, GameObjectData> goDataCache = new();

        public static Dictionary<Scene, VHierarchyDataComponent> dataComponents_byScene = new();

        static VHierarchyCache cache => VHierarchyCache.instance;



        public static void OnHierarchyChanged() { goInfoCache.Clear(); }
        public static void OnDataSerialization() { goInfoCache.Clear(); goDataCache.Clear(); }


















        static void LoadSceneBookmarkObjects() // update
        {
            if (!data) return;


            var scenesToLoadFor = unloadedSceneBookmarks_sceneGuids.Select(r => EditorSceneManager.GetSceneByPath(r.ToPath()))
                                                                   .Where(r => r.isLoaded);
            if (!scenesToLoadFor.Any()) return;



            foreach (var scene in scenesToLoadFor)
            {
                var bookmarksFromThisScene = data.bookmarks.Where(r => r.globalId.guid == scene.path.ToGuid()).ToList();

                var objectsForTheseBookmarks = bookmarksFromThisScene.Select(r => !Application.isPlaying ? r.globalId
                                                                                                         : r.globalId.UnpackForPrefab()).GetObjects();

                for (int i = 0; i < bookmarksFromThisScene.Count; i++)
                    if (objectsForTheseBookmarks[i])
                        bookmarksFromThisScene[i]._go = objectsForTheseBookmarks[i] as GameObject;
                    else
                        bookmarksFromThisScene[i].failedToLoadSceneObject = true;

            }

            unloadedSceneBookmarks_sceneGuids.Clear();


            foreach (var window in allHierarchies)
                window.Repaint();

        }

        public static HashSet<string> unloadedSceneBookmarks_sceneGuids = new();




        static void StashBookmarkObjects() // on playmode enter before awake
        {
            stashedBookmarkObjects_byBookmark.Clear();

            foreach (var bookmark in data.bookmarks)
                stashedBookmarkObjects_byBookmark[bookmark] = bookmark._go;

        }
        static void UnstashBookmarkObjects() // on playmode exit
        {
            foreach (var bookmark in data.bookmarks)
                if (stashedBookmarkObjects_byBookmark.TryGetValue(bookmark, out var stashedObject))
                    if (stashedObject != null)
                        bookmark._go = stashedObject;

        }

        static Dictionary<Bookmark, GameObject> stashedBookmarkObjects_byBookmark = new();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnPlaymodeEnter_beforeAwake()
        {
            if (!data) return;

            StashBookmarkObjects();

        }
        static void OnPlaymodeExit(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode) return;
            if (!data) return;


            UnstashBookmarkObjects();

            // scene objects can get recreated in playmode if the scene was reloaded
            // in this case their respective bookmarks will be updated in OnSceneLoaded_inPlaymode to reference the recreated versions
            // so we ensure that after playmode bookmarks reference the same objects as they did before playmode




            foreach (var bookmark in data.bookmarks)
                if (bookmark.globalId.guid == "00000000000000000000000000000000")
                    if (bookmark._go is GameObject gameObject)
                    {
                        bookmark.globalId = new GlobalID(bookmark.globalId.ToString().Replace("00000000000000000000000000000000", gameObject.scene.path.ToGuid()));
                        data.Dirty();
                    }

            // objects from DontDestroyOnLoad that were bookmarked in playmode have globalIds with blank scene guids
            // we fix this after playmode, when scene guids become available

        }




        static void RepaintOnAlt() // Update 
        {
            var lastEvent = typeof(Event).GetFieldValue<Event>("s_Current");

            if (lastEvent.alt != wasAlt)
                if (EditorWindow.mouseOverWindow is EditorWindow hoveredWindow)
                    if (hoveredWindow.GetType() == t_SceneHierarchyWindow || hoveredWindow is VHierarchySceneSelectorWindow)
                        hoveredWindow.Repaint();

            wasAlt = lastEvent.alt;

        }

        static bool wasAlt;




        static void SetPreviousTransformTool()
        {
            if (!transformToolNeedsReset) return;

            Tools.current = previousTransformTool;

            transformToolNeedsReset = false;

            // E shortcut changes transform tool in 2022
            // here we undo this

        }

        static bool transformToolNeedsReset;
        static Tool previousTransformTool;




        static void DuplicateSceneData(string originalSceneGuid, string duplicatedSceneGuid)
        {
            var originalSceneData = data.sceneDatas_byGuid[originalSceneGuid];
            var duplicatedSceneData = data.sceneDatas_byGuid[duplicatedSceneGuid] = new SceneData();

            foreach (var kvp in originalSceneData.goDatas_byGlobalId)
            {
                var duplicatedGlobalId = new GlobalID(kvp.Key.ToString().Replace(originalSceneGuid, duplicatedSceneGuid));
                var duplicatedGoData = new GameObjectData() { colorIndex = kvp.Value.colorIndex, iconNameOrGuid = kvp.Value.iconNameOrGuid };

                duplicatedSceneData.goDatas_byGlobalId[duplicatedGlobalId] = duplicatedGoData;

            }

            foreach (var bookmark in data.bookmarks.ToList().Where(r => r.globalId.guid == originalSceneGuid))
            {
                var duplicatedGlobalId = new GlobalID(bookmark.globalId.ToString().ToString().Replace(originalSceneGuid, duplicatedSceneGuid));
                var duplicatedBookmark = new Bookmark(null) { globalId = duplicatedGlobalId };

                data.bookmarks.Add(duplicatedBookmark);

            }

            data.Dirty();

        }

        static void OnSceneImported(string importedScenePath)
        {
            if (curEvent.commandName != "Duplicate" && curEvent.commandName != "Paste") return;


            var copiedAssets_paths = new List<string>();

            var assetClipboard = typeof(Editor).Assembly.GetType("UnityEditor.AssetClipboardUtility").GetMemberValue("assetClipboard").InvokeMethod<IEnumerator>("GetEnumerator");

            while (assetClipboard.MoveNext())
                copiedAssets_paths.Add(assetClipboard.Current.GetMemberValue<GUID>("guid").ToString().ToPath());



            var originalScenePath = copiedAssets_paths.FirstOrDefault(r => File.Exists(r) && new FileInfo(r).Length
                                                                                          == new FileInfo(importedScenePath).Length);
            var originalSceneGuid = originalScenePath.ToGuid();
            var duplicatedSceneGuid = importedScenePath.ToGuid();

            if (!data.sceneDatas_byGuid.ContainsKey(originalSceneGuid)) return;
            if (data.sceneDatas_byGuid.ContainsKey(duplicatedSceneGuid)) return;

            DuplicateSceneData(originalSceneGuid, duplicatedSceneGuid);

        }

        class SceneImportDetector : AssetPostprocessor
        {
            // scene data duplication won't work on earlier versions anyway
#if UNITY_2021_2_OR_NEWER 
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
            {
                if (!data) return;

                foreach (var r in importedAssets)
                    if (r.EndsWith(".unity"))
                        OnSceneImported(r);

            }
#endif
        }




        [UnityEditor.Callbacks.PostProcessBuild]
        public static void ClearCacheAfterBuild(BuildTarget _, string __) => VHierarchyCache.Clear();

        static void ClearCacheOnProjectLoaded() => VHierarchyCache.Clear();














        [InitializeOnLoadMethod]
        static void Init()
        {
            if (VHierarchyMenu.pluginDisabled) return;

            void subscribe()
            {

                // gui

                EditorApplication.hierarchyWindowItemOnGUI -= RowGUI;
                EditorApplication.hierarchyWindowItemOnGUI = RowGUI + EditorApplication.hierarchyWindowItemOnGUI;




                // wrapping updaters            

                EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGUI;
                EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;

                EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUI;
                EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

                EditorApplication.delayCall -= DelayCallLoop;
                EditorApplication.delayCall += DelayCallLoop;

                EditorApplication.update -= CheckIfFocusedWindowChanged;
                EditorApplication.update += CheckIfFocusedWindowChanged;



                // shortcuts

                var globalEventHandler = typeof(EditorApplication).GetFieldValue<EditorApplication.CallbackFunction>("globalEventHandler");
                typeof(EditorApplication).SetFieldValue("globalEventHandler", Shortcuts + (globalEventHandler - Shortcuts));




                // loading bookmarked objects

                EditorApplication.update -= LoadSceneBookmarkObjects;
                EditorApplication.update += LoadSceneBookmarkObjects;



                // other

                EditorApplication.update -= RepaintOnAlt;
                EditorApplication.update += RepaintOnAlt;

                EditorApplication.update -= SetPreviousTransformTool;
                EditorApplication.update += SetPreviousTransformTool;

                EditorApplication.hierarchyChanged -= OnHierarchyChanged;
                EditorApplication.hierarchyChanged += OnHierarchyChanged;

                var projectWasLoaded = typeof(EditorApplication).GetFieldValue<UnityEngine.Events.UnityAction>("projectWasLoaded");
                typeof(EditorApplication).SetFieldValue("projectWasLoaded", (projectWasLoaded - ClearCacheOnProjectLoaded) + ClearCacheOnProjectLoaded);

            }
            void loadData()
            {
                data = AssetDatabase.LoadAssetAtPath<VHierarchyData>(ProjectPrefs.GetString("vHierarchy-lastKnownDataPath"));


                if (data) return;

                data = AssetDatabase.FindAssets("t:VHierarchyData").Select(guid => AssetDatabase.LoadAssetAtPath<VHierarchyData>(guid.ToPath())).FirstOrDefault();


                if (!data) return;

                ProjectPrefs.SetString("vHierarchy-lastKnownDataPath", data.GetPath());

            }
            void loadPalette()
            {
                palette = AssetDatabase.LoadAssetAtPath<VHierarchyPalette>(ProjectPrefs.GetString("vHierarchy-lastKnownPalettePath"));


                if (palette) return;

                palette = AssetDatabase.FindAssets("t:VHierarchyPalette").Select(guid => AssetDatabase.LoadAssetAtPath<VHierarchyPalette>(guid.ToPath())).FirstOrDefault();


                if (!palette) return;

                ProjectPrefs.SetString("vHierarchy-lastKnownPalettePath", palette.GetPath());

            }
            void loadDataAndPaletteDelayed()
            {
                if (!data)
                    EditorApplication.delayCall += () => EditorApplication.delayCall += loadData;

                if (!palette)
                    EditorApplication.delayCall += () => EditorApplication.delayCall += loadPalette;

                // AssetDatabase isn't up to date at this point (it gets updated after InitializeOnLoadMethod)
                // and if current AssetDatabase state doesn't contain the data - it won't be loaded during Init()
                // so here we schedule an additional, delayed attempt to load the data
                // this addresses reports of data loss when trying to load it on a new machine

            }
            void migrateDataFromV1()
            {
                if (!data) return;
                if (ProjectPrefs.GetBool("vHierarchy-dataMigrationFromV1Attempted", false)) return;

                ProjectPrefs.SetBool("vHierarchy-dataMigrationFromV1Attempted", true);

                var lines = System.IO.File.ReadAllLines(data.GetPath());

                if (lines.Length < 15 || !lines[14].Contains("sceneDatasByGuid")) return;

                var sceneGuids = new List<string>();
                var globalIdLists = new List<List<string>>();
                var goDatasByInstanceIdCounts = new List<int>();
                var sceneDatas = new List<SceneData>();


                void parseSceneGuids()
                {
                    for (int i = 16; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("values:")) break;

                        var startIndex = lines[i].IndexOf("- ") + 2;

                        if (startIndex < lines[i].Length)
                            sceneGuids.Add(lines[i].Substring(startIndex));
                        else
                            sceneGuids.Add("");

                    }

                }
                void parseGlobalIdLists_andCountGoDatasByInstanceId()
                {
                    var parsingGlobalIdList = false;
                    var parsingGlobalIdListAtIndex = -1;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i];

                        void startParsing()
                        {
                            if (!line.Contains("goDatasByGlobalId")) return;

                            parsingGlobalIdList = true;
                            parsingGlobalIdListAtIndex++;

                            globalIdLists.Add(new List<string>());

                        }
                        void parse()
                        {
                            if (!parsingGlobalIdList) return;
                            if (!line.Contains("- GlobalObjectId")) return;

                            var startIndex = line.IndexOf("- ") + 2;

                            if (startIndex < line.Length)
                                globalIdLists[parsingGlobalIdListAtIndex].Add(line.Substring(startIndex));
                            else
                                globalIdLists[parsingGlobalIdListAtIndex].Add("");

                        }
                        void stopParsing_andCountDatasByInstanceId()
                        {
                            if (!line.Contains("goDatasByInstanceId")) return;


                            parsingGlobalIdList = false;


                            var goDatasByInstanceId_keysLine = lines[i + 1];
                            var goDatasByInstanceId_count = (goDatasByInstanceId_keysLine.Length - 14) / 8;

                            goDatasByInstanceIdCounts.Add(goDatasByInstanceId_count);


                        }

                        startParsing();
                        parse();
                        stopParsing_andCountDatasByInstanceId();

                    }

                }
                void parseSceneDatas()
                {
                    var firstLineIndexOfFirstSceneData = 17 + sceneGuids.Count;



                    void parseSceneData(int sceneDataIndex)
                    {
                        var sceneData = new SceneData();

                        var globalIds = globalIdLists[sceneDataIndex];
                        var firstLineIndex = getFirstLineIndex(sceneDataIndex);



                        void parseGoData(int iGoData)
                        {
                            var goData = new GameObjectData();


                            var colorLine = lines[getColorLineIndex(iGoData)];

                            if (colorLine.Length > 18)
                                goData.colorIndex = int.Parse(colorLine.Substring(18));


                            var iconLine = lines[getIconLineIndex(iGoData)];

                            if (iconLine.Length > 16)
                                goData.iconNameOrGuid = iconLine.Substring(16);



                            var globalIdString = globalIdLists[sceneDataIndex][iGoData];

                            var globalId = new GlobalID(globalIdString);



                            sceneData.goDatas_byGlobalId[globalId] = goData;
                            // sceneData.goDatas_byGlobalId.Add(globalId, goData);

                        }

                        int getColorLineIndex(int goDataIndex)
                        {
                            var index = firstLineIndex; // - goDatasByGlobalId:

                            index += 1; // keys:
                            index += globalIds.Count;

                            index += 1; // values:
                            index += 1; // zeroth godata
                            index += goDataIndex * 2;

                            return index;

                        }
                        int getIconLineIndex(int goDataIndex) => getColorLineIndex(goDataIndex) + 1;



                        for (int i = 0; i < globalIds.Count; i++)
                            parseGoData(i);

                        sceneDatas.Add(sceneData);

                    }

                    int getSceneDataLength(int sceneDataIndex)
                    {
                        int length = 0;

                        length += 1; // - goDatasByGlobalId:

                        length += 1; // - keys:
                        length += globalIdLists[sceneDataIndex].Count;

                        length += 1; // - values:
                        length += globalIdLists[sceneDataIndex].Count * 2;



                        length += 1; // - goDatasByInstanceId:

                        length += 1; // - keys: 123123123

                        length += 1; // - values:
                        length += goDatasByInstanceIdCounts[sceneDataIndex] * 2;


                        return length;

                    }
                    int getFirstLineIndex(int sceneDataIndex)
                    {
                        var index = firstLineIndexOfFirstSceneData;

                        for (int i = 0; i < sceneDataIndex; i++)
                            index += getSceneDataLength(i);

                        return index;

                    }



                    for (int i = 0; i < sceneGuids.Count; i++)
                        parseSceneData(i);

                }

                void remapColorIndexes()
                {
                    foreach (var sceneData in sceneDatas)
                        foreach (var goData in sceneData.goDatas_byGlobalId.Values)
                            if (goData.colorIndex == 7)
                                goData.colorIndex = 1;
                            else if (goData.colorIndex == 8)
                                goData.colorIndex = 2;
                            else if (goData.colorIndex >= 2)
                                goData.colorIndex += 2;

                }
                void setSceneDatasToData()
                {
                    for (int i = 0; i < sceneDatas.Count; i++)
                        data.sceneDatas_byGuid[sceneGuids[i]] = sceneDatas[i];

                    data.Dirty();
                    data.Save();

                }


                try
                {
                    parseSceneGuids();
                    parseGlobalIdLists_andCountGoDatasByInstanceId();
                    parseSceneDatas();

                    remapColorIndexes();
                    setSceneDatasToData();

                }
                catch { }

            }
            // void removeDeletedBookmarks()
            // {
            //     if (!data) return;


            //     var toRemove = data.bookmarks.Where(r => r.isDeleted);

            //     if (!toRemove.Any()) return;


            //     foreach (var r in toRemove.ToList())
            //         data.bookmarks.Remove(r);

            //     data.Dirty();


            //     // delayed to give bookmarks a chance to load in update

            // }


            subscribe();
            loadData();
            loadPalette();
            loadDataAndPaletteDelayed();
            migrateDataFromV1();

            // EditorApplication.delayCall += () => removeDeletedBookmarks();

            OnDomainReloaded();

        }

        public static VHierarchyData data;
        public static VHierarchyPalette palette;





        static IEnumerable<EditorWindow> allHierarchies => _allHierarchies ??= t_SceneHierarchyWindow.GetFieldValue<IList>("s_SceneHierarchyWindows").Cast<EditorWindow>();
        static IEnumerable<EditorWindow> _allHierarchies;

        static Type t_SceneHierarchyWindow = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        static Type t_HostView = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
        static Type t_EditorWindowDelegate = t_HostView.GetNestedType("EditorWindowDelegate", maxBindingFlags);
        static Type t_Unsupported = typeof(Editor).Assembly.GetType("UnityEditor.Unsupported");

        static Type t_VTabs = Type.GetType("VTabs.VTabs") ?? Type.GetType("VTabs.VTabs, VTabs, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        static Type t_VFavorites = Type.GetType("VFavorites.VFavorites") ?? Type.GetType("VFavorites.VFavorites, VFavorites, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

        static MethodInfo mi_WrappedGUI = typeof(VHierarchy).GetMethod(nameof(WrappedGUI), maxBindingFlags);






#if UNITY_6000_3_OR_NEWER
        public static EntityId ToIdType(this int id) => id;
        public static List<int> ToInts(this List<EntityId> ids) => ids.Select(r => (int)r).ToList();
        public static List<int> GetIdList(this object o, string listName) => o.GetMemberValue<List<EntityId>>(listName)?.ToInts();
#else
        public static int ToIdType(this int id) => id;
        public static List<int> ToInts(this List<int> ids) => ids;
        public static List<int> GetIdList(this object o, string listName) => o.GetMemberValue<List<int>>(listName);
#endif







        public const string version = "2.1.8";

    }

    #region Rules

    public class RuleAttribute : System.Attribute { }

    public class ObjectInfo
    {
        public int color = 0;
        public string icon = "";


        public ObjectInfo(GameObject gameObject) => this.gameObject = gameObject;

        public GameObject gameObject;


    }



    #endregion

}
#endif
