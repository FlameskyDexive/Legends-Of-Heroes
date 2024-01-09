using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bright.Serialization;

namespace ET
{
	/// <summary>
    /// ConfigLoader会扫描所有的有ConfigAttribute标签的配置,加载进来
    /// </summary>
    public class ConfigLoader: Singleton<ConfigLoader>, ISingletonAwake
    {
        public struct GetAllConfigBytes
        {
        }
        
        public struct GetOneConfigBytes
        {
            public string ConfigName;
        }
		
        private readonly ConcurrentDictionary<Type, IConfigSingleton> allConfig = new();
        
        public void Awake()
        {
        }

		public async ETTask Reload(Type configType)
        {
            this.allConfig.TryGetValue(configType, out IConfigSingleton oneConfig);
            if (oneConfig != null)
            {
                oneConfig.Destroy();
            }
            ByteBuf oneConfigBytes =
					await EventSystem.Instance.Invoke<GetOneConfigBytes, ETTask<ByteBuf>>(new GetOneConfigBytes() { ConfigName = configType.Name });

            // object category = MongoHelper.Deserialize(configType, oneConfigBytes, 0, oneConfigBytes.Length);
            // ASingleton singleton = category as ASingleton;

            object category = Activator.CreateInstance(configType, oneConfigBytes);
            IConfigSingleton singleton = category as IConfigSingleton;
            singleton.Register();

            this.allConfig[configType] = singleton;
			
			// World.Instance.AddSingleton(singleton);
		}
		
		public async ETTask LoadAsync()
		{
			this.allConfig.Clear();
			Dictionary<Type, ByteBuf> configBytes = await EventSystem.Instance.Invoke<GetAllConfigBytes, ETTask<Dictionary<Type, ByteBuf>>>(new GetAllConfigBytes());

			using ListComponent<Task> listTasks = ListComponent<Task>.Create();
			
			foreach (Type type in configBytes.Keys)
			{
                ByteBuf oneConfigBytes = configBytes[type];
				Task task = Task.Run(() => LoadOneInThread(type, oneConfigBytes));
				listTasks.Add(task);
			}

			await Task.WhenAll(listTasks.ToArray());
            /*foreach (IConfigSingleton category in this.allConfig.Values)
            {
                category.Register();
            }*/

            foreach (IConfigSingleton category in this.allConfig.Values)
            {
                category.Resolve(allConfig);
            }
        }
		
		private void LoadOneInThread(Type configType, ByteBuf oneConfigBytes)
        {
            object category = Activator.CreateInstance(configType, oneConfigBytes);
            IConfigSingleton singleton = category as IConfigSingleton;
            singleton.Register();
            lock (this)
            {
                this.allConfig[configType] = category as IConfigSingleton;
            }
            /*object category = MongoHelper.Deserialize(configType, oneConfigBytes, 0, oneConfigBytes.Length);
			
			lock (this)
			{
				ASingleton singleton = category as ASingleton;
				this.allConfig[configType] = singleton;
				
				World.Instance.AddSingleton(singleton);
			}*/
		}

        public object LoadOneConfig(Type configType)
        {
            this.allConfig.TryGetValue(configType, out IConfigSingleton oneConfig);
            if (oneConfig != null)
            {
                oneConfig.Destroy();
            }

            ByteBuf oneConfigBytes = EventSystem.Instance.Invoke<GetOneConfigBytes, ByteBuf>(new GetOneConfigBytes() { ConfigName = configType.Name });

            object category = Activator.CreateInstance(configType, oneConfigBytes);
            IConfigSingleton singleton = category as IConfigSingleton;
            singleton.Register();

            this.allConfig[configType] = singleton;
            return category;
        }


        public void TranslateText(Func<string, string, string> translator)
        {
            foreach (IConfigSingleton category in this.allConfig.Values)
            {
                category.TranslateText(translator);
            }
        }
    }
}