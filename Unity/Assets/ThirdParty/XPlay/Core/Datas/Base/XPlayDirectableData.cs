using Sirenix.OdinInspector;
using UnityEngine;

namespace XPlay.Runtime
{
    public class XPlayDirectableData
    {
        /// <summary>
        /// 起始时间点，为当前CutScene的相对时间点，例如600，即表示其起始时间点在这个CutScene运行600ms的时候
        /// </summary>
        [BoxGroup("基础数据")]
        [LabelText("起始时间点")]
        [Tooltip("为当前CutScene的相对时间点，例如600，即表示其起始时间点在这个CutScene运行600ms的时候")]
        [DisableInEditorMode]
        public long RelativelyStartTime;
        
        /// <summary>
        /// 时间长度，为当前CutScene的相对时间点，例如1200，即表示其结束时间点在这个CutScene运行1200ms的时候
        /// </summary>
        [BoxGroup("基础数据")]
        [LabelText("结束时间点")]
        [Tooltip("为当前CutScene的相对时间点，例如1200，即表示其结束时间点在这个CutScene运行1200ms的时候")]
        [DisableInEditorMode]
        public long RelativelyEndTime;

        public string trackName = "";

        public bool isActive = true;
    }
}