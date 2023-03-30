using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class UnitConfigCategory : ConfigSingleton<UnitConfigCategory>, IMerge
    {
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, UnitConfig> dict = new Dictionary<int, UnitConfig>();
		
        [BsonElement]
        [ProtoMember(1)]
        private List<UnitConfig> list = new List<UnitConfig>();
		
        public void Merge(object o)
        {
            UnitConfigCategory s = o as UnitConfigCategory;
            this.list.AddRange(s.list);
        }
		
		[ProtoAfterDeserialization]        
        public void ProtoEndInit()
        {
            foreach (UnitConfig config in list)
            {
                config.AfterEndInit();
                try
                {
                    this.dict.Add(config.Id, config);
                }
                catch (Exception e)
                {
                    //Log.Console($"{config.Id} error:{e}");
                    Log.Error($"UnitConfig重复key：{config.Id} error:{e}");
                }
                
            }
            
            
            this.list.Clear();
            
            this.AfterEndInit();
        }
		
        public UnitConfig Get(int id)
        {
            this.dict.TryGetValue(id, out UnitConfig item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (UnitConfig)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, UnitConfig> GetAll()
        {
            return this.dict;
        }

        public UnitConfig GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class UnitConfig: ProtoObject, IConfig
	{
		/// <summary>Id</summary>
		[ProtoMember(1)]
		public int Id { get; set; }
		/// <summary>Type</summary>
		[ProtoMember(2)]
		public int Type { get; set; }
		/// <summary>名字</summary>
		[ProtoMember(3)]
		public string Name { get; set; }
		/// <summary>位置</summary>
		[ProtoMember(4)]
		public int Position { get; set; }
		/// <summary>模型</summary>
		[ProtoMember(5)]
		public string ModelName { get; set; }
		/// <summary>模型</summary>
		[ProtoMember(7)]
		public string IconName { get; set; }
		/// <summary>初始携带技能</summary>
		[ProtoMember(8)]
		public int[] BornSkills { get; set; }

	}
}
