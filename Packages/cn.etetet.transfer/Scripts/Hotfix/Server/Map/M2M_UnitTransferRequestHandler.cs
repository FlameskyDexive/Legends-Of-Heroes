using System;
using Unity.Mathematics;

namespace ET.Server
{
    [MessageHandler(SceneType.Map)]
    public class M2M_UnitTransferRequestHandler: MessageHandler<Scene, M2M_UnitTransferRequest, M2M_UnitTransferResponse>
    {
        protected override async ETTask Run(Scene scene, M2M_UnitTransferRequest request, M2M_UnitTransferResponse response)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();

            Unit unit = request.Unit;
            if (unit != null)  // 黑科技，直接传送Unit对象
            {
                unitComponent.AddChild(unit);
                unitComponent.Add(unit);
            }
            else
            {
                unit = MongoHelper.Deserialize<Unit>(request.UnitBytes);

                unitComponent.AddChild(unit);
                unitComponent.Add(unit);

                foreach (byte[] bytes in request.EntityBytes)
                {
                    Entity entity = MongoHelper.Deserialize<Entity>(bytes);
                    unit.AddComponent(entity);
                }
            }

            unit.AddComponent<TurnComponent>();
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<PathfindingComponent, string>(scene.Name.GetSceneConfigName());
            unit.AddComponent<MailBoxComponent, int>(MailBoxType.OrderedMessage);
            unit.AddComponent<TargetComponent>();

            // 通知客户端开始切场景
            M2C_StartSceneChange m2CStartSceneChange = M2C_StartSceneChange.Create();
            m2CStartSceneChange.SceneId = scene.Id;
            m2CStartSceneChange.SceneName = scene.Name;
            MapMessageHelper.NoticeClient(unit, m2CStartSceneChange, NoticeType.Self);

            if (request.ChangeScene)
            {
                // 通知客户端创建My Unit
                M2C_CreateMyUnit m2CCreateUnits = M2C_CreateMyUnit.Create();
                m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
                MapMessageHelper.NoticeClient(unit, m2CCreateUnits, NoticeType.Self);
            }

            // 加入aoi
            unit.AddComponent<AOIEntity>();

            // 通用扩展点:单位"转移进入本地图"也要通知高层玩法包(与 mapplay UnitFactory.Create 末尾的发布对称)。
            // 否则经 transfer 进入 Copy/Line 地图的单位(如球球大作战玩家 transfer 进 BallBattle Copy)收不到
            // AfterUnitCreateServer → ballbattle 的 AfterUnitCreateServer_SetupBall 不触发(玩家无技能/碰撞、竞技场不装配、食物不刷)。
            // 订阅者自行按地图名守卫(普通地图早退),且 SetupBall 以 BallComponent==null 判空保证幂等。
            EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreateServer { Unit = unit });

            response.NewActorId = unit.GetActorId();
            await ETTask.CompletedTask;
        }
    }
}