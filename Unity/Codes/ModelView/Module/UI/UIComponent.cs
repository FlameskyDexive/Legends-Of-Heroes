using System.Collections.Generic;

namespace ET
{
    public class UIComponent : Entity
    {
        public  static UIComponent Instance                    = null;
        public HashSet<WindowID> LoadingWindows                = new HashSet<WindowID>();
        public HashSet<int> ManagedWindowIds                   = new HashSet<int>();
        public List<WindowID> VisibleWindowsQueue              = new List<WindowID>();
        public Dictionary<int, UIBaseWindow> AllWindowsDic     = new Dictionary<int, UIBaseWindow>();
        public List<WindowID> UIBaseWindowlistCached           = new List<WindowID>();
        public Dictionary<int, UIBaseWindow> VisibleWindowsDic = new Dictionary<int, UIBaseWindow>();
        public Stack<WindowID> HideWindowsStack                = new Stack<WindowID>();
        public readonly Dictionary<WindowID, IAUIEventHandler> UIEventHandlers = new Dictionary<WindowID, IAUIEventHandler>();
    }
}