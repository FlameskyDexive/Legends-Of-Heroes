﻿using System;
using UnityEngine;

namespace ET.Client
{
    // [UIEvent(UIType.UILSLogin)]
    // public class UILSLoginEvent: AUIEvent
    // {
    //     public override async ETTask<UI> OnCreate(UIComponent uiComponent, UILayer uiLayer)
    //     {
            // ResourcesComponent resourcesComponent = uiComponent.Root().GetComponent<ResourcesComponent>();
            // await uiComponent.Scene().GetComponent<ResourcesLoaderComponent>().LoadAsync(resourcesComponent.StringToAB(UIType.UILSLogin));
            // GameObject bundleGameObject = (GameObject) resourcesComponent.GetAsset(resourcesComponent.StringToAB(UIType.UILSLogin), UIType.UILSLogin);
            // GameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject, uiComponent.UIGlobalComponent.GetLayer((int)uiLayer));
            // UI ui = uiComponent.AddChild<UI, string, GameObject>(UIType.UILSLogin, gameObject);
            // ui.AddComponent<UILSLoginComponent>();
            // return ui;
    //         await ETTask.CompletedTask;
    //         return null;
    //     }
    //
    //     public override void OnRemove(UIComponent uiComponent)
    //     {
    //     }
    // }
}