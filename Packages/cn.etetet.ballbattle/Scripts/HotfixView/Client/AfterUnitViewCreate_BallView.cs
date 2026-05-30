using UnityEngine;
using UnityEngine.SceneManagement;

namespace ET.Client
{
    // 球球大作战:单位视图(GameObject)创建后的专属表现(订阅 mapplay 通用扩展事件 AfterUnitViewCreate)。
    // - 玩家球:按玩家 id 给"Sprite"子层(头像层)动态赋 Avatar{id%N};食物球保持预制体原样。
    // - 本地玩家:附加加载俯视竞技场场景(BallArena,迁自 LoH8) + 装配俯视正交相机(BallCameraComponent)。
    [Event(SceneType.Current)]
    public class AfterUnitViewCreate_BallView : AEvent<Scene, AfterUnitViewCreate>
    {
        // 竞技场场景资源(正交俯视),附加加载当背景。放在已被 yooasset 收集的 Scenes 目录,按全路径加载。
        private const string BallArenaScene = "Packages/cn.etetet.map/Bundles/Scenes/BallArena.unity";
        // 可用头像数量(Avatar0..Avatar11)
        private const int AvatarCount = 12;
        // 球预制体里头像层的节点名(LoH8 Player.prefab 结构:Player/Root/Sprite)
        private const string AvatarNodeName = "Sprite";

        protected override async ETTask Run(Scene scene, AfterUnitViewCreate args)
        {
            // 仅球球大作战地图生效,普通地图零影响
            if (scene.Name.GetSceneConfigName() != BallDefine.BallMapName)
            {
                return;
            }

            EntityRef<Scene> sceneRef = scene;
            Unit unit = args.Unit;
            if (unit == null)
            {
                return;
            }
            EntityRef<Unit> unitRef = unit;

            // 1) 玩家球:按 id 换头像(食物球保持预制体原样)
            if (unit.UnitType == UnitType.Player)
            {
                int idx = (int)(((unit.Id % AvatarCount) + AvatarCount) % AvatarCount);
                Sprite avatar = await scene.GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<Sprite>($"Avatar{idx}");

                scene = sceneRef;
                unit = unitRef;
                GameObject go = unit?.GetComponent<GameObjectComponent>()?.GameObject;
                if (avatar != null && go != null)
                {
                    SetAvatarSprite(go, avatar);
                }
            }

            // 2) 本地玩家:加载竞技场背景 + 俯视相机
            scene = sceneRef;
            unit = unitRef;
            if (unit == null)
            {
                return;
            }
            if (scene.Root().GetComponent<PlayerComponent>().MyId == unit.Id)
            {
                // 附加加载竞技场场景(Loader 内部按 location 去重,重复调用直接返回)。
                // 挂在 CurrentScene 的 ResourcesLoaderComponent 上 -> 离开地图(CurrentScene 释放)时自动 UnloadSceneAsync。
                await scene.GetComponent<ResourcesLoaderComponent>().LoadSceneAsync(BallArenaScene, LoadSceneMode.Additive);

                scene = sceneRef;
                unit = unitRef;
                if (unit != null && unit.GetComponent<BallCameraComponent>() == null)
                {
                    unit.AddComponent<BallCameraComponent>();
                }
            }
        }

        // 找到名为 "Sprite" 的子节点(头像层)的 SpriteRenderer 并赋值
        private static void SetAvatarSprite(GameObject root, Sprite sprite)
        {
            foreach (SpriteRenderer sr in root.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (sr.gameObject.name == AvatarNodeName)
                {
                    sr.sprite = sprite;
                    return;
                }
            }
        }
    }
}
