using System;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ToolbarExtension
{
    internal static class ToolbarCallback
    {
        static readonly Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        

        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;

        private static VisualElement m_ToolBarRootVisualElement;
        private static ScriptableObject m_ToolBarScriptableObject;
        private static FieldInfo m_ToolbarRootFieldInfo;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        static void RegisterCallback(string root, Action cb)
        {
            var toolbarZone = m_ToolBarRootVisualElement.Q(root);
            var parent = new VisualElement()
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                }
            };
            var container = new IMGUIContainer();
            container.style.flexGrow = 1;
            container.onGUIHandler += () =>
            {
                cb?.Invoke();
            };
            parent.Add(container);
            toolbarZone.Add(parent);
        }

        static void OnUpdate()
        {
            if (m_ToolBarRootVisualElement == null)
            {
                if (m_ToolBarScriptableObject == null)
                {
                    var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
                    m_ToolBarScriptableObject = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                }

                if (m_ToolbarRootFieldInfo == null && m_ToolBarScriptableObject != null)
                {
                    m_ToolbarRootFieldInfo = m_ToolBarScriptableObject.GetType()
                        .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                }

                if (m_ToolbarRootFieldInfo != null)
                {
                    m_ToolBarRootVisualElement = m_ToolbarRootFieldInfo.GetValue(m_ToolBarScriptableObject) as VisualElement;
                    if (m_ToolBarRootVisualElement != null)
                    {
                        RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
                        RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);
                    }
                }
            }
            else
            {
                VisualElement curRoot = m_ToolbarRootFieldInfo.GetValue(m_ToolBarScriptableObject) as VisualElement;
                if (m_ToolBarRootVisualElement != curRoot)
                {
                    m_ToolBarRootVisualElement = curRoot;
                    RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
                    RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);
                }
            }
        }
    }
}