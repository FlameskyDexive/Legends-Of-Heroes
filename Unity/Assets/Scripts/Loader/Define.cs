using YooAsset;

namespace ET
{
    public enum ELanguageType
    {
        Chinese = 0,
        English = 1,
    }

    public class CacheKeys
    {
        public static string Account { get => "Account"; }
        public static string Password { get => "Password"; }
        public static string CurLangType { get => "CurLangType"; }
        public static string KeyCodeSetting { get => "KeyCodeSetting"; }
        public static string MusicVolume { get => "MusicVolume"; }
        public static string SoundVolume { get => "SoundVolume"; }
        public static string Guidance { get => "Guidance"; }
    }


    public static partial class Define
    {
        /// <summary>
        /// 编辑器下加载热更dll的目录
        /// </summary>
        public const string CodeDir = "Assets/Bundles/Code/";

        /// <summary>
        /// VS或Rider工程生成dll的所在目录, 使用HybridCLR打包时需要使用
        /// </summary>
        public const string BuildOutputDir = "Temp/Bin/Debug";

        public static EPlayMode PlayMode;

#if DEBUG
        public static bool IsDebug = true;
#else
        public static bool IsDebug = false;
#endif

#if UNITY_EDITOR && !ASYNC
        public static bool IsAsync = false;
#else
        public static bool IsAsync = true;
#endif

#if UNITY_EDITOR
        public static bool IsEditor = true;
#else
        public static bool IsEditor = false;
#endif

#if ENABLE_DLL
        public static bool EnableDll = true;
#else
        public static bool EnableDll = false;
#endif

#if ENABLE_VIEW
        public static bool EnableView = true;
#else
        public static bool EnableView = false;
#endif

#if ENABLE_IL2CPP
        public static bool EnableIL2CPP = true;
#else
        public static bool EnableIL2CPP = false;
#endif
    }
}