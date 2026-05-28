using System;
using System.Collections.Generic;
namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class SkillComponent:Entity,IAwake,IAwake<List<int>>,IDestroy,ITransfer
    {
        public Unit Unit => this.GetParent<Unit>();
        
        public Dictionary<int, long> IdSkillMap = new Dictionary<int, long>();

        public Dictionary<ESkillAbstractType, List<long>> AbstractTypeSkills = new Dictionary<ESkillAbstractType, List<long>>();


        public Dictionary<int, long> SkillDic = new Dictionary<int, long>();

    }
}