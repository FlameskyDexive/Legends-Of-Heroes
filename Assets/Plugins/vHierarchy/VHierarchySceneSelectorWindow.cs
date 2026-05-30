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
using UnityEditor.IMGUI.Controls;
using System.Diagnostics;
using Type = System.Type;
using Delegate = System.Delegate;
using Action = System.Action;
using static VHierarchy.VHierarchy;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;
// using static VTools.VDebug;


namespace VHierarchy
{
    public class VHierarchySceneSelectorWindow : EditorWindow
    {

        void OnGUI()
        {

            void background()
            {
                windowRect.Draw(windowBackground);

            }
            void closeOnEscape()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.keyCode != KeyCode.Escape) return;

                Close();

                EditorApplication.RepaintHierarchyWindow();

                GUIUtility.ExitGUI();

            }
            void addTabOnEnter()
            {
                // if (!curEvent.isKeyDown) return; // searchfield steals fpcus
                if (curEvent.keyCode != KeyCode.Return) return;

                if (keyboardFocusedRowIndex == -1) return;
                if (keyboardFocusedEntry == null) return;

                OpenScene(keyboardFocusedEntry, curEvent.holdingAlt);

                this.Close();

            }
            void arrowNavigation()
            {
                if (!curEvent.isKeyDown) return;
                if (curEvent.keyCode != KeyCode.UpArrow && curEvent.keyCode != KeyCode.DownArrow) return;

                curEvent.Use();


                if (curEvent.keyCode == KeyCode.UpArrow)
                    if (keyboardFocusedRowIndex == 0)
                        keyboardFocusedRowIndex = rowCount - 1;
                    else
                        keyboardFocusedRowIndex--;

                if (curEvent.keyCode == KeyCode.DownArrow)
                    if (keyboardFocusedRowIndex == rowCount - 1)
                        keyboardFocusedRowIndex = 0;
                    else
                        keyboardFocusedRowIndex++;


                keyboardFocusedRowIndex = keyboardFocusedRowIndex.Clamp(0, rowCount - 1);

            }
            void updateSearch()
            {
                if (searchString == prevSearchString) return;

                prevSearchString = searchString;


                if (searchString == "") { keyboardFocusedRowIndex = -1; return; }

                UpdateSearch();

                keyboardFocusedRowIndex = 0;

            }


            void searchField_()
            {
                var searchRect = windowRect.SetHeight(18).MoveY(1).AddWidthFromMid(-2);


                if (searchField == null)
                {
                    searchField = new SearchField();
                    searchField.SetFocus();

                }


                searchString = searchField.OnGUI(searchRect, searchString);

            }
            void rows()
            {
                void bookmarked()
                {
                    if (searchString != "") return;
                    if (!bookmarkedEntries.Any()) return;

                    bookmarksRect = windowRect.SetHeight(bookmarkedEntries.Count * rowHeight + gaps.Sum());

                    BookmarksGUI();

                }
                void divider()
                {
                    if (searchString != "") return;
                    if (!bookmarkedEntries.Any()) return;

                    var splitterColor = Greyscale(.36f);
                    var splitterRect = bookmarksRect.SetHeightFromBottom(0).SetHeight(dividerHeight).SetHeightFromMid(1).AddWidthFromMid(-10);

                    splitterRect.Draw(splitterColor);

                }
                void notBookmarked()
                {
                    if (searchString != "") return;

                    if (bookmarkedEntries.Any())
                        nextRowY = bookmarksRect.yMax + dividerHeight;

                    foreach (var entry in allEntries)
                    {
                        if (bookmarkedEntries.Contains(entry)) continue;
                        if (entry == draggedBookmark) continue;

                        RowGUI(windowRect.SetHeight(rowHeight).SetY(nextRowY), entry);

                        nextRowY += rowHeight;
                        nextRowIndex++;

                    }

                }
                void searched()
                {
                    if (searchString == "") return;

                    foreach (var entry in searchedEntries)
                    {
                        RowGUI(windowRect.SetHeight(rowHeight).SetY(nextRowY), entry);

                        nextRowY += rowHeight;
                        nextRowIndex++;

                    }

                }


                scrollPos = GUI.BeginScrollView(windowRect.AddHeightFromBottom(-firstRowOffsetTop), Vector2.up * scrollPos, windowRect.SetHeight(scrollAreaHeight), GUIStyle.none, GUIStyle.none).y;

                nextRowY = 0;
                nextRowIndex = 0;

                bookmarked();
                divider();
                notBookmarked();
                searched();

                scrollAreaHeight = nextRowY + 23;
                rowCount = nextRowIndex;

                GUI.EndScrollView();

            }
            void noResults()
            {
                if (searchString == "") return;
                if (searchedEntries.Any()) return;


                GUI.enabled = false;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;

                GUI.Label(windowRect.AddHeightFromBottom(-14), "No results");

                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                GUI.enabled = true;

            }

            void outline()
            {
                if (Application.platform == RuntimePlatform.OSXEditor) return;

                position.SetPos(0, 0).DrawOutline(Greyscale(.1f));

            }
            // void resizing()
            // {

            // }


            background();
            closeOnEscape();
            addTabOnEnter();
            arrowNavigation();
            updateSearch();

            searchField_();
            rows();
            noResults();

            outline();


            if (draggingBookmark || animatingDroppedBookmark || animatingGaps)
                this.Repaint();

        }

        Rect windowRect => position.SetPos(0, 0);
        Rect bookmarksRect;

        SearchField searchField;

        Color windowBackground => Greyscale(isDarkTheme ? .23f : .8f);

        string searchString = "";
        string prevSearchString = "";

        float scrollPos;

        float rowHeight => 22;
        float dividerHeight => 11;
        float firstRowOffsetTop => bookmarkedEntries.Any() && searchString == "" ? 21 : 20;

        int nextRowIndex;
        float nextRowY;

        float scrollAreaHeight = 1232;
        int rowCount = 123;

        int keyboardFocusedRowIndex = -1;









        void RowGUI(Rect rowRect, SceneEntry entry)
        {

            var isHovered = rowRect.IsHovered();
            var isPressed = entry == pressedEntry;
            var isDragged = draggingBookmark && draggedBookmark == entry;
            var isDropped = animatingDroppedBookmark && droppedBookmark == entry;
            var isFocused = entry == keyboardFocusedEntry;
            var isBookmarked = bookmarkedEntries.Contains(entry) || entry == draggedBookmark;

            if (isDropped)
                isHovered = rowRect.SetY(droppedBookmarkYTarget).IsHovered();


            var showBlueBackground = isFocused || isPressed || isDragged;
            var showAdditiveIndicator = curEvent.holdingAlt && ((keyboardFocusedEntry?.scenePath).IsNullOrEmpty() ? isHovered : isFocused);
            var showStarButton = (isHovered || (isBookmarked && !isFocused)) && !showAdditiveIndicator;
            var showEnterHint = (isFocused && !isHovered && isDarkTheme) && !showAdditiveIndicator;


            void draggedShadow()
            {
                if (!curEvent.isRepaint) return;
                if (!isDragged) return;

                var shadowRect = rowRect.AddHeightFromMid(-4);

                var shadowOpacity = .3f;
                var shadowRadius = 13;

                shadowRect.DrawBlurred(Greyscale(0, shadowOpacity), shadowRadius);

            }
            void blueBackground()
            {
                if (!curEvent.isRepaint) return;
                if (!showBlueBackground) return;


                var backgroundRect = rowRect.AddHeightFromMid(-3);

                backgroundRect.Draw(GUIColors.selectedBackground);


            }
            void icon()
            {
                if (!curEvent.isRepaint) return;


                Texture iconTexture = EditorIcons.GetIcon("SceneAsset Icon");

                if (!iconTexture) return;


                var iconRect = rowRect.SetWidth(16).SetHeightFromMid(16).MoveX(4 + 1);

                iconRect = iconRect.SetWidthFromMid(iconRect.height * iconTexture.width / iconTexture.height);


                GUI.DrawTexture(iconRect, iconTexture);

            }
            void name()
            {
                if (!curEvent.isRepaint) return;


                var nameRect = rowRect.MoveX(21 + 1);

                var nameText = searchString != "" ? namesFormattedForFuzzySearch_byEntry[entry]
                                                  : entry.sceneName;


                if (entry.scenePath == sceneToReplace.path)
                {
                    SetLabelBold();
                    SetGUIColor(Greyscale(1.15f));

                    GUI.skin.label.richText = true;

                    GUI.Label(nameRect, nameText);

                    GUI.skin.label.richText = false;

                    ResetGUIColor();
                    ResetLabelStyle();

                }

                else
                {
                    var color = showBlueBackground ? Greyscale(123, 123)
                                                   : isHovered && !isPressed ? Greyscale(1.1f)
                                                                             : Greyscale(1);
                    SetGUIColor(color);

                    GUI.skin.label.richText = true;

                    GUI.Label(nameRect, nameText);

                    GUI.skin.label.richText = false;

                    ResetGUIColor();
                }

            }
            void curtain()
            {
                if (!curEvent.isRepaint) return;


                var curtainWidth = 20;

                var curtainColor = showBlueBackground ? GUIColors.selectedBackground : windowBackground;

                var curtainXMax = rowRect.xMax - (showStarButton ? 21 : showEnterHint ? 33 : showAdditiveIndicator ? 80 : 0);


                rowRect.SetHeightFromMid(18).SetXMax(curtainXMax).SetWidthFromRight(curtainWidth).DrawCurtainLeft(curtainColor);

                rowRect.SetHeightFromMid(18).SetX(curtainXMax).SetXMax(rowRect.xMax).Draw(curtainColor);

            }
            void starButton()
            {
                if (!showStarButton) return;


                var buttonRect = rowRect.SetWidthFromRight(16).MoveX(-6 + 1).SetSizeFromMid(rowHeight);


                var iconName = isBookmarked ^ buttonRect.IsHovered() ? "Star" : "Star Hollow";
                var iconSize = 16;
                var colorNormal = Greyscale(isDarkTheme ? (isBookmarked ? .5f : .7f) : .3f);
                var colorHovered = Greyscale(isDarkTheme ? (isBookmarked ? .9f : 1) : 0f);
                var colorPressed = Greyscale(isDarkTheme ? .75f : .5f);
                var colorDisabled = Greyscale(isDarkTheme ? .53f : .55f);


                if (!IconButton(buttonRect, iconName, iconSize, colorNormal, colorHovered, colorPressed)) return;

                if (isBookmarked)
                    bookmarkedEntries.Remove(entry);
                else
                    bookmarkedEntries.Add(entry);

            }
            void enterHint()
            {
                if (!curEvent.isRepaint) return;
                if (!showEnterHint) return;


                var hintRect = rowRect.SetWidthFromRight(33);


                SetLabelFontSize(10);
                SetGUIColor(Greyscale(.9f));

                GUI.Label(hintRect, "Enter");

                ResetGUIColor();
                ResetLabelStyle();


            }
            void additiveIndicator()
            {
                if (!curEvent.isRepaint) return;
                if (!showAdditiveIndicator) return;


                var indicatorRect = rowRect.SetWidthFromRight(81);


                SetLabelFontSize(10);
                SetGUIColor(Greyscale(.9f));

                GUI.Label(indicatorRect, "Load additively");

                ResetGUIColor();
                ResetLabelStyle();

            }
            void hoverHighlight()
            {
                if (!curEvent.isRepaint) return;
                if (!isHovered) return;
                if (isPressed || isDragged) return;


                var backgroundRect = rowRect.AddHeightFromMid(-2);

                var backgroundColor = Greyscale(isDarkTheme ? 1 : 0, isPressed ? .085f : .12f);


                backgroundRect.Draw(backgroundColor);

            }

            void mouse()
            {
                void down()
                {
                    if (!curEvent.isMouseDown) return;
                    if (!rowRect.IsHovered()) return;

                    isMousePressedOnEntry = true;
                    pressedEntry = entry;

                    mouseDownPosition = curEvent.mousePosition;

                    this.Repaint();

                }
                void up()
                {
                    if (!curEvent.isMouseUp) return;

                    isMousePressedOnEntry = false;
                    pressedEntry = null;

                    this.Repaint();


                    if (!isHovered) return;
                    if (draggingBookmark) return;
                    if ((curEvent.mousePosition - mouseDownPosition).magnitude > 2) return;

                    curEvent.Use();

                    OpenScene(entry, curEvent.holdingAlt);

                    this.Close();

                }

                down();
                up();

            }
            void setFocusedEntry()
            {
                var rowIndex = (rowRect.y / rowHeight).FloorToInt();

                if (rowIndex == keyboardFocusedRowIndex)
                    keyboardFocusedEntry = entry;

            }


            rowRect.MarkInteractive();

            draggedShadow();
            blueBackground();
            icon();
            name();
            curtain();
            starButton();
            enterHint();
            additiveIndicator();
            hoverHighlight();
            mouse();
            setFocusedEntry();

        }

        SceneEntry pressedEntry;

        bool isMousePressedOnEntry;

        Vector2 mouseDownPosition;

        SceneEntry keyboardFocusedEntry;




        void OpenScene(SceneEntry entry, bool openAdditive)
        {
            if (!Application.isPlaying)
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return; // if clicked cancel


            void openScene(string path, bool additive)
            {
                if (!EditorApplication.isPlaying)
                    EditorSceneManager.OpenScene(path, additive ? OpenSceneMode.Additive : OpenSceneMode.Single);
                else
                    SceneManager.LoadScene(path, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);

            }



            if (openAdditive)
                openScene(entry.scenePath, additive: true);

            else if (EditorSceneManager.sceneCount == 1)
                openScene(entry.scenePath, additive: false);

            else
            {
                var openedScene = EditorSceneManager.OpenScene(entry.scenePath, OpenSceneMode.Additive);

                EditorSceneManager.MoveSceneAfter(openedScene, sceneToReplace);

                if (EditorSceneManager.GetActiveScene() == sceneToReplace)
                    EditorSceneManager.SetActiveScene(openedScene);

                EditorSceneManager.CloseScene(sceneToReplace, removeScene: true);

            }


        }












        public void BookmarksGUI()
        {
            void normalBookmark(int i)
            {
                if (bookmarkedEntries[i] == droppedBookmark && animatingDroppedBookmark) return;

                var bookmarkRect = bookmarksRect.SetHeight(rowHeight)
                                                .SetY(GetBookmarY(i));

                RowGUI(bookmarkRect, bookmarkedEntries[i]);

            }
            void normalBookmarks()
            {
                for (int i = 0; i < bookmarkedEntries.Count; i++)
                    normalBookmark(i);

            }
            void draggedBookmark_()
            {
                if (!draggingBookmark) return;


                var bookmarkRect = bookmarksRect.SetHeight(rowHeight)
                                                .SetY(draggedBookmarkY);

                RowGUI(bookmarkRect, draggedBookmark);

            }
            void droppedBookmark_()
            {
                if (!animatingDroppedBookmark) return;

                var bookmarkRect = bookmarksRect.SetHeight(rowHeight)
                                                .SetY(droppedBookmarkY);

                RowGUI(bookmarkRect, droppedBookmark);

            }


            BookmarksDragging();
            BookmarksAnimations();

            normalBookmarks();
            draggedBookmark_();
            droppedBookmark_();

        }

        int GetBookmarkIndex(float mouseY)
        {
            return ((mouseY - bookmarksRect.y) / rowHeight).FloorToInt();
        }

        float GetBookmarY(int i, bool includeGaps = true)
        {
            var centerY = bookmarksRect.y
                        + i * rowHeight
                        + (includeGaps ? gaps.Take(i + 1).Sum() : 0);


            return centerY;

        }









        void BookmarksDragging()
        {
            void init()
            {
                if (draggingBookmark) return;
                if ((curEvent.mousePosition - mouseDownPosition).magnitude <= 2) return;

                if (!isMousePressedOnEntry) return;
                if (!bookmarkedEntries.Contains(pressedEntry)) return;


                var i = GetBookmarkIndex(mouseDownPosition.y);

                if (i >= bookmarkedEntries.Count) return;
                if (i < 0) return;


                animatingDroppedBookmark = false;

                draggingBookmark = true;

                draggedBookmark = bookmarkedEntries[i];
                draggedBookmarkHoldOffsetY = GetBookmarY(i) - mouseDownPosition.y;

                gaps[i] = rowHeight;


                this.RecordUndo();

                bookmarkedEntries.Remove(draggedBookmark);

            }
            void update()
            {
                if (!draggingBookmark) return;


                EditorGUIUtility.hotControl = EditorGUIUtility.GetControlID(FocusType.Passive);

                draggedBookmarkY = (curEvent.mousePosition.y + draggedBookmarkHoldOffsetY).Clamp(0, bookmarksRect.yMax - rowHeight);

                insertDraggedBookmarkAtIndex = GetBookmarkIndex(curEvent.mousePosition.y + draggedBookmarkHoldOffsetY + rowHeight / 2).Clamp(0, bookmarkedEntries.Count);

            }
            void accept()
            {
                if (!draggingBookmark) return;
                if (!curEvent.isMouseUp && !curEvent.isIgnore) return;

                curEvent.Use();
                EditorGUIUtility.hotControl = 0;

                // DragAndDrop.PrepareStartDrag(); // fixes phantom dragged component indicator after reordering bookmarks

                this.RecordUndo();

                draggingBookmark = false;
                isMousePressedOnEntry = false;

                bookmarkedEntries.AddAt(draggedBookmark, insertDraggedBookmarkAtIndex);

                gaps[insertDraggedBookmarkAtIndex] -= rowHeight;
                gaps.AddAt(0, insertDraggedBookmarkAtIndex);

                droppedBookmark = draggedBookmark;

                droppedBookmarkY = draggedBookmarkY;
                droppedBookmarkYDerivative = 0;
                animatingDroppedBookmark = true;

                draggedBookmark = null;
                pressedEntry = null;

                EditorGUIUtility.hotControl = 0;

            }

            init();
            accept();
            update();

        }

        bool draggingBookmark;

        float draggedBookmarkHoldOffsetY;

        float draggedBookmarkY;
        int insertDraggedBookmarkAtIndex;

        SceneEntry draggedBookmark;
        SceneEntry droppedBookmark;






        void BookmarksAnimations()
        {
            if (!curEvent.isLayout) return;

            void gaps_()
            {
                var makeSpaceForDraggedBookmark = draggingBookmark;

                // var lerpSpeed = 1;
                var lerpSpeed = 11;

                for (int i = 0; i < gaps.Count; i++)
                    if (makeSpaceForDraggedBookmark && i == insertDraggedBookmarkAtIndex)
                        gaps[i] = MathUtil.Lerp(gaps[i], rowHeight, lerpSpeed, editorDeltaTime);
                    else
                        gaps[i] = MathUtil.Lerp(gaps[i], 0, lerpSpeed, editorDeltaTime);



                for (int i = 0; i < gaps.Count; i++)
                    if (gaps[i].Approx(0))
                        gaps[i] = 0;



                animatingGaps = gaps.Any(r => r > .1f);


            }
            void droppedBookmark_()
            {
                if (!animatingDroppedBookmark) return;

                // var lerpSpeed = 1;
                var lerpSpeed = 8;

                droppedBookmarkYTarget = GetBookmarY(bookmarkedEntries.IndexOf(droppedBookmark), includeGaps: false);

                MathUtil.SmoothDamp(ref droppedBookmarkY, droppedBookmarkYTarget, lerpSpeed, ref droppedBookmarkYDerivative, editorDeltaTime);

                if ((droppedBookmarkY - droppedBookmarkYTarget).Abs() < .5f)
                    animatingDroppedBookmark = false;

            }

            gaps_();
            droppedBookmark_();

        }

        float droppedBookmarkY;
        float droppedBookmarkYTarget;
        float droppedBookmarkYDerivative;

        bool animatingDroppedBookmark;
        bool animatingGaps;

        List<float> gaps
        {
            get
            {
                while (_gaps.Count < bookmarkedEntries.Count + 1) _gaps.Add(0);
                while (_gaps.Count > bookmarkedEntries.Count + 1) _gaps.RemoveLast();

                return _gaps;

            }
        }
        List<float> _gaps = new();




















        public static void UpdateAllEntries()
        {
            void fillWithAllScenes()
            {

                allEntries.Clear();

                allEntries = AssetDatabase.FindAssets("t:scene")
                                          .Select(r => new SceneEntry() { scenePath = r.ToPath() })
                                          .Where(r => !r.sceneName.StartsWith("~"))
                                          .Where(r => !r.scenePath.EndsWith(".asset")) // filters out scene manager 3's stuff
                                          .ToList();

                allEntries.SortBy(r => r.scenePath.GetFilename());

            }
            // void removeAllOpen()
            // {
            //     for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            //         allEntries.RemoveAll(r => r.scenePath == EditorSceneManager.GetSceneAt(i).path);
            // }

            fillWithAllScenes();
            // removeAllOpen();

        }

        static List<SceneEntry> allEntries = new();

        [System.Serializable]
        public class SceneEntry
        {
            public string scenePath = "";

            public string sceneName => scenePath.GetFilename(withExtension: false);

        }



        void GetBookmarkedEntries()
        {
            bookmarkedEntries = data.bookmarkedScenePaths.Select(r => allEntries.FirstOrDefault(rr => rr.scenePath == r))
                                                         .Where(r => r != null)
                                                         .ToList();
        }
        void SaveBookmarkedEntries()
        {
            data.bookmarkedScenePaths = bookmarkedEntries.Select(r => r.scenePath).ToList();

            data.Dirty();

        }

        List<SceneEntry> bookmarkedEntries = new();



        void OnEnable() { UpdateAllEntries(); GetBookmarkedEntries(); }

        void OnDisable() { SaveBookmarkedEntries(); }










        void UpdateSearch()
        {

            bool tryMatch(string name, string query, int[] matchIndexes, ref float cost)
            {

                var wordInitialsIndexes = new List<int> { 0 };

                for (int i = 1; i < name.Length; i++)
                {
                    var separators = new[] { ' ', '-', '_', '.', '(', ')', '[', ']', };

                    var prevChar = name[i - 1];
                    var curChar = name[i];
                    var nextChar = i + 1 < name.Length ? name[i + 1] : default(char);

                    var isSeparatedWordStart = separators.Contains(prevChar) && !separators.Contains(curChar);
                    var isCamelcaseHump = (curChar.IsUpper() && prevChar.IsLower()) || (curChar.IsUpper() && nextChar.IsLower());
                    var isNumberStart = curChar.IsDigit() && (!prevChar.IsDigit() || prevChar == '0');
                    var isAfterNumber = prevChar.IsDigit() && !curChar.IsDigit();

                    if (isSeparatedWordStart || isCamelcaseHump || isNumberStart || isAfterNumber)
                        wordInitialsIndexes.Add(i);

                }



                var nextWordInitialsIndexMap = new int[name.Length];

                var nextWordIndex = 0;

                for (int i = 0; i < name.Length; i++)
                {
                    if (i == wordInitialsIndexes[nextWordIndex])
                        if (nextWordIndex + 1 < wordInitialsIndexes.Count)
                            nextWordIndex++;
                        else break;

                    nextWordInitialsIndexMap[i] = wordInitialsIndexes[nextWordIndex];

                }





                var iName = 0;
                var iQuery = 0;

                var prevMatchIndex = -1;

                void registerMatch(int matchIndex)
                {
                    matchIndexes[iQuery] = matchIndex;
                    iQuery++;

                    iName = matchIndex + 1;

                    prevMatchIndex = matchIndex;


                }


                cost = 0;

                while (iName < name.Length && iQuery < query.Length)
                {
                    var curQuerySymbol = query[iQuery].ToLower();
                    var curNameSymbol = name[iName].ToLower();

                    if (curNameSymbol == curQuerySymbol)
                    {
                        var gapLength = iName - prevMatchIndex - 1;

                        cost += gapLength;


                        registerMatch(iName);

                        continue;

                        // consecutive matches cost 0
                        // distance between index 0 and first match also counts as a gap

                    }



                    var nextWordInitialIndex = nextWordInitialsIndexMap[iName]; // wordInitialsIndexes.FirstOrDefault(i => i > iName);
                    var nextWordInitialSymbol = nextWordInitialIndex == default ? default : name[nextWordInitialIndex].ToLower();

                    if (nextWordInitialSymbol == curQuerySymbol)
                    {
                        var gapLength = nextWordInitialIndex - prevMatchIndex - 1;

                        cost += (gapLength * .01f).ClampMax(.9f);


                        registerMatch(nextWordInitialIndex);

                        continue;

                        // word-initial match costs less than a gap (1+) 
                        // but more than a consecutive match (0)

                    }



                    iName++;

                }






                var allCharsMatched = iQuery >= query.Length;

                return allCharsMatched;



                // this search works great in practice
                // but fails in more theoretical scenarios, mostly when user skips first letters of words
                // eg searching "arn" won't find "barn_a" because search will jump to last a (word-initial) and fail afterwards
                // so unity search is used as a fallback

            }
            bool tryMatch_unitySearch(string name, string query, int[] matchIndexes, ref float cost)
            {
                long score = 0;

                List<int> matchIndexesList = new();


                var matched = UnityEditor.Search.FuzzySearch.FuzzyMatch(searchString, name, ref score, matchIndexesList);


                for (int i = 0; i < matchIndexesList.Count; i++)
                    matchIndexes[i] = matchIndexesList[i];

                cost = 123212 - score;


                return matched;


                // this search is fast but isn't tuned for real use cases
                // quering "vis" ranks "Invisible" higher than "VInspectorState"
                // quering "lst" ranks "SmallShadowTemp" higher than "List"
                // also sometimes it favors matches that are further away from zeroth index 

            }

            string formatName(string name, IEnumerable<int> matchIndexes)
            {
                var formattedName = "";

                for (int i = 0; i < name.Length; i++)
                    if (matchIndexes.Contains(i))
                        formattedName += "<b>" + name[i] + "</b>";
                    else
                        formattedName += name[i];


                return formattedName;

            }



            var costs_byEntry = new Dictionary<SceneEntry, float>();

            var matchIndexes = new int[searchString.Length];
            var matchCost = 0f;


            foreach (var entry in allEntries)
                if (tryMatch(entry.sceneName, searchString, matchIndexes, ref matchCost) || tryMatch_unitySearch(entry.sceneName, searchString, matchIndexes, ref matchCost))
                {
                    costs_byEntry[entry] = matchCost;
                    namesFormattedForFuzzySearch_byEntry[entry] = formatName(entry.sceneName, matchIndexes);
                }


            searchedEntries = costs_byEntry.Keys.OrderBy(r => costs_byEntry[r])
                                                .ThenBy(r => r.sceneName)
                                                .ToList();
        }

        List<SceneEntry> searchedEntries = new();

        Dictionary<SceneEntry, string> namesFormattedForFuzzySearch_byEntry = new();







        void OnLostFocus()
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorWindow.focusedWindow != this)
                {
                    EditorApplication.RepaintHierarchyWindow();

                    Close();
                }
            };

            // delay is needed to prevent reopening after clicking + button for the second time
        }



        public static void Open(Vector2 rowPos, Scene sceneToReplace)
        {
            if (!VHierarchy.data)
            {
                VHierarchy.data = ScriptableObject.CreateInstance<VHierarchyData>();

                AssetDatabase.CreateAsset(VHierarchy.data, GetScriptPath("VHierarchy").GetParentPath().CombinePath("vHierarchy Data.asset"));
            }


            instance = ScriptableObject.CreateInstance<VHierarchySceneSelectorWindow>();

            instance.ShowPopup();
            instance.Focus();



            var width = 190;
            var height = 296;

            var offsetX = -14;
            var offsetY = 18;

            instance.position = instance.position.SetPos(rowPos + new Vector2(offsetX, offsetY)).SetSize(width, height);



            instance.sceneToReplace = sceneToReplace;

        }

        public Scene sceneToReplace;

        public static VHierarchySceneSelectorWindow instance;


    }
}
#endif