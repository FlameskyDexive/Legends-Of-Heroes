using UnityEngine.SceneManagement;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class SceneChangeFinishEvent : AEvent<Scene, EventType.SceneChangeFinish>
    {
        protected override async ETTask Run(Scene scene, EventType.SceneChangeFinish args)
        {
            // Scene currentScene = scene.CurrentScene();
            scene.GetComponent<CameraComponent>().Init(args.myUnit);

            await ETTask.CompletedTask;
        }
    }
}