using System;
using System.Threading;
using CommandLine;

namespace ET
{
	public class Init
    {
        [StaticField]
        private FixedUpdate fixedUpdate;
        public void Start()
		{
			try
			{
				AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
				{
					Log.Error(e.ExceptionObject.ToString());
				};
				
				// 命令行参数
				Parser.Default.ParseArguments<Options>(System.Environment.GetCommandLineArgs())
						.WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
						.WithParsed((o)=>World.Instance.AddSingleton(o));
				World.Instance.AddSingleton<Logger>().Log = new NLogger(Options.Instance.AppType.ToString(), Options.Instance.Process, 0, "../Config/NLog/NLog.config");
				ETTask.ExceptionHandler += Log.Error;
                World.Instance.AddSingleton<TimeInfo>();
                World.Instance.AddSingleton<FiberManager>();


                fixedUpdate = new FixedUpdate();
                fixedUpdate.UpdateCallback = FixedUpdate;

                World.Instance.AddSingleton<CodeLoader>();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public void Update()
		{
			TimeInfo.Instance.Update();
			FiberManager.Instance.Update();
		}
        
        public static void FixedUpdate()
        {
            FiberManager.Instance.FixedUpdate();
        }

        public void LateUpdate()
		{
			FiberManager.Instance.LateUpdate();
		}
        
	}
}
