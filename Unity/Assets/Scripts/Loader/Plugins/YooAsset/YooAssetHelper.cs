using System.Collections;
using UnityEngine;
using YooAsset;

namespace ET
{
    public static class YooAssetHelper
    {
        public static string GetCdnUrl()
        {
            //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
            string hostServerIP = "http://127.0.0.1";
            string gameVersion = "v1.0";

#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return $"{hostServerIP}/CDN/Android/{gameVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return $"{hostServerIP}/CDN/IPhone/{gameVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                return $"{hostServerIP}/CDN/WebGL/{gameVersion}";
            else
                return $"{hostServerIP}/CDN/PC/{gameVersion}";
#else
			if (Application.platform == RuntimePlatform.Android)
				return $"{hostServerIP}/CDN/Android/{gameVersion}";
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
				return $"{hostServerIP}/CDN/IPhone/{gameVersion}";
			else if (Application.platform == RuntimePlatform.WebGLPlayer)
				return $"{hostServerIP}/CDN/WebGL/{gameVersion}";
			else
				return $"{hostServerIP}/CDN/PC/{gameVersion}";
#endif
        }
        
        public static ETTask GetAwaiter(this AsyncOperationBase asyncOperationBase)
        {
            ETTask task = ETTask.Create(true);
            asyncOperationBase.Completed += _ => { task.SetResult(); };
            return task;
        }
            
        public static ETTask GetAwaiter(this AssetOperationHandle assetOperationHandle)
        {
            ETTask task = ETTask.Create(true);
            assetOperationHandle.Completed += _ => { task.SetResult(); };
            return task;
        }
            
        public static ETTask GetAwaiter(this SubAssetsOperationHandle subAssetsOperationHandle)
        {
            ETTask task = ETTask.Create(true);
            subAssetsOperationHandle.Completed += _ => { task.SetResult(); };
            return task;
        }
            
        public static ETTask GetAwaiter(this SceneOperationHandle sceneOperationHandle)
        {
            ETTask task = ETTask.Create(true);
            sceneOperationHandle.Completed += _ => { task.SetResult(); };
            return task;
        }
        
        public static ETTask GetAwaiter(this RawFileOperationHandle assetOperationHandle)
        {
            ETTask task = ETTask.Create(true);
            assetOperationHandle.Completed += _ => { task.SetResult(); };
            return task;
        }
    }
}
