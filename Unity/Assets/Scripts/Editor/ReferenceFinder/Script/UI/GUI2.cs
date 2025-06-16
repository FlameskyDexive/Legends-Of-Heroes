using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;
namespace ReferenceFinder
{
    public interface IDrawer
    {
        bool Draw(Rect rect);
        bool DrawLayout();
    }

    

    internal static class RF_GUI
    {
        internal class GUIColor  : IDisposable
        {
            private readonly Color color;
            public GUIColor(Color c)
            {
                color = GUI.color;
                GUI.color = c;
            }
            public void Dispose() => GUI.color = color;
        }
    
        internal class GUIContentColor  : IDisposable
        {
            private readonly Color color;
            public GUIContentColor(Color c)
            {
                color = GUI.contentColor;
                GUI.contentColor = c;
            }
            public void Dispose() => GUI.contentColor = color;
        }
    
        internal class GUIBackgroundColor  : IDisposable
        {
            private readonly Color color;
            public GUIBackgroundColor(Color c)
            {
                color = GUI.backgroundColor;
                GUI.backgroundColor = c;
            }
            public void Dispose() => GUI.backgroundColor = color;
        }
        
        public static GUIColor Color(Color c) => new GUIColor(c);
        public static GUIContentColor ContentColor(Color c) => new GUIContentColor(c);
        public static GUIBackgroundColor BackgroundColor(Color c) => new GUIBackgroundColor(c);
        
    }
    
    
    
    
    
    

    internal static class GUI2
    {

        internal static Dictionary<string, GUIContent> tooltipCache = new Dictionary<string, GUIContent>();

        // -----------------------

        private static GUIStyle _miniLabelAlignRight;

        public static Color darkRed = new Color(0.5f, .0f, 0f, 1f);
        public static Color darkGreen = new Color(0, .5f, 0f, 1f);
        public static Color darkBlue = new Color(0, .0f, 0.5f, 1f);
        public static Color lightRed = new Color(1f, 0.5f, 0.5f, 1f);
        
        
        public static readonly GUILayoutOption[] GLW_20 = new GUILayoutOption[] {GUILayout.Width(20f)};
        public static readonly GUILayoutOption[] GLW_24 = new GUILayoutOption[] {GUILayout.Width(24f)};
        public static readonly GUILayoutOption[] GLW_50 = new GUILayoutOption[] {GUILayout.Width(50f)};
        public static readonly GUILayoutOption[] GLW_70 = new GUILayoutOption[] {GUILayout.Width(70f)};
        public static readonly GUILayoutOption[] GLW_80 = new GUILayoutOption[] {GUILayout.Width(80f)};
        public static readonly GUILayoutOption[] GLW_100 =new GUILayoutOption[] { GUILayout.Width(100f)};
        public static readonly GUILayoutOption[] GLW_120 =new GUILayoutOption[] { GUILayout.Width(120f)};
        public static readonly GUILayoutOption[] GLW_140 =new GUILayoutOption[] { GUILayout.Width(140f)};
        public static readonly GUILayoutOption[] GLW_150 =new GUILayoutOption[] { GUILayout.Width(150f)};
        public static readonly GUILayoutOption[] GLW_160 =new GUILayoutOption[] { GUILayout.Width(160f)};
        public static readonly GUILayoutOption[] GLW_320 =new GUILayoutOption[] { GUILayout.Width(320f)};
        

        public static GUIStyle miniLabelAlignRight
        {
            get
            {
                if (_miniLabelAlignRight != null) return _miniLabelAlignRight;
                return _miniLabelAlignRight = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight };
            }
        }

        public static void Color(Action a, Color c, float? alpha = null)
        {
            if (a == null) return;

            Color cColor = GUI.color;
            if (alpha != null) c.a = alpha.Value;

            GUI.color = c;
            a();
            GUI.color = cColor;
        }

        public static void ContentColor(Action a, Color c, float? alpha = null)
        {
            if (a == null) return;

            Color cColor = GUI.contentColor;
            if (alpha != null) c.a = alpha.Value;

            GUI.contentColor = c;
            a();
            GUI.contentColor = cColor;
        }

        public static void BackgroundColor(Action a, Color c, float? alpha = null)
        {
            if (a == null) return;

            Color cColor = GUI.backgroundColor;
            if (alpha != null) c.a = alpha.Value;

            GUI.backgroundColor = c;
            a();
            GUI.backgroundColor = cColor;
        }

        public static Color Theme(Color proColor, Color indieColor)
        {
            return EditorGUIUtility.isProSkin ? proColor : indieColor;
        }

        public static Color Alpha(Color c, float a)
        {
            c.a = a;
            return c;
        }

        public static void Rect(Rect r, Color c, float? alpha = null)
        {
            Color cColor = GUI.color;
            if (alpha != null) c.a = alpha.Value;

            GUI.color = c;
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = cColor;
        }

        public static Object[] DropZone(string title, float w, float h)
        {
            Rect rect = GUILayoutUtility.GetRect(w, h);
            GUI.Box(rect, GUIContent.none, EditorStyles.textArea);

            float cx = rect.x + w / 2f;
            float cy = rect.y + h / 2f;
            float pz = w / 3f; // plus size

            var plusRect = new Rect(cx - pz / 2f, cy - pz / 2f, pz, pz);
            Color(() => { GUI.DrawTexture(plusRect, RF_Icon.Plus.image, ScaleMode.ScaleToFit); }, UnityEngine.Color.white, 0.1f);

            GUI.Label(rect, title, EditorStyles.wordWrappedMiniLabel);

            EventType eventType = Event.current.type;
            var isAccepted = false;

            if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    isAccepted = true;
                }
                Event.current.Use();
            }

            return isAccepted ? DragAndDrop.objectReferences : null;
        }

        //        public static bool ColorIconButton(Rect r, Texture icon, Vector2? iconOffset, Color? c)
        //        {
        //            if (c != null) Rect(r, c.Value);
        //            
        //            // align center
        //            if (iconOffset != null)
        //            {
        //                r.x += iconOffset.Value.x;
        //                r.y += iconOffset.Value.y;
        //            }
        //            
        //            return GUI.Button(r, icon, GUIStyle.none);
        //        }

        public static bool ColorIconButton(Rect r, Texture icon, Color? c)
        {
            Color oColor = GUI.color;
            if (c != null) GUI.color = c.Value;
            bool result = GUI.Button(r, icon, GUIStyle.none);
            GUI.color = oColor;
            return result;
        }
        
		public static bool Toggle(Rect r, ref bool value, string label, GUIStyle style, params GUILayoutOption[] options)
		{
			var guiContent = RF_GUIContent.From(label);
			var vv = GUI.Toggle(r, value, guiContent, style);
			if (vv == value) return false;
			value = vv;
			return true;
		}
        
		public static bool Toggle(ref bool value, string label, GUIStyle style, params GUILayoutOption[] options)
		{
			var guiContent = RF_GUIContent.From(label);
            bool vv = GUILayout.Toggle(value, guiContent, style, options);
            if (vv == value) return false;
            value = vv;
            return true;
        }

        public static bool Toggle(ref bool value, Texture2D tex, GUIStyle style, params GUILayoutOption[] options)
        {
            bool vv = GUILayout.Toggle(value, tex, style, options);
            if (vv == value) return false;
            value = vv;
            return true;
        }

		public static bool Toggle(Rect r, ref bool value, GUIContent tex, GUIStyle style)
		{
			var vv = GUI.Toggle(r, value, tex, style);
			if (vv == value) return false;
			value = vv;
			return true;
		}
        public static bool Toggle(ref bool value, GUIContent tex, GUIStyle style, params GUILayoutOption[] options)
        {
            bool vv = GUILayout.Toggle(value, tex, style, options);
            if (vv == value) return false;
            value = vv;
            return true;
        }

        public static bool Toggle(Rect rect, ref bool value, GUIContent tex)
        {
            bool vv = GUI.Toggle(rect, value, tex, GUIStyle.none);
            if (vv == value) return false;
            value = vv;
            return true;
        }

        public static bool Toggle(Rect rect, ref bool value)
        {
            bool vv = GUI.Toggle(rect, value, GUIContent.none);
            if (vv == value) return false;
            value = vv;
            return true;
        }

        internal static bool Toggle(bool v, string label, Action<bool> setter)
        {
            var guiContent = RF_GUIContent.From(label);
            bool v1 = GUILayout.Toggle(v, guiContent);
            if (v1 == v) return false;
            if (setter != null) setter(v1);
            return true;
        }
        
        internal static bool ToolbarToggle(Rect r, ref bool value, Texture icon, Vector2 padding, string tooltip = null)
        {
        	//TODO: FIX GC
        	var vv = GUI.Toggle(r, value, RF_GUIContent.Tooltip(tooltip), EditorStyles.toolbarButton);

        	if (icon != null)
        	{
        		var rect = GUILayoutUtility.GetLastRect();
        		rect = Padding(rect, padding.x, padding.y);
        		GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
        	}

        	if (vv == value) return false;
        	value = vv;
        	return true;
        }
        
        internal static bool GUI_ToggleOptimize(Rect r, bool value, GUIContent content)
        {
            var eventType = Event.current.type;

            if (eventType == EventType.MouseDown)
            {
                return GUI.Toggle(r, value, content, EditorStyles.toolbarButton);
            }

            if (eventType == EventType.Repaint)
            {
                GUI.DrawTexture(r, content.image, ScaleMode.ScaleToFit);
            }
            return false;
        }
        
        public static bool ToolbarToggle(ref bool value, Texture icon, Vector2 padding, string tooltip, Rect position)
        {
            // Draw the toggle button directly at the specified position
            bool newValue = GUI.Toggle(position, value, RF_GUIContent.FromTexture(icon, tooltip), EditorStyles.toolbarButton);

            // Update the reference value
            bool changed = newValue != value;
            value = newValue;

            return changed;
        }

        // Example implementation of a Toggle
        public static bool Toggle(ref bool value, GUIContent content, GUIStyle style, Rect position)
        {
            // Draw the toggle button directly at the specified position
            bool newValue = GUI.Toggle(position, value, content, style);

            // Update the reference value
            bool changed = newValue != value;
            value = newValue;

            return changed;
        }

        internal static bool ToolbarToggle(Rect r, ref bool value, GUIContent content)
        {
            if (value == false)
            {
                if (GUI.Toggle(r, value, content, EditorStyles.toolbarButton) != value)
                {
                    value = true;
                    return true;
                }

                return false;
            }

            var image = content.image;
            content.image = null;

            if (GUI.Toggle(r, value, content, EditorStyles.toolbarButton) == false)
            {
                value = false;
                content.image = image;
                return true;
            }

            if (image != null)
            {
                content.image = image;
                r.xMin += 1;
                r.xMax -= 1;
                GUI.DrawTexture(r, image, ScaleMode.ScaleToFit);
            }

            return false;
        }

        internal static bool ToolbarToggle(ref bool value, Texture icon, Vector2 padding, string tooltip = null)
        {
            var vv = false;
            Profiler.BeginSample("RF-GUI2.ToolbarToggle");
            {
                vv = GUILayout.Toggle(value, RF_GUIContent.Tooltip(tooltip), EditorStyles.toolbarButton, GUI2.GLW_24);
                if (icon != null)
                {
                    Rect rect = GUILayoutUtility.GetLastRect();
                    rect = Padding(rect, padding.x, padding.y);
                    GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
                }    
            }
            Profiler.EndSample();
            
            if (vv == value) return false;
            value = vv;
            return true;
        }

        // internal static bool GUILayoutToggle(ref bool value, string tooltip)
        // {
        //     Profiler.BeginSample("RF-GUI2.ToolbarToggle2-step1");
        //     Rect rect = GUILayoutUtility.GetRect(24, 24, 18f, 18f);
        //     Profiler.EndSample();
        //     
        //     Profiler.BeginSample("RF-GUI2.ToolbarToggle2-step2");
        //     bool oldValue = value;
        //     value = GUI.Toggle(rect, value, RF_GUIContent.Tooltip(tooltip), EditorStyles.toolbarButton);
        //     Profiler.EndSample();
        //     
        //     return oldValue != value;
        // }
        
        // TODO : optimize for performance
        // public static bool EnumPopup<T>(ref T mode, string label, float labelWidth, GUIStyle style, params GUILayoutOption[] options)
        // {
        //     var sz = EditorGUIUtility.labelWidth;
        //     EditorGUIUtility.labelWidth = labelWidth;
        //     {
        //         var obj = (Enum)(object)mode;
        //         var vv = EditorGUILayout.EnumPopup(label, obj, style, options);
        //         if (Equals(vv, obj))
        //         {
        //             EditorGUIUtility.labelWidth = sz;
        //             return false;
        //         }

        //         mode = (T)(object)vv;
        //     }
        //     EditorGUIUtility.labelWidth = sz;
        //     return true;
        // }

        // public static bool EnumPopup<T>(ref T mode, GUIContent icon, GUIStyle style, params GUILayoutOption[] options)
        // {
        //     var obj = (Enum)(object)mode;
        //     var cRect = GUILayoutUtility.GetRect(16f, 16f);

        // 	if (Event.current.type == EventType.Repaint)
        // 	{
        // 		cRect.xMin -= 2f;
        // 		cRect.yMin += 2f;
        // 		GUI.Label(cRect, icon);
        // 	}

        // 	if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        // 	{
        // 		var vv = EditorGUILayout.EnumPopup(obj, style, options);
        // 		if (Equals(vv, obj))
        // 		{
        // 			return false;
        // 		}
        // 		mode = (T)(object)vv;
        // 		return true;
        // 	}

        // 	//if (Event.current.type == EventType.Repaint)
        // 	{
        // 		EditorGUILayout.LabelField(RF_GUIContent.FromString("Hello"), style, options);
        // 	}

        // 	return false;
        // }

        public static Rect Padding(Rect r, float x, float y)
        {
            return new Rect(r.x + x, r.y + y, r.width - 2 * x, r.height - 2 * y);
        }

        public static Rect LeftRect(float w, ref Rect rect)
        {
            rect.x += w;
            rect.width -= w;
            return new Rect(rect.x - w, rect.y, w, rect.height);
        }

        public static Rect RightRect(float w, ref Rect rect)
        {
            rect.width -= w;
            return new Rect(rect.x + rect.width, rect.y, w, rect.height);
        }
    }
}
