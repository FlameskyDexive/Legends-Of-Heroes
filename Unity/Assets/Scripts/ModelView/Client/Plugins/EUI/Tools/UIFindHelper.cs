using System;
using UnityEngine;


namespace ET.Client
{
    [EnableClass]
    public class UIFindHelper
    {
        /// <summary>
        /// 查找子节点
        /// </summary>
        /// <OtherParam name="_target"></OtherParam>
        /// <OtherParam name="_childName"></OtherParam>
        /// <returns></returns>
        public static Transform FindDeepChild(GameObject _target, string _childName)
        {
            Transform resultTrs = null;
            resultTrs = _target.transform.Find(_childName);
            if (resultTrs == null)
            {
                foreach (Transform trs in _target.transform)
                {
                    resultTrs = UIFindHelper.FindDeepChild(trs.gameObject, _childName);
                    if (resultTrs != null)
                        return resultTrs;
                }
            }
            return resultTrs;
        }

        /// <summary>
        /// 根据泛型查找子节点
        /// </summary>
        /// <param name="_target"></param>
        /// <param name="_childName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindDeepChild<T>(GameObject _target, string _childName) where T : Component
        {
            Transform resultTrs = UIFindHelper.FindDeepChild(_target, _childName);
            if (resultTrs != null)
                return resultTrs.gameObject.GetComponent<T>();
            return (T)((object)null);
        }
    }
}