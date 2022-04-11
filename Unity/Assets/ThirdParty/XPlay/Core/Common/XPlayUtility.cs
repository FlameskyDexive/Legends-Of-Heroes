/****************************************************
    Author:		Flamesky
    Mail:		flamesky@live.com
    Date:		2022/3/2 15:12:35
    Function:	Nothing
*****************************************************/

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XPlay
{
    public static class XPlayUtility
    {
        public static AnimationClip LoadAnimationClip(string path)
        {
            // AnimationClip clip = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            // (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/texture.jpg");
            return clip;
        }
        public static GameObject LoadPrefabAndCreate(string path)
        {
            GameObject go = null;
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            if (obj != null)
            {
                go = GameObject.Instantiate(obj) as GameObject;
            }

            return go;
        }

        public static void CreatePrefab(GameObject obj, string localPath)
        {
            //Create a new Prefab at the path given
            PrefabUtility.SaveAsPrefabAssetAndConnect(obj, localPath, InteractionMode.AutomatedAction);
            //PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }
    }

    public enum ESkillSaveCode
    {
        Success = 0,
        ErrorNoneSelect = 1,
    }
    public enum ESkillAddCode
    {
        Success = 0,
        ErrorIllegalSkillId = 1,
        ErrorExistSkillId = 2,
    }
}

#endif
