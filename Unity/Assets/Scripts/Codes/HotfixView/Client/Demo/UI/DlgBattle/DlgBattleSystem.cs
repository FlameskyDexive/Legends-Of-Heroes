using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{

    public class DlgBattleAwakeSystem : AwakeSystem<DlgBattle>
    {
        protected override void Awake(DlgBattle self)
        {
            self.Awake();
        }
    }
    public class DlgBattleUpdateSystem : UpdateSystem<DlgBattle>
    {
        protected override void Update(DlgBattle self)
        {
            // if (Time.frameCount % 3 == 0)
                // self.Tick();

            self.Update();
        }
    }

    public class DlgBattleDestroySystem : DestroySystem<DlgBattle>
    {
        protected override void Destroy(DlgBattle self)
        {
            
        }
    }

    [FriendOf(typeof(DlgBattle))]
	public static  class DlgBattleSystem
	{
        public static void Awake(this DlgBattle self)
        {
            // self.RefreshSkillView();
        }
        
        public static void RegisterUIEvent(this DlgBattle self)
        {
            self.View.E_JoystickJoystick.OnValueChanged.AddListener(self.OnPressJoystick);
            // self.View.E_JoystickJoystick.OnSwipeEvent.AddListener(self.OnSwipeJoystick);

        }

		public static void ShowWindow(this DlgBattle self, ShowWindowDataBase contextData = null)
		{
		}


        public static void OnPressJoystick(this DlgBattle self, Vector2 v)
        {
            self.DomainScene().GetComponent<OperaComponent>().OnMove(v);

        }

        public static void Update(this DlgBattle self)
        {
            
        }
    }
}
