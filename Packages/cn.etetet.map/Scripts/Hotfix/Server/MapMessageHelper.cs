using System.Collections.Generic;

namespace ET.Server
{
    public static partial class MapMessageHelper
    {
        private static void Broadcast(Unit unit, IMessage message, bool withSelf = true)
        {
            if (message is IPool)
            {
                throw new System.Exception($"broadcast message can not be IPool: {message.GetType().FullName}");
            }

            Dictionary<long, EntityRef<AOIEntity>> dict = unit.GetBeSeePlayers();
            MessageSender messageSender = unit.Root().GetComponent<MessageSender>();
            
            foreach (AOIEntity u in dict.Values)
            {
                if (!withSelf && u.Id == unit.Id)
                {
                    continue;
                }
                // 玩家型但无客户端(如球球大作战 AI 机器人 = 玩家球但没有 gate)→ 跳过,避免 UnitGateInfoComponent 为空时空引用
                UnitGateInfoComponent gateInfo = u.Unit.GetComponent<UnitGateInfoComponent>();
                if (gateInfo == null)
                {
                    continue;
                }
                messageSender.Send(gateInfo.ActorId, message);
            }
        }

        private static void SendToClient(Unit unit, IMessage message)
        {
            if (!unit.UnitType.IsSame(UnitType.Player))
            {
                return;
            }
            // 玩家型但无客户端(AI 机器人)→ 没有 UnitGateInfoComponent, 跳过避免空引用
            UnitGateInfoComponent gateInfo = unit.GetComponent<UnitGateInfoComponent>();
            if (gateInfo == null)
            {
                return;
            }
            MessageSender messageSender = unit.Root().GetComponent<MessageSender>();
            messageSender.Send(gateInfo.ActorId, message);
        }
        
        public static void NoticeClient(Unit unit, IMessage message, NoticeType noticeType)
        {
            switch (noticeType)
            {
                case NoticeType.Broadcast:
                    Broadcast(unit, message);
                    break;
                case NoticeType.Self:
                    SendToClient(unit, message);
                    break;
                case NoticeType.NoNotice:
                    break;
                case NoticeType.BroadcastWithoutSelf:
                    Broadcast(unit, message, false);
                    break;
            }
        }
    }
}
