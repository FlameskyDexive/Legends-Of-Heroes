using System;
using System.IO;
using YooAsset;

/// <summary>
/// 文件偏移加密方式
/// </summary>
public class FileOffsetEncryption : IBundleEncryptor
{
    public BundleEncryptResult Encrypt(BundleEncryptArgs args)
    {
        if (args.FilePath.Contains("_bundles"))
        {
            int offset = 32;
            byte[] fileData = File.ReadAllBytes(args.FilePath);
            var encryptedData = new byte[fileData.Length + offset];
            Buffer.BlockCopy(fileData, 0, encryptedData, offset, fileData.Length);

            return new BundleEncryptResult(true, encryptedData);
        }

        return new BundleEncryptResult(false, null);
    }
}

/// <summary>
/// 文件流加密方式
/// </summary>
public class FileStreamEncryption : IBundleEncryptor
{
    public BundleEncryptResult Encrypt(BundleEncryptArgs args)
    {
        if (args.FilePath.Contains("_bundles"))
        {
            var fileData = File.ReadAllBytes(args.FilePath);
            for (int i = 0; i < fileData.Length; i++)
            {
                fileData[i] ^= BundleStream.KEY;
            }

            return new BundleEncryptResult(true, fileData);
        }

        return new BundleEncryptResult(false, null);
    }
}
