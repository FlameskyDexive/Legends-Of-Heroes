using System;
using UnityEngine;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class BattleUnitComponent:Entity,IAwake,IAwake<List<int>>,IDestroy,ITransfer
    {
        [BsonIgnore]
        public Unit unit => this.GetParent<Unit>();
        
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, long> IdSkillMap = new Dictionary<int, long>();
        
    }
}