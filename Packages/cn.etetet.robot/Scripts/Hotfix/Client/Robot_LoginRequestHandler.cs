namespace ET.Client
{
    [MessageHandler(SceneType.Client)]
    public class Robot_LoginRequestHandler: MessageHandler<Scene, Robot_LoginRequest, Robot_LoginResponse>
    {
        protected override async ETTask Run(Scene root, Robot_LoginRequest request, Robot_LoginResponse response)
        {
            EntityRef<Scene> rootRef = root;
            await LoginHelper.Login(root, request.Address, request.Account, request.Password);
            root = rootRef;
            // 匹配机器人进入"球球大作战"专属地图(BallBattle Copy), 与真人在同一房间对战。
            // 当前机器人仅用于球球大作战匹配兜底, 故直接进 BallBattle; 后续若需多玩法机器人, 可让 Robot_LoginRequest 携带地图名。
            await EnterMapHelper.EnterMapAsync(root, "BallBattle");
        }
    }
}
