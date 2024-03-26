using System;
﻿using System.Collections.Generic;
using System.Reflection;
﻿using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SubjectNerd.QuickToggle
{
    [InitializeOnLoad]
    public class QuickToggle
    {
		#region Constants
		private const string PrefKeyShowToggle = "UnityToolbag.QuickToggle.Visible";
	    private const string PrefKeyShowDividers = "UnityToolbag.QuickToggle.Dividers";
		private const string PrefKeyShowIcons = "UnityToolbag.QuickToggle.Icons";
		private const string PrefKeyGutterLevel = "UnityToolbag.QuickToggle.Gutter";
		private const string PrefKeyShowVisIcon = "UnityToolbag.QuickToggle.VisIcon";

		private const string MENU_NAME = "Window/Hierarchy Quick Toggle/Show Toggles";
		private const string MENU_DIVIDER = "Window/Hierarchy Quick Toggle/Dividers";
		private const string MENU_ICONS = "Window/Hierarchy Quick Toggle/Object Icons";
		private const string MENU_VIS_ICONS = "Window/Hierarchy Quick Toggle/Show Visibility Icon";
		private const string MENU_GUTTER_0 = "Window/Hierarchy Quick Toggle/Right Gutter/0";
		private const string MENU_GUTTER_1 = "Window/Hierarchy Quick Toggle/Right Gutter/1";
		private const string MENU_GUTTER_2 = "Window/Hierarchy Quick Toggle/Right Gutter/2";
		#endregion

		private static readonly Type HierarchyWindowType;
		private static readonly MethodInfo getObjectIcon;

		private static bool stylesBuilt;
		private static GUIStyle styleLock, styleUnlocked,
								styleVisOn, styleVisOff,
								styleDivider;

	    private static bool showDivider, showIcons, showVisIcon;

		#region Menu stuff
	    [MenuItem(MENU_NAME, false, 1)]
        private static void QuickToggleMenu()
        {
            bool toggle = EditorPrefs.GetBool(PrefKeyShowToggle);
            ShowQuickToggle(!toggle);
			Menu.SetChecked(MENU_NAME, !toggle);
		}

		[MenuItem(MENU_NAME, true)]
	    private static bool SetupMenuCheckMarks()
		{
			Menu.SetChecked(MENU_NAME, EditorPrefs.GetBool(PrefKeyShowToggle));
			Menu.SetChecked(MENU_VIS_ICONS, EditorPrefs.GetBool(PrefKeyShowVisIcon));
			Menu.SetChecked(MENU_DIVIDER, EditorPrefs.GetBool(PrefKeyShowDividers));
			Menu.SetChecked(MENU_ICONS, EditorPrefs.GetBool(PrefKeyShowIcons));

			int gutterLevel = EditorPrefs.GetInt(PrefKeyGutterLevel, 0);
			gutterCount = gutterLevel;
			UpdateGutterMenu(gutterCount);
			return true;
	    }

		[MenuItem(MENU_VIS_ICONS, false, 20)]
		private static void ToggleVisIcons()
        {
			ToggleSettings(PrefKeyShowVisIcon, MENU_VIS_ICONS, out showVisIcon);
        }

		[MenuItem(MENU_DIVIDER, false, 21)]
	    private static void ToggleDivider()
		{
			ToggleSettings(PrefKeyShowDividers, MENU_DIVIDER, out showDivider);
		}

		[MenuItem(MENU_ICONS, false, 22)]
	    private static void ToggleIcons()
	    {
			ToggleSettings(PrefKeyShowIcons, MENU_ICONS, out showIcons);
	    }

	    private static void ToggleSettings(string prefKey, string menuString, out bool valueBool)
	    {
		    valueBool = !EditorPrefs.GetBool(prefKey);
		    EditorPrefs.SetBool(prefKey, valueBool);
		    Menu.SetChecked(menuString, valueBool);
		    EditorApplication.RepaintHierarchyWindow();
	    }

	    [MenuItem(MENU_GUTTER_0, false, 40)]
	    private static void SetGutter0() { SetGutterLevel(0); }
		[MenuItem(MENU_GUTTER_1, false, 41)]
		private static void SetGutter1() { SetGutterLevel(1); }
		[MenuItem(MENU_GUTTER_2, false, 42)]
		private static void SetGutter2() { SetGutterLevel(2); }

		private static void SetGutterLevel(int gutterLevel)
	    {
		    gutterLevel = Mathf.Clamp(gutterLevel, 0, 2);
			EditorPrefs.SetInt(PrefKeyGutterLevel, gutterLevel);
			gutterCount = gutterLevel;
			UpdateGutterMenu(gutterCount);
			EditorApplication.RepaintHierarchyWindow();
		}

	    private static void UpdateGutterMenu(int gutterLevel)
	    {
			string[] gutterKeys = new[] { MENU_GUTTER_0, MENU_GUTTER_1, MENU_GUTTER_2 };
			bool[] gutterValues = null;
			switch (gutterLevel)
			{
				case 1:
					gutterValues = new[] { false, true, false };
					break;
				case 2:
					gutterValues = new[] { false, false, true };
					break;
				default:
					gutterValues = new[] { true, false, false };
					break;
			}
			for (int i = 0; i < gutterKeys.Length; i++)
			{
				string key = gutterKeys[i];
				bool isChecked = gutterValues[i];
				Menu.SetChecked(key, isChecked);
			}
		}
	    #endregion

	    static QuickToggle()
	    {
			// Setup initial state of editor prefs if there are no prefs keys yet
			string[] resetPrefs = new string[] {PrefKeyShowToggle, PrefKeyShowDividers,
												PrefKeyShowIcons, PrefKeyShowVisIcon};
			foreach (string prefKey in resetPrefs)
			{
				if (EditorPrefs.HasKey(prefKey) == false)
					EditorPrefs.SetBool(prefKey, false);
			}

			// Fetch some reflection/type stuff for use later on
		    Assembly editorAssembly = typeof(EditorWindow).Assembly;
		    HierarchyWindowType = editorAssembly.GetType("UnityEditor.SceneHierarchyWindow");

			var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
			Type editorGuiUtil = typeof (EditorGUIUtility);
		    getObjectIcon = editorGuiUtil.GetMethod("GetIconForObject", flags, null, new Type[] { typeof(UnityEngine.Object) }, null);

			// Not calling BuildStyles() in constructor because script gets loaded
			// on Unity initialization, styles might not be loaded yet

			// Reset mouse state
			ResetVars();
			// Setup quick toggle
            ShowQuickToggle(EditorPrefs.GetBool(PrefKeyShowToggle));
	    }

	    private static void ShowQuickToggle(bool show)
		{
			EditorPrefs.SetBool(PrefKeyShowToggle, show);
		    showDivider = EditorPrefs.GetBool(PrefKeyShowDividers, false);
		    showIcons = EditorPrefs.GetBool(PrefKeyShowIcons, false);
			showVisIcon = EditorPrefs.GetBool(PrefKeyShowVisIcon, false);
		    gutterCount = EditorPrefs.GetInt(PrefKeyGutterLevel);

		    if (show)
            {
				ResetVars();
				EditorApplication.update += HandleEditorUpdate;
                EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyItem;
            }
            else
			{
				EditorApplication.update -= HandleEditorUpdate;
                EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyItem;
            }
            EditorApplication.RepaintHierarchyWindow();
        }

	    private struct PropagateState
	    {
		    public bool isVisibility;
		    public bool propagateValue;

		    public PropagateState(bool isVisibility, bool propagateValue)
		    {
			    this.isVisibility = isVisibility;
			    this.propagateValue = propagateValue;
		    }
	    }

	    private static PropagateState	propagateState;

		// Because we can't hook into OnGUI of HierarchyWindow, doing a hack
		// button that involves the editor update loop and the hierarchy item draw event
		private static bool	isFrameFresh;
	    private static bool	isMousePressed;

	    private static int gutterCount = 0;

	    private static void ResetVars()
	    {
		    isFrameFresh = false;
		    isMousePressed = false;
	    }

	    private static void HandleEditorUpdate()
	    {   
		    EditorWindow window = EditorWindow.mouseOverWindow;
		    if (window == null)
		    {
				ResetVars();
			    return;
		    }

		    if (window.GetType() == HierarchyWindowType)
		    {
			    if (window.wantsMouseMove == false)
				    window.wantsMouseMove = true;
				
			    isFrameFresh = true;
			}
	    }

	    private static void DrawHierarchyItem(int instanceId, Rect selectionRect)
        {
            BuildStyles();

            GameObject target = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (target == null)
                return;

			// Get states
			bool isVisible = target.activeSelf;
			bool isLocked = (target.hideFlags & HideFlags.NotEditable) > 0;

			// Check if mouse is over hierarchy item, to act like Unity vis toggle
			Event evt = Event.current;
			if (isLocked == false && selectionRect.Contains(evt.mousePosition) == false)
				return;

			// Reserve the draw rects
			float gutterX = selectionRect.height*gutterCount;
		    if (gutterX > 0)
			    gutterX += selectionRect.height*0.1f;
		    float xMax = selectionRect.xMax - gutterX;

            Rect visRect = new Rect(selectionRect)
            {
                xMin = xMax - (selectionRect.height * 2.1f),
                xMax = xMax - selectionRect.height
            };
            Rect lockRect = new Rect(selectionRect)
            {
                xMin = xMax - (selectionRect.height * 1.05f),
				xMax = xMax - (selectionRect.height * 0.05f)
            };

			if (showVisIcon)
			{
				// Draw the visibility toggle
				GUIStyle visStyle = (isVisible) ? styleVisOn : styleVisOff;
				GUI.Label(visRect, GUIContent.none, visStyle);
			}

			// Draw lock toggle
			GUIStyle lockStyle = (isLocked) ? styleLock : styleUnlocked;
            GUI.Label(lockRect, GUIContent.none, lockStyle);

			// Draw optional divider
			if (showDivider)
			{
				Rect lineRect = new Rect(selectionRect)
				{
					yMin = selectionRect.yMax - 1f,
					yMax = selectionRect.yMax + 2f
				};
				GUI.Label(lineRect, GUIContent.none, styleDivider);
			}
			// Draw optional object icons
			if (showIcons && getObjectIcon != null)
			{
				Texture2D iconImg = getObjectIcon.Invoke(null, new object[] { target }) as Texture2D;
				if (iconImg != null)
				{
					Rect iconRect = new Rect(selectionRect)
					{
						xMin = visRect.xMin - 30,
						xMax = visRect.xMin - 5
					};
					GUI.DrawTexture(iconRect, iconImg, ScaleMode.ScaleToFit);
				}
			}

			if (Event.current == null)
			    return;

			HandleMouse(target, isVisible, isLocked, visRect, lockRect);
        }

	    private static void HandleMouse(GameObject target, bool isVisible, bool isLocked, Rect visRect, Rect lockRect)
	    {
			Event evt = Event.current;

			bool toggleActive = visRect.Contains(evt.mousePosition);
			bool toggleLock = lockRect.Contains(evt.mousePosition);
			bool stateChanged = (toggleActive || toggleLock);

			bool doMouse = false;
			switch (evt.type)
			{
				case EventType.MouseDown:
					// Checking is frame fresh so mouse state is only tested once per frame
					// instead of every time a hierarchy item is drawn
					bool isMouseDown = false;
					if (isFrameFresh && stateChanged)
					{
						isMouseDown = !isMousePressed;
						isMousePressed = true;
						isFrameFresh = false;
					}

					if (stateChanged && isMouseDown)
					{
						doMouse = true;
						if (toggleActive) isVisible = !isVisible;
						if (toggleLock) isLocked = !isLocked;

						propagateState = new PropagateState(toggleActive, (toggleActive) ? isVisible : isLocked);
						evt.Use();
					}
					break;
				case EventType.MouseDrag:
					doMouse = isMousePressed;
					break;
				case EventType.DragPerform:
				case EventType.DragExited:
				case EventType.DragUpdated:
				case EventType.MouseUp:
					ResetVars();
					break;
			}

			if (doMouse && stateChanged)
			{
				if (propagateState.isVisibility)
					SetVisible(target, propagateState.propagateValue);
				else
					SetLockObject(target, propagateState.propagateValue);

				EditorApplication.RepaintHierarchyWindow();
			}
		}

        private static Object[] GatherObjects(GameObject root)
        {
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            Stack<GameObject> recurseStack = new Stack<GameObject>(new GameObject[] { root });

            while (recurseStack.Count > 0)
            {
                GameObject obj = recurseStack.Pop();
                objects.Add(obj);

                foreach (Transform childT in obj.transform)
                    recurseStack.Push(childT.gameObject);
            }
            return objects.ToArray();
        }

        private static void SetLockObject(GameObject target, bool isLocked)
        {
	        bool objectLockState = (target.hideFlags & HideFlags.NotEditable) > 0;
            if (objectLockState == isLocked)
		        return;

			List<GameObject> setGameObjects = new List<GameObject>() { target };

			// If target object is part of selection, logical thing is to set state of selection
			List<GameObject> selectionList = new List<GameObject>(Selection.gameObjects);
			if (selectionList.Contains(target))
				setGameObjects = selectionList;

			List<Object> gatheredObjects = new List<Object>();
			foreach (GameObject setGameObject in setGameObjects)
            {
				gatheredObjects.AddRange(GatherObjects(setGameObject));
            }

            foreach (Object obj in gatheredObjects)
            {
                GameObject go = (GameObject)obj;
				string undoString = string.Format("{0} {1}", isLocked ? "Lock" : "Unlock", go.name);
				Undo.RecordObject(go, undoString);

                // Set state according to isLocked
                if (isLocked)
                {
                    go.hideFlags |= HideFlags.NotEditable;
                }
                else
                {
                    go.hideFlags &= ~HideFlags.NotEditable;
                }

                // Set hideflags of components
                foreach (Component comp in go.GetComponents<Component>())
                {
                    if (comp is Transform)
                        continue;
                    if (isLocked)
                    {
                        comp.hideFlags |= HideFlags.NotEditable;
                        comp.hideFlags |= HideFlags.HideInHierarchy;
                    }
                    else
                    {
                        comp.hideFlags &= ~HideFlags.NotEditable;
                        comp.hideFlags &= ~HideFlags.HideInHierarchy;
                    }
                    EditorUtility.SetDirty(comp);
                }
                EditorUtility.SetDirty(go);
            }
			Undo.IncrementCurrentGroup();
        }

        private static void SetVisible(GameObject target, bool isActive)
        {
			if (target.activeSelf == isActive) return;
	        
            string undoString = string.Format("{0} {1}",
                                        isActive ? "Show" : "Hide",
                                        target.name);
            Undo.RecordObject(target, undoString);

            target.SetActive(isActive);
            EditorUtility.SetDirty(target);
        }

        private static void BuildStyles()
        {
            // All of the styles have been built, don't do anything
	        if (stylesBuilt)
		        return;
			
			// Now build the GUI styles
			// Using icons different from regular lock button so that
			// it would look darker
	        var tempStyle = GUI.skin.FindStyle("IN LockButton");
	        styleLock = new GUIStyle(tempStyle)
	        {
				normal = tempStyle.onNormal,
				active = tempStyle.onActive,
				hover = tempStyle.onHover,
				focused = tempStyle.onFocused,
	        };
			
			// Unselected just makes the normal states have no lock images
			tempStyle = GUI.skin.FindStyle("OL Toggle");
	        styleUnlocked = new GUIStyle(tempStyle);
#if UNITY_2018_3_OR_NEWER
            tempStyle = new GUIStyle()
            {
                normal = new GUIStyleState() { background = EditorGUIUtility.Load("Icons/d_VisibilityOff.png") as Texture2D },
                onNormal = new GUIStyleState() { background = EditorGUIUtility.Load("Icons/d_VisibilityOn.png") as Texture2D },
                fixedHeight = 16,
                fixedWidth = 16,
                border = new RectOffset(2, 2, 2, 2),
                overflow = new RectOffset(-1, 1, -2, 2),
                padding = new RectOffset(3, 3, 3, 3),
                richText = false,
                stretchHeight = false,
                stretchWidth = false,
            };
#else
            tempStyle = GUI.skin.FindStyle("VisibilityToggle");
#endif
            

            styleVisOff = new GUIStyle(tempStyle);
            styleVisOn = new GUIStyle(tempStyle)
            {
				normal = new GUIStyleState() { background = tempStyle.onNormal.background }
            };

	        styleDivider = GUI.skin.FindStyle("EyeDropperHorizontalLine");

	        stylesBuilt = (styleLock != null && styleUnlocked != null &&
	                       styleVisOn != null && styleVisOff != null &&
	                       styleDivider != null);
        }
    }
}