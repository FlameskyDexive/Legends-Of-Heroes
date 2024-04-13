using UnityEditor;
using UnityEngine;
using System.IO;
using YooAsset;
using System;
using System.Collections.Generic;
using HybridCLR.Editor.Commands;
using UnityEditor.Build.Reporting;
using YooAsset.Editor;

namespace ET
{
    public enum PlatformType
    {
        None,
        Android,
        IOS,
        Windows,
        MacOS,
        Linux
    }

    public enum ConfigFolder
    {
        Localhost,
        Release,
        RouterTest,
        Benchmark
    }

    /// <summary>
    /// ET菜单顺序
    /// </summary>
    public static class ETMenuItemPriority
    {
        public const int BuildTool = 1001;
        public const int ChangeDefine = 1002;
        public const int Compile = 1003;
        public const int Reload = 1004;
        public const int NavMesh = 1005;
        public const int ServerTools = 1006;
    }

    public class BuildEditor : EditorWindow
    {
        private PlatformType activePlatform;
        private PlatformType platformType;
        private ConfigFolder configFolder;
        private bool clearFolder;
        private BuildOptions buildOptions;
        private BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None;

        private GlobalConfig globalConfig;

        [MenuItem("ET/Build Tool", false, ETMenuItemPriority.BuildTool)]
        public static void ShowWindow()
        {
            GetWindow<BuildEditor>(DockDefine.Types);
        }

        private void OnEnable()
        {
            // globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Resources/GlobalConfig.asset");
            globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Bundles/Config/GlobalConfig.asset");

#if UNITY_ANDROID
            activePlatform = PlatformType.Android;
#elif UNITY_IOS
            activePlatform = PlatformType.IOS;
#elif UNITY_STANDALONE_WIN
            activePlatform = PlatformType.Windows;
#elif UNITY_STANDALONE_OSX
            activePlatform = PlatformType.MacOS;
#elif UNITY_STANDALONE_LINUX
            activePlatform = PlatformType.Linux;
#else
            activePlatform = PlatformType.None;
#endif
            platformType = activePlatform;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("PlatformType ");
            this.platformType = (PlatformType)EditorGUILayout.EnumPopup(platformType);

            EditorGUILayout.LabelField("BuildOptions ");
            this.buildOptions = (BuildOptions)EditorGUILayout.EnumFlagsField(this.buildOptions);

            GUILayout.Space(5);

            if (GUILayout.Button("BuildPackage"))
            {
                if (this.platformType == PlatformType.None)
                {
                    Log.Error("please select platform!");
                    return;
                }

                if (this.globalConfig.CodeMode != CodeMode.Client)
                {
                    Log.Error("build package CodeMode must be CodeMode.Client, please select Client");
                    return;
                }

                // if (this.globalConfig.EPlayMode == EPlayMode.EditorSimulateMode)
                // {
                //     Log.Error("build package EPlayMode must not be EPlayMode.EditorSimulateMode, please select HostPlayMode");
                //     return;
                // }

                if (platformType != activePlatform)
                {
                    switch (EditorUtility.DisplayDialogComplex("Warning!", $"current platform is {activePlatform}, if change to {platformType}, may be take a long time", "change", "cancel", "no change"))
                    {
                        case 0:
                            activePlatform = platformType;
                            break;
                        case 1:
                            return;
                        case 2:
                            platformType = activePlatform;
                            break;
                    }
                }

                BuildHelper.Build(this.platformType, this.buildOptions);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            this.configFolder = (ConfigFolder)EditorGUILayout.EnumPopup(this.configFolder, GUILayout.Width(200f));
            if (GUILayout.Button("ExcelExporter"))
            {
                // ToolsEditor.ExcelExporter();
                
                ToolsEditor.ExcelExporter(this.globalConfig.CodeMode, this.configFolder);
                
                const string clientProtoDir = "../Unity/Assets/Bundles/Config/GameConfig";
                if (Directory.Exists(clientProtoDir))
                {
                    Directory.Delete(clientProtoDir, true);
                }
                FileHelper.CopyDirectory("../Config/Excel/c/GameConfig", clientProtoDir);
                
                AssetDatabase.Refresh();
                return;
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Proto2CS"))
            {
                ToolsEditor.Proto2CS();
                return;
            }

            GUILayout.Space(5);
        }


        public static void BuildAssetBundle()
        {
            string outputRoot = CommandLineReader.GetCustomArgument("outputRoot");
            if (string.IsNullOrEmpty(outputRoot))
            {
                Debug.LogError($"Build Asset Bundle Error！outputRoot is null");
                return;
            }

            string packageVersion = CommandLineReader.GetCustomArgument("packageVersion");
            if (string.IsNullOrEmpty(packageVersion))
            {
                Debug.LogError($"Build Asset Bundle Error！packageVersion is null");
                return;
            }

            string platform = CommandLineReader.GetCustomArgument("platform");
            if (string.IsNullOrEmpty(platform))
            {
                Debug.LogError($"Build Asset Bundle Error！platform is null");
                return;
            }

            BuildTarget target = GetBuildTarget(platform);
            BuildInternal(target, outputRoot);
            Debug.LogWarning($"Start BuildPackage BuildTarget:{target} outputPath:{outputRoot}");
        }

        private static BuildTarget GetBuildTarget(string platform)
        {
            BuildTarget target = BuildTarget.NoTarget;
            switch (platform)
            {
                case "Android":
                    target = BuildTarget.Android;
                    break;
                case "IOS":
                    target = BuildTarget.iOS;
                    break;
                case "Windows":
                    target = BuildTarget.StandaloneWindows64;
                    break;
                case "MacOS":
                    target = BuildTarget.StandaloneOSX;
                    break;
                case "Linux":
                    target = BuildTarget.StandaloneLinux64;
                    break;
                case "WebGL":
                    target = BuildTarget.WebGL;
                    break;
                case "Switch":
                    target = BuildTarget.Switch;
                    break;
                case "PS4":
                    target = BuildTarget.PS4;
                    break;
                case "PS5":
                    target = BuildTarget.PS5;
                    break;
            }

            return target;
        }

        private static void BuildInternal(BuildTarget buildTarget, string outputRoot, string packageVersion = "1.0")
        {
            Debug.Log($"开始构建 : {buildTarget}");
            string packageName = "DefaultPackage";

            // 构建参数
            ScriptableBuildParameters buildParameters = new ScriptableBuildParameters();
            // BuiltinBuildParameters buildParameters = new BuiltinBuildParameters();
            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = EBuildPipeline.ScriptableBuildPipeline.ToString();
            // buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline.ToString();
            buildParameters.BuildTarget = buildTarget;
            buildParameters.BuildMode = EBuildMode.IncrementalBuild;
            buildParameters.PackageName = packageName;
            buildParameters.PackageVersion = packageVersion;
            buildParameters.EnableSharePackRule = true;
            buildParameters.VerifyBuildingResult = true;
            buildParameters.FileNameStyle = AssetBundleBuilderSetting.GetPackageFileNameStyle(packageName, EBuildPipeline.ScriptableBuildPipeline);
            buildParameters.BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyAll;
            buildParameters.BuildinFileCopyParams = AssetBundleBuilderSetting.GetPackageBuildinFileCopyParams(packageName, EBuildPipeline.ScriptableBuildPipeline);
            // buildParameters.EncryptionServices = CreateEncryptionInstance();
            buildParameters.CompressOption = ECompressOption.LZMA;

            // 执行构建
            ScriptableBuildPipeline pipeline = new ScriptableBuildPipeline();
            // BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
            var buildResult = pipeline.Run(buildParameters, true);
            if (buildResult.Success)
            {
                Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
                // EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
            }
            else
            {
                Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
            }
        }

        [MenuItem("ET/Build/AssetBundle-Windows", false, 30)]
        public static void AutomationBuildAssetBundle()
        {
            AssemblyTool.DoCompile();
            AssetDatabase.Refresh();
            BuildInternal(BuildTarget.StandaloneWindows64, Application.dataPath + "/../Builds/Windows", GetBuildPackageVersion());
        }

        [MenuItem("ET/Build/一键打包Windows", false, 30)]
        public static void AutomationBuild()
        {
            AssemblyTool.DoCompile();
            if (Define.EnableIL2CPP)
            {
                CompileDllCommand.CompileDllActiveBuildTarget();
                PrebuildCommand.GenerateAll();
            }

            if (Define.EnableIL2CPP)
                HybridCLREditor.CopyAotDll();
            AssetDatabase.Refresh();
            string packageVersion = GetBuildPackageVersion();
            BuildInternal(BuildTarget.StandaloneWindows64, Application.dataPath + "/../Builds/Windows", packageVersion);
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, $"{Application.dataPath}/../Builds/Windows/Windows-{packageVersion}/ETPlus.exe");
        }

        // 构建版本相关
        private static string GetBuildPackageVersion()
        {
            int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }

        [MenuItem("ET/Build/一键打包Android", false, 30)]
        public static void AutomationBuildAndroid()
        {
            AssemblyTool.DoCompile();
            if (Define.EnableIL2CPP)
            {
                CompileDllCommand.CompileDllActiveBuildTarget();
                PrebuildCommand.GenerateAll();
            }

            if (Define.EnableIL2CPP)
                HybridCLREditor.CopyAotDll();
            AssetDatabase.Refresh();
            string packageVersion = GetBuildPackageVersion();
            BuildInternal(BuildTarget.Android, outputRoot: Application.dataPath + "/../Bundles", packageVersion);
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.Android, BuildTarget.Android, $"{Application.dataPath}/../Build/Android/{packageVersion}Android.apk");
        }

        [MenuItem("ET/Build/一键打包IOS", false, 30)]
        public static void AutomationBuildIOS()
        {
            AssemblyTool.DoCompile();
            if (Define.EnableIL2CPP)
            {
                CompileDllCommand.CompileDllActiveBuildTarget();
                PrebuildCommand.GenerateAll();
            }

            if (Define.EnableIL2CPP)
                HybridCLREditor.CopyAotDll();
            AssetDatabase.Refresh();
            BuildInternal(BuildTarget.iOS, outputRoot: Application.dataPath + "/../Bundles", packageVersion: GetBuildPackageVersion());
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.iOS, BuildTarget.iOS, $"{Application.dataPath}/../Build/IOS/XCode_Project");
        }

        public static void BuildImp(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, string locationPathName)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, BuildTarget.StandaloneWindows64);
            AssetDatabase.Refresh();
            var scenes = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    scenes.Add(scene.path);
                }
            }

            if (scenes.Count == 0)
            {
                Debug.Log("打包异常，尚未添加Scene");
                return;
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                // scenes = new[] { "Assets/Scenes/Init.unity" },
                scenes = scenes.ToArray(),
                locationPathName = locationPathName,
                targetGroup = buildTargetGroup,
                target = buildTarget,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"Build success: {summary.totalSize / 1024 / 1024} MB");
                EditorUtility.RevealInFinder(locationPathName);
            }
            else
            {
                Debug.Log($"Build Failed" + summary.result);
            }
        }
    }
}