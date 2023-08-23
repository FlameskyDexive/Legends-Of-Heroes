using System;
using System.Collections;
using System.Threading;
using CommandLine;
using UnityEngine;
using YooAsset;

namespace ET
{
	public class Init: MonoBehaviour
    {
        private bool initCode = false;
        public EPlayMode PlayMode;
        
        private void Start()
		{
			DontDestroyOnLoad(gameObject);
			
			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Log.Error(e.ExceptionObject.ToString());
			};

			// 命令行参数
			string[] args = "".Split(" ");
			Parser.Default.ParseArguments<Options>(args)
				.WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
				.WithParsed((o)=>World.Instance.AddSingleton(o));
			Options.Instance.StartConfig = $"StartConfig/Localhost";
			
			World.Instance.AddSingleton<Logger>().Log = new UnityLogger();
			ETTask.ExceptionHandler += Log.Error;

            World.Instance.AddSingleton<TimeInfo>();
            World.Instance.AddSingleton<FiberManager>();

            this.StartAsync().Coroutine();
        }

        private async ETTask StartAsync()
        {
            await MonoResComponent.Instance.InitAsync(PlayMode);
            World.Instance.AddSingleton<TimeInfo>();
            World.Instance.AddSingleton<FiberManager>();
            World.Instance.AddSingleton<CodeLoader>().Start();
            this.initCode = true;

        }

        public void Restart()
        {
            World.Instance.Dispose(); AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error(e.ExceptionObject.ToString());
            };

            // 命令行参数
            string[] args = "".Split(" ");
            Parser.Default.ParseArguments<Options>(args)
                    .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                    .WithParsed((o) => World.Instance.AddSingleton(o));
            Options.Instance.StartConfig = $"StartConfig/Localhost";

            World.Instance.AddSingleton<Logger>().Log = new UnityLogger();
            ETTask.ExceptionHandler += Log.Error;

            this.StartAsync().Coroutine();
        }

        private void Update()
        {
	        if (!this.initCode)
		        return;
			TimeInfo.Instance.Update();
			FiberManager.Instance.Update();
		}

		private void LateUpdate()
		{
			if (!this.initCode)
				return;
			FiberManager.Instance.LateUpdate();
		}

		private void OnApplicationQuit()
		{
			World.Instance.Dispose();
		}
	}
	
	
}