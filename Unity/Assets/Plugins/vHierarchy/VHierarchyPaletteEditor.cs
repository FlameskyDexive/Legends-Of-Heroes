#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using UnityEditorInternal;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.IMGUI.Controls;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;
// using static VTools.VDebug;
using static VHierarchy.VHierarchyPalette;


namespace VHierarchy
{
    [CustomEditor(typeof(VHierarchyPalette))]
    class VHierarchyPaletteEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            void colors()
            {
                var rowRect = ExpandWidthLabelRect(cellSize).SetX(rowsOffsetX).SetWidth(rowWidth + 16);

                void backgroundHovered()
                {
                    if (!rowRect.IsHovered()) return;
                    if (pickingColor) return;
                    if (draggingRow) return;

                    rowRect.Draw(hoveredRowBackground);

                }
                void toggle()
                {
                    var toggleRect = rowRect.SetWidth(16).MoveX(5);

                    var prevEnabled = palette.colorsEnabled;
                    var newEnabled = EditorGUI.Toggle(toggleRect, palette.colorsEnabled);

                    if (prevEnabled != newEnabled)
                        palette.RecordUndo();

                    palette.colorsEnabled = newEnabled;

                    if (prevEnabled != newEnabled)
                        palette.Dirty();

                }
                void crossIcon()
                {
                    var crossIconRect = rowRect.SetX(rowsOffsetX + iconsOffsetX + iconSpacing / 2).SetWidth(iconSize).SetHeightFromMid(iconSize);

                    SetGUIColor(palette.colorsEnabled ? Color.white : disabledRowTint);

                    GUI.DrawTexture(crossIconRect, EditorIcons.GetIcon("CrossIcon"));

                    ResetGUIColor();

                }
                void color(int i)
                {
                    var cellRect = rowRect.MoveX(iconsOffsetX + (i + 1) * cellSize).SetWidth(cellSize).SetHeightFromMid(cellSize);

                    void backgroundPicking()
                    {
                        if (!pickingColor) return;
                        if (i != pickingColorAtIndex) return;

                        cellRect.DrawRounded(pickingBackground, 2);

                    }
                    void backgroundHovered_andStartPickingColor()
                    {
                        if (pickingColor) return;
                        if (!cellRect.IsHovered()) return;


                        SetGUIColor(Color.clear);

                        var clicked = GUI.Button(cellRect.Resize(1), "");

                        ResetGUIColor();




                        var isPressed = GUIUtility.hotControl == typeof(EditorGUIUtility).GetFieldValue<int>("s_LastControlID");

                        cellRect.DrawRounded(Greyscale(isPressed ? .39f : .43f), 2);




                        if (!clicked) return;

                        colorPickerWindow = EditorUtils.OpenColorPicker((c) => { palette.RecordUndo(); palette.Dirty(); palette.colors[i] = c; }, palette.colors[i], true, false);

                        colorPickerWindow.MoveTo(EditorGUIUtility.GUIToScreenPoint(cellRect.Move(-3, 50).position));

                        pickingColor = true;
                        pickingColorAtIndex = i;

                    }
                    void colorOutline()
                    {
                        var outlineColor = i < VHierarchyPalette.greyColorsCount ? Greyscale(.0f, .4f) : Greyscale(.15f, .2f);

                        if (!palette.colorsEnabled)
                            outlineColor *= disabledRowTint;


                        cellRect.Resize(3).DrawRounded(outlineColor, 4);

                    }
                    void color()
                    {
                        var brightness = palette.colorBrightness;
                        var saturation = palette.colorSaturation;
                        var drawGradients = palette.colorGradientsEnabled;

                        if (!palette.colorGradientsEnabled)
                            brightness *= isDarkTheme ? .75f : .97f;

                        if (i < VHierarchyPalette.greyColorsCount)
                        {
                            saturation = brightness = 1;
                            drawGradients = false;
                        }


                        var colorRaw = palette.colors[i];

                        var color = MathUtil.Lerp(Greyscale(.2f), colorRaw, brightness);

                        Color.RGBToHSV(color, out float h, out float s, out float v);
                        color = Color.HSVToRGB(h, s * saturation, v);

                        color = MathUtil.Lerp(color, colorRaw, .5f).SetAlpha(1);

                        if (!palette.colorsEnabled)
                            color *= disabledRowTint;

                        if (i >= VHierarchyPalette.greyColorsCount && isDarkTheme)
                            color *= 1.41f;




                        cellRect.Resize(4).DrawRounded(color, 3);

                        if (drawGradients)
                            cellRect.Resize(4).AddWidthFromRight(-2).DrawCurtainLeft(GUIColors.windowBackground.SetAlpha(.45f));

                    }
                    void updatePickingColor()
                    {
                        if (!pickingColor) return;

                        EditorApplication.RepaintHierarchyWindow();

                    }
                    void stopPickingColor()
                    {
                        if (!pickingColor) return;
                        if (colorPickerWindow) return;

                        pickingColor = false;

                    }


                    cellRect.MarkInteractive();


                    backgroundPicking();
                    backgroundHovered_andStartPickingColor();

                    colorOutline();
                    color();

                    updatePickingColor();
                    stopPickingColor();

                }
                void adjustColorsButton()
                {
                    var cellRect = rowRect.MoveX(iconsOffsetX + (palette.colors.Count + 1) * cellSize).SetWidth(cellSize).SetHeightFromMid(cellSize).MoveX(-1f);


                    var iconSize = 16;
                    var iconName = "Preset.Context";
                    var iconColor = Greyscale(.75f, palette.colorsEnabled ? (isDarkTheme ? 1 : .8f) : .5f);

                    if (!IconButton(cellRect, iconName, iconSize, iconColor)) return;


                    if (adjustColorsWindow) { adjustColorsWindow.Close(); return; }

                    var windowX = 107f.Min(this.GetMemberValue<EditorWindow>("propertyViewer").position.width - 310);
                    var windowY = cellRect.y + 25;
                    var windowWidth = 270;
                    var windowHeight = 92;

                    adjustColorsWindow = ScriptableObject.CreateInstance<AdjustColorsWindow>();
                    adjustColorsWindow.palette = palette;
                    adjustColorsWindow.paletteEditor = this;

                    adjustColorsWindow.ShowPopup();
                    adjustColorsWindow.Focus();

                    adjustColorsWindow.position = EditorGUIUtility.GUIToScreenRect(new Rect(windowX, windowY, windowWidth, windowHeight));

                }


                backgroundHovered();
                toggle();
                crossIcon();

                for (int i = 0; i < palette.colors.Count; i++)
                    color(i);

                adjustColorsButton();

                Space(rowSpacing - 2);

            }
            void icons()
            {
                void row(Rect rowRect, IconRow row)
                {
                    var isLastRow = row == palette.iconRows.Last();
                    var isDraggedRow = row == draggedRow;
                    var spaceForCrossIcon = 0f;


                    void updatePickingIcon()
                    {
                        if (!pickingIcon) return;
                        if (pickingIconAtRow != row) return;
                        if (pickingIconAtIndex >= row.customIcons.Count) return; // somehow happens if RecordUndo is used

                        palette.RecordUndo();
                        palette.Dirty();

                        row.customIcons[pickingIconAtIndex] = addIconWindow.hoveredIconName;

                    }
                    void stopPickingIcon()
                    {
                        if (!pickingIcon) return;
                        if (pickingIconAtRow != row) return;
                        if (addIconWindow) return;

                        if (pickingIconAtIndex < row.customIcons.Count)
                            if (row.customIcons[pickingIconAtIndex] == null)
                                row.customIcons.RemoveAt(pickingIconAtIndex);

                        pickingIcon = false;

                    }
                    void dragndrop()
                    {
                        if (!rowRect.IsHovered()) return;
                        if (!row.isCustom) return;

                        if (curEvent.isDragUpdate && DragAndDrop.objectReferences.First() is Texture2D)
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (!curEvent.isDragPerform) return;
                        if (!(DragAndDrop.objectReferences.Any(r => r is Texture2D))) return;

                        DragAndDrop.AcceptDrag();

                        palette.RecordUndo();
                        palette.Dirty();

                        foreach (var icon in DragAndDrop.objectReferences.Where(r => r is Texture2D))
                            row.customIcons.Add(icon.GetPath().ToGuid());

                    }

                    void calcSpaceForCrossIcon()
                    {
                        if (row == curFirstEnabledRow)
                            spaceForCrossIcon = crossIconAnimationT * cellSize;

                        if (row == crossIconAnimationSourceRow)
                            spaceForCrossIcon = (1 - crossIconAnimationT) * cellSize;

                    }

                    void backgroundHovered()
                    {
                        if (!rowRect.IsHovered()) return;
                        if (pickingColor) return;
                        if (pickingIcon) return;
                        if (draggingRow) return;
                        if (DragAndDrop.objectReferences.Any() && !row.isCustom) return;


                        rowRect.Draw(hoveredRowBackground);

                    }
                    void backgroundDragged()
                    {
                        if (!isDraggedRow) return;

                        rowRect.DrawBlurred(Greyscale(0, .3f), 12);
                        rowRect.Draw(draggedRowBackground);

                    }
                    void toggle()
                    {
                        var prevEnabled = row.enabled;
                        var newEnabled = EditorGUI.Toggle(rowRect.SetWidth(16).MoveX(5), row.enabled);

                        if (prevEnabled != newEnabled)
                            palette.RecordUndo();

                        row.enabled = newEnabled;

                        if (prevEnabled != newEnabled)
                            palette.Dirty();

                    }
                    void addIconButton()
                    {
                        if (!row.isCustom) return;
                        if (pickingIcon && pickingIconAtRow == row) return;


                        var cellRect = rowRect.MoveX(iconsOffsetX + row.customIcons.Count * cellSize + spaceForCrossIcon).SetWidth(cellSize).SetHeightFromMid(cellSize);

                        var iconSize = 16;
                        var iconName = "Toolbar Plus";
                        var iconColor = Greyscale(.73f, row.enabled ? (isDarkTheme ? 1 : .65f) : .5f);

                        if (!IconButton(cellRect, iconName, iconSize, iconColor)) return;



                        palette.RecordUndo();

                        row.customIcons.Add(null);



                        var windowX = 15;
                        var windowY = cellRect.y + 23;
                        var windowWidth = (this.GetMemberValue<EditorWindow>("propertyViewer").position.width - 26).Min(679);
                        var windowHeight = windowWidth * 1.2f;

                        windowWidth = (windowWidth / AddIconWindow.cellSize).FloorToInt() * AddIconWindow.cellSize - 3;


                        addIconWindow = ScriptableObject.CreateInstance<AddIconWindow>();
                        addIconWindow.palette = palette;
                        addIconWindow.paletteEditor = this;

                        addIconWindow.ShowPopup();
                        addIconWindow.Focus();

                        addIconWindow.position = EditorGUIUtility.GUIToScreenRect(new Rect(windowX, windowY, windowWidth, windowHeight));

                        addIconWindow.Init();


                        pickingIcon = true;
                        pickingIconAtIndex = row.customIcons.Count - 1;
                        pickingIconAtRow = row;


                    }
                    void icon(int i)
                    {
                        var cellRect = rowRect.MoveX(iconsOffsetX + spaceForCrossIcon + i * cellSize).SetWidth(cellSize).SetHeightFromMid(cellSize);


                        void backgroundPicking()
                        {
                            if (!pickingIcon) return;
                            if (pickingIconAtRow != row) return;
                            if (pickingIconAtIndex != i) return;

                            cellRect.DrawRounded(pickingBackground, 2);

                        }
                        void backgroundHovered_andEditIconButton()
                        {
                            if (!row.isCustom) return;
                            if (pickingIcon) return;
                            if (!cellRect.IsHovered()) return;
                            if (DragAndDrop.objectReferences.Any()) return;


                            SetGUIColor(Color.clear);

                            var clicked = GUI.Button(cellRect.Resize(1), "");

                            ResetGUIColor();




                            var isPressed = GUIUtility.hotControl == typeof(EditorGUIUtility).GetFieldValue<int>("s_LastControlID");

                            cellRect.DrawRounded(Greyscale(isPressed ? .39f : .45f), 2);




                            if (!clicked) return;

                            GenericMenu menu = new();

                            menu.AddItem(new GUIContent("Move left"), false, i == 0 ? null : () =>
                            {
                                palette.RecordUndo();
                                palette.Dirty();

                                var icon = row.customIcons[i];

                                row.customIcons.RemoveAt(i);
                                row.customIcons.AddAt(icon, i - 1);

                            });
                            menu.AddItem(new GUIContent("Move right"), false, i == row.customIcons.Count - 1 ? null : () =>
                            {
                                palette.RecordUndo();
                                palette.Dirty();

                                var icon = row.customIcons[i];

                                row.customIcons.RemoveAt(i);
                                row.customIcons.AddAt(icon, i + 1);

                            });

                            menu.AddSeparator("");

                            menu.AddItem(new GUIContent("Remove icon"), false, () => { palette.RecordUndo(); row.customIcons.RemoveAt(i); palette.Dirty(); });


                            menu.ShowAsContext();

                        }

                        void drawIcon()
                        {
                            var iconNameOrGuid = row.isCustom ? row.customIcons[i] : row.builtinIcons[i];

                            if (iconNameOrGuid == null) return;

                            var iconNameOrPath = iconNameOrGuid.Length == 32 ? iconNameOrGuid.ToPath() : iconNameOrGuid;
                            var icon = EditorIcons.GetIcon(iconNameOrPath) ?? Texture2D.blackTexture;


                            var cellRect = rowRect.MoveX(iconsOffsetX + spaceForCrossIcon + i * cellSize).SetWidth(cellSize).SetHeightFromMid(cellSize);
                            var iconRect = cellRect.SetSizeFromMid(iconSize);

                            if (icon.width < icon.height) iconRect = iconRect.SetWidthFromMid(iconRect.height * icon.width / icon.height);
                            if (icon.height < icon.width) iconRect = iconRect.SetHeightFromMid(iconRect.width * icon.height / icon.width);



                            SetGUIColor(row.enabled ? Color.white : disabledRowTint);

                            GUI.DrawTexture(iconRect, icon);

                            ResetGUIColor();

                        }


                        cellRect.MarkInteractive();

                        backgroundPicking();
                        backgroundHovered_andEditIconButton();

                        drawIcon();

                    }


                    rowRect.MarkInteractive();

                    updatePickingIcon();
                    stopPickingIcon();
                    dragndrop();

                    calcSpaceForCrossIcon();
                    backgroundHovered();
                    backgroundDragged();
                    toggle();
                    addIconButton();

                    for (int i = 0; i < row.iconCount; i++)
                        icon(i);

                }

                void updateRowsCount()
                {
                    palette.iconRows.RemoveAll(r => r.isEmpty && r != palette.iconRows.Last());

                    if (!palette.iconRows.Last().isEmpty)
                        palette.iconRows.Add(new IconRow());

                }
                void updateRowGapsCount()
                {
                    while (rowGaps.Count < palette.iconRows.Count)
                        rowGaps.Add(0);

                    while (rowGaps.Count > palette.iconRows.Count)
                        rowGaps.RemoveLast();

                }

                void normalRow(int i)
                {
                    Space(rowGaps[i] * (cellSize + rowSpacing));

                    if (i == 0 && lastRect.y != 0)
                        firstRowY = lastRect.y;

                    Space(cellSize + rowSpacing);

                    var rowRect = Rect.zero.SetPos(rowsOffsetX, lastRect.y).SetSize(rowWidth, cellSize);

                    if (curEvent.isRepaint)
                        if (rowRect.IsHovered())
                            hoveredRow = palette.iconRows[i];


                    row(rowRect, palette.iconRows[i]);

                }
                void draggedRow_()
                {
                    if (!draggingRow) return;

                    draggedRowY = (curEvent.mousePosition.y + draggedRowHoldOffset).Clamp(firstRowY, firstRowY + (palette.iconRows.Count - 1) * (cellSize + rowSpacing));

                    var rowRect = Rect.zero.SetPos(rowsOffsetX, draggedRowY).SetSize(rowWidth, cellSize);

                    row(rowRect, draggedRow);

                }
                void crossIcon()
                {
                    if (!palette.iconRows.Any(r => r.enabled)) return;

                    var rect = Rect.zero.SetPos(rowsOffsetX + iconsOffsetX, crossIconY).SetSize(cellSize, cellSize).Resize(iconSpacing / 2);

                    GUI.DrawTexture(rect, EditorIcons.GetIcon("CrossIcon"));

                }


                updateRowsCount();
                updateRowGapsCount();


                if (curEvent.isRepaint)
                    hoveredRow = null;

                for (int i = 0; i < palette.iconRows.Count; i++)
                    normalRow(i);

                crossIcon();

                draggedRow_();


            }
            void tutor()
            {
                SetGUIEnabled(false);


                Space(4);
                GUILayout.Label("Add icons with drag-and-drop or by clicking '+'");

                Space(4);
                GUILayout.Label("Click added icon to move or remove it");

                Space(4);
                GUILayout.Label("Drag rows to reorder them");


                ResetGUIEnabled();

            }


            Space(15);
            colors();

            Space(15);
            icons();

            Space(22);
            tutor();

            UpdateAnimations();

            UpdateDragging();

            palette.Dirty();

            if (draggingRow || animatingCrossIcon)
                Repaint();

        }

        float iconSize => 14;
        float iconSpacing => 6;
        float cellSize => iconSize + iconSpacing;
        float rowSpacing = 1;
        float rowsOffsetX => 14;
        float iconsOffsetX => 27;

        Color hoveredRowBackground => Greyscale(isDarkTheme ? 1 : 0, .05f);
        Color draggedRowBackground => Greyscale(isDarkTheme ? .3f : .9f);
        Color pickingBackground => Greyscale(1, .17f);
        Color disabledRowTint => Greyscale(1, .45f);

        float rowWidth => cellSize * Mathf.Max(palette.colors.Count, palette.iconRows.Max(r => r.iconCount + 1)) + 55;

        bool pickingColor;
        int pickingColorAtIndex;
        EditorWindow colorPickerWindow;

        bool pickingIcon;
        int pickingIconAtIndex;
        IconRow pickingIconAtRow;
        AddIconWindow addIconWindow;

        IconRow hoveredRow;

        float firstRowY = 51;

        static AdjustColorsWindow adjustColorsWindow;





        void UpdateAnimations()
        {
            void lerpRowGaps()
            {
                if (!curEvent.isLayout) return;

                var lerpSpeed = draggingRow ? 12 : 12321;

                for (int i = 0; i < rowGaps.Count; i++)
                    rowGaps[i] = MathUtil.Lerp(rowGaps[i], draggingRow && i == insertDraggedRowAtIndex ? 1 : 0, lerpSpeed, editorDeltaTime);

                for (int i = 0; i < rowGaps.Count; i++)
                    if (rowGaps[i].Approx(0))
                        rowGaps[i] = 0;
                    else if (rowGaps[i].Approx(1))
                        rowGaps[i] = 1;


            }

            void lerpCrossIconAnimationT()
            {
                if (!curEvent.isLayout) return;

                var lerpSpeed = 12;

                MathUtil.Lerp(ref crossIconAnimationT, 1, lerpSpeed, editorDeltaTime);

            }
            void startCrossIconAnimation()
            {
                if (prevFirstEnabledRow == null) { prevFirstEnabledRow = curFirstEnabledRow; return; }
                if (prevFirstEnabledRow == curFirstEnabledRow) return;

                crossIconAnimationT = 0;
                crossIconAnimationSourceRow = prevFirstEnabledRow;

                prevFirstEnabledRow = curFirstEnabledRow;

            }
            void stopCrossIconAnimation()
            {
                if (!crossIconAnimationT.Approx(1)) return;

                crossIconAnimationT = 1;
                crossIconAnimationSourceRow = null;

            }
            void calcCrossIconY()
            {
                var indexOfFirstEnabled = palette.iconRows.IndexOfFirst(r => r.enabled);
                var yOfFirstEnabled = firstRowY + indexOfFirstEnabled * (cellSize + rowSpacing);
                for (int i = 0; i < indexOfFirstEnabled + 1; i++)
                    yOfFirstEnabled += rowGaps[i] * (cellSize + rowSpacing);


                var indexOfSourceRow = palette.iconRows.IndexOf(crossIconAnimationSourceRow);
                var yOfSourceRow = firstRowY + indexOfSourceRow * (cellSize + rowSpacing);
                for (int i = 0; i < indexOfSourceRow + 1; i++)
                    yOfSourceRow += rowGaps[i] * (cellSize + rowSpacing);

                if (crossIconAnimationSourceRow == draggedRow)
                    yOfSourceRow = draggedRowY;


                crossIconY = MathUtil.Lerp(yOfSourceRow, yOfFirstEnabled, crossIconAnimationT);

                if (indexOfFirstEnabled == indexOfSourceRow)
                    crossIconAnimationT = 1;

            }


            lerpRowGaps();

            lerpCrossIconAnimationT();
            startCrossIconAnimation();
            stopCrossIconAnimation();
            calcCrossIconY();

        }

        List<float> rowGaps = new();

        float crossIconY = 51;
        float crossIconAnimationT = 1;
        IconRow crossIconAnimationSourceRow;
        bool animatingCrossIcon => crossIconAnimationT != 1;

        [System.NonSerialized] IconRow prevFirstEnabledRow;
        IconRow curFirstEnabledRow => palette.iconRows.FirstOrDefault(r => r.enabled);






        void UpdateDragging()
        {
            void startDragging()
            {
                if (draggingRow) return;
                if (!curEvent.isMouseDrag) return;
                if (hoveredRow == null) return;
                if (hoveredRow == palette.iconRows.Last()) return;

                palette.RecordUndo();

                draggingRow = true;
                draggedRow = hoveredRow;
                draggingRowFromIndex = palette.iconRows.IndexOf(hoveredRow);
                draggedRowHoldOffset = firstRowY + draggingRowFromIndex * (cellSize + rowSpacing) - curEvent.mousePosition.y;

                palette.iconRows.Remove(hoveredRow);
                rowGaps[draggingRowFromIndex] = 1;

            }
            void updateDragging()
            {
                if (!draggingRow) return;

                insertDraggedRowAtIndex = ((curEvent.mousePosition.y - firstRowY) / (cellSize + rowSpacing)).FloorToInt().Clamp(0, palette.iconRows.Count - 1);

                EditorGUIUtility.hotControl = EditorGUIUtility.GetControlID(FocusType.Passive);

            }
            void stopDragging()
            {
                if (!draggingRow) return;
                if (!curEvent.isMouseUp) return;

                palette.RecordUndo();
                palette.Dirty();

                palette.iconRows.AddAt(draggedRow, insertDraggedRowAtIndex);

                rowGaps[insertDraggedRowAtIndex] = 0;

                draggingRow = false;
                draggedRow = null;

                EditorGUIUtility.hotControl = 0;

            }


            startDragging();
            updateDragging();
            stopDragging();

        }

        IconRow draggedRow;
        bool draggingRow;
        int draggingRowFromIndex;
        float draggedRowHoldOffset;
        float draggedRowY;
        int insertDraggedRowAtIndex;






        VHierarchyPalette palette => target as VHierarchyPalette;

    }


    class AddIconWindow : EditorWindow
    {

        void OnGUI()
        {
            void header()
            {
                var headerRect = Rect.zero.SetHeight(20).SetWidth(position.width);
                var closeButtonRect = headerRect.SetWidthFromRight(16).SetHeightFromMid(16).Move(-3, -.5f);

                void background()
                {
                    headerRect.Draw(EditorGUIUtility.isProSkin ? Greyscale(.18f) : Greyscale(.7f));
                }
                void title()
                {
                    SetGUIColor(Greyscale(.8f));
                    SetLabelAlignmentCenter();

                    GUI.Label(headerRect.MoveY(-1), "Add icon");

                    ResetLabelStyle();
                    ResetGUIColor();

                }
                void closeButton()
                {
                    var colorNormal = isDarkTheme ? Greyscale(.55f) : Greyscale(.35f);
                    var colorHovered = isDarkTheme ? Greyscale(.9f) : colorNormal;

                    var iconSize = 14;

                    if (IconButton(closeButtonRect, "CrossIcon", iconSize, colorNormal, colorHovered))
                        Close();

                }
                void escHint()
                {
                    if (!closeButtonRect.IsHovered()) return;

                    var textRect = headerRect.SetWidthFromRight(42).MoveY(-.5f).MoveX(1);
                    var fontSize = 11;
                    var color = Greyscale(.65f);


                    SetLabelFontSize(fontSize);
                    SetGUIColor(color);

                    GUI.Label(textRect, "Esc");

                    ResetGUIColor();
                    ResetLabelStyle();

                }

                background();
                title();
                closeButton();
                escHint();

                Space(headerRect.height);

            }
            void search()
            {
                var backgroundRect = ExpandWidthLabelRect(height: 21).SetWidthFromMid(position.width);
                var backgroundColor = isDarkTheme ? Greyscale(.25f) : Greyscale(.8f);

                backgroundRect.Draw(backgroundColor);


                var lineRect = backgroundRect.SetHeightFromBottom(1).MoveY(.5f);
                var lineColor = isDarkTheme ? Greyscale(.15f) : Greyscale(.7f);

                lineRect.Draw(lineColor);


                var searchRect = backgroundRect.Resize(2);

                EditorGUI.BeginChangeCheck();

                searchString = searchField.OnGUI(searchRect, searchString);

                if (EditorGUI.EndChangeCheck())
                {
                    GenerateRows();
                    FilterIconsBySearch();
                    GenerateRows();
                }

            }
            void icons()
            {
                void row(int i)
                {
                    var rowRect = position.SetPos(0, 0).SetHeight(cellSize).MoveY(i * rowHeight);
                    var iconNames = rows[i];

                    void icon(int i)
                    {
                        var iconName = iconNames[i];
                        var icon = EditorIcons.GetIcon(iconName);

                        var cellRect = rowRect.SetWidth(cellSize).MoveX(i * cellSize + paddingLeft);
                        var iconRect = cellRect.SetSizeFromMid(iconSize);

                        if (icon.width < icon.height) iconRect = iconRect.SetWidthFromMid(iconRect.height * icon.width / icon.height);
                        if (icon.height < icon.width) iconRect = iconRect.SetHeightFromMid(iconRect.width * icon.height / icon.width);



                        var hoverRect = cellRect.AddHeightFromMid(rowSpacing);

                        hoverRect.MarkInteractive();

                        if (hoverRect.IsHovered())
                            cellRect.Draw(Greyscale(isDarkTheme ? .42f : .69f));



                        GUI.DrawTexture(iconRect, EditorIcons.GetIcon(iconName));



                        if (hoverRect.IsHovered())
                            hoveredIconName = iconName;

                        if (hoverRect.IsHovered() && curEvent.isMouseDown)
                            Close();

                    }

                    for (int ii = 0; ii < iconNames.Count; ii++)
                        icon(ii);
                }


                if (curEvent.isRepaint)
                    hoveredIconName = null;


                scrollPos = GUILayout.BeginScrollView(new Vector2(0, scrollPos)).y;

                GUILayout.Space(rows.Count * rowHeight + 23);


                var i0 = (scrollPos / rowHeight).FloorToInt();
                var i1 = (i0 + ((position.height - 30) / rowHeight).CeilToInt()).Min(rows.Count);

                for (int ii = i0; ii < i1; ii++)
                    row(ii);


                GUILayout.EndScrollView();

            }
            void hoveredIconLabel()
            {
                if (hoveredIconName == null) return;


                var nameRect = position.SetPos(0, 0).SetHeightFromBottom(18).SetWidth(hoveredIconName.GetLabelWidth() + 6).Move(1, -1);

                var shadowRect = nameRect.AddWidthFromRight(10).AddHeight(10);


                shadowRect.DrawBlurred(GUIColors.windowBackground, 12);
                shadowRect.DrawBlurred(GUIColors.windowBackground.SetAlpha(.4f), 8);


                SetLabelAlignmentCenter();

                GUI.Label(nameRect, hoveredIconName);

                ResetLabelStyle();

            }
            void closeOnEsc()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.keyCode != KeyCode.Escape) return;

                hoveredIconName = null;

                Close();

            }
            void outline()
            {
                if (Application.platform == RuntimePlatform.OSXEditor) return;

                position.SetPos(0, 0).DrawOutline(Greyscale(.1f));

            }

            header();
            search();
            icons();
            hoveredIconLabel();
            closeOnEsc();
            outline();

            paletteEditor.Repaint();

            if (EditorWindow.focusedWindow != this)
                Close();

        }

        public static float iconSize => 16;
        public static float iconSpacing => 6;
        public static float cellSize => iconSize + iconSpacing;
        public static float rowSpacing = 1;

        public static float rowHeight => cellSize + rowSpacing;

        public static float paddingLeft => 3;
        public static float paddingRight => 3;

        public string hoveredIconName;

        static string searchString = "";
        static float scrollPos;





        void LoadAllIcons()
        {
            if (allIconNames != null) return;


            (float h, float s, float v, float a) GetAverageColor(Texture2D texture)
            {

                var readableTexture = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);

                Graphics.CopyTexture(texture, readableTexture);

                var pixels = readableTexture.GetPixels32();

                readableTexture.DestroyImmediate();


                float hSum = 0;
                float sSum = 0;
                float vSum = 0;

                int nonTransparentPxCount = pixels.Length;
                int coloredPxCount = pixels.Length;

                for (var i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i].a <= .1f) { nonTransparentPxCount--; coloredPxCount--; continue; }

                    Color.RGBToHSV(pixels[i], out float h, out float s, out float v);

                    if (s > .1f)
                        hSum += h;
                    else
                        coloredPxCount--;

                    sSum += s;

                    vSum += v;

                }

                var hAvg = hSum / coloredPxCount;
                var sAvg = sSum / nonTransparentPxCount;
                var vAvg = vSum / nonTransparentPxCount;
                var aAvg = nonTransparentPxCount / pixels.Length.ToFloat();

                if (coloredPxCount == 0)
                    hAvg = -1;


                return (hAvg, sAvg, vAvg, aAvg);

            }



            var editorAssetBundle = typeof(EditorGUIUtility).InvokeMethod<AssetBundle>("GetEditorAssetBundle");

            allIconNames = (

                from path in editorAssetBundle.GetAllAssetNames()


                let icon = editorAssetBundle.LoadAsset<Texture2D>(path)

                where icon


                where path.StartsWith("icons/")
                where !path.Contains("avatarinspector")

                where !icon.name.ToLower().StartsWith("d_")

                where !icon.name.ToLower().EndsWith(".small")
                where !icon.name.ToLower().EndsWith("_sml")

                where !icon.name.Contains("@")
                where !icon.name.Contains("TreeEditor")
                where !icon.name.Contains("scene-template")
                where !icon.name.Contains("StateMachineEditor.Background")
                where !icon.name.Contains("SpeedTree")
                where !icon.name.Contains("TextMesh")
                where !icon.name.Contains("Profiler.Instrumentation")
                where !icon.name.Contains("Profiler.Record")
                where !icon.name.Contains("SocialNetworks")
                where !icon.name.Contains("Groove")



                let avgColor = GetAverageColor(icon)

                where avgColor.a > .1f



                orderby avgColor.h * -3f
                      + avgColor.s * .09f
                      + avgColor.v * 0f, icon.name



                select icon.name

                            ).ToHashSet()
                             .ToList();

        }

        static List<string> allIconNames;



        void FilterIconsBySearch()
        {
            filteredIcons = (

                from iconName in allIconNames

                where iconName.ToLower().Contains(searchString.ToLower())

                orderby iconName.ToLower().IndexOf(searchString.ToLower(), System.StringComparison.Ordinal)

                select iconName

                            ).ToList();
        }

        static List<string> filteredIcons;



        void GenerateRows()
        {

            var iconsPerRow = ((position.width - paddingLeft - paddingRight) / cellSize).FloorToInt();

            rows = new();

            var curRow = new List<string>();

            foreach (var icon in filteredIcons)
            {
                curRow.Add(icon);

                if (curRow.Count == iconsPerRow)
                {
                    rows.Add(curRow);
                    curRow = new();
                }
            }

            if (curRow.Any())
                rows.Add(curRow);

        }

        static List<List<string>> rows;






        public void Init()
        {
            LoadAllIcons();
            FilterIconsBySearch();
            GenerateRows();

            searchField = new();

        }

        SearchField searchField;





        public VHierarchyPalette palette;
        public VHierarchyPaletteEditor paletteEditor;

    }

    class AdjustColorsWindow : EditorWindow
    {
        void OnGUI()
        {
            void header()
            {
                var headerRect = Rect.zero.SetHeight(20).SetWidth(position.width);
                var closeButtonRect = headerRect.SetWidthFromRight(16).SetHeightFromMid(16).Move(-3, -.5f);

                void background()
                {
                    headerRect.Draw(EditorGUIUtility.isProSkin ? Greyscale(.18f) : Greyscale(.7f));
                }
                void title()
                {
                    SetGUIColor(Greyscale(.8f));
                    SetLabelAlignmentCenter();

                    GUI.Label(headerRect.MoveY(-1), "Adjust colors");

                    ResetLabelStyle();
                    ResetGUIColor();

                }
                void closeButton()
                {
                    var colorNormal = isDarkTheme ? Greyscale(.55f) : Greyscale(.35f);
                    var colorHovered = isDarkTheme ? Greyscale(.9f) : colorNormal;

                    var iconSize = 14;

                    if (IconButton(closeButtonRect, "CrossIcon", iconSize, colorNormal, colorHovered))
                        Close();

                }
                void escHint()
                {
                    if (!closeButtonRect.IsHovered()) return;

                    var textRect = headerRect.SetWidthFromRight(42).MoveY(-.5f).MoveX(1);
                    var fontSize = 11;
                    var color = Greyscale(.65f);


                    SetLabelFontSize(fontSize);
                    SetGUIColor(color);

                    GUI.Label(textRect, "Esc");

                    ResetGUIColor();
                    ResetLabelStyle();

                }
                void outline()
                {
                    if (Application.platform == RuntimePlatform.OSXEditor) return;

                    position.SetPos(0, 0).DrawOutline(Greyscale(.1f));

                }

                background();
                title();
                closeButton();
                escHint();
                outline();

                Space(headerRect.height);

            }
            void body()
            {
                EditorGUIUtility.labelWidth = 85;
                EditorGUIUtility.keyboardControl = 0;

                palette.RecordUndo();


                EditorGUI.BeginChangeCheck();

                palette.colorBrightness = (EditorGUILayout.Slider("Brightness", palette.colorBrightness, 0, 2) / .1f).RoundToInt() * .1f;
                palette.colorSaturation = (EditorGUILayout.Slider("Saturation", palette.colorSaturation, 0, 2) / .1f).RoundToInt() * .1f;

                palette.colorGradientsEnabled = EditorGUILayout.Toggle("Gradients", palette.colorGradientsEnabled);

                if (EditorGUI.EndChangeCheck())
                {
                    VHierarchy.goInfoCache.Clear();

                    paletteEditor.Repaint();
                    EditorApplication.RepaintHierarchyWindow();
                    palette.Dirty();
                }


                EditorGUIUtility.labelWidth = 0;

            }
            void closeOnEsc()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.keyCode != KeyCode.Escape) return;

                Close();

            }


            header();

            Space(7);
            BeginIndent(10);

            body();

            EndIndent(5);



            closeOnEsc();

            if (EditorWindow.focusedWindow != this)
                Close();

            Repaint(); // for undo

        }

        public VHierarchyPalette palette;
        public VHierarchyPaletteEditor paletteEditor;

    }

}
#endif