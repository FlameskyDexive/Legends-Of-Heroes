using System;
using System.IO;

namespace ET.Client
{
    [Event(SceneType.Process)]
    public class EntryEvent3_InitClient: AEvent<ET.EventType.EntryEvent3>
    {
        protected override async ETTask Run(Scene scene, ET.EventType.EntryEvent3 args)
        {
            // 加载配置
            // Root.Instance.Scene.AddComponent<ResourcesComponent>();
            
            Root.Instance.Scene.AddComponent<GlobalComponent>();
            Root.Instance.Scene.AddComponent<FsmDispatcherComponent>();


            // await ResourcesComponent.Instance.LoadBundleAsync("unit.unity3d");
            
            Scene clientScene = await SceneFactory.CreateClientScene(1, "Game");
            
            //测试加载多key表格数据
            SkillLevelConfig skillLevel = SkillLevelConfigCategory.Instance.GetByKeys(1001, 2);
            Log.Info($"测试加载多key, skillId {1001}, level{2}: {skillLevel.Name}");
            
            // 热更流程
            await ResComponent.Instance.InitResourceAsync(clientScene);

            await EventSystem.Instance.PublishAsync(clientScene, new EventType.AppStartInitFinish());
        }
    }
}