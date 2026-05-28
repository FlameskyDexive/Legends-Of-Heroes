using System;
using System.Collections.Generic;
namespace ET
{
    namespace EventType
    {
        public struct ActionEventData
        {
            public EActionEventType actionEventType;
            public Unit owner;
            public Unit target;

        }
    }
    
    [ComponentOf(typeof(Unit))]
    public class SkillComponent:Entity,IAwake,IAwake<List<int>>,IDestroy,ITransfer
    {
        public Unit Unit => this.GetParent<Unit>();
        
        public Dictionary<int, long> IdSkillMap = new Dictionary<int, long>();

        public Dictionary<ESkillAbstractType, List<long>> AbstractTypeSkills = new Dictionary<ESkillAbstractType, List<long>>();


        public Dictionary<int, long> SkillDic = new Dictionary<int, long>();

    }
}