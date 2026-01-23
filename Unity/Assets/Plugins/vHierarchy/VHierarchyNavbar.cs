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
using static VHierarchy.VHierarchy;



namespace VHierarchy
{
    public class VHierarchyNavbar
    {

        public void OnGUI(Rect navbarRect)
        {
            void updateState()
            {
                if (!curEvent.isLayout) return;



                var isTreeFocused = window.GetFieldValue("m_SceneHierarchy").GetMemberValue<int>("m_TreeViewKeyboardControlID") == GUIUtility.keyboardControl;

                var isWindowFocused = window == EditorWindow.focusedWindow;



                if (!isTreeFocused && isSearchActive)
                    EditorGUI.FocusTextInControl("SearchFilter");


                if (isTreeFocused || !isWindowFocused)
                    if (window.GetMemberValue("m_SearchFilter").ToString().IsNullOrEmpty())
                        isSearchActive = false;


                // in vFolders the following is used to check if search is active:
                // GUI.GetNameOfFocusedControl() == "SearchFilter";
                // but in hierarchy focused control changes erratically when multiple scene headers are visible
                // so a bool state is used instead




                this.defaultParent = typeof(SceneView).InvokeMethod<Transform>("GetDefaultParentObjectIfSet")?.gameObject;

            }

            void background()
            {
                var backgroundColor = Greyscale(isDarkTheme ? .235f : .8f);
                var lineColor = Greyscale(isDarkTheme ? .13f : .58f);

                navbarRect.Draw(backgroundColor);

                navbarRect.SetHeightFromBottom(1).MoveY(1).Draw(lineColor);

            }
            void hiddenMenu()
            {
                if (!curEvent.holdingAlt) return;
                if (!curEvent.isMouseUp) return;
                if (curEvent.mouseButton != 1) return;
                if (!navbarRect.IsHovered()) return;


                void selectData()
                {
                    Selection.activeObject = data;
                }
                void selectPalette()
                {
                    Selection.activeObject = palette;
                }
                void clearCache()
                {
                    VHierarchyCache.Clear();
                }



                GenericMenu menu = new();

                menu.AddDisabledItem(new GUIContent("vHierarchy hidden menu"));

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Select data"), false, selectData);
                menu.AddItem(new GUIContent("Select palette"), false, selectPalette);

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Clear cache"), false, clearCache);

                menu.ShowAsContext();

            }


            void plusButton()
            {

                var buttonRect = navbarRect.SetWidth(28).MoveX(4.5f);

                if (Application.unityVersion.StartsWith("6000"))
                    buttonRect = buttonRect.MoveY(-.49f);


                var iconName = "Plus Thicker";
                var iconSize = 16;
                var colorNormal = Greyscale(isDarkTheme ? .7f : .44f);
                var colorHovered = Greyscale(isDarkTheme ? 1f : .42f);
                var colorPressed = Greyscale(isDarkTheme ? .75f : .6f);

                if (!IconButton(buttonRect, iconName, iconSize, colorNormal, colorHovered, colorPressed)) return;


                GUIUtility.hotControl = 0;

                var sceneHierarchy = window.GetMemberValue("m_SceneHierarchy");
                var m_CustomParentForNewGameObjects = window.GetMemberValue("m_SceneHierarchy").GetMemberValue<Transform>("m_CustomParentForNewGameObjects");
                var targetSceneHandle = m_CustomParentForNewGameObjects != null ? (int)m_CustomParentForNewGameObjects.gameObject.scene.handle : 0;


                var menu = new GenericMenu();

#if UNITY_6000_3_OR_NEWER
                sceneHierarchy.GetType().GetMethod("AddCreateGameObjectItemsToSceneMenu", maxBindingFlags).Invoke(sceneHierarchy, new object[] { menu, SceneManager.GetActiveScene() });
#else
                sceneHierarchy.GetType().GetMethod("AddCreateGameObjectItemsToMenu", maxBindingFlags).Invoke(sceneHierarchy, new object[] { menu, null, true, true, false, targetSceneHandle, 3 });
#endif




                typeof(UnityEditor.SceneManagement.SceneHierarchyHooks).InvokeMethod("AddCustomItemsToCreateMenu", menu);

                menu.DropDown(buttonRect);


            }

            void searchButton()
            {
                if (searchAnimationT == 1) return;


                var buttonRect = navbarRect.SetWidthFromRight(28).MoveX(-5);

                var iconName = "Search_";
                var iconSize = 16;
                var colorNormal = Greyscale(isDarkTheme ? .75f : .2f);
                var colorHovered = Greyscale(isDarkTheme ? 1f : .2f);
                var colorPressed = Greyscale(isDarkTheme ? .75f : .5f);


                if (!IconButton(buttonRect, iconName, iconSize, colorNormal, colorHovered, colorPressed)) return;

                EditorGUI.FocusTextInControl("SearchFilter");

                EditorApplication.delayCall += () => EditorGUI.FocusTextInControl("SearchFilter");

                isSearchActive = true;

            }
            void searchOnCtrlF()
            {
                if (searchAnimationT == 1) return;

                if (!curEvent.isKeyDown) return;
                if (!curEvent.holdingCmd && !curEvent.holdingCtrl) return;
                if (curEvent.keyCode != KeyCode.F) return;


                EditorGUI.FocusTextInControl("SearchFilter");

                EditorApplication.delayCall += () => EditorGUI.FocusTextInControl("SearchFilter");

                isSearchActive = true;


                curEvent.Use();

            }
            void collapseAllButton()
            {
                if (searchAnimationT == 1) return;


                var buttonRect = navbarRect.SetWidthFromRight(28).MoveX(-33);

                var iconName = "Collapse";
                var iconSize = 16;
                var colorNormal = Greyscale(isDarkTheme ? .71f : .44f);
                var colorHovered = Greyscale(isDarkTheme ? 1f : .42f);
                var colorPressed = Greyscale(isDarkTheme ? .75f : .6f);


                if (!IconButton(buttonRect, iconName, iconSize, colorNormal, colorHovered, colorPressed)) return;

                controller.CollapseAll();

            }
            void bookmarks()
            {
                if (searchAnimationT == 1) return;
                if (isSearchActive && !curEvent.isRepaint) return;

                void createData()
                {
                    if (data) return;
                    if (!navbarRect.IsHovered()) return;
                    if (!DragAndDrop.objectReferences.Any()) return;

                    data = ScriptableObject.CreateInstance<VHierarchyData>();

                    AssetDatabase.CreateAsset(data, GetScriptPath("VHierarchy").GetParentPath().CombinePath("vHierarchy Data.asset"));

                }
                void createReorderableRow()
                {
                    if (!data) return;
                    if (reorderableRow != null) return;

                    reorderableRow = new ReorderableRow<Bookmark>();

                    reorderableRow.items = data.bookmarks;
                    reorderableRow.itemsHolderObject = data;

                    reorderableRow.ItemGUI = BookmarkGUI;

                    reorderableRow.GetItemIndex = GetBookmarkIndex;
                    reorderableRow.GetItemWidth = (_) => bookmarkWidth;
                    reorderableRow.GetItemCenterX_withGaps = (i) => GetBookmarkCenterX(i, true);
                    reorderableRow.GetItemCenterX_withoutGaps = (i) => GetBookmarkCenterX(i, true);

                    reorderableRow.CanCreateItemFrom = CanCreateItemFrom;
                    reorderableRow.CreateItem = CreateItem;

                }
                void divider()
                {
                    if (!data) return;
                    if (!data.bookmarks.Any(r => r.go)) return;


                    var dividerRect = navbarRect.SetWidthFromRight(1).SetHeightFromMid(16).MoveX(-65).MoveX(1.5f);

                    var dividerColor = Greyscale(isDarkTheme ? .33f : .64f);


                    dividerRect.Draw(dividerColor);

                }
                void reorderableRowGui()
                {
                    if (!data) return;

                    this.navbarRect = navbarRect;
                    this.bookmarksRect = navbarRect.AddWidth(-69).AddWidthFromRight(-60).MoveX(2).MoveX(-3);

                    reorderableRow.OnGUI(bookmarksRect);

                }

                createData();
                createReorderableRow();
                divider();
                reorderableRowGui();

            }

            void searchField()
            {
                if (searchAnimationT == 0) return;

                var searchFieldRect = navbarRect.SetHeightFromMid(20).AddWidth(-33).SetWidthFromRight(200f.Min(window.position.width - 120)).Move(-1, 2);


                GUILayout.BeginArea(searchFieldRect);
                GUILayout.BeginHorizontal();

                Space(2);
                window.InvokeMethod("SearchFieldGUI");

                GUILayout.EndHorizontal();
                GUILayout.EndArea();

            }
            void closeSearchButton()
            {
                if (searchAnimationT == 0) return;


                var buttonRect = navbarRect.SetWidthFromRight(30).MoveX(-4);

                var iconName = "CrossIcon";
                var iconSize = 15;
                var colorNormal = Greyscale(isDarkTheme ? .9f : .2f);
                var colorHovered = Greyscale(isDarkTheme ? 1f : .2f);
                var colorPressed = Greyscale(isDarkTheme ? .75f : .5f);


                if (!IconButton(buttonRect, iconName, iconSize, colorNormal, colorHovered, colorPressed)) return;

                window.InvokeMethod("ClearSearchFilter");

                GUIUtility.keyboardControl = 0;

                isSearchActive = false;

            }
            void closeSearchOnEsc()
            {
                if (!isSearchActive) return;
                if (curEvent.keyCode != KeyCode.Escape) return;

                window.InvokeMethod("ClearSearchFilter");

                GUIUtility.keyboardControl = 0;

                isSearchActive = false;

            }

            void searchAnimation()
            {
                if (!curEvent.isLayout) return;


                var lerpSpeed = 8f;

                if (isSearchActive)
                    MathUtil.SmoothDamp(ref searchAnimationT, 1, lerpSpeed, ref searchAnimationDerivative, editorDeltaTime);
                else
                    MathUtil.SmoothDamp(ref searchAnimationT, 0, lerpSpeed, ref searchAnimationDerivative, editorDeltaTime);


                if (isSearchActive && searchAnimationT > .99f)
                    searchAnimationT = 1;

                if (!isSearchActive && searchAnimationT < .01f)
                    searchAnimationT = 0;


                animatingSearch = searchAnimationT != 0 && searchAnimationT != 1;

            }

            void buttonsAndBookmarks()
            {
                SetGUIColor(Greyscale(1, (1 - searchAnimationT).Pow(2)));
                GUI.BeginGroup(window.position.SetPos(0, 0).MoveX(-searchAnimationDistance * searchAnimationT));

                searchButton();
                searchOnCtrlF();
                collapseAllButton();
                bookmarks();

                GUI.EndGroup();
                ResetGUIColor();

            }
            void search()
            {
                SetGUIColor(Greyscale(1, searchAnimationT.Pow(2)));
                GUI.BeginGroup(window.position.SetPos(0, 0).MoveX(searchAnimationDistance * (1 - searchAnimationT)));

                searchField();
                closeSearchButton();
                closeSearchOnEsc();

                GUI.EndGroup();
                ResetGUIColor();

            }



            updateState();

            background();
            hiddenMenu();

            plusButton();

            searchAnimation();
            buttonsAndBookmarks();
            search();



            if (animatingSearch || reorderableRow?.animatingItemMovement == true || reorderableRow?.animatingTooltip == true)
                window.Repaint();

        }

        bool animatingSearch;
        float searchAnimationDistance = 90;
        float searchAnimationT;
        float searchAnimationDerivative;

        string openedFolderPath;

        public bool isSearchActive;

        bool isDefaultParentTextPressed;

        GameObject defaultParent;

        GUIStyle defaultParentTextGUIStyle;

        Rect navbarRect;
        Rect bookmarksRect;

        ReorderableRow<Bookmark> reorderableRow;











        void BookmarkGUI(Rect bookmarkRect, Bookmark bookmark)
        {
            if (bookmark == null) return;
            if (bookmark.go == null) return;
            if (curEvent.isLayout) return;


            var pressedBookmark = reorderableRow.pressedItem;
            var draggedBookmark = reorderableRow.draggedItem;
            var draggingBookmark = reorderableRow.draggingItem;
            var lastHoveredBookmark = reorderableRow.lastHoveredItem;
            var tooltipOpacity = reorderableRow.tooltipOpacity;

            void shadow()
            {
                if (!draggingBookmark) return;
                if (draggedBookmark != bookmark) return;

                bookmarkRect.SetSizeFromMid(bookmarkWidth - 4, bookmarkWidth - 4).DrawBlurred(Greyscale(0, .3f), 15);

            }
            void background()
            {
                if (!bookmarkRect.IsHovered()) return;
                if (draggingBookmark && draggedBookmark != bookmark) return;

                var backgroundColor = Greyscale(isDarkTheme ? .35f : .7f);

                var backgroundRect = bookmarkRect.SetSizeFromMid(bookmarkRect.width - 2, bookmarkWidth - 2);

                backgroundRect.DrawRounded(backgroundColor, 4);


            }
            void icon()
            {
                var opacity = 1f;
                var iconTexture = default(Texture);

                void set_opacity()
                {
                    var opacityNormal = .9f;
                    var opacityHovered = 1f;
                    var opacityPressed = .75f;
                    var opacityDragged = .75f;
                    var opacityDisabled = .4f;

                    var isDisabled = !bookmark.isLoadable;


                    opacity = opacityNormal;

                    if (draggingBookmark)
                        opacity = bookmark == draggedBookmark ? opacityDragged : opacityNormal;

                    else if (bookmark == pressedBookmark)
                        opacity = opacityPressed;

                    else if (bookmarkRect.IsHovered())
                        opacity = opacityHovered;

                    if (isDisabled)
                        opacity = opacityDisabled;

                }
                void getTexture()
                {
                    var iconName = "";

                    if (VHierarchy.GetGameObjectData(bookmark.go, createDataIfDoesntExist: false) is GameObjectData goData && !goData.iconNameOrGuid.IsNullOrEmpty())
                        iconName = goData.iconNameOrGuid.Length == 32 ? goData.iconNameOrGuid.ToPath() : goData.iconNameOrGuid;
                    else
                        iconName = AssetPreview.GetMiniThumbnail(bookmark.go).name;

                    if (iconName.IsNullOrEmpty())
                        iconName = "GameObject icon";


                    iconTexture = EditorIcons.GetIcon(iconName);

                }
                void drawTexture()
                {
                    if (!iconTexture) return;


                    SetGUIColor(Greyscale(1, opacity));

                    GUI.DrawTexture(bookmarkRect.SetSizeFromMid(iconSize), iconTexture);

                    ResetGUIColor();

                }


                set_opacity();
                getTexture();
                drawTexture();

            }
            void tooltip()
            {
                if (bookmark != (draggingBookmark ? (draggedBookmark) : (lastHoveredBookmark))) return;
                if (tooltipOpacity == 0) return;

                var fontSize = 11;
                var tooltipText = bookmark.name;

                Rect tooltipRect;

                void set_tooltipRect()
                {
                    var width = tooltipText.GetLabelWidth(fontSize) + 6;
                    var height = 16 + (fontSize - 12) * 2;

                    var yOffset = 28;
                    var rightMargin = -1;


                    tooltipRect = Rect.zero.SetMidPos(bookmarkRect.center.x, bookmarkRect.center.y + yOffset).SetSizeFromMid(width, height);


                    var maxXMax = navbarRect.xMax - rightMargin;

                    if (tooltipRect.xMax > maxXMax)
                        tooltipRect = tooltipRect.MoveX(maxXMax - tooltipRect.xMax);

                }
                void shadow()
                {
                    var shadowAmount = .33f;
                    var shadowRadius = 10;

                    tooltipRect.DrawBlurred(Greyscale(0, shadowAmount).MultiplyAlpha(tooltipOpacity), shadowRadius);

                }
                void background()
                {
                    var cornerRadius = 5;

                    var backgroundColor = Greyscale(isDarkTheme ? .13f : .9f);
                    var outerEdgeColor = Greyscale(isDarkTheme ? .25f : .6f);
                    var innerEdgeColor = Greyscale(isDarkTheme ? .0f : .95f);

                    tooltipRect.Resize(-1).DrawRounded(outerEdgeColor.SetAlpha(tooltipOpacity.Pow(2)), cornerRadius + 1);
                    tooltipRect.Resize(0).DrawRounded(innerEdgeColor.SetAlpha(tooltipOpacity.Pow(2)), cornerRadius + 0);
                    tooltipRect.Resize(1).DrawRounded(backgroundColor.SetAlpha(tooltipOpacity), cornerRadius - 1);

                }
                void text()
                {
                    var textRect = tooltipRect.MoveY(-.5f);

                    var textColor = Greyscale(1f);

                    SetLabelAlignmentCenter();
                    SetLabelFontSize(fontSize);
                    SetGUIColor(textColor.SetAlpha(tooltipOpacity));

                    GUI.Label(textRect, tooltipText);

                    ResetLabelStyle();
                    ResetGUIColor();

                }

                set_tooltipRect();
                shadow();
                background();
                text();

            }
            void click()
            {
                if (!bookmarkRect.IsHovered()) return;
                if (!curEvent.isMouseUp) return;

                curEvent.Use();


                if (draggingBookmark) return;
                if ((curEvent.mousePosition - reorderableRow.mouseDownPosiion).magnitude > 2) return;
                if (!bookmark.isLoadable) return;

                controller.RevealObject(bookmark.go, expand: true, highlight: true, snapToTopMargin: true);

                reorderableRow.lastClickedItem = bookmark;

                reorderableRow.hideTooltip = true;



                if (curEvent.mouseButton == 2 && VHierarchyMenu.setDefaultParentEnabled)
                    EditorUtility.SetDefaultParentObject(bookmark.go);

            }


            bookmarkRect.MarkInteractive();

            shadow();
            background();
            icon();
            tooltip();
            click();

        }

        float iconSize => 16;
        float iconSpacing => 1;
        float bookmarkSpacing => 16;

        float bookmarkWidth => 24;





        public Bookmark CreateItem(Object draggedObject) => new Bookmark(draggedObject as GameObject);

        public bool CanCreateItemFrom(Object draggedObject) => draggedObject is GameObject;





        int GetBookmarkIndex(float mouseX)
        {
            var curBookmarkWidthSum = 0f;

            for (int i = 0; i < data.bookmarks.Count; i++)
            {
                if (!data.bookmarks[i].go) continue;

                curBookmarkWidthSum += bookmarkWidth;

                if (bookmarksRect.xMax - curBookmarkWidthSum < mouseX + .5f)
                    return i;
            }

            return data.bookmarks.IndexOfLast(r => r.go) + 1;

        }

        float GetBookmarkCenterX(int i, bool includeGaps = true)
        {
            return bookmarksRect.xMax
                 - bookmarkWidth / 2
                 - data.bookmarks.Take(i).Sum(r => r.go ? bookmarkWidth : 0)
                 - (includeGaps ? reorderableRow.gaps.Take(i + 1).Sum() : 0);
        }













        public VHierarchyNavbar(EditorWindow window) => this.window = window;

        public EditorWindow window;

        public VHierarchyController controller => VHierarchy.controllers_byWindow[window];


    }
}
#endif