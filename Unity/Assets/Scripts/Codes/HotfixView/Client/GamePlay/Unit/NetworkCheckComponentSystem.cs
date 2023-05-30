using System;
using UnityEngine;

namespace ET.Client
{
	[FriendOf(typeof(NetworkCheckComponent))]
	public static class NetworkCheckComponentSystem
	{
		[ObjectSystem]
		public class NetworkCheckComponentAwakeSystem : AwakeSystem<NetworkCheckComponent>
		{
			protected override void Awake(NetworkCheckComponent self)
			{
				self.Awake();
			}
		}

		/*
		[ObjectSystem]
		public class NetworkCheckComponentUpdateSystem : UpdateSystem<NetworkCheckComponent>
		{
			protected override void Update(NetworkCheckComponent self)
			{
				// self.Update();
			}
		}*/
	
		[ObjectSystem]
		public class NetworkCheckComponentDestroySystem : DestroySystem<NetworkCheckComponent>
		{
			protected override void Destroy(NetworkCheckComponent self)
            {
                TimerComponent.Instance.Remove(ref self.timer);
            }
		}


        [Invoke(TimerInvokeType.NetworkCheck)]
        public class PlayerOfflineOutTime : ATimer<NetworkCheckComponent>
        {
            protected override void Run(NetworkCheckComponent self)
            {
                try
                {
                    self.NetworkCheck();
                }
                catch (Exception e)
                {
                    Log.Error($"NetworkCheck error:");
                }
            }
        }

        public static void Awake(this NetworkCheckComponent self)
		{
			
		}
        
        public static void StartCheck(this NetworkCheckComponent self)
        {
            self.timer = TimerComponent.Instance.NewRepeatedTimer(self.interval, TimerInvokeType.NetworkCheck, self);
        }

		public static void NetworkCheck(this NetworkCheckComponent self)
		{
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Log.Info($"network not reachable:");
                self.DomainScene().GetComponent<ReconnectComponent>().StartReconnect();
            }
		}
		
	}
}