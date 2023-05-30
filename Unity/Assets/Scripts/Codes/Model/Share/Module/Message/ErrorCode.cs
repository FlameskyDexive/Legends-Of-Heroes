namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误

        // 110000以下的错误请看ErrorCore.cs

        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常


        public const int ERR_ResourceInitError = 300000;            // 资源初始化失败
        public const int ERR_ResourceUpdateVersionError = 300001;   // 资源更新版本号失败
        public const int ERR_ResourceUpdateManifestError = 300002;  // 资源更新清单失败
        public const int ERR_ResourceUpdateDownloadError = 300003;  // 资源更新下载失败
        
        
        public const int ERR_LoginError = 400003;  // 登录失败
        
        
        
    }
}