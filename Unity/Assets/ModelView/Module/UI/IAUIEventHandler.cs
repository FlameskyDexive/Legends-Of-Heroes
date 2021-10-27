namespace ET
{
    public interface IAUIEventHandler
    {
        void OnInitWindowCoreData(UIBaseWindow uiBaseWindow);
        void OnInitComponent(UIBaseWindow uiBaseWindow);
    

        void OnRegisterUIEvent(UIBaseWindow uiBaseWindow);

        void OnShowWindow(UIBaseWindow uiBaseWindow, Entity contextData = null);
        
        void OnHideWindow(UIBaseWindow uiBaseWindow);

        
        void BeforeUnload(UIBaseWindow uiBaseWindow);
    }
}