using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIComponent))]
    [FriendOf(typeof(ShowWindowData))]
    [FriendOf(typeof(UIPathComponent))]
    [FriendOf(typeof(UIBaseWindow))]
    [FriendOf(typeof(UIComponent))]
    public static partial class UIComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIComponent self)
        {
            self.IsPopStackWndStatus = false;
            self.AllWindowsDic?.Clear();
            self.VisibleWindowsDic?.Clear();
            self.StackWindowsQueue?.Clear();
            self.UIBaseWindowlistCached?.Clear();

            // 接好 LoopScrollRect 的 item 取物委托（Loader 程序集无法直接访问 ResourcesLoaderComponent）
            LoopScrollPoolBridge.Bind(self.Root());
        }

        [EntitySystem]
        private static void Destroy(this UIComponent self)
        {
            self.CloseAllWindow();
        }

        public static bool IsWindowVisible(this UIComponent self, WindowID id)
        {
            return self.VisibleWindowsDic.ContainsKey((int)id);
        }

        public static T GetDlgLogic<T>(this UIComponent self, bool isNeedShowState = false) where T : Entity, IUILogic
        {
            WindowID windowsId = self.GetWindowIdByGeneric<T>();
            UIBaseWindow baseWindow = self.GetUIBaseWindow(windowsId);
            if (null == baseWindow)
            {
                Log.Warning($"{windowsId} is not created!");
                return null;
            }
            if (!baseWindow.IsPreLoad)
            {
                Log.Warning($"{windowsId} is not loaded!");
                return null;
            }

            if (isNeedShowState && !self.IsWindowVisible(windowsId))
            {
                Log.Warning($"{windowsId} is need show state!");
                return null;
            }

            return baseWindow.GetComponent<T>();
        }

        public static WindowID GetWindowIdByGeneric<T>(this UIComponent self) where T : Entity
        {
            if (self.Root().GetComponent<UIPathComponent>().WindowTypeIdDict.TryGetValue(typeof(T).Name, out int windowsId))
            {
                return (WindowID)windowsId;
            }
            Log.Error($"{typeof(T).FullName} is not have any windowId!");
            return WindowID.WindowID_Invaild;
        }

        public static void ShowStackWindow<T>(this UIComponent self) where T : Entity, IUILogic
        {
            WindowID id = self.GetWindowIdByGeneric<T>();
            self.ShowStackWindow(id);
        }

        public static void ShowStackWindow(this UIComponent self, WindowID id)
        {
            self.StackWindowsQueue.Enqueue(id);

            if (self.IsPopStackWndStatus)
            {
                return;
            }
            self.IsPopStackWndStatus = true;
            self.PopStackUIBaseWindow();
        }

        private static void PopStackUIBaseWindow(this UIComponent self)
        {
            if (self.StackWindowsQueue.Count > 0)
            {
                WindowID windowID = self.StackWindowsQueue.Dequeue();
                self.ShowWindow(windowID);
                UIBaseWindow uiBaseWindow = self.GetUIBaseWindow(windowID);
                uiBaseWindow.IsInStackQueue = true;
            }
            else
            {
                self.IsPopStackWndStatus = false;
            }
        }

        private static void PopNextStackUIBaseWindow(this UIComponent self, WindowID id)
        {
            UIBaseWindow uiBaseWindow = self.GetUIBaseWindow(id);
            if (uiBaseWindow != null && !uiBaseWindow.IsDisposed && self.IsPopStackWndStatus && uiBaseWindow.IsInStackQueue)
            {
                uiBaseWindow.IsInStackQueue = false;
                self.PopStackUIBaseWindow();
            }
        }

        public static void ShowWindow(this UIComponent self, WindowID id, ShowWindowData showData = null)
        {
            UIBaseWindow baseWindow = self.ReadyToShowBaseWindow(id, showData);
            if (null != baseWindow)
            {
                self.RealShowWindow(baseWindow, id, showData);
            }
        }

        public static void ShowWindow<T>(this UIComponent self, ShowWindowData showData = null) where T : Entity, IUILogic
        {
            WindowID windowsId = self.GetWindowIdByGeneric<T>();
            self.ShowWindow(windowsId, showData);
        }

        public static async ETTask ShowWindowAsync(this UIComponent self, WindowID id, ShowWindowData showData = null)
        {
            EntityRef<UIComponent> selfRef = self;
            EntityRef<ShowWindowData> showDataRef = showData;
            try
            {
                UIBaseWindow baseWindow = await self.ShowBaseWindowAsync(id, showData);
                self = selfRef;
                showData = showDataRef;
                if (null != baseWindow)
                {
                    self.RealShowWindow(baseWindow, id, showData);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ETTask ShowWindowAsync<T>(this UIComponent self, ShowWindowData showData = null) where T : Entity, IUILogic
        {
            WindowID windowsId = self.GetWindowIdByGeneric<T>();
            await self.ShowWindowAsync(windowsId, showData);
        }

        public static void HideWindow(this UIComponent self, WindowID id)
        {
            if (!self.VisibleWindowsDic.ContainsKey((int)id))
            {
                Log.Warning($"检测关闭 WindowsID: {id} 失败！");
                return;
            }

            UIBaseWindow baseWindow = self.VisibleWindowsDic[(int)id];
            if (baseWindow == null || baseWindow.IsDisposed)
            {
                Log.Error($"UIBaseWindow is null or isDisposed,  WindowsID: {id} 失败！");
                return;
            }

            baseWindow.UIPrefabGameObject?.SetActive(false);
            self.Root().GetComponent<UIEventComponent>().GetUIEventHandler(id)?.OnHideWindow(baseWindow);

            self.VisibleWindowsDic.Remove((int)id);

            self.PopNextStackUIBaseWindow(id);
        }

        public static void HideWindow<T>(this UIComponent self) where T : Entity
        {
            WindowID hideWindowId = self.GetWindowIdByGeneric<T>();
            self.HideWindow(hideWindowId);
        }

        public static void UnLoadWindow(this UIComponent self, WindowID id, bool isDispose = true)
        {
            UIBaseWindow baseWindow = self.GetUIBaseWindow(id);
            if (null == baseWindow)
            {
                Log.Error($"UIBaseWindow WindowId {id} is null!!!");
                return;
            }
            self.Root().GetComponent<UIEventComponent>().GetUIEventHandler(id)?.BeforeUnload(baseWindow);
            if (baseWindow.IsPreLoad)
            {
                UnityEngine.Object.Destroy(baseWindow.UIPrefabGameObject);
                baseWindow.UIPrefabGameObject = null;
            }
            if (isDispose)
            {
                self.AllWindowsDic.Remove((int)id);
                self.VisibleWindowsDic.Remove((int)id);
                baseWindow.Dispose();
            }
        }

        public static void UnLoadWindow<T>(this UIComponent self) where T : Entity, IUILogic
        {
            WindowID hideWindowId = self.GetWindowIdByGeneric<T>();
            self.UnLoadWindow(hideWindowId);
        }

        private static UIBaseWindow ReadyToShowBaseWindow(this UIComponent self, WindowID id, ShowWindowData showData = null)
        {
            UIBaseWindow baseWindow = self.GetUIBaseWindow(id);
            if (null == baseWindow)
            {
                baseWindow = self.AddChild<UIBaseWindow>();
                baseWindow.WindowID = id;
                self.LoadBaseWindows(baseWindow);
            }

            if (!baseWindow.IsPreLoad)
            {
                self.LoadBaseWindows(baseWindow);
            }
            return baseWindow;
        }

        private static async ETTask<UIBaseWindow> ShowBaseWindowAsync(this UIComponent self, WindowID id, ShowWindowData showData = null)
        {
            EntityRef<UIComponent> selfRef = self;
            using var _ = await self.Root().CoroutineLockComponent.Wait(CoroutineLockType.LoadUIBaseWindows, (int)id);
            self = selfRef;

            UIBaseWindow baseWindow = self.GetUIBaseWindow(id);
            if (null == baseWindow)
            {
                if (self.Root().GetComponent<UIPathComponent>().WindowPrefabPath.ContainsKey((int)id))
                {
                    baseWindow = self.AddChild<UIBaseWindow>();
                    baseWindow.WindowID = id;
                    EntityRef<UIBaseWindow> baseWindowRef = baseWindow;
                    await self.LoadBaseWindowsAsync(baseWindow);
                    self = selfRef;
                    baseWindow = baseWindowRef;
                }
            }

            if (baseWindow != null && !baseWindow.IsPreLoad)
            {
                EntityRef<UIBaseWindow> baseWindowRef = baseWindow;
                await self.LoadBaseWindowsAsync(baseWindow);
                baseWindow = baseWindowRef;
            }
            return baseWindow;
        }

        private static void RealShowWindow(this UIComponent self, UIBaseWindow baseWindow, WindowID id, Entity showData = null)
        {
            Entity contextData = showData == null ? null : showData;
            baseWindow.UIPrefabGameObject?.SetActive(true);
            self.Root().GetComponent<UIEventComponent>().GetUIEventHandler(id)?.OnShowWindow(baseWindow, contextData);

            self.VisibleWindowsDic[(int)id] = baseWindow;
            Log.Debug("### current Navigation window " + baseWindow.WindowID);
        }

        private static UIBaseWindow GetUIBaseWindow(this UIComponent self, WindowID id)
        {
            if (self.AllWindowsDic.TryGetValue((int)id, out EntityRef<UIBaseWindow> baseWindow))
            {
                return baseWindow;
            }
            return null;
        }

        public static void CloseWindow(this UIComponent self, WindowID windowId)
        {
            if (!self.VisibleWindowsDic.ContainsKey((int)windowId))
            {
                return;
            }
            self.HideWindow(windowId);
            self.UnLoadWindow(windowId);
            Log.Debug("## close window without PopNavigationWindow() ##");
        }

        public static void CloseWindow<T>(this UIComponent self) where T : Entity, IUILogic
        {
            WindowID hideWindowId = self.GetWindowIdByGeneric<T>();
            self.CloseWindow(hideWindowId);
        }

        public static void CloseAllWindow(this UIComponent self)
        {
            self.IsPopStackWndStatus = false;
            if (self.AllWindowsDic == null)
            {
                return;
            }
            foreach (KeyValuePair<int, EntityRef<UIBaseWindow>> window in self.AllWindowsDic)
            {
                UIBaseWindow baseWindow = window.Value;
                if (baseWindow == null || baseWindow.IsDisposed)
                {
                    continue;
                }
                self.HideWindow(baseWindow.WindowID);
                self.UnLoadWindow(baseWindow.WindowID, false);
                baseWindow.Dispose();
            }
            self.AllWindowsDic.Clear();
            self.VisibleWindowsDic.Clear();
            self.StackWindowsQueue.Clear();
            self.UIBaseWindowlistCached.Clear();
        }

        public static void HideAllShownWindow(this UIComponent self, bool includeFixed = false)
        {
            self.IsPopStackWndStatus = false;
            self.UIBaseWindowlistCached.Clear();
            foreach (KeyValuePair<int, EntityRef<UIBaseWindow>> windowBase in self.VisibleWindowsDic)
            {
                UIBaseWindow window = windowBase.Value;
                if (window.windowType == UIWindowType.Fixed && !includeFixed)
                {
                    continue;
                }
                if (window.IsDisposed)
                {
                    continue;
                }

                self.UIBaseWindowlistCached.Add((WindowID)windowBase.Key);
                window.UIPrefabGameObject?.SetActive(false);
                self.Root().GetComponent<UIEventComponent>().GetUIEventHandler(window.WindowID)?.OnHideWindow(window);
            }
            for (int i = 0; i < self.UIBaseWindowlistCached.Count; i++)
            {
                self.VisibleWindowsDic.Remove((int)self.UIBaseWindowlistCached[i]);
            }
            self.StackWindowsQueue.Clear();
        }

        private static void LoadBaseWindows(this UIComponent self, UIBaseWindow baseWindow)
        {
            if (!self.Root().GetComponent<UIPathComponent>().WindowPrefabPath.TryGetValue((int)baseWindow.WindowID, out string value))
            {
                Log.Error($"{baseWindow.WindowID} uiPath is not Exist!");
                return;
            }

            var go = self.Scene<Scene>().GetComponent<ResourcesLoaderComponent>().LoadAssetSync<GameObject>(value);
            baseWindow.UIPrefabGameObject = UnityEngine.Object.Instantiate(go);
            baseWindow.UIPrefabGameObject.name = go.name;

            UIEventComponent uiEventComponent = self.Root().GetComponent<UIEventComponent>();
            uiEventComponent.GetUIEventHandler(baseWindow.WindowID)?.OnInitWindowCoreData(baseWindow);

            baseWindow.SetRoot(EUIRootHelper.GetTargetRoot(self.Root(), baseWindow.windowType));
            baseWindow.uiTransform.SetAsLastSibling();

            uiEventComponent.GetUIEventHandler(baseWindow.WindowID)?.OnInitComponent(baseWindow);
            uiEventComponent.GetUIEventHandler(baseWindow.WindowID)?.OnRegisterUIEvent(baseWindow);

            self.AllWindowsDic[(int)baseWindow.WindowID] = baseWindow;
        }

        private static async ETTask LoadBaseWindowsAsync(this UIComponent self, UIBaseWindow baseWindow)
        {
            if (!self.Root().GetComponent<UIPathComponent>().WindowPrefabPath.TryGetValue((int)baseWindow.WindowID, out string value))
            {
                Log.Error($"{baseWindow.WindowID} is not Exist!");
                return;
            }

            EntityRef<UIComponent> selfRef = self;
            EntityRef<UIBaseWindow> baseWindowRef = baseWindow;
            var go = await self.Scene<Scene>().GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<GameObject>(value);
            self = selfRef;
            baseWindow = baseWindowRef;

            baseWindow.UIPrefabGameObject = UnityEngine.Object.Instantiate(go);
            baseWindow.UIPrefabGameObject.name = go.name;

            UIEventComponent uiEventComponent = self.Root().GetComponent<UIEventComponent>();
            uiEventComponent.GetUIEventHandler(baseWindow.WindowID)?.OnInitWindowCoreData(baseWindow);

            baseWindow.SetRoot(EUIRootHelper.GetTargetRoot(self.Root(), baseWindow.windowType));
            baseWindow.uiTransform.SetAsLastSibling();

            uiEventComponent.GetUIEventHandler(baseWindow.WindowID)?.OnInitComponent(baseWindow);
            uiEventComponent.GetUIEventHandler(baseWindow.WindowID)?.OnRegisterUIEvent(baseWindow);

            self.AllWindowsDic[(int)baseWindow.WindowID] = baseWindow;
        }
    }
}
