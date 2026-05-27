using UnityEngine;

namespace ET.Client
{
    [EnableClass]
    public class UIFindHelper
    {
        public static Transform FindDeepChild(GameObject target, string childName)
        {
            Transform resultTrs = target.transform.Find(childName);
            if (resultTrs == null)
            {
                foreach (Transform trs in target.transform)
                {
                    resultTrs = FindDeepChild(trs.gameObject, childName);
                    if (resultTrs != null)
                    {
                        return resultTrs;
                    }
                }
            }
            return resultTrs;
        }

        public static T FindDeepChild<T>(GameObject target, string childName) where T : Component
        {
            Transform resultTrs = FindDeepChild(target, childName);
            if (resultTrs != null)
            {
                return resultTrs.gameObject.GetComponent<T>();
            }
            return null;
        }
    }
}
