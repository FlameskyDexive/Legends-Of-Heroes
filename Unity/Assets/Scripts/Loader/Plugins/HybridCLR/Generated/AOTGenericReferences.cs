using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"System.Core.dll",
		"System.dll",
		"Unity.Core.dll",
		"Unity.Loader.dll",
		"Unity.ThirdParty.dll",
		"UnityEngine.CoreModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// ET.AEvent<object,ET.EventType.ChangePosition>
	// ET.AEvent<object,ET.EventType.OnCollisionContact>
	// ET.AEvent<object,ET.EventType.EntryEvent1>
	// ET.AEvent<object,ET.EventType.NumbericChange>
	// ET.AEvent<object,ET.Client.NetClientComponentOnRead>
	// ET.AEvent<object,ET.EventType.EntryEvent3>
	// ET.AEvent<object,ET.EventType.AfterCreateClientScene>
	// ET.AEvent<object,ET.EventType.SceneChangeFinish>
	// ET.AEvent<object,ET.EventType.SceneChangeStart>
	// ET.AEvent<object,ET.EventType.AfterCreateCurrentScene>
	// ET.AEvent<object,ET.EventType.OnPatchDownloadProgress>
	// ET.AEvent<object,ET.EventType.OnPatchDownlodFailed>
	// ET.AEvent<object,ET.EventType.LoginFinish>
	// ET.AEvent<object,ET.EventType.UpdateRoomPlayers>
	// ET.AEvent<object,ET.EventType.AfterUnitCreate>
	// ET.AEvent<object,ET.EventType.ChangeRotation>
	// ET.AEvent<object,ET.EventType.AfterMyUnitCreate>
	// ET.AInvokeHandler<ET.ConfigComponent.GetOneConfigBytes,object>
	// ET.AInvokeHandler<ET.ConfigComponent.GetAllConfigBytes,object>
	// ET.ATimer<object>
	// ET.AwakeSystem<object>
	// ET.AwakeSystem<object,System.Net.Sockets.AddressFamily>
	// ET.AwakeSystem<object,int>
	// ET.AwakeSystem<object,object>
	// ET.AwakeSystem<object,object,int>
	// ET.AwakeSystem<object,int,int>
	// ET.ConfigSingleton<object>
	// ET.DestroySystem<object>
	// ET.EntityRef<object>
	// ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>
	// ET.ETAsyncTaskMethodBuilder<byte>
	// ET.ETAsyncTaskMethodBuilder<int>
	// ET.ETAsyncTaskMethodBuilder<System.ValueTuple<uint,object>>
	// ET.ETAsyncTaskMethodBuilder<uint>
	// ET.ETAsyncTaskMethodBuilder<UnityEngine.SceneManagement.Scene>
	// ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_UnitStop>
	// ET.ETAsyncTaskMethodBuilder<object>
	// ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>
	// ET.ETTask<System.ValueTuple<uint,object>>
	// ET.ETTask<int>
	// ET.ETTask<uint>
	// ET.ETTask<ET.Client.Wait_CreateMyUnit>
	// ET.ETTask<UnityEngine.SceneManagement.Scene>
	// ET.ETTask<byte>
	// ET.ETTask<ET.Client.Wait_UnitStop>
	// ET.ETTask<ET.Client.Wait_SceneChangeFinish>
	// ET.ETTask<object>
	// ET.FixedUpdateSystem<object>
	// ET.IAwake<object>
	// ET.IAwake<int>
	// ET.IAwake<System.Net.Sockets.AddressFamily>
	// ET.IAwake<object,int>
	// ET.IAwake<int,int>
	// ET.LateUpdateSystem<object>
	// ET.ListComponent<long>
	// ET.ListComponent<object>
	// ET.ListComponent<Unity.Mathematics.float3>
	// ET.LoadSystem<object>
	// ET.Singleton<object>
	// ET.UpdateSystem<object>
	// System.Action<float>
	// System.Action<int>
	// System.Action<long>
	// System.Action<object>
	// System.Action<object,int>
	// System.Action<long,int>
	// System.Action<long,long,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<ET.ESkillEventType,object>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,long>
	// System.Collections.Generic.Dictionary<ET.WindowID,object>
	// System.Collections.Generic.Dictionary<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary<ushort,object>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<int,long>
	// System.Collections.Generic.Dictionary<ET.ESkillAbstractType,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.HashSet<ushort>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.IReadOnlyCollection<object>
	// System.Collections.Generic.IReadOnlyList<object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<int,long>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<Unity.Mathematics.float3>
	// System.Collections.Generic.List<ET.WindowID>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<Unity.Mathematics.float3>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.Queue<ET.WindowID>
	// System.Collections.Generic.SortedDictionary<int,object>
	// System.Collections.Generic.SortedDictionary<long,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<int,object>
	// System.Func<object>
	// System.Func<int,object>
	// System.Nullable<long>
	// System.Nullable<int>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Threading.Tasks.Task<System.ValueTuple<uint,uint>>
	// System.Threading.Tasks.Task<object>
	// System.ValueTuple<uint,uint>
	// System.ValueTuple<int,object>
	// System.ValueTuple<uint,object>
	// UnityEngine.Events.UnityAction<UnityEngine.Vector2>
	// UnityEngine.Events.UnityAction<int>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityEvent<UnityEngine.Vector2>
	// UnityEngine.Events.UnityEvent<object>
	// UnityEngine.Events.UnityEvent<byte>
	// }}

	public void RefMethods()
	{
		// object ET.Entity.AddChild<object>(bool)
		// object ET.Entity.AddChild<object,int,int>(int,int,bool)
		// object ET.Entity.AddChild<object,object>(object,bool)
		// object ET.Entity.AddChildWithId<object,int>(long,int,bool)
		// object ET.Entity.AddComponent<object,object>(object,bool)
		// object ET.Entity.AddComponent<object>(bool)
		// object ET.Entity.AddComponent<object,int,int>(int,int,bool)
		// object ET.Entity.AddComponent<object,object,int>(object,int,bool)
		// object ET.Entity.AddComponent<object,System.Net.Sockets.AddressFamily>(System.Net.Sockets.AddressFamily,bool)
		// object ET.Entity.GetChild<object>(long)
		// object ET.Entity.GetComponent<object>()
		// object ET.Entity.GetParent<object>()
		// System.Void ET.Entity.RemoveComponent<object>()
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>,object>(System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,object>(ET.ETTaskCompleted&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder.Start<object>(object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<UnityEngine.SceneManagement.Scene>.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<ET.ETTaskCompleted,object>(ET.ETTaskCompleted&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<System.ValueTuple<uint,object>>.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<uint>.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_UnitStop>.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<object,object>(object&,object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<int>.Start<object>(object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<System.ValueTuple<uint,object>>.Start<object>(object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<uint>.Start<object>(object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<object>.Start<object>(object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>.Start<object>(object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<UnityEngine.SceneManagement.Scene>.Start<object>(object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>.Start<object>(object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<byte>.Start<object>(object&)
		// System.Void ET.ETAsyncTaskMethodBuilder<ET.Client.Wait_UnitStop>.Start<object>(object&)
		// object ET.EventSystem.Invoke<ET.NavmeshComponent.RecastFileLoader,object>(ET.NavmeshComponent.RecastFileLoader)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.NumbericChange>(object,ET.EventType.NumbericChange)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.OnCollisionContact>(object,ET.EventType.OnCollisionContact)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.SceneChangeStart>(object,ET.EventType.SceneChangeStart)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.SceneChangeFinish>(object,ET.EventType.SceneChangeFinish)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.AfterMyUnitCreate>(object,ET.EventType.AfterMyUnitCreate)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.AfterCreateClientScene>(object,ET.EventType.AfterCreateClientScene)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.OnPatchDownloadProgress>(object,ET.EventType.OnPatchDownloadProgress)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.OnPatchDownlodFailed>(object,ET.EventType.OnPatchDownlodFailed)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.EnterMapFinish>(object,ET.EventType.EnterMapFinish)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.UpdateRoomPlayers>(object,ET.EventType.UpdateRoomPlayers)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.MoveStop>(object,ET.EventType.MoveStop)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.HitResult>(object,ET.EventType.HitResult)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.MoveStart>(object,ET.EventType.MoveStart)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.ChangePosition>(object,ET.EventType.ChangePosition)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.AfterCreateCurrentScene>(object,ET.EventType.AfterCreateCurrentScene)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.ChangeRotation>(object,ET.EventType.ChangeRotation)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.AfterUnitCreate>(object,ET.EventType.AfterUnitCreate)
		// System.Void ET.EventSystem.Publish<object,ET.Client.NetClientComponentOnRead>(object,ET.Client.NetClientComponentOnRead)
		// ET.ETTask ET.EventSystem.PublishAsync<object,ET.EventType.EntryEvent2>(object,ET.EventType.EntryEvent2)
		// ET.ETTask ET.EventSystem.PublishAsync<object,ET.EventType.AppStartInitFinish>(object,ET.EventType.AppStartInitFinish)
		// ET.ETTask ET.EventSystem.PublishAsync<object,ET.EventType.EntryEvent1>(object,ET.EventType.EntryEvent1)
		// ET.ETTask ET.EventSystem.PublishAsync<object,ET.EventType.EntryEvent3>(object,ET.EventType.EntryEvent3)
		// ET.ETTask ET.EventSystem.PublishAsync<object,ET.EventType.LoginFinish>(object,ET.EventType.LoginFinish)
		// object ET.Game.AddSingleton<object>()
		// object ET.JsonHelper.FromJson<object>(string)
		// System.Void ET.RandomGenerator.BreakRank<object>(System.Collections.Generic.List<object>)
		// string ET.StringHelper.ArrayToString<float>(float[])
		// object ReferenceCollector.Get<object>(string)
		// ET.Client.Wait_CreateMyUnit System.Activator.CreateInstance<ET.Client.Wait_CreateMyUnit>()
		// ET.Client.Wait_UnitStop System.Activator.CreateInstance<ET.Client.Wait_UnitStop>()
		// ET.Client.Wait_SceneChangeFinish System.Activator.CreateInstance<ET.Client.Wait_SceneChangeFinish>()
		// ET.RpcInfo[] System.Linq.Enumerable.ToArray<ET.RpcInfo>(System.Collections.Generic.IEnumerable<ET.RpcInfo>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.List<int> System.Linq.Enumerable.ToList<int>(System.Collections.Generic.IEnumerable<int>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Threading.Tasks.Task<object> System.Threading.Tasks.TaskFactory.StartNew<object>(System.Func<object>,System.Threading.CancellationToken)
		// object UnityEngine.Component.GetComponent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object YooAsset.AssetOperationHandle.GetAssetObject<object>()
		// YooAsset.AssetOperationHandle YooAsset.YooAssets.LoadAssetAsync<object>(string)
		// YooAsset.AssetOperationHandle YooAsset.YooAssets.LoadAssetSync<object>(string)
	}
}