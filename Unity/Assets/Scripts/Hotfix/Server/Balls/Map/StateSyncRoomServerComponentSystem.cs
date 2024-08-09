using System;
using System.Collections.Generic;

namespace ET.Server
{
    [EntitySystemOf(typeof(StateSyncRoomServerComponent))]
    [FriendOf(typeof(StateSyncRoomServerComponent))]
    public static partial class StateSyncRoomServerComponentSystem
    {
        [EntitySystem]
        private static void Destroy(this StateSyncRoomServerComponent self)
        {
            
        }
        [EntitySystem]
        private static void Awake(this StateSyncRoomServerComponent self, List<long> playerIds)
        {
            foreach (long id in playerIds)
            {
                StateSyncRoomPlayer roomPlayer = self.AddChildWithId<StateSyncRoomPlayer>(id);
            }
        }
        
        [EntitySystem]
        private static void FixedUpdate(this StateSyncRoomServerComponent self)
        {
            self.UpdateRoomPlayer();
        }
        /// <summary>
        /// 暂定每帧同步角色位置/朝向信息
        /// </summary>
        /// <param name="self"></param>
        private static void UpdateRoomPlayer(this StateSyncRoomServerComponent self)
        {
            M2C_SyncUnitTransforms sync = M2C_SyncUnitTransforms.Create();

            foreach (StateSyncRoomPlayer roomPlayer in self.Children.Values)
            {
                if (roomPlayer.IsOnline)
                {
                    Unit unit = roomPlayer.Unit;
                    TransformInfo info = TransformInfo.Create();
                    info.Forward = unit.Forward;
                    info.Position = unit.Position;
                    sync.TransformInfos.Add(info);
                }
            }
            StateSyncRoomMessageHelper.BroadCast(self.GetParent<StateSyncRoom>(), sync);
        }

        public static bool IsAllPlayerProgress100(this StateSyncRoomServerComponent self)
        {
            foreach (StateSyncRoomPlayer roomPlayer in self.Children.Values)
            {
                if (roomPlayer.Progress != 100)
                {
                    return false;
                }
            }
            return true;
        }
    }
}