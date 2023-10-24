using System;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(GlobalComponent))]
    public static class EUIRootHelper
    {
        public static void Init()
        {
          
        }
        
        public static Transform GetTargetRoot(Scene root, UIWindowType type)
        {
            if (type == UIWindowType.Normal)
            {
                return root.GetComponent<GlobalComponent>().NormalRoot;
            }
            else if (type == UIWindowType.Fixed)
            {
                return root.GetComponent<GlobalComponent>().FixedRoot;
            }
            else if (type == UIWindowType.PopUp)
            {
                return root.GetComponent<GlobalComponent>().PopUpRoot;// GlobalComponent.Instance.PopUpRoot;
            }
            else if (type == UIWindowType.Other)
            {
                return root.GetComponent<GlobalComponent>().OtherRoot;
            }

            Log.Error("uiroot type is error: " + type.ToString());
            return null;
        }
    }
}