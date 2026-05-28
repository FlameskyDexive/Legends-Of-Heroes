using System;

namespace YooAsset
{
    /// <summary>
    /// 编辑器模拟构建的调用入口
    /// </summary>
    public static class EditorSimulateBuildInvoker
    {
        /// <summary>
        /// 执行编辑器模拟构建
        /// </summary>
        /// <param name="packageName">包裹名称</param>
        /// <param name="buildBundleType">构建资源包类型</param>
        /// <returns>构建结果</returns>
        /// <remarks>
        /// 此方法通过反射调用 ET.YooAssets.Editor 程序集中的 YooAsset.Editor.BundleSimulateBuilder，存在隐式耦合。
        /// （C# namespace 仍是 YooAsset.Editor；asmdef 名已重命名为 ET.YooAssets.Editor 以避免与上游冲突。）
        /// </remarks>
        public static PackageBuildResult Build(string packageName, int buildBundleType)
        {
            if (packageName == null)
                throw new ArgumentNullException(nameof(packageName));

#if UNITY_EDITOR
            var buildParam = new PackageBuildParameters(packageName);
            buildParam.BuildPipelineName = "EditorSimulateBuildPipeline";
            buildParam.BuildBundleType = buildBundleType;
            buildParam.AssemblyName = "ET.YooAssets.Editor";
            buildParam.TypeFullName = "YooAsset.Editor.BundleSimulateBuilder";
            buildParam.MethodName = "SimulateBuild";
            return PackageBuildInvoker.InvokeBuild(buildParam);
#else
            throw new PlatformNotSupportedException("This feature is only supported on the Unity Editor platform.");
#endif
        }
    }
}