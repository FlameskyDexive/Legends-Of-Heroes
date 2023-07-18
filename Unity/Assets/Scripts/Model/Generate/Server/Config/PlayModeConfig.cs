using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{
    [Config]
    public partial class PlayModeConfigCategory : ConfigSingleton<PlayModeConfigCategory>, IMerge
    {
        [BsonElement]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        private Dictionary<int, PlayModeConfig> dict = new Dictionary<int, PlayModeConfig>();
		
        public void Merge(object o)
        {
            PlayModeConfigCategory s = o as PlayModeConfigCategory;
            foreach (var kv in s.dict)
            {
                this.dict.Add(kv.Key, kv.Value);
            }
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

	public partial class PlayModeConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		public int Id { get; set; }
		/// <summary>Type</summary>
		public int PlayModeType { get; set; }
		/// <summary>名字</summary>
		public string Name { get; set; }
		/// <summary>房间最大玩家数量</summary>
		public int RoomMaxPlayer { get; set; }

	}
}
