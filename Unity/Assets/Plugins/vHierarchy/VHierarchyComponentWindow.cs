#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Type = System.Type;
using static VHierarchy.VHierarchyData;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;
// using static VTools.VDebug;


namespace VHierarchy
{
    public class VHierarchyComponentWindow : EditorWindow
    {

        void OnGUI()
        {
            if (!component) component = _EditorUtility_InstanceIDToObject(componentIid) as Component;
            if (!component) { Close(); return; }

            if (!editor) Init(component);


            void background()
            {
                position.SetPos(0, 0).Draw(GUIColors.windowBackground);
            }
            void header()
            {
                var headerRect = ExpandWidthLabelRect(18).Resize(-1).AddWidthFromMid(6);
                var pinButtonRect = headerRect.SetWidthFromRight(17).SetHeightFromMid(17).Move(-21, .5f);
                var closeButtonRect = headerRect.SetWidthFromRight(16).SetHeightFromMid(16).Move(-3, .5f);

                var backgroundColor = isDarkTheme ? Greyscale(.25f) : GUIColors.windowBackground;

                void startDragging()
                {
                    if (isResizingVertically) return;
                    if (isResizingHorizontally) return;
                    if (isDragged) return;
                    if (!curEvent.isMouseDrag) return;
                    if (!headerRect.IsHovered()) return;


                    isDragged = true;

                    dragStartMousePos = EditorGUIUtility.GUIToScreenPoint(curEvent.mousePosition);
                    dragStartWindowPos = position.position;


                    isPinned = true;

                    if (floatingInstance == this)
                        floatingInstance = null;

                    EditorApplication.RepaintHierarchyWindow();


                }
                void updateDragging()
                {
                    if (!isDragged) return;


                    var draggedPosition = dragStartWindowPos + EditorGUIUtility.GUIToScreenPoint(curEvent.mousePosition) - dragStartMousePos;

                    if (!curEvent.isRepaint)
                        position = position.SetPos(draggedPosition);


                    EditorGUIUtility.hotControl = EditorGUIUtility.GetControlID(FocusType.Passive);

                }
                void stopDragging()
                {
                    if (!isDragged) return;
                    if (!curEvent.isMouseUp) return;


                    isDragged = false;

                    EditorGUIUtility.hotControl = 0;

                }

                void background()
                {
                    headerRect.Draw(backgroundColor);

                    headerRect.SetHeightFromBottom(1).Draw(isDarkTheme ? Greyscale(.2f) : Greyscale(.7f));

                }
                void icon()
                {
                    var iconRect = headerRect.SetWidth(20).MoveX(14).MoveY(-1);

                    GUI.Label(iconRect, VHierarchy.GetComponentIcon(component));

                }
                void toggle()
                {
                    var toggleRect = headerRect.MoveX(36).SetSize(20, 20);


                    var pi_enabled = component.GetType().GetProperty("enabled") ??
                                     component.GetType().BaseType?.GetProperty("enabled") ??
                                     component.GetType().BaseType?.BaseType?.GetProperty("enabled") ??
                                     component.GetType().BaseType?.BaseType?.BaseType?.GetProperty("enabled");


                    if (pi_enabled == null) return;

                    var enabled = (bool)pi_enabled.GetValue(component);


                    if (GUI.Toggle(toggleRect, enabled, "") == enabled) return;

                    component.RecordUndo();
                    pi_enabled.SetValue(component, !enabled);

                }
                void name()
                {
                    var nameRect = headerRect.MoveX(54).MoveY(-1);

                    var s = new GUIContent(EditorGUIUtility.ObjectContent(component, component.GetType())).text;
                    s = s.Substring(s.LastIndexOf('(') + 1);
                    s = s.Substring(0, s.Length - 1);

                    if (isPinned)
                        s += " of " + component.gameObject.name;


                    SetLabelBold();

                    GUI.Label(nameRect, s);

                    ResetLabelStyle();

                }
                void nameCurtain()
                {
                    var flatColorRect = headerRect.SetX(pinButtonRect.x + 3).SetXMax(headerRect.xMax);
                    var gradientRect = headerRect.SetXMax(flatColorRect.x).SetWidthFromRight(30);

                    flatColorRect.Draw(backgroundColor);
                    gradientRect.DrawCurtainLeft(backgroundColor);

                }
                void pinButton()
                {
                    if (!isPinned && closeButtonRect.IsHovered()) return;


                    var normalColor = isDarkTheme ? Greyscale(.65f) : Greyscale(.8f);
                    var hoveredColor = isDarkTheme ? Greyscale(.9f) : normalColor;
                    var activeColor = Color.white;



                    SetGUIColor(isPinned ? activeColor : pinButtonRect.IsHovered() ? hoveredColor : normalColor);

                    GUI.Label(pinButtonRect, EditorGUIUtility.IconContent("pinned"));

                    ResetGUIColor();


                    SetGUIColor(Color.clear);

                    var clicked = GUI.Button(pinButtonRect, "");

                    ResetGUIColor();


                    if (!clicked) return;

                    isPinned = !isPinned;

                    if (isPinned && floatingInstance == this)
                        floatingInstance = null;

                    if (!isPinned && !floatingInstance)
                        floatingInstance = this;

                    EditorApplication.RepaintHierarchyWindow();


                }
                void closeButton()
                {

                    SetGUIColor(Color.clear);

                    if (GUI.Button(closeButtonRect, ""))
                        Close();

                    ResetGUIColor();


                    var normalColor = isDarkTheme ? Greyscale(.65f) : Greyscale(.35f);
                    var hoveredColor = isDarkTheme ? Greyscale(.9f) : normalColor;


                    SetGUIColor(closeButtonRect.IsHovered() ? hoveredColor : normalColor);

                    GUI.Label(closeButtonRect, EditorGUIUtility.IconContent("CrossIcon"));

                    ResetGUIColor();


                    if (isPinned) return;

                    var escRect = closeButtonRect.Move(-22, -1).SetWidth(70);

                    SetGUIEnabled(false);

                    if (closeButtonRect.IsHovered())
                        GUI.Label(escRect, "Esc");

                    ResetGUIEnabled();

                }
                void rightClick()
                {
                    if (!curEvent.isMouseDown) return;
                    if (curEvent.mouseButton != 1) return;
                    if (!headerRect.IsHovered()) return;

                    typeof(EditorUtility).InvokeMethod("DisplayObjectContextMenu", Rect.zero.SetPos(curEvent.mousePosition), component, 0);

                }

                startDragging();
                updateDragging();
                stopDragging();

                background();
                icon();
                toggle();
                name();
                nameCurtain();
                pinButton();
                closeButton();
                rightClick();

            }
            void body()
            {
                EditorGUIUtility.labelWidth = (this.position.width * .4f).Max(120);


                scrollPosition = EditorGUILayout.BeginScrollView(Vector2.up * scrollPosition).y;
                BeginIndent(17);


                editor?.OnInspectorGUI();

                updateHeight();


                EndIndent(1);
                EditorGUILayout.EndScrollView();


                EditorGUIUtility.labelWidth = 0;

            }
            void outline()
            {
                // if (Application.platform == RuntimePlatform.OSXEditor) return;

                position.SetPos(0, 0).DrawOutline(Greyscale(.1f));

            }

            void updateHeight()
            {

                ExpandWidthLabelRect(height: -5);

                if (!curEvent.isRepaint) return;
                if (isResizingVertically) return;


                targetHeight = lastRect.y + 30;

                position = position.SetHeight(targetHeight.Min(maxHeight));


                prevHeight = position.height;

            }
            void updatePosition()
            {
                if (!curEvent.isLayout) return;

                void calcDeltaTime()
                {
                    deltaTime = (float)(EditorApplication.timeSinceStartup - lastLayoutTime);

                    if (deltaTime > .05f)
                        deltaTime = .0166f;

                    lastLayoutTime = EditorApplication.timeSinceStartup;

                }
                void resetCurPos()
                {
                    if (currentPosition != default && !isPinned) return;

                    currentPosition = position.position; // position.position is always int, which can't be used for lerping

                }
                void lerpCurPos()
                {
                    if (isPinned) return;

                    var speed = 9;

                    MathUtil.SmoothDamp(ref currentPosition, targetPosition, speed, ref positionDeriv, deltaTime);
                    // MathfUtils.Lerp(ref currentPosition, targetPosition, speed, deltaTime);

                }
                void setCurPos()
                {
                    if (isPinned) return;

                    position = position.SetPos(currentPosition);

                }

                calcDeltaTime();
                resetCurPos();
                lerpCurPos();
                setCurPos();

            }
            void closeOnEscape()
            {
                if (isPinned) return;
                if (!curEvent.isKeyDown) return;
                if (curEvent.keyCode != KeyCode.Escape) return;

                Close();
            }

            void horizontalResize()
            {
                var showingScrollbar = targetHeight > maxHeight;

                var resizeArea = this.position.SetPos(0, 0).SetWidthFromRight(showingScrollbar ? 3 : 5).AddHeightFromBottom(-20);

                void startResize()
                {
                    if (isDragged) return;
                    if (isResizingHorizontally) return;
                    if (!curEvent.isMouseDown && !curEvent.isMouseDrag) return;
                    if (!resizeArea.IsHovered()) return;

                    isResizingHorizontally = true;

                    resizeStartMousePos = curEvent.mousePosition_screenSpace;
                    resizeStartWindowSize = this.position.size;

                }
                void updateResize()
                {
                    if (!isResizingHorizontally) return;


                    var resizedWidth = resizeStartWindowSize.x + curEvent.mousePosition_screenSpace.x - resizeStartMousePos.x;

                    var width = resizedWidth.Max(300);

                    if (!curEvent.isRepaint)
                        position = position.SetWidth(width);


                    EditorGUIUtility.hotControl = EditorGUIUtility.GetControlID(FocusType.Passive);
                    // GUI.focused

                }
                void stopResize()
                {
                    if (!isResizingHorizontally) return;
                    if (!curEvent.isMouseUp) return;

                    isResizingHorizontally = false;

                    EditorGUIUtility.hotControl = 0;

                }


                EditorGUIUtility.AddCursorRect(resizeArea, MouseCursor.ResizeHorizontal);

                startResize();
                updateResize();
                stopResize();

            }
            void verticalResize()
            {
                var resizeArea = this.position.SetPos(0, 0).SetHeightFromBottom(5);

                void startResize()
                {
                    if (isDragged) return;
                    if (isResizingVertically) return;
                    if (!curEvent.isMouseDown && !curEvent.isMouseDrag) return;
                    if (!resizeArea.IsHovered()) return;

                    isResizingVertically = true;

                    resizeStartMousePos = curEvent.mousePosition_screenSpace;
                    resizeStartWindowSize = this.position.size;

                }
                void updateResize()
                {
                    if (!isResizingVertically) return;


                    var resizedHeight = resizeStartWindowSize.y + curEvent.mousePosition_screenSpace.y - resizeStartMousePos.y;

                    var height = resizedHeight.Min(targetHeight).Max(50);

                    if (!curEvent.isRepaint)
                        position = position.SetHeight(height);

                    maxHeight = height;


                    EditorGUIUtility.hotControl = EditorGUIUtility.GetControlID(FocusType.Passive);
                    // GUI.focused

                }
                void stopResize()
                {
                    if (!isResizingVertically) return;
                    if (!curEvent.isMouseUp) return;

                    isResizingVertically = false;

                    EditorGUIUtility.hotControl = 0;

                }


                EditorGUIUtility.AddCursorRect(resizeArea, MouseCursor.ResizeVertical);

                startResize();
                updateResize();
                stopResize();

            }


            background();

            horizontalResize();
            verticalResize();


            header();

            Space(3);
            body();
            outline();

            Space(7);


            updatePosition();
            closeOnEscape();


            if (!isPinned)
                Repaint();

            EditorApplication.delayCall -= Repaint;
            EditorApplication.delayCall += Repaint;

        }

        public Vector2 targetPosition;
        public Vector2 currentPosition;
        Vector2 positionDeriv;
        float deltaTime;
        double lastLayoutTime;

        bool isDragged;
        Vector2 dragStartMousePos;
        Vector2 dragStartWindowPos;

        public bool isResizingHorizontally;
        public bool isResizingVertically;
        public Vector2 resizeStartMousePos;
        public Vector2 resizeStartWindowSize;

        public float scrollPosition;

        public float targetHeight;
        public float maxHeight;
        public float prevHeight;





        void OnLostFocus()
        {
            if (isPinned) return;

            if (curEvent.holdingAlt && EditorWindow.focusedWindow.GetType().Name == "SceneHierarchyWindow")
                CloseNextFrameIfNotRefocused();
            else
                Close();

        }

        void CloseNextFrameIfNotRefocused()
        {
            EditorApplication.delayCall += () => { if (EditorWindow.focusedWindow != this) Close(); };
        }

        public bool isPinned;





        public void Init(Component component)
        {
            if (editor)
                editor.DestroyImmediate();

            this.component = component;
            this.componentIid = component.GetInstanceID();
            this.editor = Editor.CreateEditor(component);

        }

        void OnDestroy()
        {
            editor?.DestroyImmediate();

            editor = null;
            component = null;

            EditorPrefs.SetFloat("vHierarchy-componentWindowWidth", position.width);

        }

        public Component component;
        public Editor editor;

        public int componentIid;





        public static void CreateFloatingInstance(Vector2 position)
        {
            floatingInstance = ScriptableObject.CreateInstance<VHierarchyComponentWindow>();

            // floatingInstance.ShowPopup();
            typeof(EditorWindow).GetMethod("ShowWithMode", maxBindingFlags).Invoke(floatingInstance, new object[] { 3 }); // show in NoShadow mode


            floatingInstance.maxHeight = EditorGUIUtility.GetMainWindowPosition().height * .7f;


            var savedWidth = EditorPrefs.GetFloat("vHierarchy-componentWindowWidth", minWidth);

            var width = savedWidth.Max(minWidth);

            floatingInstance.position = Rect.zero.SetPos(position).SetWidth(width).SetHeight(200);
            floatingInstance.prevHeight = floatingInstance.position.height;

            floatingInstance.targetPosition = position;

        }

        public static VHierarchyComponentWindow floatingInstance;

        public static float minWidth => 300;

    }
}
#endif