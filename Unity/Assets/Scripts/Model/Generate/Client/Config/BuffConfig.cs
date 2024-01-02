using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{
    [Config]
    public partial class BuffConfigCategory : Singleton<BuffConfigCategory>, IMerge
    {
        [BsonElement]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        private Dictionary<int, BuffConfig> dict = new();
		
        public void Merge(object o)
        {
            BuffConfigCategory s = o as BuffConfigCategory;
            foreach (var kv in s.dict)
            {
                this.dict.Add(kv.Key, kv.Value);
            }
        }
		
        public BuffConfig Get(int id)
        {
            this.dict.TryGetValue(id, out BuffConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (BuffConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, BuffConfig> GetAll()
        {
            return this.dict;
        }

        public BuffConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

	public partial class BuffConfig: ProtoObject, IConfig
	{
		/// <summary>buffID</summary>
		public int Id { get; set; }
		/// <summary>效果类型</summary>
		public int BuffType { get; set; }
		/// <summary>效果参数</summary>
		public int NumericType { get; set; }
		/// <summary>效果值</summary>
		public int NumericValue { get; set; }
		/// <summary>持续时间</summary>
		public int Duration { get; set; }
		/// <summary>触发间隔</summary>
		public int Interval { get; set; }
		/// <summary>分组</summary>
		public int Goup { get; set; }
		/// <summary>特效挂点</summary>
		public string EffectRoot { get; set; }
		/// <summary>特效</summary>
		public string EffectRes { get; set; }
		/// <summary>特效缩放(100倍)</summary>
		public int EffectScale { get; set; }
		/// <summary>触发时的特效挂点</summary>
		public string TriggerEffectRoot { get; set; }
		/// <summary>触发时的特效</summary>
		public string TriggerEffectRes { get; set; }
		/// <summary>触发时的特效缩放</summary>
		public int TriggerEffectScale { get; set; }

	}
}
