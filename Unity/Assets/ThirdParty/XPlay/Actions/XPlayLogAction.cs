using UnityEngine;

namespace XPlay.Runtime
{
    public class XPlayLogAction: XPlayAction<XPlayLogActionData>
    {
        public override bool OnInitialize(uint currentFrame)
        {
            Debug.Log($"LogInfo Initialize");
            return true;
        }

        public override void OnEnter(uint currentFrame)
        {
            base.OnEnter(currentFrame);
            
            Debug.Log($"LogInfo Enter");
        }

        public override void OnUpdate(uint currentFrame, uint previousFrame)
        {
            Debug.Log($"LogInfo: {this.BindingData.LogInfo} 帧数：{currentFrame}");
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Debug.Log($"LogInfo Exit");
        }
    }
}