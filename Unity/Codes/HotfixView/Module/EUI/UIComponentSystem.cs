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
            self.Awake();
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
        public static void Awake(this UIComponent self)
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
                Log.Warning($"检测关闭 WindowsID: {id} 失败！");
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
        public static void UnLoadWindow(this UIComponent self,WindowID id,bool isDispose = true)
        {
            UIBaseWindow baseWindow = self.GetUIBaseWindow(id);
            if (null == baseWindow)
            {
              Log.Error($"UIBaseWindow WindowId {id} is null!!!");
              return;
            }
            UIEventComponent.Instance.GetUIEventHandler(id).BeforeUnload(baseWindow);
            if(baseWindow.IsPreLoad)
            {
                Game.Scene.GetComponent<ResourcesComponent>()?.UnloadBundle(baseWindow.UIPrefabGameObject.name.StringToAB());
                UnityEngine.Object.Destroy( baseWindow.UIPrefabGameObject);
                baseWindow.UIPrefabGameObject = null;
            }
            if (isDispose)
            {
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
            UIBaseWindow baseWindow = self.GetUIBaseWindow(id);
            // 如果UI不存在开始实例化新的窗口
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

        private static async ETTask<UIBaseWindow> ShowBaseWindowAsync(this UIComponent self,WindowID id, ShowWindowData showData = null)
        {
            UIBaseWindow baseWindow = self.GetUIBaseWindow(id);
            if (null == baseWindow)
            {
                if (UIPathComponent.Instance.WindowPrefabPath.ContainsKey((int)id))
                {
                    baseWindow          = self.AddChild<UIBaseWindow>();
                    baseWindow.WindowID = id;
                    await self.LoadBaseWindowsAsync(baseWindow);
                }
            }
            if (!baseWindow.IsPreLoad)
            {
                await self.LoadBaseWindowsAsync(baseWindow);
            }
            return baseWindow;
        }
        
        public static void Destroy(this UIComponent self)
        {
            self.ClearAllWindow();
        }

        private static UIBaseWindow GetUIBaseWindow(this UIComponent self,WindowID id)
        {
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
            if ( UIPathComponent.Instance.WindowTypeIdDict.TryGetValue(typeof(T).Name,out int windowsId) )
            {
                return (WindowID)windowsId;
            }
            Log.Error($"{typeof(T).FullName} is not have any windowId!" );
            return  WindowID.WindowID_Invaild;
        }
        
        public static void CloseWindow(this UIComponent self,WindowID windowId)
        {
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
            if (self.AllWindowsDic == null)
            {
                return;
            }
            foreach (KeyValuePair<int, UIBaseWindow> window in self.AllWindowsDic)
            {
                UIBaseWindow baseWindow = window.Value;
                if (baseWindow == null|| baseWindow.IsDisposed)
                {
                    continue;
                }
                self.HideWindow(baseWindow.WindowID);
                self.UnLoadWindow(baseWindow.WindowID,false);
                baseWindow?.Dispose();
            }
            self.AllWindowsDic.Clear();
            self.VisibleWindowsDic.Clear();
            self.LoadingWindows.Clear();
            self.VisibleWindowsQueue.Clear();
            self.HideWindowsStack.Clear();
            self.UIBaseWindowlistCached.Clear();
        }
        
        public static void HideAllShownWindow(this UIComponent self,bool includeFixed = false)
        {
            self.UIBaseWindowlistCached.Clear();
            foreach (KeyValuePair<int, UIBaseWindow> window in self.VisibleWindowsDic)
            {
                if (window.Value.WindowData.windowType == UIWindowType.Fixed && !includeFixed)
                    continue;
                if (window.Value.IsDisposed)
                {
                    continue;
                }
                
                self.UIBaseWindowlistCached.Add((WindowID)window.Key);
                window.Value.UIPrefabGameObject?.SetActive(false);
                UIEventComponent.Instance.GetUIEventHandler(window.Value.WindowID).OnHideWindow(window.Value);
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
            baseWindow.UIPrefabGameObject?.SetActive(true);
            UIEventComponent.Instance.GetUIEventHandler(id).OnShowWindow(baseWindow,contextData);
            
            self.VisibleWindowsDic[(int)id] = baseWindow;
            if (preWindowID != WindowID.WindowID_Invaild)
            {
                self.HideWindowsStack.Push(preWindowID);
            }
         
            Debug.Log("<color=magenta>### current Navigation window </color>" + baseWindow.WindowID.ToString());
        }
        
        private static bool CheckDirectlyHide(this UIComponent self,WindowID id)
        {
            if (!self.VisibleWindowsDic.ContainsKey((int)id))
            {
                return false;
            }

            UIBaseWindow baseWindow = self.VisibleWindowsDic[(int)id];
            if (baseWindow != null && !baseWindow.IsDisposed )
            {
                baseWindow.UIPrefabGameObject?.SetActive(false);
                UIEventComponent.Instance.GetUIEventHandler(id).OnHideWindow(baseWindow);
            }
            self.VisibleWindowsDic.Remove((int)id);
            self.VisibleWindowsQueue.Remove(id);
            return true;
        }
        
        /// <summary>
        /// 同步加载
        /// </summary>
        private static void LoadBaseWindows(this UIComponent self,  UIBaseWindow baseWindow)
        {
            if ( !UIPathComponent.Instance.WindowPrefabPath.TryGetValue((int)baseWindow.WindowID,out string value) )
            {
                Log.Error($"{baseWindow.WindowID} uiPath is not Exist!");
                return;
            }
            ResourcesComponent.Instance.LoadBundle(value.StringToAB());
            GameObject go                      = ResourcesComponent.Instance.GetAsset(value.StringToAB(), value ) as GameObject;
            baseWindow.UIPrefabGameObject      = UnityEngine.Object.Instantiate(go);
            baseWindow.UIPrefabGameObject.name = go.name;
            
            baseWindow?.SetRoot(EUIRootHelper.GetTargetRoot(baseWindow.WindowData.windowType));
            baseWindow.uiTransform.SetAsLastSibling();
            
            UIEventComponent.Instance.GetUIEventHandler(baseWindow.WindowID).OnInitWindowCoreData(baseWindow);
            UIEventComponent.Instance.GetUIEventHandler(baseWindow.WindowID).OnInitComponent(baseWindow);
            UIEventComponent.Instance.GetUIEventHandler(baseWindow.WindowID).OnRegisterUIEvent(baseWindow);
            
            self.AllWindowsDic[(int)baseWindow.WindowID] = baseWindow;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        private static async ETTask LoadBaseWindowsAsync(this UIComponent self,  UIBaseWindow baseWindow)
        {
            
            if ( !UIPathComponent.Instance.WindowPrefabPath.TryGetValue((int)baseWindow.WindowID,out string value) )
            {
                Log.Error($"{baseWindow.WindowID} is not Exist!");
                return;
            }
            self.LoadingWindows.Add(baseWindow.WindowID);
            await ResourcesComponent.Instance.LoadBundleAsync(value.StringToAB());
            GameObject go                      = ResourcesComponent.Instance.GetAsset(value.StringToAB(), value ) as GameObject;
            baseWindow.UIPrefabGameObject      = UnityEngine.Object.Instantiate(go);
            baseWindow.UIPrefabGameObject.name = go.name;
            
            baseWindow?.SetRoot(EUIRootHelper.GetTargetRoot(baseWindow.WindowData.windowType));
            baseWindow.uiTransform.SetAsLastSibling();
            
            UIEventComponent.Instance.GetUIEventHandler(baseWindow.WindowID).OnInitWindowCoreData(baseWindow);
            UIEventComponent.Instance.GetUIEventHandler(baseWindow.WindowID).OnInitComponent(baseWindow);
            UIEventComponent.Instance.GetUIEventHandler(baseWindow.WindowID).OnRegisterUIEvent(baseWindow);
            
            self.AllWindowsDic[(int)baseWindow.WindowID] = baseWindow;
            self.LoadingWindows.Remove(baseWindow.WindowID);
        }
       

    }
}