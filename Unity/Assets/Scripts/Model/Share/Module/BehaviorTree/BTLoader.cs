using System;
using System.Collections.Generic;

namespace ET
{
    [Code]
    public class BTLoader : Singleton<BTLoader>, ISingletonAwake
    {
        public const string ClientBehaviorTreeBytesDir = "Assets/Bundles/AI/Bytes";
        public const string ServerBehaviorTreeBytesDir = "Config/AI";
        public const string BTAssetDir = "Assets/Bundles/AI";

        public struct GetOneBehaviorTreeBytes
        {
            public string TreeName;
        }

        private readonly Dictionary<string, byte[]> bytesCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, BTPackage> packageCache = new(StringComparer.OrdinalIgnoreCase);

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

            byte[] bytes = await EventSystem.Instance.Invoke<GetOneBehaviorTreeBytes, ETTask<byte[]>>(new GetOneBehaviorTreeBytes()
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

            byte[] bytes = EventSystem.Instance.Invoke<GetOneBehaviorTreeBytes, byte[]>(new GetOneBehaviorTreeBytes()
            {
                TreeName = treeName,
            });

            if (bytes != null && bytes.Length > 0 && useCache)
            {
                this.bytesCache[treeName] = bytes;
            }

            return bytes;
        }

        public async ETTask<BTPackage> LoadPackageAsync(string treeName, bool useCache = true)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                Log.Error("behavior tree name is empty");
                return null;
            }

            if (useCache && this.packageCache.TryGetValue(treeName, out BTPackage cachePackage))
            {
                return cachePackage;
            }

            byte[] bytes = await this.LoadBytesAsync(treeName, useCache);
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            BTPackage package = BTSerializer.Deserialize(bytes);
            if (package != null && useCache)
            {
                this.packageCache[treeName] = package;
            }

            return package;
        }

        public BTPackage LoadPackage(string treeName, bool useCache = true)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                Log.Error("behavior tree name is empty");
                return null;
            }

            if (useCache && this.packageCache.TryGetValue(treeName, out BTPackage cachePackage))
            {
                return cachePackage;
            }

            byte[] bytes = this.LoadBytes(treeName, useCache);
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            BTPackage package = BTSerializer.Deserialize(bytes);
            if (package != null && useCache)
            {
                this.packageCache[treeName] = package;
            }

            return package;
        }

        public void Clear(string treeName = "")
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                this.bytesCache.Clear();
                this.packageCache.Clear();
                return;
            }

            this.bytesCache.Remove(treeName);
            this.packageCache.Remove(treeName);
        }
    }
}
