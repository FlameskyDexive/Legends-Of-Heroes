using System.Collections.Generic;

namespace ET.Client
{
    public static class WindowItemRes
    {
        [StaticField]
        public static Dictionary<WindowID, List<string>> WindowItemResDictionary = new Dictionary<WindowID, List<string>>()
        {
			{ WindowID.WindowID_Login, new List<string>(){"Item_test",}},
        };
    }
}
