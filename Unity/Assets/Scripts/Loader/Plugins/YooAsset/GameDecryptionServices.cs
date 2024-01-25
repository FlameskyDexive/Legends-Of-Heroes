using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

namespace ET
{
    /// <summary>
    /// 资源文件解密服务类
    /// </summary>
    public class GameDecryptionServices : IDecryptionServices
    {
        public AssetBundle LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return null;
        }

        /// <summary>
        /// 异步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        public AssetBundleCreateRequest LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return null;
        }
        public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            return 32;
        }

        public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }

        public Stream LoadFromStream(DecryptFileInfo fileInfo)
        {
            BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return bundleStream;
        }

        public uint GetManagedReadBufferSize()
        {
            return 1024;
        }
    }
}
