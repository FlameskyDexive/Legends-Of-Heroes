using Unity.Mathematics;
using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterUnitCreate_CreateUnitView: AEvent<Scene, AfterUnitCreate>
    {
        protected override async ETTask Run(Scene scene, AfterUnitCreate args)
        {
            EntityRef<Scene> sceneRef = scene;
            Unit unit = args.Unit;
            EntityRef<Unit> unitRef = unit;
            // Unit View层
            long unitId = unit.Id;
            int unitType = (int)unit.UnitType;
            string assetsName = $"Packages/cn.etetet.map/Bundles/Units/{unit.Config().Name}.prefab";
            GameObject bundleGameObject;
            try
            {
                bundleGameObject = await scene.GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<GameObject>(assetsName);
            }
            catch (System.Exception e)
            {
                Log.Error($"[单位视图] 加载预制体异常 '{assetsName}'(unit={unitId} type={unitType}): {e}");
                return;
            }
            if (bundleGameObject == null)
            {
                Log.Error($"[单位视图] 预制体加载为 null '{assetsName}'(unit={unitId} type={unitType}) —— 检查该预制体是否存在/已 Unity 导入/被 yooasset 收录");
                return;
            }

            scene = sceneRef;
            GlobalComponent globalComponent = scene.Root().GetComponent<GlobalComponent>();
            GameObject go = UnityEngine.Object.Instantiate(bundleGameObject, globalComponent.Unit, true);
            unit = unitRef;
            // 异步加载预制体期间单位可能已被移除/销毁(球球大作战食物加载时被吃掉)→ unit 为 null/已销毁。
            // 销毁刚实例化的视图并返回,避免 GameObjectEntityRef.Entity = 已销毁单位 / 后续访问 unit.Position 空引用。
            if (unit == null || unit.IsDisposed)
            {
                UnityEngine.Object.Destroy(go);
                return;
            }
            go.AddComponent<GameObjectEntityRef>().Entity = unit;
            go.transform.position = unit.Position;
            go.transform.rotation = unit.Rotation;

            {
                using var _ = await scene.Root().CoroutineLockComponent.Wait(CoroutineLockType.SceneChange, 0);
                GameObjectPosHelper.OnTerrain(go.transform);
            }

            scene = sceneRef;
            unit = unitRef;
            unit.AddComponent<GameObjectComponent>().GameObject = go;
            // 仅当预制体确有 Animator 时才加 AnimatorComponent(球球大作战的球无动画,加了会被 Speed 数值监听器空引用)
            if (go.GetComponent<Animator>() != null)
            {
                unit.AddComponent<AnimatorComponent>();
            }

            // 仅当预制体带骨骼绑点(BindPointComponent,即人形 3D 单位)时,才装配 3D 跟随相机/输入/血条。
            // 球球大作战等 2D 精灵单位无绑点 -> 跳过(否则 CinemachineComponent 取 Head 骨骼会空引用崩);
            // 其专属表现(俯视相机/换皮肤)由 AfterUnitViewCreate 的订阅者(ballbattle)负责。
            bool hasBindPoints = go.GetComponent<BindPointComponent>() != null;
            if (hasBindPoints)
            {
                if (scene.Root().GetComponent<PlayerComponent>().MyId == unit.Id)
                {
                    unit.AddComponent<CinemachineComponent>();
                    unit.AddComponent<InputSystemComponent>();
                    unit.AddComponent<SpellIndicatorComponent>();
                }
                else
                {
                    //临时做法 //不是玩家的Unit，添加一个血条显示
                    //await YIUIFactory.InstantiateAsync<HPView3DComponent>(unit, go.transform);
                    await YIUIFactory.InstantiateAsync<HPViewComponent>(scene, unit, scene.YIUIMgr().UICache);
                }
            }

            // 视图创建完成,发布通用扩展事件(球球大作战据此按 id 换头像 + 给本地玩家切俯视相机)
            scene = sceneRef;
            unit = unitRef;
            EventSystem.Instance.Publish(scene, new AfterUnitViewCreate() { Unit = unit });
        }
    }
}