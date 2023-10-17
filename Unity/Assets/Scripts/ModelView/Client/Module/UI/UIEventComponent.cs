using System;
using System.Collections.Generic;

namespace ET.Client
{
	/// <summary>
	/// 管理所有UI GameObject
	/// </summary>
	[Code]
	public class UIEventComponent: Singleton<UIEventComponent>, ISingletonAwake
    {
        public readonly Dictionary<WindowID, IAUIEventHandler> UIEventHandlers = new Dictionary<WindowID, IAUIEventHandler>();
        // public Dictionary<string, AUIEvent> UIEvents { get; } = new();
        public bool IsClicked { get; set; }
		
        public void Awake()
        {
            var uiEvents = CodeTypes.Instance.GetTypes(typeof (AUIEventAttribute));
            foreach (Type type in uiEvents)
            {
                object[] attrs = type.GetCustomAttributes(typeof (AUIEventAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                AUIEventAttribute uiEventAttribute = attrs[0] as AUIEventAttribute;
                IAUIEventHandler aUIEvent = Activator.CreateInstance(type) as IAUIEventHandler;
                // this.UIEvents.Add(uiEventAttribute.UIType, aUIEvent);
                this.UIEventHandlers.Add(uiEventAttribute.WindowID, aUIEvent);
            }
        }

        // public override void Load()
        // {
        //     World.Instance.AddSingleton<UIEventComponent>(true);
        // }
        public IAUIEventHandler GetUIEventHandler(WindowID windowID)
        {
            if (this.UIEventHandlers.TryGetValue(windowID, out IAUIEventHandler handler))
            {
                return handler;
            }
            Log.Error($"windowId : {windowID} is not have any uiEvent");
            return null;
        }

        public void SetUIClicked(bool isClicked)
        {
            this.IsClicked = isClicked;
        }
    }
}