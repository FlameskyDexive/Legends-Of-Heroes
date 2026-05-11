using System.IO;
using YooAsset;

namespace ET
{
    /// <summary>
    /// 资源文件流加载解密类
    /// </summary>
    public class FileStreamDecryption : IBundleStreamDecryptor
    {
        Stream IBundleStreamDecryptor.CreateDecryptionStream(BundleDecryptArgs args)
        {
            return new BundleStream(args.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        int IBundleStreamDecryptor.GetBufferSize(BundleDecryptArgs args)
        {
            return 1024;
        }
    }

    /// <summary>
    /// 资源文件偏移加载解密类
    /// </summary>
    public class FileOffsetDecryption : IBundleOffsetDecryptor
    {
        long IBundleOffsetDecryptor.GetFileOffset(BundleDecryptArgs args)
        {
            return 32L;
        }
    }
}
