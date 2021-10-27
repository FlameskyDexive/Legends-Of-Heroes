using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ET
{
    public enum WindowID
    {
        WindowID_Invaild = 0,
        WindowID_MessageBox,
        WindowID_Lobby,
        WindowID_Login
        
    }



    public static class UIGlobalDefine
    {
        public static Dictionary<int, string> WindowPrefabPath = new Dictionary<int, string>();
        
        public static Dictionary<string,int> WindowTypeIdDict = new Dictionary<string, int>();
    }
}