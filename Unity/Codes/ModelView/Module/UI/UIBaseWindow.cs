using UnityEngine;

namespace ET
{
    public class UIBaseWindow : Entity
    {
        public bool IsPreLoad
        {
            get
            {
                return this.m_uiPrefabGameObject != null ;
            }
        }
        
        public Transform uiTransform
        {
            get
            {
                if (null != this.m_uiPrefabGameObject)
                {
                    return m_uiPrefabGameObject.transform;
                }
                return null;
            }
        }
        
        public WindowID WindowID
        {
            get
            {
                if (this.m_windowID == WindowID.WindowID_Invaild)
                {
                    Debug.LogError("window id is " + WindowID.WindowID_Invaild);
                }
                return m_windowID;
            }
            set { m_windowID = value; }
        }
        public WindowID PreWindowID
        {
            get { return m_preWindowID; }
            set { m_preWindowID = value; }
        }

        
        public WindowID m_preWindowID = WindowID.WindowID_Invaild;
        public WindowID m_windowID = WindowID.WindowID_Invaild;
        
        public GameObject m_uiPrefabGameObject = null;
        public WindowCoreData WindowData = null;
    }
}