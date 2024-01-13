namespace ET.Server
{

    public static partial class StateSyncRoomMessageHelper
    {
        public static void BroadCast(StateSyncRoom room, IMessage message)
        {
            // 广播的消息不能被池回收
            (message as MessageObject).IsFromPool = false;

            StateSyncRoomServerComponent roomServerComponent = room.GetComponent<StateSyncRoomServerComponent>();

            MessageLocationSenderComponent messageLocationSenderComponent = room.Root().GetComponent<MessageLocationSenderComponent>();
            foreach (var kv in roomServerComponent.Children)
            {
                StateSyncRoomPlayer roomPlayer = kv.Value as StateSyncRoomPlayer;

                if (!roomPlayer.IsOnline)
                {
                    continue;
                }
                
                messageLocationSenderComponent.Get(LocationType.GateSession).Send(roomPlayer.Id, message);
            }
        }
    }
}