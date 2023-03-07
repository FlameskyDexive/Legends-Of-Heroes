using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class SkillLevelConfigCategory : ConfigSingleton<SkillLevelConfigCategory>, IMerge
    {
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<long, SkillLevelConfig> dict = new Dictionary<long, SkillLevelConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<SkillLevelConfig> list = new List<SkillLevelConfig>();
		
        public void Merge(object o)
        {
            SkillLevelConfigCategory s = o as SkillLevelConfigCategory;
            this.list.AddRange(s.list);
        }
		
		[ProtoAfterDeserialization]        
        public void ProtoEndInit()
        {
            foreach (SkillLevelConfig config in list)
            {
                config.AfterEndInit();
                try
                {
                    this.dict.Add(GetMultiKeyMerge(config.Id, config.Level), config);
                }
                catch (Exception e)
                {
                    //Log.Console($"{GetMultiKeyMerge(config.Id, config.Level)} error:{e}");
	                Log.Error($"数据异常，策划检查多个key是否相同。{config.Id}, {config.Level}, \n{e}");
                }
                
            }
            
            this.list.Clear();
            
            this.AfterEndInit();
        }

        private long GetMultiKeyMerge(int a = 0, int b = 0, int c = 0, int d = 0)
        {
	        //合并：高32位-中16位-中8位-低8位
	        return (long)a << 32 | ((long)b << 16) | ((long)c << 8) | (long)d;
        }

        public SkillLevelConfig GetByKeys(int key1 = 0, int key2 = 0, int key3 = 0, int key4 = 0)
        {
	        long key = GetMultiKeyMerge(key1, key2, key3, key4);
	        return Get(key);
        }
		
        private SkillLevelConfig Get(long id)
        {
            this.dict.TryGetValue(id, out SkillLevelConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (SkillLevelConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int key1 = 0, int key2 = 0, int key3 = 0, int key4 = 0)
        {
	        long key = GetMultiKeyMerge(key1, key2, key3, key4);
            return this.dict.ContainsKey(key);
        }

        public Dictionary<long, SkillLevelConfig> GetAll()
        {
            return this.dict;
        }

        public SkillLevelConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class SkillLevelConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>Type</summary>
		[ProtoMember(2)]
		public int Level { get; set; }
		/// <summary>名字</summary>
		[ProtoMember(3)]
		public string Name { get; set; }

	}
}
