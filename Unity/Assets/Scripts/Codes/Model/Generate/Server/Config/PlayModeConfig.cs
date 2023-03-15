using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class PlayModeConfigCategory : ConfigSingleton<PlayModeConfigCategory>, IMerge
    {
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, PlayModeConfig> dict = new Dictionary<int, PlayModeConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<PlayModeConfig> list = new List<PlayModeConfig>();
		
        public void Merge(object o)
        {
            PlayModeConfigCategory s = o as PlayModeConfigCategory;
            this.list.AddRange(s.list);
        }
		
		[ProtoAfterDeserialization]        
        public void ProtoEndInit()
        {
            foreach (PlayModeConfig config in list)
            {
                config.AfterEndInit();
                try
                {
                    this.dict.Add(config.Id, config);
                }
                catch (Exception e)
                {
                    //Log.Console($"{config.Id} error:{e}");
                    Log.Error($"PlayModeConfig重复key：{config.Id} error:{e}");
                }
                
            }
            
            
            this.list.Clear();
            
            this.AfterEndInit();
        }
		
        public PlayModeConfig Get(int id)
        {
            this.dict.TryGetValue(id, out PlayModeConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (PlayModeConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, PlayModeConfig> GetAll()
        {
            return this.dict;
        }

        public PlayModeConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class PlayModeConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>Type</summary>
		[ProtoMember(2)]
		public int PlayModeType { get; set; }
		/// <summary>名字</summary>
		[ProtoMember(3)]
		public string Name { get; set; }
		/// <summary>房间最大玩家数量</summary>
		[ProtoMember(5)]
		public int RoomMaxPlayer { get; set; }

	}
}
