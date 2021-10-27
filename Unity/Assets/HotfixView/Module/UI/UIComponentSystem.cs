using UnityEngine;
using System;
using System.Collections.Generic;

namespace ET
{
    [ObjectSystem]
    public class UIComponentAwakeSystem : AwakeSystem<UIComponent>
    {
        public override void Awake(UIComponent self)
        {
            self.Init();
        }
    }
    
    [ObjectSystem]
    public class UIComponentUpdateSystem : UpdateSystem<UIComponent>
    {
        public override void Update(UIComponent self)
        {
          
        }
    }
    
    [ObjectSystem]
    public class UIComponentDestroySystem : DestroySystem<UIComponent>
    {
        public override void Destroy(UIComponent self)
        {
            self.Destroy();
        }
    }
    
    public static class UIComponentSystem
    {
        public static void Init(this UIComponent self)
        {
            UIComponent.Instance = self;
            self.InitWindowManager();
            self.InitWindowControl();
            self.InitUIEventHandler();
        }
        
        private static void InitWindowManager(this UIComponent self)
        {
            if (null != self.AllWindowsDic)
            {
                self.AllWindowsDic.Clear();
            }
            if (null != self.VisibleWindowsDic)
            {
                self.VisibleWindowsDic.Clear();
            }
            if (self.VisibleWindowsQueue != null)
            {
                self.VisibleWindowsQueue.Clear();
            }
            if (self.HideWindowsStack != null)
            {
                self.HideWindowsStack.Clear();
            }
        }
        
        private static void InitWindowControl(this UIComponent self)
        {
            self.ManagedWindowIds.Clear();
            UIGlobalDefine.WindowPrefabPath.Clear();
            UIGlobalDefine.WindowTypeIdDict.Clear();
            foreach (WindowID windowID in Enum.GetValues(typeof(WindowID)))
            {
                string dlgName = "Dlg" + windowID.ToString().Split('_')[1];
                UIGlobalDefine.WindowPrefabPath.Add((int)windowID , dlgName);
                UIGlobalDefine.WindowTypeIdDict.Add(dlgName, (int)windowID );
                self.ManagedWindowIds.Add((int)windowID);
            }
        }
        
        private static void InitUIEventHandler(this UIComponent self)
        {
            self.UIEventHandlers.Clear();
            foreach (Type v in Game.EventSystem.GetTypes(typeof (AUIEventAttribute)))
            {
                Type inter = v.GetInterface(typeof (IAUIEventHandler).Name);
                if (inter == null)
                {
                    continue;
                }
                AUIEventAttribute attr = v.GetCustomAttributes(typeof (AUIEventAttribute), false)[0] as AUIEventAttribute;
                self.UIEventHandlers.Add(attr.WindowID, Activator.CreateInstance(v) as IAUIEventHandler);
            }
        }
        
        
        /// <summary>
        /// 窗口是否是正在显示的 
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <returns></returns>
        public static bool IsWindowVisible(this UIComponent self,WindowID id)
        {
            return self.VisibleWindowsDic.ContainsKey((int)id);
        }
        
        
        /// <summary>
        /// 隐藏最新出现的窗口
        /// </summary>
        public static void HideLastWindows(this UIComponent self)
        {
            if (self.VisibleWindowsQueue.Count <= 0)
            {
                return;
            }
            WindowID windowID  = self.VisibleWindowsQueue[self.VisibleWindowsQueue.Count - 1];
            if (!self.IsWindowVisible(windowID))
            {
               return;
            }
            self.HideWindow(windowID);
        }
        
        /// <summary>
        /// 彻底关闭最新出现的窗口
        /// </summary>
        public static void CloseLastWindows(this UIComponent self)
        {
            if (self.VisibleWindowsQueue.Count <= 0)
            {
                return;
            }
            WindowID windowID  = self.VisibleWindowsQueue[self.VisibleWindowsQueue.Count - 1];
            if (!self.IsWindowVisible(windowID))
            {
                return;
            }
            self.CloseWindow(windowID);
        }
        
        public static void ShowWindow<T>(this UIComponent self,WindowID preWindowID = WindowID.WindowID_Invaild, ShowWindowData showData = null) where T : Entity
        {
            WindowID windowsId = self.GetWindowIdByGeneric<T>();
            self.ShowWindow(windowsId,preWindowID,showData);
        }
        
        /// <summary>
        /// 现实ID指定的UI窗口
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <OtherParam name="showData"></OtherParam>
        public static void ShowWindow(this UIComponent self,WindowID id, WindowID preWindowID = WindowID.WindowID_Invaild, ShowWindowData showData = null)
        {
            UIBaseWindow baseWindow = self.ReadyToShowBaseWindow(id, showData);
            if (null != baseWindow)
            {
                self.RealShowWindow(baseWindow, id, showData,preWindowID);
            }
        }

        public static async ETTask ShowWindowAsync(this UIComponent self,WindowID id,WindowID preWindowID = WindowID.WindowID_Invaild, ShowWindowData showData = null)
        {
            try
            {
                if (self.LoadingWindows.Contains(id))
                {
                    return;
                }
                UIBaseWindow baseWindow = await self.ShowBaseWindowAsync(id, showData);
                if (null != baseWindow)
                {
                    self.RealShowWindow(baseWindow, id, showData,preWindowID);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
        
        public static async ETTask ShowWindowAsync<T>(this UIComponent self,WindowID preWindowID = WindowID.WindowID_Invaild, ShowWindowData showData = null) where T : Entity
        {
            WindowID windowsId = self.GetWindowIdByGeneric<T>();
           await self.ShowWindowAsync(windowsId,preWindowID,showData);
        }
        
        public static void HideAndShowWindowStack(this UIComponent self,WindowID hideWindowId, WindowID showWindowId)
        {
            self.HideWindow(hideWindowId,true);
            self.ShowWindow(showWindowId,preWindowID:hideWindowId);
        }
        
        public static void HideAndShowWindowStack<T,K>(this UIComponent self) where T : Entity  where K : Entity
        {
            WindowID hideWindowId = self.GetWindowIdByGeneric<T>();
            WindowID showWindowId = self.GetWindowIdByGeneric<K>();
            self.HideAndShowWindowStack(hideWindowId,showWindowId);
        }
        
        public static async ETTask HideAndShowWindowStackAsync(this UIComponent self,WindowID hideWindowId, WindowID showWindowId)
        {
            self.HideWindow(hideWindowId,true);
            await self.ShowWindowAsync(showWindowId,preWindowID:hideWindowId);
        }
        
        public static async ETTask HideAndShowWindowStackAsync<T,K>(this UIComponent self) where T : Entity  where K : Entity
        {
            WindowID hideWindowId = self.GetWindowIdByGeneric<T>();
            WindowID showWindowId = self.GetWindowIdByGeneric<K>();
            await self.HideAndShowWindowStackAsync(hideWindowId,showWindowId);
        }
        
        
        /// <summary>
        /// 隐藏ID指定的UI窗口
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        /// <OtherParam name="onComplete"></OtherParam>
        public static void HideWindow(this UIComponent self,WindowID id,bool isPushToStack = false)
        {
            if ( !self.CheckDirectlyHide(id))
            {
                Log.Error($"检测关闭 WindowsID: {id} 失败！");
                return;
            }

            if ( isPushToStack )
            {
                return;
            }

            if (self.HideWindowsStack.Count <= 0)
            {
                return;
            }

            WindowID preWindowID = self.HideWindowsStack.Pop(); ;
            self.ShowWindow(preWindowID);
        }
        
        public static void  HideWindow<T>(this UIComponent self,bool isPushToStack = false) where T : Entity 
        {
            WindowID hideWindowId = self.GetWindowIdByGeneric<T>();
            self.HideWindow(hideWindowId,isPushToStack);
        }
        
        
        /// <summary>
        /// 卸载指定的UI窗口实例
        /// </summary>
        /// <OtherParam name="id"></OtherParam>
        public static void UnLoadWindow(this UIComponent self,WindowID id)
        {
            UIBaseWindow baseWindow = self.GetUIBaseWindow(id);
            if (null != baseWindow)
            {
                self.GetUIEventHandler(id).BeforeUnload(baseWindow);
                if(baseWindow.IsPreLoad)
                {
                    if (null != baseWindow.m_uiPrefabGameObject)
                    {
                        Game.Scene.GetComponent<ResourcesComponent>()?.UnloadBundle(baseWindow.m_uiPrefabGameObject.name.StringToAB());
                        UnityEngine.Object.Destroy( baseWindow.m_uiPrefabGameObject);
                        baseWindow.m_uiPrefabGameObject = null;
                    }
                }
                self.AllWindowsDic.Remove((int) id);
                self.VisibleWindowsDic.Remove((int) id);
                self.VisibleWindowsQueue.Remove(id);
                baseWindow?.Dispose();
            }
        }

        public static void  UnLoadWindow<T>(this UIComponent self) where T : Entity 
        {
            WindowID hideWindowId = self.GetWindowIdByGeneric<T>();
            self.UnLoadWindow(hideWindowId);
        }

        private static  UIBaseWindow  ReadyToShowBaseWindow(this UIComponent self,WindowID id, ShowWindowData showData = null)
        {
            // 检查状态
            if (!self.IsWindowInControl(id))
            {
                return null;
            }

            UIBaseWindow baseWindow = self.GetUIBaseWindow(id);
            // 如果UI不存在开始实例化新的窗口
            if (null == baseWindow)
            {
                if (UIGlobalDefine.WindowPrefabPath.ContainsKey((int)id))
                {
                    baseWindow = self.AddChild<UIBaseWindow>();
                    baseWindow.WindowID = id;
                    baseWindow.Load();
                    self.GetUIEventHandler(id).OnInitWindowCoreData(baseWindow);
                    self.GetUIEventHandler(id).OnInitComponent(baseWindow);
                    self.GetUIEventHandler(id).OnRegisterUIEvent(baseWindow);
                    if (baseWindow.IsPreLoad)
                    {
                        if (baseWindow.WindowID != id)
                        {
                            Debug.LogError(string.Format("<color=cyan>[BaseWindowId :{0} != shownWindowId :{1}]</color>", baseWindow.WindowID, id));
                            return null;
                        }
                       
                        // 设置根节点
                        baseWindow.SetRoot(self.GetTargetRoot(baseWindow.WindowData.windowType));
                        self.AllWindowsDic[(int)id] = baseWindow;
                    }
                }
            }
            else
            {
                if (!baseWindow.IsPreLoad)
                {
                    baseWindow.Load();
                    self.GetUIEventHandler(id).OnInitWindowCoreData(baseWindow);
                    self.GetUIEventHandler(id).OnInitComponent(baseWindow);
                    self.GetUIEventHandler(id).OnRegisterUIEvent(baseWindow);
                    baseWindow.SetRoot(self.GetTargetRoot(baseWindow.WindowData.windowType));
                    self.AllWindowsDic[(int)id] = baseWindow;
                }

            }
            if (baseWindow == null)
            {
                Debug.LogError("[window m_singleton is null.]" + id.ToString());
                return null;
            }
            
            // 改变层级关系
            if (baseWindow.uiTransform != null)
            {
                baseWindow.uiTransform.SetAsLastSibling();
            }
            //baseWindow.uiTransform.SetAsLastSibling();
            return baseWindow;
        }

        private static async ETTask<UIBaseWindow> ShowBaseWindowAsync(this UIComponent self,WindowID id, ShowWindowData showData = null)
        {
            // 检查状态
            if (!self.IsWindowInControl(id))
            {
                return null;
            }

            UIBaseWindow baseWindow = self.GetUIBaseWindow(id);
            if (null == baseWindow)
            {
                if (UIGlobalDefine.WindowPrefabPath.ContainsKey((int)id))
                {
                    self.LoadingWindows.Add(id);
                    baseWindow          = self.AddChild<UIBaseWindow>();
                    baseWindow.WindowID = id;
                    baseWindow.WindowID = id;
                    await baseWindow.LoadAsync();
                    self.GetUIEventHandler(id).OnInitWindowCoreData(baseWindow);
                    self.GetUIEventHandler(id).OnInitComponent(baseWindow);
                    self.GetUIEventHandler(id).OnRegisterUIEvent(baseWindow);
                    if (baseWindow.IsPreLoad)
                    {
                        if (baseWindow.WindowID != id)
                        {
                            Debug.LogError(string.Format("<color=cyan>[BaseWindowId :{0} != shownWindowId :{1}]</color>", baseWindow.WindowID, id));
                            return null;
                        }
                        // 设置根节点
                        baseWindow.SetRoot(self.GetTargetRoot(baseWindow.WindowData.windowType));
                        self.AllWindowsDic[(int)id] = baseWindow;
                    }
                    self.LoadingWindows.Remove(id);
                }
            }
            else
            {
                if (!baseWindow.IsPreLoad)
                {
                    await baseWindow.LoadAsync();
                    self.GetUIEventHandler(id).OnInitWindowCoreData(baseWindow);
                    self.GetUIEventHandler(id).OnInitComponent(baseWindow);
                    self.GetUIEventHandler(id).OnRegisterUIEvent(baseWindow);
                    baseWindow.SetRoot(self.GetTargetRoot(baseWindow.WindowData.windowType));
                    self.AllWindowsDic[(int)id] = baseWindow;
                }
            }
            if (baseWindow == null)
            {
                Debug.LogError("[window m_singleton is null.]" + id.ToString());
                return null;
            }
            
            // 改变层级关系
            if (baseWindow.uiTransform != null)
            {
                baseWindow.uiTransform.SetAsLastSibling();
            }
            return baseWindow;
        }
        

        private static Transform GetTargetRoot(this UIComponent self,UIWindowType type)
        {
            if (type == UIWindowType.Normal)
            {
                return Game.Scene.GetComponent<GlobalComponent>().NormalRoot;
            }
            else if (type == UIWindowType.Fixed)
            {
                return Game.Scene.GetComponent<GlobalComponent>().FixedRoot;
            }
            else if (type == UIWindowType.PopUp)
            {
                return Game.Scene.GetComponent<GlobalComponent>().PopUpRoot;
            }
            else if (type == UIWindowType.Other)
            {
                return Game.Scene.GetComponent<GlobalComponent>().OtherRoot;
            }

            Log.Error("uiroot type is error: " + type.ToString());
            return null;
        }
        
        public static void Destroy(this UIComponent self)
        {
            self.ClearAllWindow();
        }

        private static UIBaseWindow GetUIBaseWindow(this UIComponent self,WindowID id)
        {
            if ( !self.IsWindowInControl(id) )
            {
                return null;
            }
            if (self.AllWindowsDic.ContainsKey((int)id))
            {
                return self.AllWindowsDic[(int)id];
            }
            return null;
        }
        
        public static T GetDlgLogic<T>(this UIComponent self,bool isNeedShowState = false) where  T : Entity
        {
            WindowID windowsId = self.GetWindowIdByGeneric<T>();
            UIBaseWindow baseWindow = self.GetUIBaseWindow(windowsId);
            if ( null == baseWindow )
            {
                Log.Warning($"{windowsId} is not created!");
                return null;
            }
            if ( !baseWindow.IsPreLoad )
            {
                Log.Warning($"{windowsId} is not loaded!");
                return null;
            }

            if (isNeedShowState )
            {
                if ( !self.IsWindowVisible(windowsId) )
                {
                    Log.Warning($"{windowsId} is need show state!");
                    return null;
                }
            }
            
            return baseWindow.GetComponent<T>();
        }
        
        public static WindowID GetWindowIdByGeneric<T>(this UIComponent self) where  T : Entity
        {
            if ( UIGlobalDefine.WindowTypeIdDict.TryGetValue(typeof(T).Name,out int windowsId) )
            {
                return (WindowID)windowsId;
            }
            Log.Error($"{typeof(T).FullName} is not have any windowId!" );
            return  WindowID.WindowID_Invaild;
        }
        
        public static void CloseWindow(this UIComponent self,WindowID windowId)
        {
            if (!self.IsWindowInControl(windowId))
            {
                Debug.LogError("## Current UI Manager has no control power of " + windowId.ToString());
                return;
            }

            if (!self.VisibleWindowsDic.ContainsKey((int)windowId))
            {
                return;
            }
            self.HideWindow(windowId);
            self.UnLoadWindow(windowId);
            Debug.Log("<color=magenta>## close window without PopNavigationWindow() ##</color>");
        }
        
        public static void  CloseWindow<T>(this UIComponent self) where T : Entity 
        {
            WindowID hideWindowId = self.GetWindowIdByGeneric<T>();
            self.CloseWindow(hideWindowId);
        }
        
        public static void ClearAllWindow(this UIComponent self)
        {
            if (self.AllWindowsDic != null)
            {
                foreach (KeyValuePair<int, UIBaseWindow> window in self.AllWindowsDic)
                {
                    UIBaseWindow baseWindow = window.Value;
                    self.GetUIEventHandler(baseWindow.WindowID).BeforeUnload(baseWindow);
                    (baseWindow as Entity)?.Dispose();
                }
                self.AllWindowsDic.Clear();
                self.VisibleWindowsDic.Clear();
                self.LoadingWindows.Clear();
                self.VisibleWindowsQueue.Clear();
                self.HideWindowsStack.Clear();
            }
        }
        public static void HideAllShownWindow(this UIComponent self,bool includeFixed = false)
        {
            self.UIBaseWindowlistCached.Clear();
            foreach (KeyValuePair<int, UIBaseWindow> window in self.VisibleWindowsDic)
            {
                if (window.Value.WindowData.windowType == UIWindowType.Fixed && !includeFixed)
                    continue;
                self.UIBaseWindowlistCached.Add((WindowID)window.Key);
                
                if (null != window.Value.m_uiPrefabGameObject)
                {
                    window.Value.m_uiPrefabGameObject.SetActive(false);
                }
                self.GetUIEventHandler(window.Value.WindowID).OnHideWindow(window.Value);
            }
            if (self.UIBaseWindowlistCached.Count > 0)
            {
                for (int i = 0; i < self.UIBaseWindowlistCached.Count; i++)
                {
                    self.VisibleWindowsDic.Remove((int)self.UIBaseWindowlistCached[i]);
                }
            }
            self.VisibleWindowsQueue.Clear();
            self.HideWindowsStack.Clear();
        }
        
        private static void RealShowWindow(this UIComponent self,UIBaseWindow baseWindow, WindowID id, ShowWindowData showData = null,WindowID preWindowID = WindowID.WindowID_Invaild)
        {
            if (baseWindow.WindowData.windowType == UIWindowType.PopUp && baseWindow.WindowID != WindowID.WindowID_MessageBox)
            {
                self.VisibleWindowsQueue.Add(id);
            }
            
            Entity contextData = showData == null ? null : showData.contextData;
            if (null != baseWindow.m_uiPrefabGameObject)
            {
                baseWindow.m_uiPrefabGameObject.SetActive(true);
            }
            self.GetUIEventHandler(id).OnShowWindow(baseWindow,showData);
            
            self.VisibleWindowsDic[(int)id] = baseWindow;
            if (preWindowID != WindowID.WindowID_Invaild)
            {
                self.HideWindowsStack.Push(preWindowID);
            }
         
            Debug.Log("<color=magenta>### current Navigation window </color>" + baseWindow.WindowID.ToString());
        }
        
        private static bool CheckDirectlyHide(this UIComponent self,WindowID id)
        {
            if (!self.IsWindowInControl(id))
            {
                Debug.Log("## Current UI Manager has no control power of " + id.ToString());
                return false;
            }
            
            if (!self.VisibleWindowsDic.ContainsKey((int)id))
            {
                return false;
            }

            UIBaseWindow baseWindow = self.VisibleWindowsDic[(int)id];
            if (null != baseWindow.m_uiPrefabGameObject)
            {
                baseWindow.m_uiPrefabGameObject.SetActive(false);
            }
          
            self.GetUIEventHandler(id).OnHideWindow(baseWindow);
            self.VisibleWindowsDic.Remove((int)id);
            self.VisibleWindowsQueue.Remove(id);
            return true;
        }
        
        private static bool IsWindowInControl(this UIComponent self,WindowID id)
        {
            return self.ManagedWindowIds.Contains((int)id);
        }
        
        public static IAUIEventHandler GetUIEventHandler(this UIComponent self,WindowID windowID)
        {
            if (self.UIEventHandlers.TryGetValue(windowID, out IAUIEventHandler handler))
            {
                return handler;
            }
            Log.Error($"windowId : {windowID} is not have any uiEvent");
            return null;
        }

    }
}