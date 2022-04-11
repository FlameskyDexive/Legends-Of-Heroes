using Sirenix.OdinInspector;

namespace XPlay.Runtime
{
    [HideLabel]
    [HideReferenceObjectPicker]
    public class XPlayEventData: XPlayActionData
    {
        [LabelText("将要发送的的事件")]
        [BoxGroup("自定义数据")]
        public string EventInfo;
        [LabelText("产生的战斗行为树事件ID")]
        public uint eventId;


    }
}