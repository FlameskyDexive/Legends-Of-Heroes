using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ReferenceFinder
{
	internal class RF_EnumDrawer
	{
		internal class EnumInfo
		{
			public static readonly Dictionary<Type, EnumInfo> cache = new Dictionary<Type, EnumInfo>();
			public readonly GUIContent[] contents;
			public readonly Array values;
			public static EnumInfo Get(Type type)
			{
				if (cache.TryGetValue(type, out EnumInfo result))
				{
					return result;
				}

				result = new EnumInfo(type);
				cache.Add(type, result);
				return result;
			}
			public EnumInfo(Type enumType)
			{
				string[] names = Enum.GetNames(enumType);

				values = Enum.GetValues(enumType);
				contents = new GUIContent[names.Length];
				for (var i = 0; i < names.Length; i++)
				{
					contents[i] = RF_GUIContent.FromString(names[i]);
				}
			}
			
			public EnumInfo(params object[] enumValues)
			{
				values = enumValues;
				contents = new GUIContent[values.Length];
				for (var i = 0; i < values.Length; i++)
				{
					contents[i] = RF_GUIContent.FromString(enumValues[i].ToString());
				}
			}

			public int IndexOf(object enumValue)
			{
				return Array.IndexOf(values, enumValue);
			}

			public object ValueAt(int index)
			{
				return values.GetValue(index);
			}
		}

		[NonSerialized] internal EnumInfo fr2_enum;
		public int index;
		public string tooltip;

		public bool DrawLayout<T>(ref T enumValue, params GUILayoutOption[] options)
		{
			if (fr2_enum == null)
			{
				Type enumType = enumValue.GetType();
				fr2_enum = EnumInfo.Get(enumType);
				index = fr2_enum.IndexOf(enumValue);
			}

			if (Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout)
			{
				GUILayout.Label(fr2_enum.contents[index], EditorStyles.toolbarPopup, options);
				return false;
			}
			
			int nIndex = EditorGUILayout.Popup(index, fr2_enum.contents, EditorStyles.toolbarPopup, options);
			if (nIndex == index)
			{
				// Debug.LogWarning($"Same index: {nIndex} | {index}");
				return false;
			}
			index = nIndex;
			enumValue = (T)fr2_enum.ValueAt(index);
			return true;
		}
		
		public bool Draw<T>(Rect rect, ref T enumValue)
		{
			if (fr2_enum == null)
			{
				var enumType = enumValue.GetType();
				fr2_enum = EnumInfo.Get(enumType);
				index = fr2_enum.IndexOf(enumValue);
			}

			if (Event.current.type == EventType.Layout) return false;
			if (Event.current.type == EventType.Repaint)
			{
				GUIContent content = fr2_enum.contents[index];
				if (!string.IsNullOrEmpty(tooltip)) content.tooltip = tooltip;
				GUI.Label(rect, content, EditorStyles.toolbarPopup);
				return false;
			}
			
			var nIndex = EditorGUI.Popup(rect, index, fr2_enum.contents, EditorStyles.toolbarPopup); //, options
			if (nIndex != index)
			{
				index = nIndex;
				enumValue = (T)fr2_enum.ValueAt(index);
				return true;
			}

			return false;
		}
	}
}