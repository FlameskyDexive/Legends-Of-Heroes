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
using static VHierarchy.VHierarchy;
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
    public class VHierarchyGUI
    {

        public void RowGUI_GameObject(Rect rowRect, GameObject go)
        {
            var fullRowRect = rowRect.SetX(32).SetXMax(rowRect.xMax + 16);

            var isRowHovered = fullRowRect.AddWidthFromRight(32).IsHovered();


            var isRowSelected = false;
            var isRowBeingRenamed = false;
            var isDefaultParent = go == defaultParent;

            void setState()
            {
                void set_isRowSelected()
                {
                    if (!curEvent.isRepaint) return;


                    var dragging = dragSelectionList != null
                                && dragSelectionList.Any();

                    isRowSelected = dragging ? (dragSelectionList.Contains(go.GetInstanceID())) : Selection.Contains(go);

                }
                void set_lastVisibleSelectedRowRect()
                {
                    if (!Selection.gameObjects.Contains(go)) return;

                    lastVisibleSelectedRowRect = rowRect;

                }
                void set_mousePressed()
                {
                    if (curEvent.isMouseDown && isRowHovered)
                        mousePressed = true;

                    if (curEvent.isMouseUp || curEvent.isMouseLeaveWindow || curEvent.isDragPerform)
                        mousePressed = false;

                }
                void set_hoveredGo()
                {
                    if (curEvent.isLayout)
                        VHierarchy.hoveredGo = null;

                    if (curEvent.isRepaint && isRowHovered)
                        VHierarchy.hoveredGo = go;

                }

                set_isRowSelected();
                set_lastVisibleSelectedRowRect();
                set_mousePressed();
                set_hoveredGo();

                isRowBeingRenamed = renamingRow && isRowSelected;

            }


            void drawing()
            {
                if (!curEvent.isRepaint) { hierarchyLines_isFirstRowDrawn = false; return; }

                var goInfo = GetGameObjectInfo(go);

                var drawBackgroundColor = goInfo.hasColor;
                var drawCustomIcon = goInfo.hasIcon;
                var drawDefaultIcon = !drawCustomIcon && (isRowBeingRenamed || (!VHierarchyMenu.minimalModeEnabled || (PrefabUtility.IsAddedGameObjectOverride(go) && PrefabUtility.IsPartOfPrefabInstance(go))));

                var makeTriangleBrighter = drawBackgroundColor && !goInfo.isGreyColor && isDarkTheme;
                var makeNameBrighter = drawBackgroundColor && !goInfo.isGreyColor && isDarkTheme && !isDefaultParent;
                var makeNameBold = isDefaultParent && isDarkTheme;

                Color defaultBackground;


                void set_defaultBackground()
                {
                    var selectedFocused = GUIColors.selectedBackground;
                    var selectedUnfocused = isDarkTheme ? Greyscale(.3f) : Greyscale(.68f);
                    var hovered = isDarkTheme ? Greyscale(.265f) : Greyscale(.7f);
                    var normal = GUIColors.windowBackground;

                    if (isRowSelected && !isRowBeingRenamed)
                        defaultBackground = isTreeFocused ? selectedFocused : selectedUnfocused;

                    else if (isRowHovered)
                        defaultBackground = hovered;

                    else
                        defaultBackground = normal;

                }
                void hideDefaultIcon()
                {
                    if (drawDefaultIcon) return;

                    rowRect.SetWidth(16).Draw(defaultBackground);

                }
                void hideName()
                {
                    if (!drawBackgroundColor && (drawCustomIcon || drawDefaultIcon) && !makeNameBold) return;

                    var nameRect = rowRect.MoveX(16).SetWidth(go.name.GetLabelWidth(isBold: isDefaultParent));
#if UNITY_2023_2_OR_NEWER
                    if (!go.activeInHierarchy && PrefabUtility.IsPartOfPrefabInstance(go))
                        nameRect.width *= 1.1f;
#endif

                    nameRect.Draw(defaultBackground);

                }

                void backgroundColor()
                {
                    if (!drawBackgroundColor) return;



                    var color = goInfo.color;

                    if (isRowHovered)
                        color *= isDarkTheme ? 1.1f : .92f;

                    if (isRowSelected)
                        color *= isDarkTheme ? 1.2f : .8f;

                    if (palette?.colorGradientsEnabled == false)
                        color = MathUtil.Lerp(color, Greyscale(.2f), isDarkTheme ? .25f : .03f);

                    if (goInfo.hasColorByRecursion)
                        color = MathUtil.Lerp(color, Greyscale(isDarkTheme ? .25f : .8f), .5f);





                    var colorRect = rowRect.AddWidthFromRight(28).AddWidth(16);

                    if (goInfo.hasColorByRecursion)
                        colorRect = colorRect.AddWidthFromRight(goInfo.maxColorRecursionDepth * 14);

                    if (!isRowSelected && !goInfo.hasColorByRecursion)
                        colorRect = colorRect.AddHeightFromMid(EditorGUIUtility.pixelsPerPoint >= 2 ? -.5f : -1);

                    if (goInfo.hasColorByRecursion)
                        colorRect = colorRect.MoveY(EditorGUIUtility.pixelsPerPoint >= 2 ? -.25f : -.5f);

                    if (palette?.colorGradientsEnabled == false || goInfo.isGreyColor) { colorRect.Draw(color); return; }



                    var hasLeftGradient = colorRect.x > 32;

                    if (hasLeftGradient)
                        colorRect = colorRect.AddWidthFromRight(3);

                    if (PrefabUtility.HasPrefabInstanceAnyOverrides(go, false) && PrefabUtility.IsOutermostPrefabInstanceRoot(go) && !hasLeftGradient)
                        colorRect = colorRect.AddWidthFromRight(EditorGUIUtility.pixelsPerPoint >= 2 ? -2.5f : -3);



                    var leftGradientWith = hasLeftGradient ? 22 : 0;
                    var rightGradientWidth = (fullRowRect.width * .77f).Min(colorRect.width - leftGradientWith);

                    var leftGradientRect = colorRect.SetWidth(leftGradientWith);
                    var rightGradientRect = colorRect.SetWidthFromRight(rightGradientWidth);

                    var flatColorRect = colorRect.SetX(leftGradientRect.xMax).SetXMax(rightGradientRect.x);






                    leftGradientRect.AddWidth(1).DrawCurtainLeft(color);

                    flatColorRect.AddWidth(1).Draw(color);

                    rightGradientRect.Draw(color.MultiplyAlpha(.1f));
                    rightGradientRect.DrawCurtainRight(color);


                }
                void triangle()
                {
                    if (!drawBackgroundColor) return;
                    if (go.transform.childCount == 0) return;

                    var triangleRect = rowRect.MoveX(-15.5f).SetWidth(16).Resize(1.5f);

                    GUI.DrawTexture(triangleRect, EditorIcons.GetIcon(controller.expandedIds.Contains(go.GetInstanceID()) ? "IN_foldout_on" : "IN_foldout"));


                    if (!makeTriangleBrighter) return;

                    GUI.DrawTexture(triangleRect, EditorIcons.GetIcon(controller.expandedIds.Contains(go.GetInstanceID()) ? "IN_foldout_on" : "IN_foldout"));

                }
                void name()
                {
                    if (!drawBackgroundColor && (drawCustomIcon || drawDefaultIcon) && !makeNameBold) return;
                    if (isRowBeingRenamed) return;


                    var nameRect = rowRect.MoveX(18).AddHeight(1);

                    if (VHierarchyMenu.minimalModeEnabled && !drawCustomIcon && !drawDefaultIcon)
                        nameRect = nameRect.MoveX(-17);

                    if (drawBackgroundColor && !goInfo.isGreyColor)
                        nameRect = nameRect.MoveY(.5f);

                    if (!go.activeInHierarchy) // correcting unity's style padding inconsistencies
                        if (PrefabUtility.IsPartOfAnyPrefab(go))
                            nameRect = nameRect.MoveY(-1);
                        else
                            nameRect = nameRect.Move(-1, -1.5f);

                    if (makeNameBrighter && go.activeInHierarchy && !makeNameBold)
                        nameRect = nameRect.MoveX(-2).MoveY(-.5f);



                    var styleName = PrefabUtility.IsPartOfAnyPrefab(go) ?
                        (go.activeInHierarchy ? "PR PrefabLabel" : "PR DisabledPrefabLabel") :
                        (go.activeInHierarchy ? "TV Line" : "PR DisabledLabel");

                    if (makeNameBrighter && go.activeInHierarchy)
                        styleName = "WhiteLabel";

                    if (makeNameBold)
                        styleName = "TV LineBold";



                    if (makeNameBrighter)
                        SetGUIColor(Greyscale(!go.activeInHierarchy ? 1.4f : isRowSelected ? 1 : goInfo.hasColorByRecursion ? .9f : .95f));

                    GUI.skin.GetStyle(styleName).Draw(nameRect, go.name, false, false, isRowSelected || makeNameBold, isTreeFocused || makeNameBold);

                    if (makeNameBrighter)
                        ResetGUIColor();

                }
                void defaultIcon()
                {
                    if (!drawBackgroundColor) return;
                    if (!drawDefaultIcon) return;

                    var iconRect = rowRect.SetWidth(16);
                    var icon = PrefabUtility.GetIconForGameObject(go);

                    if (!isDarkTheme && isRowSelected && isTreeFocused && icon.name == "GameObject Icon")
                        icon = EditorIcons.GetIcon("GameObject On Icon");


                    SetGUIColor(go.activeInHierarchy ? Color.white : Greyscale(1, .4f));

                    GUI.DrawTexture(iconRect, icon);

                    if (PrefabUtility.IsAddedGameObjectOverride(go))
                        GUI.DrawTexture(iconRect, EditorIcons.GetIcon("PrefabOverlayAdded Icon"));

                    ResetGUIColor();

                }
                void customIcon()
                {
                    if (!drawCustomIcon) return;

                    var icon = EditorIcons.GetIcon(goInfo.iconNameOrPath) ?? Texture2D.blackTexture;

                    var iconRect = rowRect.SetWidth(16);

                    if (icon.width < icon.height) iconRect = iconRect.SetWidthFromMid(iconRect.height * icon.width / icon.height);
                    if (icon.height < icon.width) iconRect = iconRect.SetHeightFromMid(iconRect.width * icon.height / icon.width);


                    SetGUIColor(go.activeInHierarchy ? Color.white : Greyscale(1, .4f));

                    GUI.DrawTexture(iconRect, icon);

                    ResetGUIColor();

                }
                void hierarchyLines()
                {
                    if (!VHierarchyMenu.hierarchyLinesEnabled) return;


                    var lineThickness = 1f;
                    var lineColor = isDarkTheme ? Greyscale(1, .165f) : Greyscale(0, .23f);

                    var depth = ((rowRect.x - 60) / 14).RoundToInt().Max(0);

                    bool isLastChild(Transform transform) => transform.parent?.GetChild(transform.parent.childCount - 1) == transform;
                    bool hasChilren(Transform transform) => transform.childCount > 0;

                    void calcVerticalGaps_beforeFirstRowDrawn()
                    {
                        if (hierarchyLines_isFirstRowDrawn) return;

                        hierarchyLines_verticalGaps.Clear();

                        var curTransform = go.transform.parent;
                        var curDepth = depth - 1;

                        while (curTransform != null && curTransform.parent != null)
                        {
                            if (isLastChild(curTransform))
                                hierarchyLines_verticalGaps.Add(curDepth - 1);

                            curTransform = curTransform.parent;
                            curDepth--;
                        }

                    }
                    void updateVerticalGaps_beforeNextRowDrawn()
                    {
                        if (isLastChild(go.transform))
                            hierarchyLines_verticalGaps.Add(depth - 1);

                        if (depth < hierarchyLines_prevRowDepth)
                            hierarchyLines_verticalGaps.RemoveAll(r => r >= depth);

                    }

                    void drawVerticals()
                    {
                        for (int i = 0; i < depth; i++)
                            if (!hierarchyLines_verticalGaps.Contains(i))
                                rowRect.SetX(53 + i * 14 - lineThickness / 2)
                                       .SetWidth(lineThickness)
                                       .SetHeight(isLastChild(go.transform) && i == depth - 1 ? 8 + lineThickness / 2 : 16)
                                       .Draw(lineColor);

                    }
                    void drawHorizontals()
                    {
                        if (depth == 0) return;

                        rowRect.MoveX(-21)
                               .SetHeightFromMid(lineThickness)
                               .SetWidth(hasChilren(go.transform) ? 7 : 17)
                               .AddWidthFromRight(-lineThickness / 2f)
                               .Draw(lineColor);

                    }



                    calcVerticalGaps_beforeFirstRowDrawn();

                    drawVerticals();
                    drawHorizontals();

                    updateVerticalGaps_beforeNextRowDrawn();

                    hierarchyLines_prevRowDepth = depth;
                    hierarchyLines_isFirstRowDrawn = true;

                }
                void zebraStriping()
                {
                    if (!VHierarchyMenu.zebraStripingEnabled) return;
                    if (isRowSelected) return;
                    if (goInfo.goData?.colorIndex == 1) return;


                    var contrast = isDarkTheme ? .033f : .05f;


                    var t = rowRect.y.PingPong(16f) / 16f;

                    if (isRowHovered || isRowSelected)
                        t = 1;

                    if (t.Approx(0)) return;



                    fullRowRect.Draw(Greyscale(isDarkTheme ? 1 : 0, contrast * t));


                }
                void highlight()
                {
                    if (!controller.animatingHighlight) return;
                    if (go != controller.objectToHighlight) return;


                    var highlightBrightness = .16f;


                    var highlightAmount = controller.highlightAmount.Clamp01();

                    highlightAmount = highlightAmount * highlightAmount * (3 - 2 * highlightAmount);


                    fullRowRect.AddWidthFromRight(123).Draw(Greyscale(1, highlightBrightness * highlightAmount));

                }


                set_defaultBackground();
                hideDefaultIcon();
                hideName();

                backgroundColor();
                hierarchyLines();

                triangle();
                name();
                defaultIcon();
                customIcon();
                zebraStriping();
                highlight();

            }

            void componentMinimap()
            {
                if (!VHierarchyMenu.componentMinimapEnabled) return;

                void componentButton(Rect buttonRect, Component component)
                {
                    void componentIcon()
                    {
                        if (!curEvent.isRepaint) return;


                        var normalOpacity = isDarkTheme ? .47f : .7f;
                        var activeOpacity = 1;
                        var pressedOpacity = isDarkTheme ? .65f : .9f;

                        var isActive = (buttonRect.IsHovered() && curEvent.holdingAlt) || VHierarchyComponentWindow.floatingInstance?.component == component;
                        var isPressed = buttonRect.IsHovered() && mousePressed;

                        var icon = GetComponentIcon(component);


                        if (!icon) return;

                        SetGUIColor(Greyscale(1, isActive ? (isPressed ? pressedOpacity : activeOpacity) : normalOpacity));

                        GUI.DrawTexture(buttonRect.SetSizeFromMid(12, 12), icon);

                        ResetGUIColor();

                    }

                    void mouseDown()
                    {
                        if (!curEvent.holdingAlt) return;
                        if (!curEvent.isMouseDown) return;
                        if (!buttonRect.IsHovered()) return;

                        curEvent.Use();

                        mouseDownPos = curEvent.mousePosition;

                    }
                    void mouseUp()
                    {
                        if (!curEvent.holdingAlt) return;
                        if (!curEvent.isMouseUp) return;
                        if (!buttonRect.IsHovered()) return;

                        curEvent.Use();

                        if (VHierarchyComponentWindow.floatingInstance?.component == component) { VHierarchyComponentWindow.floatingInstance.Close(); return; }


                        var position = EditorGUIUtility.GUIToScreenPoint(new Vector2(rowRect.xMax + 25, rowRect.y));

                        if (!VHierarchyComponentWindow.floatingInstance)
                            VHierarchyComponentWindow.CreateFloatingInstance(position);

                        VHierarchyComponentWindow.floatingInstance.Init(component);
                        VHierarchyComponentWindow.floatingInstance.Focus();

                        VHierarchyComponentWindow.floatingInstance.targetPosition = position;

                    }


                    if (curEvent.holdingAlt)
                        buttonRect.MarkInteractive();

                    componentIcon();

                    mouseDown();
                    mouseUp();

                }

                void transformComponent()
                {
                    if (!isRowHovered) return;
                    if (!curEvent.holdingAlt) return;
                    if (!go.GetComponent<Transform>()) return;

                    componentButton(fullRowRect.SetWidth(13).MoveX(1.5f), go.GetComponent<Transform>());

                }
                void otherComponetns()
                {
                    var buttonWidth = 13;
                    var minButtonX = rowRect.x + go.name.GetLabelWidth() + buttonWidth + 2;
                    var buttonRect = fullRowRect.SetWidthFromRight(buttonWidth).MoveX(-1.5f);

                    if (PrefabUtility.IsAnyPrefabInstanceRoot(go) && !PrefabUtility.IsPartOfModelPrefab(go))
                        buttonRect = buttonRect.MoveX(-13);

                    foreach (var component in go.GetComponents<Component>())
                    {
                        if (component is Transform) continue;
                        if (buttonRect.x < minButtonX) continue;

                        componentButton(buttonRect, component);

                        buttonRect = buttonRect.MoveX(-buttonWidth);

                    }


                }

                transformComponent();
                otherComponetns();

            }
            void activationToggle()
            {
                if (!VHierarchyMenu.activationToggleEnabled) return;
                if (!isRowHovered) return;

                var toggleRect = fullRowRect.SetWidth(16).MoveX(1);


                SetGUIColor(Greyscale(1, .9f));

                var newActiveSelf = EditorGUI.Toggle(toggleRect, go.activeSelf);

                ResetGUIColor();


                if (newActiveSelf == go.activeSelf) return;

                var gos = Selection.gameObjects.Contains(go) ? Selection.gameObjects : new[] { go };
                var newActive = gos != null && !gos.Any(r => r && r.activeSelf);

                foreach (var r in gos)
                    r.RecordUndo();

                foreach (var r in gos)
                    r.SetActive(newActiveSelf);

                GUI.FocusControl(null);

            }
            void defaultParentIndicator()
            {
                if (!isDefaultParent) return;



                var drawCustomIcon = GetGameObjectInfo(go).hasIcon;
                var drawDefaultIcon = !drawCustomIcon && (isRowBeingRenamed || (!VHierarchyMenu.minimalModeEnabled || (PrefabUtility.IsAddedGameObjectOverride(go) && PrefabUtility.IsPartOfPrefabInstance(go))));

                var indicatorRect = rowRect.MoveX(go.name.GetLabelWidth(isBold: true) + 16.5f);

                if (!drawCustomIcon && !drawDefaultIcon)
                    indicatorRect = indicatorRect.MoveX(-16);



                SetGUIColor(Greyscale(1, .6f));
                SetLabelFontSize(10);

                GUI.Label(indicatorRect, "Default parent");

                ResetLabelStyle();
                ResetGUIColor();





                if (!fullRowRect.IsHovered()) return;


                var buttonRect = indicatorRect.MoveX(68.5f).SetWidth(16).MoveY(.49f);

                var iconName = "CrossIcon";
                var iconSize = 10;
                var colorNormal = Greyscale(isDarkTheme ? .75f : .2f, .55f);
                var colorHovered = Greyscale(isDarkTheme ? 123f : .2f, 123f);
                var colorPressed = Greyscale(isDarkTheme ? .75f : .5f);
                var colorDisabled = Greyscale(isDarkTheme ? .53f : .55f);


                if (!IconButton(buttonRect, iconName, iconSize, colorNormal, colorHovered, colorPressed)) return;

                EditorUtility.ClearDefaultParentObject();


            }

            void altDrag()
            {
                if (!curEvent.holdingAlt) return;

                void mouseDown()
                {
                    if (!curEvent.isMouseDown) return;
                    if (!rowRect.IsHovered()) return;

                    mouseDownPos = curEvent.mousePosition;

                }
                void mouseDrag()
                {
                    if (!curEvent.isMouseDrag) return;
                    if ((curEvent.mousePosition - mouseDownPos).magnitude < 5) return;
                    if (!rowRect.Contains(mouseDownPos)) return;
                    if (!rowRect.Contains(curEvent.mousePosition - curEvent.mouseDelta)) return;
                    if (DragAndDrop.objectReferences.Any()) return;

                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.objectReferences = new[] { go };
                    DragAndDrop.StartDrag(go.name);

                }

                mouseDown();
                mouseDrag();

                // altdrag has to be set up manually before altClick because altClick will use() mouseDown event to prevent selection change
            }
            void altClick()
            {
                if (!fullRowRect.IsHovered()) return;
                if (!curEvent.holdingAlt) return;
                if (Application.isPlaying) return;

                void mouseDown()
                {
                    if (!curEvent.isMouseDown) return;

                    curEvent.Use();

                }
                void mouseUp()
                {
                    if (!curEvent.isMouseUp) return;

                    var editMultiSelection = Selection.gameObjects.Length > 1 && Selection.gameObjects.Contains(go);

                    var gosToEdit = (editMultiSelection ? Selection.gameObjects : new[] { go }).ToList();


                    if (VHierarchyPaletteWindow.instance && VHierarchyPaletteWindow.instance.gameObjects.SequenceEqual(gosToEdit)) { VHierarchyPaletteWindow.instance.Close(); return; }

                    var openNearRect = editMultiSelection ? lastVisibleSelectedRowRect : rowRect;
                    var position = EditorGUIUtility.GUIToScreenPoint(new Vector2(curEvent.mousePosition.x + 20, openNearRect.y - 13));
                    // var position = EditorGUIUtility.GUIToScreenPoint(new Vector2(openNearRect.x - 14, openNearRect.y + 18));

                    if (!VHierarchyPaletteWindow.instance)
                        VHierarchyPaletteWindow.CreateInstance(position);

                    VHierarchyPaletteWindow.instance.Init(gosToEdit);
                    VHierarchyPaletteWindow.instance.Focus();

                    VHierarchyPaletteWindow.instance.targetPosition = position;

                    if (editMultiSelection)
                        Selection.objects = null;

                }

                mouseDown();
                mouseUp();

            }



            setState();

            drawing();

            componentMinimap();
            activationToggle();
            defaultParentIndicator();

            altDrag();
            altClick();

        }

        List<int> hierarchyLines_verticalGaps = new();
        bool hierarchyLines_isFirstRowDrawn;
        int hierarchyLines_prevRowDepth;

        bool mousePressed;
        Vector2 mouseDownPos;

        Rect lastVisibleSelectedRowRect;




        public void RowGUI_Scene(Rect rowRect, Scene scene)
        {
            var fullRowRect = rowRect.SetX(32).SetXMax(rowRect.xMax + 16);

            var isRowHovered = fullRowRect.AddWidthFromRight(32).IsHovered();
            var isActiveScene = EditorSceneManager.GetActiveScene() == scene;
            var isStickyHeader = rowRect.y != 0 && EditorGUIUtility.GUIToScreenPoint(rowRect.position).y - window.position.y == 45;


            void set_hoveredScene()
            {
                if (curEvent.isLayout)
                    VHierarchy.hoveredScene = default;

                if (curEvent.isRepaint && isRowHovered)
                    VHierarchy.hoveredScene = scene;

            }
            void sceneSelector()
            {
                if (!VHierarchyMenu.sceneSelectorEnabled) return;
                if (!scene.isLoaded) return;


                var nameWidth = (scene.name == "" ? "Untitled" : scene.name).GetLabelWidth(isBold: isActiveScene) + (scene.isDirty ? 5 : 0) + (isActiveScene ? -.5f : -1f);
                var selectorRect = rowRect.MoveX(18).SetWidth(nameWidth + 16);

                var id = EditorGUIUtility.GUIToScreenRect(selectorRect).GetHashCode();
                var isPressed = id == pressedSceneSelectorId;

                var highlightName = selectorRect.IsHovered() || (VHierarchySceneSelectorWindow.instance && VHierarchySceneSelectorWindow.instance.sceneToReplace == scene);



                void dummyRow()
                {
                    if (!highlightName) return;
                    if (!isDarkTheme) return;

                    void background()
                    {
                        var backgroundColor = Application.unityVersion.Contains("2021") ? Greyscale(isDarkTheme ? .32f : .9f)
                                                                                        : Greyscale(isDarkTheme ? .16f : .9f);

                        rowRect.AddWidthFromMid(123).AddHeight(-1).Draw(backgroundColor);

                    }
                    void tripleDotButton()
                    {
                        GUI.DrawTexture(rowRect.SetWidthFromRight(16).MoveX(12), EditorIcons.GetIcon("More"));
                    }
                    void sceneIcon()
                    {
                        GUI.DrawTexture(rowRect.SetWidth(16), EditorIcons.GetIcon("SceneAsset Icon"));
                    }
                    void foldoutIcon()
                    {
                        if (scene.rootCount == 0) return;

                        var isSceneExpanded = controller.expandedIds.Contains(scene.handle);

                        GUI.DrawTexture(rowRect.SetWidth(16).MoveX(-15.5f).SetSizeFromMid(13), EditorIcons.GetIcon(isSceneExpanded ? "IN_foldout_on" : "IN_foldout"));
                    }

                    background();
                    tripleDotButton();
                    sceneIcon();
                    foldoutIcon();

                }

                void dropdownIcon()
                {
                    var iconRect = rowRect.MoveY(-.5f).MoveX(nameWidth + 14).SetWidth(16);


                    var iconBrightness = highlightName ? 1 : .78f;

                    if (!isActiveScene)
                        iconBrightness *= .82f;

                    if (isPressed)
                        iconBrightness *= .83f;

                    if (!isDarkTheme)
                        iconBrightness = .35f;



                    SetGUIColor(Greyscale(iconBrightness));

                    GUI.DrawTexture(iconRect, EditorIcons.GetIcon("Dropdown"));

                    ResetGUIColor();

                }
                void highlightedName()
                {
                    if (!curEvent.isRepaint) return;
                    if (!highlightName) return;

                    var nameRect = rowRect.MoveX(18).SetWidth(nameWidth + 32);




                    var nameStyle = isActiveScene ? "TV LineBold" : "TV Line";

                    var nameText = scene.name == "" ? "Untitled" : scene.name;

                    if (scene.isDirty)
                        nameText += "*";


                    var nameBrightness = 1f;

                    if (isPressed)
                        nameBrightness *= .83f;

                    if (!isDarkTheme)
                        nameBrightness = .0f;



                    SetGUIColor(Greyscale(nameBrightness));

                    GUI.skin.GetStyle(nameStyle).Draw(nameRect, nameText, false, false, true, true);

                    ResetGUIColor();

                }
                void buttonLogic()
                {
                    void mouseDown()
                    {
                        var couldBeMouseDown = isStickyHeader && isRowHovered && curEvent.isUsed && !isPressed; // gets used on sticky headers by default row gui

                        if (!curEvent.isMouseDown && !couldBeMouseDown) return;
                        if (!selectorRect.IsHovered()) return;


                        pressedSceneSelectorId = id;

                        mouseDownOnSelectorPos = curEvent.mousePosition;

                        curEvent.Use();

                    }
                    void mouseUp()
                    {
                        var couldBeMouseUp = isStickyHeader && isRowHovered && curEvent.isUsed && isPressed; // gets used on sticky headers by default row gui

                        if (!curEvent.isMouseUp && !couldBeMouseUp) return;
                        if (!isPressed) return;


                        pressedSceneSelectorId = 0;

                        curEvent.Use();


                        if (!selectorRect.IsHovered()) return;

                        if (VHierarchySceneSelectorWindow.instance)
                            VHierarchySceneSelectorWindow.instance.Close();
                        else
                            VHierarchySceneSelectorWindow.Open(EditorGUIUtility.GUIToScreenPoint(rowRect.position), scene);

                    }
                    void mouseDrag()
                    {
                        if (!curEvent.isMouseDrag) return;
                        if (!isPressed) return;

                        if (curEvent.mousePosition.DistanceTo(mouseDownOnSelectorPos) < 3) { curEvent.Use(); return; }

                        pressedSceneSelectorId = 0;


                        var sceneHierarchy = window?.GetFieldValue("m_SceneHierarchy");

                        var treeViewController = sceneHierarchy.GetFieldValue("m_TreeView");
                        var treeViewControllerData = treeViewController.GetMemberValue("data");

                        var item = treeViewControllerData.InvokeMethod<TreeViewItem>("FindItem", scene.handle);

                        treeViewController.GetMemberValue("dragging").InvokeMethod("StartDrag", item, new List<int>());

                    }


                    selectorRect.MarkInteractive();

                    mouseDown();
                    mouseUp();
                    mouseDrag();

                }



                dummyRow();

                dropdownIcon();
                highlightedName();
                buttonLogic();

            }

            set_hoveredScene();
            sceneSelector();

        }

        int pressedSceneSelectorId;

        Vector2 mouseDownOnSelectorPos;









        public void UpdateState()
        {

            var sceneHierarchy = window?.GetFieldValue("m_SceneHierarchy");

            var treeViewController = sceneHierarchy.GetFieldValue("m_TreeView");
            var treeViewControllerData = treeViewController.GetMemberValue("data");


            isTreeFocused = EditorWindow.focusedWindow == window
                         && GUIUtility.keyboardControl == sceneHierarchy?.GetMemberValue<int>("m_TreeViewKeyboardControlID");

            renamingRow = EditorGUIUtility.editingTextField
                       && treeViewController?.GetMemberValue("state")?.GetMemberValue("renameOverlay")?.InvokeMethod<bool>("IsRenaming") == true;


#if UNITY_2021_1_OR_NEWER
            dragSelectionList = treeViewController?.GetFieldValue("m_DragSelection")?.GetIdList("m_List") ?? new();
#else
            dragSelectionList = treeViewController?.GetFieldValue<List<int>>("m_DragSelection");
#endif


            defaultParent = typeof(SceneView).InvokeMethod<Transform>("GetDefaultParentObjectIfSet")?.gameObject;


        }

        public bool isTreeFocused;
        public bool renamingRow;

        public List<int> dragSelectionList = new();

        GameObject defaultParent;











        public VHierarchyGUI(EditorWindow window) => this.window = window;

        public EditorWindow window;

        public VHierarchyController controller => VHierarchy.controllers_byWindow[window];

    }
}
#endif