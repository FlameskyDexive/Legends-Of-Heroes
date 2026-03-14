using System;
using System.Collections.Generic;

namespace ET
{
    public class BTBytesLoader : Singleton<BTBytesLoader>, ISingletonAwake
    {
        public const string ClientBehaviorTreeBytesDir = "Assets/Bundles/AI/Bytes";
        public const string ServerBehaviorTreeBytesDir = "Config/AI";
        public const string BTAssetDir = "Assets/Bundles/AI";

        public struct GetOneBehaviorTreeBytes
        {
            public string TreeName;
        }

        private readonly Dictionary<string, byte[]> bytesCache = new(StringComparer.OrdinalIgnoreCase);

        public void Awake()
        {
        }

        public async ETTask<byte[]> LoadBytesAsync(string treeName, bool useCache = true)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                Log.Error("behavior tree name is empty");
                return null;
            }

            if (useCache && this.bytesCache.TryGetValue(treeName, out byte[] cacheBytes))
            {
                return cacheBytes;
            }

            byte[] bytes = await EventSystem.Instance.Invoke<GetOneBehaviorTreeBytes, ETTask<byte[]>>(new GetOneBehaviorTreeBytes
            {
                TreeName = treeName,
            });

            if (bytes != null && bytes.Length > 0 && useCache)
            {
                this.bytesCache[treeName] = bytes;
            }

            return bytes;
        }

        public byte[] LoadBytes(string treeName, bool useCache = true)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                Log.Error("behavior tree name is empty");
                return null;
            }

            if (useCache && this.bytesCache.TryGetValue(treeName, out byte[] cacheBytes))
            {
                return cacheBytes;
            }

            byte[] bytes = EventSystem.Instance.Invoke<GetOneBehaviorTreeBytes, byte[]>(new GetOneBehaviorTreeBytes
            {
                TreeName = treeName,
            });

            if (bytes != null && bytes.Length > 0 && useCache)
            {
                this.bytesCache[treeName] = bytes;
            }

            return bytes;
        }

        public void Clear(string treeName = "")
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                this.bytesCache.Clear();
                return;
            }

            this.bytesCache.Remove(treeName);
        }
    }
}
