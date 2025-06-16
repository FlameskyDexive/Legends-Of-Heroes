using System;
using UnityEditor;
using UnityEngine;

namespace ReferenceFinder
{
    internal static class GUIExtension
    {
        internal static bool drawDebug = false;
        internal static readonly float debugAlpha = 0.02f;
        internal static int debugCount;
        internal static readonly Color[] debugColors = new Color[]
        {
            Color.cyan.Alpha(debugAlpha),
            Color.blue.Alpha(debugAlpha),
            Color.green.Alpha(debugAlpha),
            Color.yellow.Alpha(debugAlpha),
            Color.magenta.Alpha(debugAlpha),
            Color.red.Alpha(debugAlpha),
            Color.white.Alpha(debugAlpha)
        };
        
#if RF_DEV
        [MenuItem("Window/RF/Toggle Draw Debug")]
        internal static void ToggleDrawDebug()
        {
            drawDebug = !drawDebug;
        }
#endif

        internal static Color Alpha(this Color c, float alpha)
        {
            return new Color(c.r, c.g, c.b, alpha);
        }
        
        internal static bool isRepaint => Event.current.type == EventType.Repaint;
        internal static bool isMouse => Event.current.isMouse;
        internal static bool isLayout => Event.current.type == EventType.Layout;

        public static Rect DrawOverlayDebug(this Rect rect, float alpha  = 0.1f)
        {
            var saved = GUI.color;
            GUI.color = debugColors[(debugCount++) % debugColors.Length];
            {
                GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill);    
            }
            GUI.color = saved;
            return rect;
        }
        
        public static Rect LFoldout(this Rect rect, ref bool isOpen, bool drawCondition)
        {
            var (iconRect, flexRect) = rect.ExtractLeft(16f);
            if (!drawCondition) return flexRect;
            
            isOpen = EditorGUI.Foldout(iconRect, isOpen, GUIContent.none);
            if (drawDebug) DrawOverlayDebug(iconRect.Move(0, -2f));
            return flexRect;
        }
        
        static Rect DrawIcon(Rect rect, Texture icon, float w, bool left, bool drawCondition)
        {
            if (!drawCondition) return rect;
            // Debug.Log($"DrawIcon: {w}");
            
            var (iconRect, flexRect) = left ? rect.ExtractLeft(w) : rect.ExtractRight(w);
            if (icon != null && isRepaint) GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            if (drawDebug) DrawOverlayDebug(iconRect);
            return flexRect;
        }
        
        
        static Rect DrawLabel(Rect rect, GUIContent content, GUIStyle style, Color? color, bool left, bool drawCondition, float yOffset = 0)
        {
            if (content == null || content == GUIContent.none || !drawCondition) return rect;
            float w = style.CalcSize(content).x;
            // Debug.Log($"DrawLabel: {w}");
            var (labelRect, flexRect) = left ? rect.ExtractLeft(w) : rect.ExtractRight(w);
            if (drawDebug) DrawOverlayDebug(labelRect);
            if (!isRepaint) return flexRect;
            
            var c = GUI.color;
            if (color != null) GUI.color = color.Value;
            {
                GUI.Label(labelRect.Move(0f, yOffset), content, style);    
            }
            GUI.color = c;
            return flexRect;
        }

        public static Rect LColumn(this Rect rect, ref float columnW, Func<Rect, Rect> drawer)
        {
            var (lRect, flex) = rect.ExtractLeft(columnW);
            if (drawer == null) return flex;
            
            var padRect = drawer(lRect);
            // Debug.Log($"columnW = {columnW} | lRect = {lRect} | flex = {flex} | padRectWidth = {padRect.width}");
            
            if (padRect.width < 0f) columnW -= padRect.width;
            return flex;
        }
        
        public static Rect RColumn(this Rect rect, ref float columnW, Func<Rect, Rect> drawer)
        {
            var (lRect, flex) = rect.ExtractRight(columnW);
            if (drawer == null) return flex;
            
            var padRect = drawer(lRect);
            if (padRect.width < 0f) columnW -= padRect.width;
            return flex;
        }
        
        
        
        // TAB
        public static Rect LColumnAlign(this Rect rect, ref float columnX)
        {
            if (isLayout)
            {
                columnX = 0;
                return rect;
            }
            
            columnX = Mathf.Max(rect.x, columnX);
            rect.xMin = columnX;
            
            return rect;
        }
        
        public static Rect RColumnAlign(this Rect rect, ref float columnX, string name)
        {
            if (rect.xMax <= 0)
            {
                columnX = -1;
                return rect;
            }
            
            if (columnX < 0 || rect.xMax < columnX)
            {
                columnX = rect.xMax;
                // Debug.Log($"{name} : {Event.current.type} {columnX} | {rect.xMax}");
            } else
            {
                rect.xMax = columnX;
            }
            
            return rect;
        }
        
        public static Rect LDrawIcon(this Rect rect, Texture icon, float w = 16f, bool drawCondition = true)
            => DrawIcon(rect, icon, w, true, drawCondition);
        
        public static Rect RDrawIcon(this Rect rect, Texture icon, float w = 16f, bool drawCondition = true)
            => DrawIcon(rect, icon, w, false, drawCondition);
        
        // Label
        public static Rect LDrawLabel(this Rect rect, string label, Color? color = null, bool drawCondition = true)
            => DrawLabel(rect, RF_GUIContent.FromString(label), EditorStyles.label, color, true, drawCondition);
        
        public static Rect RDrawLabel(this Rect rect, string label, Color? color = null, bool drawCondition = true)
            => DrawLabel(rect, RF_GUIContent.FromString(label), EditorStyles.label, color, false, drawCondition);
        
        
        public static Rect LDrawMiniLabel(this Rect rect, GUIContent label, Color? color = null, bool drawCondition = true)
            => DrawLabel(rect, label, EditorStyles.miniLabel, color,true, drawCondition, 1f);
        
        public static Rect LDrawMiniLabel(this Rect rect, string label, Color? color = null, bool drawCondition = true)
            => LDrawMiniLabel(rect, RF_GUIContent.FromString(label), color, drawCondition);
        
        public static Rect RDrawMiniLabel(this Rect rect, GUIContent label, Color? color = null, bool drawCondition = true)
            => DrawLabel(rect, label, EditorStyles.miniLabel, color, false, drawCondition, 1f);
        
        public static Rect RDrawMiniLabel(this Rect rect, string label, Color? color = null, bool drawCondition = true)
            => RDrawMiniLabel(rect, RF_GUIContent.FromString(label), color, drawCondition);
        
        public static Rect LDrawLabel(this Rect rect, GUIContent content, GUIStyle style = null, Color? color = null, bool drawCondition = true)
        {
            if (content == null || content == GUIContent.none || !drawCondition) return rect;
            
            if (style == null) style = EditorStyles.label;
            float w = style.CalcSize(content).x;
            
            var (labelRect, rightRect) = rect.ExtractLeft(w);
            if (!isRepaint) return rightRect;
            
            var c = GUI.color;
            if (color != null) GUI.color = color.Value;
            {
                GUI.Label(labelRect, content, style);    
            }
            GUI.color = c;
            return rightRect;
        }
        
        public static Rect RDrawLabel(this Rect rect, GUIContent content, GUIStyle style = null, Color? color = null, bool drawCondition = true)
        {
            if (content == null || content == GUIContent.none || !drawCondition) return rect;
            if (style == null) style = EditorStyles.label;
            float w = Mathf.Min(style.CalcSize(content).x, rect.width);
            var (labelRect, rightRect) = rect.ExtractLeft(w);
            if (drawDebug) DrawOverlayDebug(labelRect);
            if (!isRepaint) return rightRect;

            var c = GUI.color;
            if (color != null) GUI.color = color.Value;
            {
                GUI.Label(labelRect, content, style);    
            }
            GUI.color = c;
            return rightRect;
        }
        
        public static Rect OnRightClick(this Rect rect, Action onRightClick, float padding = 0f)
        {
            var pingRect = rect;
            if (padding > 0f)
            {
                pingRect.xMin -= padding;
                pingRect.xMax += padding;
                pingRect.yMin -= padding;
                pingRect.yMax += padding;
            }
            
            if (drawDebug) DrawOverlayDebug(pingRect);
            
            bool contains = pingRect.Contains(Event.current.mousePosition);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && contains)
            {
                Event.current.Use();
                onRightClick();
            }
            
            return rect;
        }
        

        public static Rect OnLeftClick(this Rect rect, Action onClick, float padding = 0f)
        {
            var pingRect = rect;
            if (padding > 0f)
            {
                pingRect.xMin -= padding;
                pingRect.xMax += padding;
                pingRect.yMin -= padding;
                pingRect.yMax += padding;
            }
            
            if (drawDebug) DrawOverlayDebug(pingRect);
            
            bool contains = pingRect.Contains(Event.current.mousePosition);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && contains)
            {
                // Debug.Log($"OnClick: {Event.current} | {pingRect}");
                Event.current.Use();
                onClick();
            }
            
            return rect;
        }
        
    }
    
    
    
    internal static class RectExtension
    {
        internal static (Rect, Rect) HzSplit(this Rect r, float space = 8f, float ratio = 0.5f)
        {
            float w = r.width - space;
            float w1 = w * ratio;
            float w2 = w * (1 - ratio);
            return (new Rect(r.x, r.y, w1, r.height), new Rect(r.x + w1 + space, r.y, w2, r.height));
        }
        
        internal static (Rect left, Rect flex) ExtractLeft(this Rect r, float leftWidth = 16f, float space = 0f)
        {
            float deltaW = leftWidth + space;
            var (left, flex) = (new Rect(r.x, r.y, leftWidth, r.height), new Rect(r.x + deltaW, r.y, r.width - deltaW, r.height));
            if (leftWidth < 0f)
            {
                Debug.LogWarning($"Invalid:: r {r} | leftWidth = {leftWidth} | left = {left} | flex = {flex}");
            }
            return (left, flex);
        }
        
        internal static (Rect right, Rect flex) ExtractRight(this Rect r, float rightWidth = 16f, float space = 0f)
        {
            float deltaW = rightWidth + space;
            return (new Rect(r.x + r.width - deltaW, r.y, deltaW, r.height), new Rect(r.x, r.y, r.width - deltaW, r.height));
        }
        
        internal static Rect LPad(this Rect r, float padding = 8f, bool padCondition = true)
            => padCondition ? new Rect(r.x + padding, r.y, r.width-padding, r.height) : r;
        
        internal static Rect RPad(this Rect r, float padding = 8f, bool padCondition = true)
            => padCondition ? new Rect(r.x, r.y, r.width-padding, r.height): r;
        
        internal static Rect Pad(this Rect r, float padLeft = 8f, float padRight = 8f, bool padCondition = true)
            => padCondition ? new Rect(r.x + padLeft, r.y, r.width-(padLeft + padRight), r.height) : r;
        
        internal static Rect Move(this Rect r, float dx = 0f, float dy = 0f)
            => new Rect(r.x + dx, r.y + dy, r.width, r.height);
        
        internal static Rect SetHeight(this Rect r, float h)
            => new Rect(r.x, r.y, r.width, h);
        
        internal static Rect SetWidth(this Rect r, float w)
            => new Rect(r.x, r.y, w, r.height);

        
        
    }
}
