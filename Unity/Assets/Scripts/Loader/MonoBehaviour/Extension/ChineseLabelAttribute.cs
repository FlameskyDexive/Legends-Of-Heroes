#region ScriptInfo
/****************************************************
    Author:		Flamesky
    Mail:		flamesky@live.com
    Date:		2022/2/24 10:48:36
    Function:	Nothing
*****************************************************/
#endregion


using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChineseLabelAttribute : HeaderAttribute
{
    public ChineseLabelAttribute(string header) : base(header)
    {
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ChineseLabelAttribute))]
public class ChineseLabelDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = attribute as ChineseLabelAttribute;
        EditorGUI.PropertyField(position, property, new GUIContent(att.header), true);
    }
}
#endif
