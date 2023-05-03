using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterUnitCreate_CreateUnitView: AEvent<Scene, EventType.AfterUnitCreate>
    {
        protected override async ETTask Run(Scene scene, EventType.AfterUnitCreate args)
        {
            Unit unit = args.Unit;
            GameObject go = null;
            // Unit View层
            // 这里可以改成异步加载，demo就不搞了
            // GameObject bundleGameObject = (GameObject)ResourcesComponent.Instance.GetAsset("Unit.unity3d", "Unit");
            // GameObject prefab = bundleGameObject.Get<GameObject>("Skeleton");
            switch (unit.Type)
            {
                case UnitType.Player:
                {
                    GameObject prefab = await ResComponent.Instance.LoadAssetAsync<GameObject>("Player");

                    go = UnityEngine.Object.Instantiate(prefab, GlobalComponent.Instance.Unit, true);
                    Transform iconTrans = go.transform.Find("Root/Sprite");
                    if(iconTrans != null)
                    {
                        SpriteRenderer spriteRenderer = iconTrans.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.sprite = ResComponent.Instance.LoadAsset<Sprite>($"Avatar{unit.Config.Id - 1000}");
                        }
                    }

                    int hp = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp);
                    if(hp > 10)
                        go.transform.localScale = Vector3.one * hp / 50f;
                    break;
                } 
                case UnitType.Bullet:
                {
                    GameObject prefab = await ResComponent.Instance.LoadAssetAsync<GameObject>("Bullet_001");
                    go = UnityEngine.Object.Instantiate(prefab, GlobalComponent.Instance.Unit, true);
                    break;
                } 
            }

            go.transform.position = unit.Position;
            GameObjectComponent gameObjectComponent = unit.AddComponent<GameObjectComponent>();
            gameObjectComponent.Init(go);
            
            unit.AddComponent<AnimatorComponent>();
            await ETTask.CompletedTask;
        }
    }
}