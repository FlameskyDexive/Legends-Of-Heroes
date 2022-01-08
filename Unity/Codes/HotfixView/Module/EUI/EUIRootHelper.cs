using System;
using UnityEngine;

namespace ET
{
    public class EUIRootHelper
    {
        public static void Init()
        {
          
        }
        
        public static Transform GetTargetRoot(UIWindowType type)
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
    }
}