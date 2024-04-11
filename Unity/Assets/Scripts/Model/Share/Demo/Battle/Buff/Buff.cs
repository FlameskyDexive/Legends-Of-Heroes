using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{
    public enum EBuffType
    {
        /// <summary>
        /// 纯标记
        /// </summary>
        OnlySign = 0,
        ChangeNumeric,
    }

    public enum EBuffAddType
    {
        RefreshTime,
        CantOverlay,
        OverlayAddLayerRefreshTime,
        OnlyAddTime,
    }
    
    [ChildOf(typeof(BuffComponent))]
    public class Buff:Entity,IAwake<int>, IFixedUpdate, IDestroy,ITransfer
    {
        [BsonIgnore]
        public Unit Unit => this.GetParent<Unit>();

        public long LifeTimer;
        public long IntervalTimer;
        
        /// <summary>
        /// buff表id
        /// </summary>
        public int BuffId;

        public long StartTime { get; set; }
        
        public long NextTriggerTime { get; set; }
        
        
        public uint LayerCount { get; set; }

        private BuffConfig buffConfig;
        public BuffConfig BuffConfig
        {
            get
            {
                if (this.buffConfig == null)
                    this.buffConfig = BuffConfigCategory.Instance.Get(this.BuffId);
                return this.buffConfig;
            }
        }


    }
}