using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace ET
{
    [Config]
    public partial class LanguageConfigCategory : Singleton<LanguageConfigCategory>, IMerge
    {
        [BsonElement]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        private Dictionary<int, LanguageConfig> dict = new();
		
        public void Merge(object o)
        {
            LanguageConfigCategory s = o as LanguageConfigCategory;
            foreach (var kv in s.dict)
            {
                this.dict.Add(kv.Key, kv.Value);
            }
        }
		
        public LanguageConfig Get(int id)
        {
            this.dict.TryGetValue(id, out LanguageConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (LanguageConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, LanguageConfig> GetAll()
        {
            return this.dict;
        }

        public LanguageConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

	public partial class LanguageConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		public int Id { get; set; }
		/// <summary>索引标识</summary>
		public string Key { get; set; }
		/// <summary>简体中文</summary>
		public string Chinese { get; set; }
		/// <summary>英文</summary>
		public string English { get; set; }

	}
}
