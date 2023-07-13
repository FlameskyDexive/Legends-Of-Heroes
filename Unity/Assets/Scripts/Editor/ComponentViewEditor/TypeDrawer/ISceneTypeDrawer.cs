#if ENABLE_VIEW && UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;

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
            EditorGUILayout.ObjectField(memberName, iScene.ViewGO, memberType, true);
            return value;
        }
    }
}
#endif