namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask<int> Login(Scene root, string account, string password)
        {
            root.RemoveComponent<ClientSenderCompnent>();
            ClientSenderCompnent clientSenderCompnent = root.AddComponent<ClientSenderCompnent>();

            long playerId = await clientSenderCompnent.LoginAsync(account, password);

            root.GetComponent<PlayerComponent>().MyId = playerId;
            
            await EventSystem.Instance.PublishAsync(root, new EventType.LoginFinish());
            return ErrorCode.ERR_Success;
        }
    }
}