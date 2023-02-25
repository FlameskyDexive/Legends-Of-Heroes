using System;
using UnityEngine.UI;

namespace ET
{
	public static class ActionHelper
	{
		public static void Add(this Button.ButtonClickedEvent buttonClickedEvent, Action action)
		{
			buttonClickedEvent.AddListener(()=> { action(); });
		}
		public static void Add(this Toggle.ToggleEvent toggleEvent, Action action)
		{
		    toggleEvent.AddListener((isToggle) => { action(); });
		}
		public static void Add(this Dropdown.DropdownEvent dropdownEvent, Action action)
		{
		    dropdownEvent.AddListener((index) => { action(); });
		}
		public static void Add(this Slider.SliderEvent sliderEvent, Action action)
		{
		    sliderEvent.AddListener((index) => { action(); });
		}
		public static void Add(this Slider.SliderEvent sliderEvent, Action<float> action)
		{
		    sliderEvent.AddListener((value) => { action(value); });
		}
		/*public static void Add(this LongPressButton.LongPressButtonClickedEvent clickEvent, Action action)
		{
		    clickEvent.AddListener(() => { action(); });
		}
		public static void Add(this LongPressButton.LongPressButtonLongPressedEvent longPressEvent, Action action)
		{
		    longPressEvent.AddListener(() => { action(); });
		}*/
	}
}