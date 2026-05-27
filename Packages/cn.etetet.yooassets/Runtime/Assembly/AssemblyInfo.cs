using System.Runtime.CompilerServices;

// 内部友元（asmdef 已重命名为 ET.YooAssets / ET.YooAssets.Editor）
[assembly: InternalsVisibleTo("ET.YooAssets.Editor")]
[assembly: InternalsVisibleTo("ET.YooAssets.Tests")]
[assembly: InternalsVisibleTo("ET.YooAssets.Tests.Editor")]

// 外部友元
[assembly: InternalsVisibleTo("YooAsset.MiniGame")]
[assembly: InternalsVisibleTo("YooAsset.Extension")]
[assembly: InternalsVisibleTo("YooAsset.Extension.Editor")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]