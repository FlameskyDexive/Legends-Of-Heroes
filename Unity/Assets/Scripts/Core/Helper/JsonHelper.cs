using System;

namespace ET
{
	public static class JsonHelper
	{
        public static string ToJson(object message)
        {

            return LitJson.JsonMapper.ToJson(message);
        }

        public static object FromJson(Type type, string json)
        {
            return LitJson.JsonMapper.ToObject(type, json);

        }

        public static T FromJson<T>(string json)
        {
            return LitJson.JsonMapper.ToObject<T>(json);
        }
    }
}