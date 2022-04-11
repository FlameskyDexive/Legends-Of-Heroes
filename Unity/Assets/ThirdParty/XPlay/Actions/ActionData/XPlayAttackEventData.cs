using Sirenix.OdinInspector;

namespace XPlay.Runtime
{
    [HideLabel]
    [HideReferenceObjectPicker]
    public class XPlayAttackEventData : XPlayActionData
    {
        [LabelText("攻击事件")]
        [BoxGroup("自定义数据")]
        public string attackInfo;


        [LabelText("发射器ID")]
        public uint emitterId;

    }
}