using UnityEngine;

namespace ET.Client
{
    [ChildOf(typeof(UIComponent))]
    public class UIBaseWindow : Entity, IAwake, IDestroy
    {
        public bool IsPreLoad => this.UIPrefabGameObject != null;

        public Transform uiTransform => this.UIPrefabGameObject != null ? this.UIPrefabGameObject.transform : null;

        public WindowID WindowID
        {
            get
            {
                if (this.m_windowID == WindowID.WindowID_Invaild)
                {
                    Log.Error("window id is " + WindowID.WindowID_Invaild);
                }
                return this.m_windowID;
            }
            set => this.m_windowID = value;
        }

        public bool IsInStackQueue { get; set; }

        public WindowID m_windowID = WindowID.WindowID_Invaild;
        public GameObject UIPrefabGameObject = null;
        public UIWindowType windowType = UIWindowType.Normal;
    }
}
