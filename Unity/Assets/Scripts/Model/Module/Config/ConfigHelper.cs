using System;
using UnityEngine;

namespace ETModel
{
	public static class ConfigHelper
	{
		public static GameObject configBundle;
		public static string GetText(string key)
		{
			try
			{
				GameObject config = (GameObject)Game.Scene.GetComponent<ResourcesComponent>().GetAsset("config.unity3d", "Config");
				string configStr = config.Get<TextAsset>(key).text;
				return configStr;
			}
			catch (Exception e)
			{
				throw new Exception($"load config file fail, key: {key}", e);
			}
		}
		
		public static string GetGlobal()
		{
			try
			{
				GameObject config = (GameObject)ResourcesHelper.Load("KV");
				string configStr = config.Get<TextAsset>("GlobalProto").text;
				return configStr;
			}
			catch (Exception e)
			{
				throw new Exception($"load global config file fail", e);
			}
		}


		public static void LoadConfigBundle()
		{
			ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
			resourcesComponent.LoadBundle($"Config.unity3d");
			configBundle = (GameObject)resourcesComponent.GetAsset($"Config.unity3d", $"Config");
			GameObject go = UnityEngine.Object.Instantiate(configBundle);
		}

		public static string GetCsvText(string csvName)
		{
			try
			{
				csvName = csvName.Replace(".csv", "");
				//Log.Debug($"---csv--{csvName}--");
				//GameObject config = (GameObject)Resources.Load("KV");
				string configStr = configBundle.Get<TextAsset>(csvName).text;
				//Log.Debug($"--{csvName}--{configStr}");
				return configStr;
			}
			catch (Exception e)
			{
				throw new Exception($"load global config file fail", e);
			}
		}

		public static T ToObject<T>(string str)
		{
			return JsonHelper.FromJson<T>(str);
		}
	}
}