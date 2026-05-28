using System;
using System.Collections.Generic;
namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class BulletComponent:Entity,IAwake, IFixedUpdate,IDestroy
    {
        public Unit Unit => this.GetParent<Unit>();

        public EntityRef<Unit> OwnerUnit;
        public EntityRef<Skill> OwnerSkill;
        public long EndTime;

    }
}