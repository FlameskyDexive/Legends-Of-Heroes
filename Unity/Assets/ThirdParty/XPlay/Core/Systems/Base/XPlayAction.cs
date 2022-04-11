using System.Collections.Generic;

namespace XPlay.Runtime
{
    public class XPlayAction<T> : XPlayIDirectable where T : XPlayDirectableData
    {
        public XPlayDirectableData DirectableData { get; set; }

        public T BindingData => DirectableData as T;

        /// <summary>
        /// 起始帧，为运行时计算的帧数
        /// </summary>
        public uint StartFrame { get; set; }

        /// <summary>
        /// 结束帧，为运行时计算的帧数
        /// </summary>
        public uint EndFrame { get; set; }

        public bool Initialize(uint currentFrame, XPlayDirectableData xPlayDirectableData)
        {
            this.DirectableData = xPlayDirectableData;
            StartFrame =
                currentFrame + XPlayTimeToFrameCaculator.CaculateFrameCountFromTimeLength(this.DirectableData.RelativelyStartTime);
            EndFrame =
                currentFrame + XPlayTimeToFrameCaculator.CaculateFrameCountFromTimeLength(this.DirectableData.RelativelyEndTime);
            return OnInitialize(currentFrame);
        }

        void XPlayIDirectable.Enter(uint currentFrame)
        {
            OnEnter(currentFrame);
        }

        void XPlayIDirectable.Update(uint currentFrame, uint previousFrame)
        {
            OnUpdate(currentFrame, previousFrame);
        }

        void XPlayIDirectable.Exit()
        {
            OnExit();
        }

        public virtual bool OnInitialize(uint currentFrame)
        {
            return true;
        }

        public virtual void OnEnter(uint currentFrame)
        {
        }

        public virtual void OnUpdate(uint currentFrame, uint previousFrame)
        {
        }

        public virtual void OnExit()
        {
        }
    }
}