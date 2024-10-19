using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ToolbarExtension
{
    [InitializeOnLoad]
    internal static class ToolbarExtension
    {
        private static readonly List<(int, Action)> s_LeftToolbarGUI = new List<(int, Action)>();
        private static readonly List<(int, Action)> s_RightToolbarGUI = new List<(int, Action)>();

        static ToolbarExtension()
        {
            ToolbarCallback.OnToolbarGUILeft = GUILeft;
            ToolbarCallback.OnToolbarGUIRight = GUIRight;
            Type attributeType = typeof(ToolbarAttribute);
            
            foreach (var methodInfo in TypeCache.GetMethodsWithAttribute<ToolbarAttribute>())
            {
                var attributes = methodInfo.GetCustomAttributes(attributeType, false);
                if (attributes.Length > 0)
                {
                    ToolbarAttribute attribute = (ToolbarAttribute)attributes[0];
                    if (attribute.Side == OnGUISide.Left)
                    {
                        s_LeftToolbarGUI.Add((attribute.Priority, delegate
                        {
                            methodInfo.Invoke(null, null);
                        }));
                        continue;
                    }
                    if (attribute.Side == OnGUISide.Right)
                    {
                        s_RightToolbarGUI.Add((attribute.Priority, delegate
                        {
                            methodInfo.Invoke(null, null);
                        }));
                        continue;
                    }
                }
            }
            s_LeftToolbarGUI.Sort((tuple1, tuple2) => tuple1.Item1 - tuple2.Item1);
            s_RightToolbarGUI.Sort((tuple1, tuple2) => tuple2.Item1 - tuple1.Item1);
        }

        static void GUILeft()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            foreach (var handler in s_LeftToolbarGUI)
            {
                handler.Item2();
            }

            GUILayout.EndHorizontal();
        }

        static void GUIRight()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in s_RightToolbarGUI)
            {
                handler.Item2();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}