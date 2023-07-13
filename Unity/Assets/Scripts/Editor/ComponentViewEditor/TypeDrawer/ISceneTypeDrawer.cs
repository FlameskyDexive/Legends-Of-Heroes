using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer]
    public class ISceneTypeDrawer: ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            return type == typeof (IScene);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            Entity iScene = (Entity)value;
#if ENABLE_VIEW && UNITY_EDITOR
            EditorGUILayout.ObjectField(memberName, iScene.ViewGO, memberType, true);
#endif
            return value;
        }
    }
}