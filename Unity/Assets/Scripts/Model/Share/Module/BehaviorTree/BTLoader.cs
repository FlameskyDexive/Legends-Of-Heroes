using System;
using System.Collections.Generic;

namespace ET
{
    [Code]
    public class BTLoader : Singleton<BTLoader>, ISingletonAwake
    {
        public const string ClientBehaviorTreeBytesDir = BTBytesLoader.ClientBehaviorTreeBytesDir;
        public const string ServerBehaviorTreeBytesDir = BTBytesLoader.ServerBehaviorTreeBytesDir;
        public const string BTAssetDir = BTBytesLoader.BTAssetDir;

        private readonly Dictionary<string, BTPackage> packageCache = new(StringComparer.OrdinalIgnoreCase);

        public void Awake()
        {
        }

        public async ETTask<byte[]> LoadBytesAsync(string treeName, bool useCache = true)
        {
            return await BTBytesLoader.Instance.LoadBytesAsync(treeName, useCache);
        }

        public byte[] LoadBytes(string treeName, bool useCache = true)
        {
            return BTBytesLoader.Instance.LoadBytes(treeName, useCache);
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
                this.packageCache.Clear();
                BTBytesLoader.Instance?.Clear();
                return;
            }

            this.packageCache.Remove(treeName);
            BTBytesLoader.Instance?.Clear(treeName);
        }
    }
}
