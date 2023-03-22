public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	// System.Core.dll
	// System.dll
	// Unity.Core.dll
	// Unity.Loader.dll
	// Unity.ThirdParty.dll
	// UnityEngine.CoreModule.dll
	// mscorlib.dll
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// ET.AEvent<ET.EventType.AfterUnitCreate>
	// ET.AEvent<ET.EventType.NumbericChange>
	// ET.AEvent<ET.Client.NetClientComponentOnRead>
	// ET.AEvent<ET.EventType.EntryEvent3>
	// ET.AEvent<ET.EventType.AfterCreateCurrentScene>
	// ET.AEvent<ET.EventType.SceneChangeFinish>
	// ET.AEvent<ET.EventType.SceneChangeStart>
	// ET.AEvent<ET.EventType.AfterCreateClientScene>
	// ET.AEvent<ET.EventType.UpdateRoomPlayers>
	// ET.AEvent<ET.EventType.ChangePosition>
	// ET.AEvent<ET.EventType.ChangeRotation>
	// ET.AEvent<ET.EventType.LoginFinish>
	// ET.AEvent<ET.EventType.EntryEvent1>
	// ET.AInvokeHandler<ET.ConfigComponent.GetOneConfigBytes,object>
	// ET.AInvokeHandler<ET.ConfigComponent.GetAllConfigBytes,object>
	// ET.ATimer<object>
	// ET.AwakeSystem<object>
	// ET.AwakeSystem<object,int>
	// ET.AwakeSystem<object,System.Net.Sockets.AddressFamily>
	// ET.AwakeSystem<object,object>
	// ET.AwakeSystem<object,object,int>
	// ET.ConfigSingleton<object>
	// ET.DestroySystem<object>
	// ET.ETAsyncTaskMethodBuilder<int>
	// ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>
	// ET.ETAsyncTaskMethodBuilder<UnityEngine.SceneManagement.Scene>
	// ET.ETAsyncTaskMethodBuilder<uint>
	// ET.ETAsyncTaskMethodBuilder<System.ValueTuple<uint,object>>
	// ET.ETAsyncTaskMethodBuilder<byte>
	// ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_UnitStop>
	// ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>
	// ET.ETAsyncTaskMethodBuilder<object>
	// ET.ETTask<System.ValueTuple<uint,object>>
	// ET.ETTask<ET.Client.Wait_CreateMyUnit>
	// ET.ETTask<byte>
	// ET.ETTask<object>
	// ET.ETTask<int>
	// ET.ETTask<ET.Client.Wait_SceneChangeFinish>
	// ET.ETTask<ET.Client.Wait_UnitStop>
	// ET.ETTask<uint>
	// ET.ETTask<UnityEngine.SceneManagement.Scene>
	// ET.IAwake<int>
	// ET.IAwake<System.Net.Sockets.AddressFamily>
	// ET.IAwake<object>
	// ET.IAwake<object,int>
	// ET.LateUpdateSystem<object>
	// ET.ListComponent<object>
	// ET.ListComponent<Unity.Mathematics.float3>
	// ET.LoadSystem<object>
	// ET.Singleton<object>
	// ET.UpdateSystem<object>
	// System.Action<long>
	// System.Action<object>
	// System.Action<float>
	// System.Action<int>
	// System.Action<long,int>
	// System.Action<object,int>
	// System.Action<long,long,object>
	// System.Collections.Generic.Dictionary<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary<ushort,object>
	// System.Collections.Generic.Dictionary<object,long>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<ET.WindowID,object>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<int,long>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.HashSet<ushort>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.KeyValuePair<int,long>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<Unity.Mathematics.float3>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List<ET.WindowID>
	// System.Collections.Generic.List.Enumerator<Unity.Mathematics.float3>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.Queue<ET.WindowID>
	// System.Collections.Generic.SortedDictionary<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<int,object>
	// System.Func<object>
	// System.Func<int,object>
	// System.Nullable<int>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>
	// System.Threading.Tasks.Task<System.ValueTuple<uint,uint>>
	// System.Threading.Tasks.Task<object>
	// System.ValueTuple<uint,uint>
	// System.ValueTuple<int,object>
	// System.ValueTuple<uint,object>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityAction<UnityEngine.Vector2>
	// UnityEngine.Events.UnityAction<int>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<UnityEngine.Vector2>
	// UnityEngine.Events.UnityEvent<byte>
	// UnityEngine.Events.UnityEvent<object>
	// }}

	public void RefMethods()
	{
		// object ET.Entity.AddChild<object>(bool)
		// object ET.Entity.AddChildWithId<object,int>(long,int,bool)
		// object ET.Entity.AddComponent<object,object>(object,bool)
		// object ET.Entity.AddComponent<object,object,int>(object,int,bool)
		// object ET.Entity.AddComponent<object,System.Net.Sockets.AddressFamily>(System.Net.Sockets.AddressFamily,bool)
		// object ET.Entity.AddComponent<object>(bool)
		// object ET.Entity.GetChild<object>(long)
		// object ET.Entity.GetComponent<object>()
		// object ET.Entity.GetParent<object>()
		// System.Void ET.Entity.RemoveComponent<object>()
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.EntryEvent3_InitClient.<Run>d__0>(object&,ET.Client.EntryEvent3_InitClient.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.M2C_RemoveUnitsHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.M2C_RemoveUnitsHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>,ET.Client.RouterCheckComponentAwakeSystem.<CheckAsync>d__1>(System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>&,ET.Client.RouterCheckComponentAwakeSystem.<CheckAsync>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.M2C_CreateUnitsHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.M2C_CreateUnitsHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.EntryEvent1_InitShare.<Run>d__0>(ET.ETTaskCompleted&,ET.EntryEvent1_InitShare.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.FsmDonwloadWebFiles.<OnEnter>d__0>(object&,ET.Client.FsmDonwloadWebFiles.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.M2C_CreateMyUnitHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.M2C_CreateMyUnitHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.ConsoleComponentSystem.<Start>d__3>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.ConsoleComponentSystem.<Start>d__3&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.FsmCreateDownloader.<OnEnter>d__0>(ET.ETTaskCompleted&,ET.Client.FsmCreateDownloader.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.UIComponentSystem.<ShowWindowAsync>d__12>(object&,ET.Client.UIComponentSystem.<ShowWindowAsync>d__12&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.SceneChangeStart_AddComponent.<Run>d__0>(object&,ET.Client.SceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.ReloadConfigConsoleHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.ReloadConfigConsoleHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>(object&,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.DlgLobbySystem.<OnEnterMapClickHandler>d__2>(object&,ET.Client.DlgLobbySystem.<OnEnterMapClickHandler>d__2&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0>(object&,ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.RouterCheckComponentAwakeSystem.<CheckAsync>d__1>(object&,ET.Client.RouterCheckComponentAwakeSystem.<CheckAsync>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.DlgRoomSystem.<OnEnterMapClickHandler>d__7>(object&,ET.Client.DlgRoomSystem.<OnEnterMapClickHandler>d__7&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.DlgRoomSystem.<StartCountDown>d__10>(object&,ET.Client.DlgRoomSystem.<StartCountDown>d__10&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.UpdateRoomPlayersView.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.UpdateRoomPlayersView.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.SceneChangeFinishEvent.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.SceneChangeFinishEvent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.ReloadDllConsoleHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.ReloadDllConsoleHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.ConsoleComponentSystem.<Start>d__3>(object&,ET.ConsoleComponentSystem.<Start>d__3&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.NumericChangeEvent_NotifyWatcher.<Run>d__0>(ET.ETTaskCompleted&,ET.NumericChangeEvent_NotifyWatcher.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Entry.<StartAsync>d__2>(object&,ET.Entry.<StartAsync>d__2&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.M2C_StopHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.M2C_StopHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.M2C_PathfindingResultHandler.<Run>d__0>(object&,ET.Client.M2C_PathfindingResultHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.M2C_JoystickMoveHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.M2C_JoystickMoveHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.LoginHelper.<Login>d__0>(object&,ET.Client.LoginHelper.<Login>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.EUIHelper.<>c__DisplayClass14_0.<<AddListenerAsync>g__clickActionAsync|0>d>(object&,ET.Client.EUIHelper.<>c__DisplayClass14_0.<<AddListenerAsync>g__clickActionAsync|0>d&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.PingComponentAwakeSystem.<PingAsync>d__1>(object&,ET.Client.PingComponentAwakeSystem.<PingAsync>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.RouterAddressComponentSystem.<Init>d__1>(object&,ET.Client.RouterAddressComponentSystem.<Init>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.NetClientComponentOnReadEvent.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.NetClientComponentOnReadEvent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.M2C_StartSceneChangeHandler.<Run>d__0>(object&,ET.Client.M2C_StartSceneChangeHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2>(object&,ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3>(object&,ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.AFsmNodeHandler.<OnEnter>d__4>(ET.ETTaskCompleted&,ET.AFsmNodeHandler.<OnEnter>d__4&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.EUIHelper.<>c__DisplayClass13_0.<<AddListenerAsyncWithId>g__clickActionAsync|0>d>(object&,ET.Client.EUIHelper.<>c__DisplayClass13_0.<<AddListenerAsyncWithId>g__clickActionAsync|0>d&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.EnterMapHelper.<EnterMapAsync>d__0>(object&,ET.Client.EnterMapHelper.<EnterMapAsync>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.LobbyHelper.<JoinOrCreateRoom>d__0>(object&,ET.Client.LobbyHelper.<JoinOrCreateRoom>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.L2C_JoinOrCreateRoomHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.L2C_JoinOrCreateRoomHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.G2C_UpdateRoomPlayersHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.G2C_UpdateRoomPlayersHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.SceneChangeHelper.<SceneChangeTo>d__0>(object&,ET.Client.SceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.G2C_StartEnterMapHandler.<Run>d__0>(ET.ETTaskCompleted&,ET.Client.G2C_StartEnterMapHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.FsmUpdateStaticVersion.<OnEnter>d__0>(object&,ET.Client.FsmUpdateStaticVersion.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.FsmUpdateManifest.<OnEnter>d__0>(object&,ET.Client.FsmUpdateManifest.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.FsmResourceInit.<OnEnter>d__0>(ET.ETTaskCompleted&,ET.Client.FsmResourceInit.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.FsmPatchDone.<OnEnter>d__0>(ET.ETTaskCompleted&,ET.Client.FsmPatchDone.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.G2C_StartEnterMapHandler.<Run>d__0>(object&,ET.Client.G2C_StartEnterMapHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.AI_XunLuo.<Execute>d__1>(object&,ET.Client.AI_XunLuo.<Execute>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.AI_Attack.<Execute>d__1>(object&,ET.Client.AI_Attack.<Execute>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.LobbyHelper.<JoinOrCreateRoom>d__0>(ET.ETTaskCompleted&,ET.Client.LobbyHelper.<JoinOrCreateRoom>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,ET.Client.MoveHelper.<MoveToAsync>d__1>(object&,ET.Client.MoveHelper.<MoveToAsync>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0>(ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.UIComponentSystem.<ShowWindowAsync>d__12>(ET.Client.UIComponentSystem.<ShowWindowAsync>d__12&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.FsmResourceInit.<OnEnter>d__0>(ET.Client.FsmResourceInit.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.FsmCreateDownloader.<OnEnter>d__0>(ET.Client.FsmCreateDownloader.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.FsmDonwloadWebFiles.<OnEnter>d__0>(ET.Client.FsmDonwloadWebFiles.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.FsmPatchDone.<OnEnter>d__0>(ET.Client.FsmPatchDone.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.FsmUpdateManifest.<OnEnter>d__0>(ET.Client.FsmUpdateManifest.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.FsmUpdateStaticVersion.<OnEnter>d__0>(ET.Client.FsmUpdateStaticVersion.<OnEnter>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0>(ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.UIComponentSystem.<LoadBaseWindowsAsync>d__28>(ET.Client.UIComponentSystem.<LoadBaseWindowsAsync>d__28&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>(ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.DlgLobbySystem.<OnEnterMapClickHandler>d__2>(ET.Client.DlgLobbySystem.<OnEnterMapClickHandler>d__2&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.DlgRoomSystem.<StartCountDown>d__10>(ET.Client.DlgRoomSystem.<StartCountDown>d__10&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.MoveHelper.<MoveToAsync>d__1>(ET.Client.MoveHelper.<MoveToAsync>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.M2C_StopHandler.<Run>d__0>(ET.Client.M2C_StopHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.M2C_PathfindingResultHandler.<Run>d__0>(ET.Client.M2C_PathfindingResultHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.M2C_JoystickMoveHandler.<Run>d__0>(ET.Client.M2C_JoystickMoveHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.LoginHelper.<Login>d__0>(ET.Client.LoginHelper.<Login>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.EnterMapHelper.<EnterMapAsync>d__0>(ET.Client.EnterMapHelper.<EnterMapAsync>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.LobbyHelper.<JoinOrCreateRoom>d__0>(ET.Client.LobbyHelper.<JoinOrCreateRoom>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.UpdateRoomPlayersView.<Run>d__0>(ET.Client.UpdateRoomPlayersView.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.PingComponentAwakeSystem.<PingAsync>d__1>(ET.Client.PingComponentAwakeSystem.<PingAsync>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.G2C_UpdateRoomPlayersHandler.<Run>d__0>(ET.Client.G2C_UpdateRoomPlayersHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.AI_XunLuo.<Execute>d__1>(ET.Client.AI_XunLuo.<Execute>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.AI_Attack.<Execute>d__1>(ET.Client.AI_Attack.<Execute>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.NumericChangeEvent_NotifyWatcher.<Run>d__0>(ET.NumericChangeEvent_NotifyWatcher.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.ReloadDllConsoleHandler.<Run>d__0>(ET.ReloadDllConsoleHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.ReloadConfigConsoleHandler.<Run>d__0>(ET.ReloadConfigConsoleHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.ConsoleComponentSystem.<Start>d__3>(ET.ConsoleComponentSystem.<Start>d__3&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.EntryEvent1_InitShare.<Run>d__0>(ET.EntryEvent1_InitShare.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.AFsmNodeHandler.<OnEnter>d__4>(ET.AFsmNodeHandler.<OnEnter>d__4&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.G2C_StartEnterMapHandler.<Run>d__0>(ET.Client.G2C_StartEnterMapHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.RouterAddressComponentSystem.<Init>d__1>(ET.Client.RouterAddressComponentSystem.<Init>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.L2C_JoinOrCreateRoomHandler.<Run>d__0>(ET.Client.L2C_JoinOrCreateRoomHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3>(ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.DlgRoomSystem.<OnCancelClickHandler>d__8>(ET.Client.DlgRoomSystem.<OnCancelClickHandler>d__8&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.DlgRoomSystem.<OnEnterMapClickHandler>d__7>(ET.Client.DlgRoomSystem.<OnEnterMapClickHandler>d__7&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0>(ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2>(ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.SceneChangeStart_AddComponent.<Run>d__0>(ET.Client.SceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.SceneChangeFinishEvent.<Run>d__0>(ET.Client.SceneChangeFinishEvent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0>(ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0>(ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.EntryEvent3_InitClient.<Run>d__0>(ET.Client.EntryEvent3_InitClient.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.M2C_RemoveUnitsHandler.<Run>d__0>(ET.Client.M2C_RemoveUnitsHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.DlgLobbySystem.<OnSoloClickHandler>d__3>(ET.Client.DlgLobbySystem.<OnSoloClickHandler>d__3&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Entry.<StartAsync>d__2>(ET.Entry.<StartAsync>d__2&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.M2C_CreateUnitsHandler.<Run>d__0>(ET.Client.M2C_CreateUnitsHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.M2C_CreateMyUnitHandler.<Run>d__0>(ET.Client.M2C_CreateMyUnitHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.NetClientComponentOnReadEvent.<Run>d__0>(ET.Client.NetClientComponentOnReadEvent.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.EUIHelper.<>c__DisplayClass13_0.<<AddListenerAsyncWithId>g__clickActionAsync|0>d>(ET.Client.EUIHelper.<>c__DisplayClass13_0.<<AddListenerAsyncWithId>g__clickActionAsync|0>d&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.SceneChangeHelper.<SceneChangeTo>d__0>(ET.Client.SceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.M2C_StartSceneChangeHandler.<Run>d__0>(ET.Client.M2C_StartSceneChangeHandler.<Run>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.EUIHelper.<>c__DisplayClass14_0.<<AddListenerAsync>g__clickActionAsync|0>d>(ET.Client.EUIHelper.<>c__DisplayClass14_0.<<AddListenerAsync>g__clickActionAsync|0>d&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<ET.Client.RouterCheckComponentAwakeSystem.<CheckAsync>d__1>(ET.Client.RouterCheckComponentAwakeSystem.<CheckAsync>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,ET.Client.RouterHelper.<CreateRouterSession>d__0>(object&,ET.Client.RouterHelper.<CreateRouterSession>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder<System.ValueTuple<uint,object>>.AwaitUnsafeOnCompleted<object,ET.Client.RouterHelper.<GetRouterAddress>d__1>(object&,ET.Client.RouterHelper.<GetRouterAddress>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.Client.HttpClientHelper.<Get>d__0>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.Client.HttpClientHelper.<Get>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,ET.Client.MoveHelper.<MoveToAsync>d__0>(object&,ET.Client.MoveHelper.<MoveToAsync>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,ET.Client.ResComponentSystem.<LoadRawFileTextAsync>d__15>(object&,ET.Client.ResComponentSystem.<LoadRawFileTextAsync>d__15&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,ET.Client.ResComponentSystem.<LoadRawFileDataAsync>d__14>(object&,ET.Client.ResComponentSystem.<LoadRawFileDataAsync>d__14&)
		// System.Void ET.ETAsyncTaskMethodBuilder<UnityEngine.SceneManagement.Scene>.AwaitUnsafeOnCompleted<object,ET.Client.ResComponentSystem.<LoadSceneAsync>d__13>(object&,ET.Client.ResComponentSystem.<LoadSceneAsync>d__13&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,ET.Client.ResComponentSystem.<LoadAssetAsync>d__12>(object&,ET.Client.ResComponentSystem.<LoadAssetAsync>d__12&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,ET.Client.SceneFactory.<CreateClientScene>d__0>(ET.ETTaskCompleted&,ET.Client.SceneFactory.<CreateClientScene>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,ET.SessionSystem.<Call>d__3>(object&,ET.SessionSystem.<Call>d__3&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,ET.SessionSystem.<Call>d__4>(object&,ET.SessionSystem.<Call>d__4&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,ET.Client.ResComponentSystem.<DonwloadWebFilesAsync>d__7>(object&,ET.Client.ResComponentSystem.<DonwloadWebFilesAsync>d__7&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,ET.Client.UIComponentSystem.<ShowBaseWindowAsync>d__19>(object&,ET.Client.UIComponentSystem.<ShowBaseWindowAsync>d__19&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,ET.Client.ResComponentSystem.<UpdateManifestAsync>d__5>(object&,ET.Client.ResComponentSystem.<UpdateManifestAsync>d__5&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,ET.Client.ResComponentSystem.<UpdateVersionAsync>d__4>(object&,ET.Client.ResComponentSystem.<UpdateVersionAsync>d__4&)
		// System.Void ET.ETAsyncTaskMethodBuilder<uint>.AwaitUnsafeOnCompleted<object,ET.Client.RouterHelper.<Connect>d__2>(object&,ET.Client.RouterHelper.<Connect>d__2&)
		// System.Void ET.ETAsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<object,ET.MoveComponentSystem.<MoveToAsync>d__5>(object&,ET.MoveComponentSystem.<MoveToAsync>d__5&)
		// System.Void ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>.Start<ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_SceneChangeFinish>>(ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_SceneChangeFinish>&)
		// System.Void ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_UnitStop>.Start<ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_UnitStop>>(ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_UnitStop>&)
		// System.Void ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>.Start<ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_CreateMyUnit>>(ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_CreateMyUnit>&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.Client.ResComponentSystem.<LoadAssetAsync>d__11<object>>(ET.Client.ResComponentSystem.<LoadAssetAsync>d__11<object>&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.Client.ResComponentSystem.<LoadAssetAsync>d__12>(ET.Client.ResComponentSystem.<LoadAssetAsync>d__12&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.IconHelper.<LoadIconSpriteAsync>d__1>(ET.IconHelper.<LoadIconSpriteAsync>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.Start<ET.Client.ResComponentSystem.<UpdateVersionAsync>d__4>(ET.Client.ResComponentSystem.<UpdateVersionAsync>d__4&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.Start<ET.Client.ResComponentSystem.<UpdateManifestAsync>d__5>(ET.Client.ResComponentSystem.<UpdateManifestAsync>d__5&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.Start<ET.Client.ResComponentSystem.<DonwloadWebFilesAsync>d__7>(ET.Client.ResComponentSystem.<DonwloadWebFilesAsync>d__7&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.SessionSystem.<Call>d__4>(ET.SessionSystem.<Call>d__4&)
		// System.Void ET.ETAsyncTaskMethodBuilder<UnityEngine.SceneManagement.Scene>.Start<ET.Client.ResComponentSystem.<LoadSceneAsync>d__13>(ET.Client.ResComponentSystem.<LoadSceneAsync>d__13&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.Client.ResComponentSystem.<LoadRawFileDataAsync>d__14>(ET.Client.ResComponentSystem.<LoadRawFileDataAsync>d__14&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.Client.UIComponentSystem.<ShowBaseWindowAsync>d__19>(ET.Client.UIComponentSystem.<ShowBaseWindowAsync>d__19&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.SessionSystem.<Call>d__3>(ET.SessionSystem.<Call>d__3&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.Client.ResComponentSystem.<LoadRawFileTextAsync>d__15>(ET.Client.ResComponentSystem.<LoadRawFileTextAsync>d__15&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.Client.SceneFactory.<CreateClientScene>d__0>(ET.Client.SceneFactory.<CreateClientScene>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder<uint>.Start<ET.Client.RouterHelper.<Connect>d__2>(ET.Client.RouterHelper.<Connect>d__2&)
		// System.Void ET.ETAsyncTaskMethodBuilder<System.ValueTuple<uint,object>>.Start<ET.Client.RouterHelper.<GetRouterAddress>d__1>(ET.Client.RouterHelper.<GetRouterAddress>d__1&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.Client.RouterHelper.<CreateRouterSession>d__0>(ET.Client.RouterHelper.<CreateRouterSession>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<ET.Client.HttpClientHelper.<Get>d__0>(ET.Client.HttpClientHelper.<Get>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.Start<ET.Client.MoveHelper.<MoveToAsync>d__0>(ET.Client.MoveHelper.<MoveToAsync>d__0&)
		// System.Void ET.ETAsyncTaskMethodBuilder<byte>.Start<ET.MoveComponentSystem.<MoveToAsync>d__5>(ET.MoveComponentSystem.<MoveToAsync>d__5&)
		// object ET.EventSystem.Invoke<ET.NavmeshComponent.RecastFileLoader,object>(ET.NavmeshComponent.RecastFileLoader)
		// System.Void ET.EventSystem.Publish<ET.Client.NetClientComponentOnRead>(ET.Scene,ET.Client.NetClientComponentOnRead)
		// System.Void ET.EventSystem.Publish<ET.EventType.AfterUnitCreate>(ET.Scene,ET.EventType.AfterUnitCreate)
		// System.Void ET.EventSystem.Publish<ET.EventType.ChangeRotation>(ET.Scene,ET.EventType.ChangeRotation)
		// System.Void ET.EventSystem.Publish<ET.EventType.NumbericChange>(ET.Scene,ET.EventType.NumbericChange)
		// System.Void ET.EventSystem.Publish<ET.EventType.MoveStart>(ET.Scene,ET.EventType.MoveStart)
		// System.Void ET.EventSystem.Publish<ET.EventType.AfterCreateClientScene>(ET.Scene,ET.EventType.AfterCreateClientScene)
		// System.Void ET.EventSystem.Publish<ET.EventType.MoveStop>(ET.Scene,ET.EventType.MoveStop)
		// System.Void ET.EventSystem.Publish<ET.EventType.SceneChangeFinish>(ET.Scene,ET.EventType.SceneChangeFinish)
		// System.Void ET.EventSystem.Publish<ET.EventType.UpdateRoomPlayers>(ET.Scene,ET.EventType.UpdateRoomPlayers)
		// System.Void ET.EventSystem.Publish<ET.EventType.SceneChangeStart>(ET.Scene,ET.EventType.SceneChangeStart)
		// System.Void ET.EventSystem.Publish<ET.EventType.EnterMapFinish>(ET.Scene,ET.EventType.EnterMapFinish)
		// System.Void ET.EventSystem.Publish<ET.EventType.ChangePosition>(ET.Scene,ET.EventType.ChangePosition)
		// System.Void ET.EventSystem.Publish<ET.EventType.AfterCreateCurrentScene>(ET.Scene,ET.EventType.AfterCreateCurrentScene)
		// ET.ETTask ET.EventSystem.PublishAsync<ET.EventType.EntryEvent1>(ET.Scene,ET.EventType.EntryEvent1)
		// ET.ETTask ET.EventSystem.PublishAsync<ET.EventType.EntryEvent2>(ET.Scene,ET.EventType.EntryEvent2)
		// ET.ETTask ET.EventSystem.PublishAsync<ET.EventType.AppStartInitFinish>(ET.Scene,ET.EventType.AppStartInitFinish)
		// ET.ETTask ET.EventSystem.PublishAsync<ET.EventType.EntryEvent3>(ET.Scene,ET.EventType.EntryEvent3)
		// ET.ETTask ET.EventSystem.PublishAsync<ET.EventType.LoginFinish>(ET.Scene,ET.EventType.LoginFinish)
		// object ET.Game.AddSingleton<object>()
		// object ET.JsonHelper.FromJson<object>(string)
		// System.Void ET.RandomGenerator.BreakRank<object>(System.Collections.Generic.List<object>)
		// string ET.StringHelper.ArrayToString<float>(float[])
		// object ReferenceCollector.Get<object>(string)
		// ET.Client.Wait_CreateMyUnit System.Activator.CreateInstance<ET.Client.Wait_CreateMyUnit>()
		// ET.Client.Wait_UnitStop System.Activator.CreateInstance<ET.Client.Wait_UnitStop>()
		// ET.Client.Wait_SceneChangeFinish System.Activator.CreateInstance<ET.Client.Wait_SceneChangeFinish>()
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// ET.RpcInfo[] System.Linq.Enumerable.ToArray<ET.RpcInfo>(System.Collections.Generic.IEnumerable<ET.RpcInfo>)
		// System.Threading.Tasks.Task<object> System.Threading.Tasks.TaskFactory.StartNew<object>(System.Func<object>,System.Threading.CancellationToken)
		// object UnityEngine.Component.GetComponent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Resources.Load<object>(string)
		// YooAsset.AssetOperationHandle YooAsset.YooAssets.LoadAssetSync<object>(string)
	}
}