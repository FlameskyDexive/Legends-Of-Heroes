using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public static class TransformHelper
    {
        public static void SetParentCustom(this Transform self, Transform parent, float scale = 1)
        {
            self.parent = parent;
            self.localPosition = Vector3.zero;
            self.localScale = Vector3.one * scale;
            self.localRotation = Quaternion.identity;
        }
    }
}
