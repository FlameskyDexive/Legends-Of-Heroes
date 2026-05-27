using UnityEngine;

namespace ET.Client
{
    public static class EUIRootHelper
    {
        public static Transform GetTargetRoot(Scene root, UIWindowType type)
        {
            EUIRootComponent rootComp = root.GetComponent<EUIRootComponent>();
            if (rootComp == null)
            {
                Log.Error("EUIRootComponent is not attached to root scene.");
                return null;
            }

            switch (type)
            {
                case UIWindowType.Normal:
                    return rootComp.NormalRoot;
                case UIWindowType.Fixed:
                    return rootComp.FixedRoot;
                case UIWindowType.PopUp:
                    return rootComp.PopUpRoot;
                case UIWindowType.Other:
                    return rootComp.OtherRoot;
            }

            Log.Error("uiroot type is error: " + type);
            return null;
        }
    }
}
