using UnityEngine;

namespace XPlay.Runtime
{
    public class XPlayEvent: XPlayAction<XPlayEventData>
    {
        public override void OnEnter(uint currentFrame)
        {
            base.OnEnter(currentFrame);
            Debug.LogError($"Event抛出一个事件：{this.BindingData.EventInfo} 帧数：{currentFrame}");
        }
    }
}