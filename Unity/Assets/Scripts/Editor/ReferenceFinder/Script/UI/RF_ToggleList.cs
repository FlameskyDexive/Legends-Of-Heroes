using System;
using System.Collections.Generic;
using UnityEngine;
using ReferenceFinder;

namespace ReferenceFinder
{
	internal class RF_ToggleList
	{
		internal class Info
		{
			public float w;
			public bool status;
			public GUIContent contentOn;
			public GUIContent contentOff;
			public Action<bool> onChange;
		}

		public int current;
		public List<Info> listInfo = new List<Info>();

		public RF_ToggleList AddInfo(GUIContent content, bool status, Action<bool> onChange, float w = 20f)
		{
			listInfo.Add(new Info()
			{
				contentOn = content, contentOff = content, status = status, onChange = onChange, w = w
			});
			return this;
		}

		public RF_ToggleList AddInfo(GUIContent contentOn, GUIContent contentOff, Action<bool> onChange, float w = 20)
		{
			listInfo.Add(new Info()
			{
				contentOn = contentOn, contentOff = contentOff, status = false, onChange = onChange, w = w
			});
			return this;
		}
		public RF_ToggleList AddInfo(GUIContent content, Action<bool> onChange, float w = 20)
		{
			listInfo.Add(new Info()
			{
				contentOn = content, contentOff = content, status = false, onChange = onChange, w = w
			});
			return this;
		}

		public RF_ToggleList AddInfo(GUIContent content, float w = 20)
		{
			listInfo.Add(new Info()
			{
				contentOn = content, contentOff = content, w = w
			});
			return this;
		}

		public void Draw(ref Rect rect)
		{
			if (Event.current.type == EventType.Layout) return;

			for (var i = 0; i < listInfo.Count; i++)
			{
				Info info = listInfo[i];
				rect.width = info.w;

				if (GUI2.ToolbarToggle(rect, ref info.status, info.status ? info.contentOn : info.contentOff))
				{
					info.onChange?.Invoke(info.status);
				}

				rect.x += info.w;
			}
		}

		public void Draw(ref Rect rect, int index, ref bool b1)
		{
			if (Event.current.type == EventType.Layout) return;

			var info = listInfo[index];

			rect.width = info.w;
			if (GUI2.ToolbarToggle(rect, ref b1, b1 ? info.contentOn : info.contentOff))
			{
				if (info.onChange != null) info.onChange(info.status);
			}
			;

			// GUI.DrawTexture(rect, Texture2D.whiteTexture);
			rect.x += info.w;
		}
	}
}