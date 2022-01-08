using System.Collections.Generic;

namespace ET
{
    public class UIComponent : Entity,IAwake,IDestroy
    {
        public HashSet<WindowID> LoadingWindows                = new HashSet<WindowID>();
        public List<WindowID> VisibleWindowsQueue              = new List<WindowID>();
        public Dictionary<int, UIBaseWindow> AllWindowsDic     = new Dictionary<int, UIBaseWindow>();
        public List<WindowID> UIBaseWindowlistCached           = new List<WindowID>();
        public Dictionary<int, UIBaseWindow> VisibleWindowsDic = new Dictionary<int, UIBaseWindow>();
        public Stack<WindowID> HideWindowsStack                = new Stack<WindowID>();
    }
}