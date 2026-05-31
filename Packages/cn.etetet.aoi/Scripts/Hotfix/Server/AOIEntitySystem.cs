using System.Collections.Generic;
using Unity.Mathematics;

namespace ET.Server
{
    [EntitySystemOf(typeof(AOIEntity))]
    public static partial class AOIEntitySystem2
    {
        [EntitySystem]
        private static void Awake(this AOIEntity self)
        {
            Unit unit = self.GetParent<Unit>();
            NumericComponent numericComponent = unit.NumericComponent;
            self.Phase = (PhaseType)numericComponent.Get(NumericType.Phase);
            self.NumericComponent = numericComponent;
            self.Scene().GetComponent<AOIManagerComponent>().Add(self, unit.Position.x, unit.Position.z);
        }

        [EntitySystem]
        private static void Destroy(this AOIEntity self)
        {
            Scene scene = self.Scene();
            if (scene.IsDisposed)
            {
                return;
            }
            scene.GetComponent<AOIManagerComponent>()?.Remove(self);
        }
    }

    public static partial class AOIEntitySystem
    {
        // 获取在自己视野中的对象
        public static Dictionary<long, EntityRef<AOIEntity>> GetSeeUnits(this AOIEntity self)
        {
            return self.SeeUnits;
        }

        public static Dictionary<long, EntityRef<AOIEntity>> GetBeSeePlayers(this AOIEntity self)
        {
            return self.BeSeePlayers;
        }

        public static Dictionary<long, EntityRef<AOIEntity>> GetSeePlayers(this AOIEntity self)
        {
            return self.SeePlayers;
        }

        // cell中的unit进入self的视野
        public static void SubEnter(this AOIEntity self, Cell cell)
        {
            cell.SubsEnterEntities.Add(self.Id, self);
            foreach (KeyValuePair<long, EntityRef<AOIEntity>> kv in cell.AOIUnits)
            {
                if (kv.Key == self.Id)
                {
                    continue;
                }

                self.EnterSight(kv.Value);
            }
        }

        public static void UnSubEnter(this AOIEntity self, Cell cell)
        {
            cell.SubsEnterEntities.Remove(self.Id);
        }

        public static void SubLeave(this AOIEntity self, Cell cell)
        {
            cell.SubsLeaveEntities.Add(self.Id, self);
        }

        // cell中的unit离开self的视野
        public static void UnSubLeave(this AOIEntity self, Cell cell)
        {
            foreach (KeyValuePair<long, EntityRef<AOIEntity>> kv in cell.AOIUnits)
            {
                if (kv.Key == self.Id)
                {
                    continue;
                }

                self.LeaveSight(kv.Value);
            }

            cell.SubsLeaveEntities.Remove(self.Id);
        }
        
        public static bool SamePhase(this AOIEntity a, AOIEntity b)
        {
            return (a.Phase & b.Phase) != 0;
        }

        // enter进入self视野
        public static void EnterSight(this AOIEntity self, AOIEntity enter)
        {
            if (!self.SamePhase(enter))
            {
                return;
            }
            
            // 有可能之前在Enter，后来出了Enter还在LeaveCell，这样仍然没有删除，继续进来Enter，这种情况不需要处理
            if (self.SeeUnits.ContainsKey(enter.Id))
            {
                return;
            }
            
            if (!AOISeeCheckHelper.IsCanSee(self, enter))
            {
                return;
            }

            if (self.Unit.UnitType == UnitType.Player)
            {
                if (enter.Unit.UnitType == UnitType.Player)
                {
                    self.SeeUnits.Add(enter.Id, enter);
                    enter.BeSeeUnits.Add(self.Id, self);
                    self.SeePlayers.Add(enter.Id, enter);
                    enter.BeSeePlayers.Add(self.Id, self);
                    
                }
                else
                {
                    self.SeeUnits.Add(enter.Id, enter);
                    enter.BeSeeUnits.Add(self.Id, self);
                    enter.BeSeePlayers.Add(self.Id, self);
                }
            }
            else
            {
                if (enter.Unit.UnitType == UnitType.Player)
                {
                    self.SeeUnits.Add(enter.Id, enter);
                    enter.BeSeeUnits.Add(self.Id, self);
                    self.SeePlayers.Add(enter.Id, enter);
                }
                else
                {
                    self.SeeUnits.Add(enter.Id, enter);
                    enter.BeSeeUnits.Add(self.Id, self);
                }
            }
            EventSystem.Instance.Publish(self.Scene(), new UnitEnterSightRange() { A = self.Unit, B = enter.Unit });
        }

        // leave离开self视野
        public static void LeaveSight(this AOIEntity self, AOIEntity leave)
        {
            if (self.Id == leave.Id)
            {
                return;
            }

            if (!self.SeeUnits.Remove(leave.Id))
            {
                return;
            }

            if (leave.Unit.UnitType == UnitType.Player)
            {
                self.SeePlayers.Remove(leave.Id);
            }

            leave.BeSeeUnits.Remove(self.Id);
            if (self.Unit.UnitType == UnitType.Player)
            {
                leave.BeSeePlayers.Remove(self.Id);
            }

            // 仅当观察者 self 仍有效才发"离开视野"事件(通知其客户端移除 leave)。
            // self 已销毁(被移除单位作为 self 走 UnSubLeave→LeaveSight 路径)时跳过:无人可通知,
            // 且 A=self.Unit 会对"已销毁(InstanceId=0)的 self"建 EntityRef<Unit> 而抛 disposed。
            Unit selfUnit = self.Unit;
            if (selfUnit != null && !selfUnit.IsDisposed)
            {
                EventSystem.Instance.Publish(self.Scene(), new UnitLeaveSightRange { A = selfUnit, BId = leave.Unit.Id });
            }
        }

        /// <summary>
        /// 是否在Unit视野范围内
        /// </summary>
        /// <param name="self"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public static bool IsBeSee(this AOIEntity self, long unitId)
        {
            return self.BeSeePlayers.ContainsKey(unitId);
        }
    }
}