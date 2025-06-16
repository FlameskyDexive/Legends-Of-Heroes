using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ReferenceFinder
{
	internal static class RF_GUIContent
	{
		// Cache to improve performance
		private static readonly Dictionary<string, GUIContent> stringMap = new Dictionary<string, GUIContent>();
		private static readonly Dictionary<string, GUIContent> tooltipMap = new Dictionary<string, GUIContent>();
		private static readonly Dictionary<int, GUIContent> intMap = new Dictionary<int, GUIContent>();
		private static readonly Dictionary<Texture, GUIContent> texMap = new Dictionary<Texture, GUIContent>();
		private static readonly Dictionary<string, GUIContent> stringTexMap = new Dictionary<string, GUIContent>();
		private static readonly Dictionary<Type, GUIContent> typeMap = new Dictionary<Type, GUIContent>();

		public static void Release()
		{
			stringMap.Clear();
			texMap.Clear();
		}

		public static GUIContent FromString(string title, string tooltip = null)
		{
			if (string.IsNullOrEmpty(title))
			{
				Debug.LogWarning("Title is null or empty!");
				return GUIContent.none;
			}
			
			if (stringMap.TryGetValue(title, out GUIContent result)) return result;
			result = new GUIContent(title, tooltip);
			stringMap.Add(title, result);
			return result;
		}

		public static GUIContent FromType(Type t, string tooltip = null)
		{
			if (typeMap.TryGetValue(t, out GUIContent result)) return result;
			result = new GUIContent(EditorGUIUtility.ObjectContent(null, t).image, tooltip);
			typeMap.Add(t, result);
			return result;
		}
		
		public static GUIContent Tooltip(string tooltip)
		{
			if (tooltipMap.TryGetValue(tooltip, out GUIContent result)) return result;
			result = new GUIContent(string.Empty, tooltip);
			tooltipMap.Add(tooltip, result);
			return result;
		}

		public static GUIContent From(object data)
		{
			if (data is GUIContent content) return content;
			if (data is Texture texture) return FromTexture(texture);
			if (data is string s) return FromString(s);
			if (data is Type t) return FromType(t);
			return data is int i ? FromInt(i) : GUIContent.none;
		}

		public static GUIContent FromInt(int val)
		{
			if (intMap.TryGetValue(val, out GUIContent result)) return result;

			var str = val.ToString();
			result = FromString(str);
			intMap.Add(val, result);
			return result;
		}

		public static GUIContent FromTexture(Texture tex, string tooltip = null)
		{
			if (texMap.TryGetValue(tex, out GUIContent result)) return result;
			result = new GUIContent(tex, tooltip);
			texMap.Add(tex, result);
			return result;
		}

		public static GUIContent From(string title, Texture tex, string tooltip = null)
		{
			if (stringTexMap.TryGetValue(title, out GUIContent result)) return result;
			result = new GUIContent(title, tex, tooltip);
			stringTexMap.Add(title, result);
			return result;
		}

		public static GUIContent[] FromArrayLabelIcon(params object[] data)
		{
			var result = new List<GUIContent>();
			for (var i = 0; i < data.Length; i++)
			{
				result.Add(From(data[0].ToString(), (Texture)data[1]));
			}
			return result.ToArray();
		}

		public static GUIContent[] FromArray(params object[] data)
		{
			var result = new List<GUIContent>();
			foreach (object item in data)
			{
				if (item is string s)
				{
					result.Add(FromString(s));
					continue;
				}

				if (item is Texture texture)
				{
					result.Add(FromTexture(texture));
					continue;
				}

				if (item is GUIContent content)
				{
					result.Add(content);
					continue;
				}

				Debug.LogWarning("Unsupported type: " + item);
			}

			return result.ToArray();
		}

		public static GUIContent[] FromEnum(Type enumType)
		{
			Array values = Enum.GetValues(enumType);
			var result = new List<GUIContent>();
			foreach (object item in values)
			{
				result.Add(FromString(item.ToString()));
			}
			return result.ToArray();
		}
	}
}