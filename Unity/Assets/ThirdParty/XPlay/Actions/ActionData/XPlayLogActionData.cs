using Sirenix.OdinInspector;

namespace XPlay.Runtime
{
    [HideLabel]
    [HideReferenceObjectPicker]
    public class XPlayLogActionData : XPlayActionData
    {
        [LabelText("将要打印的信息")]
        [BoxGroup("自定义数据")]
        public string LogInfo;
    }
}