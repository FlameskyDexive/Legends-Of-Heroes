using System;
using System.Collections.Generic;

namespace ET
{

    [EntitySystemOf(typeof(BuffComponent))]
    [FriendOf(typeof(BuffComponent))]
    [FriendOf(typeof(BattleUnitComponent))]
    public static partial class BuffComponentSystem
    {

        [EntitySystem]
        private static void Awake(this BuffComponent self)
        {
            
        }
        [EntitySystem]
        private static void Destroy(this BuffComponent self)
        {
            foreach (var buff in self.BuffDic)
            {
                self.Remove(buff.Value.Id);
            }
            self.BuffDic.Clear();
        }
        private static void AddBuffS(this BuffComponent self, List<int> buffIds)
        {
            foreach (int buffId in buffIds)
            {
                self.AddBuff(buffId);
            }
        }
        
        
        /// <summary>
        /// 添加buff
        /// </summary>
        public static bool AddBuff(this BuffComponent self, int buffId = 0)
        {
            Buff buff = null;

            if (!self.BuffDic.TryGetValue(buffId, out buff))
            {
                buff = self.AddChild<Buff, int>(buffId);
                buff.LayerCount = 1;
            }
            self.BuffDic[buffId] = buff;
            return true;
        }
        public static bool RemoveBuff(this BuffComponent self, int buffId = 0)
        {

            if (!self.BuffDic.TryGetValue(buffId, out Buff buff))
            {

                return false;
            }
            self.BuffDic.Remove(buffId);
            self.Remove(buff.Id);
            return true;
        }
        public static void Remove(this BuffComponent self, long id)
        {
            Buff buff = self.GetChild<Buff>(id);
            buff?.Dispose();
        }
    }

   
}