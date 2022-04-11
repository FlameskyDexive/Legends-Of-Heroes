using Sirenix.OdinInspector;
using UnityEngine;

namespace XPlay.Runtime
{
    [HideLabel]
    [HideReferenceObjectPicker]
    public class XPlayPlayAnimationData : XPlayActionData
    {
        [LabelText("动画名字")]
        // [BoxGroup("自定义数据")]
        public string animName;
        [LabelText("动画速度(放大十倍)")]
        [Range(1, 100)]
        public uint speed = 10;
        [LabelText("从动画的第x比例开始播放")]
        [Range(0, 1)]
        public float normalizedTime = 0;
        [LabelText("到动画的第x比例结束播放")]
        [Range(0, 1)]
        public float normalizedEndTime = 1;
    }
}