using System;
using System.IO;
using System.Linq;

namespace YooAsset.Editor
{
    /// <summary>
    /// 收集预制体资源+所有图片
    /// 没有图集
    /// </summary>
    [DisplayName("YIUI_预制体+所有图片")]
    public class YIUIFilterRule : IAssetFilterRule
    {
        public string FindAssetType => EAssetFilterType.All.ToString();

        public bool IsCollectAsset(AssetFilterRuleData data)
        {
            if (Path.GetExtension(data.AssetPath) == ".prefab")
            {
                return data.AssetPath.Contains("/Prefabs/");
            }

            if (data.AssetPath.IndexOf("/Sprites/", StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 指定文件下的所有资源,不包含子文件夹内的资源
    /// </summary>
    [DisplayName("YIUI_根文件")]
    public class YIUIFilterRule_Root : IAssetFilterRule
    {
        public string FindAssetType => EAssetFilterType.All.ToString();

        public bool IsCollectAsset(AssetFilterRuleData data)
        {
            var path = data.AssetPath;
            var collectPath = data.CollectPath;

            var relativePath = path.Substring(collectPath.Length).TrimStart('/', '\\');

            if (!relativePath.Contains('/') && !relativePath.Contains('\\'))
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 只收集预制体资源
    /// </summary>
    [DisplayName("YIUI_预制体")]
    public class YIUIFilterRule_Prefab : IAssetFilterRule
    {
        public string FindAssetType => EAssetFilterType.Prefab.ToString();

        public bool IsCollectAsset(AssetFilterRuleData data)
        {
            if (Path.GetExtension(data.AssetPath) == ".prefab")
            {
                return data.AssetPath.Contains("/Prefabs/");
            }

            return false;
        }
    }

    /// <summary>
    /// 只收集图片资源
    /// </summary>
    [DisplayName("YIUI_图片")]
    public class YIUIFilterRule_Sprite : IAssetFilterRule
    {
        public string FindAssetType => EAssetFilterType.Sprite.ToString();

        public bool IsCollectAsset(AssetFilterRuleData data)
        {
            if (data.AssetPath.IndexOf("/Sprites/", StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 只收集图集资源
    /// </summary>
    [DisplayName("YIUI_图集")]
    public class YIUIFilterRule_Atlas : IAssetFilterRule
    {
        public string FindAssetType => EAssetFilterType.All.ToString();

        public bool IsCollectAsset(AssetFilterRuleData data)
        {
            if (data.AssetPath.IndexOf("/Atlas/", StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 只收集没有图集的图片
    /// </summary>
    [DisplayName("YIUI_没有图集的图片")]
    public class YIUIFilterRule_NoAtlas_Sprite : IAssetFilterRule
    {
        public string FindAssetType => EAssetFilterType.Sprite.ToString();

        public bool IsCollectAsset(AssetFilterRuleData data)
        {
            var path = data.AssetPath;
            var spritesIndex = path.IndexOf("/Sprites/", StringComparison.Ordinal);
            if (spritesIndex < 0)
            {
                return false;
            }

            if (path.IndexOf("/Sprites/AtlasIgnore/", StringComparison.Ordinal) >= 0)
            {
                return true; //忽略图集的图片
            }

            var relativePath = path.Substring(spritesIndex + "/Sprites/".Length);
            int slashIndex = relativePath.IndexOf('/');
            if (slashIndex < 0)
            {
                return true; // Sprites根目录下的图片，默认收集
            }

            var spritesDir = path.Substring(0, spritesIndex + "/Sprites/".Length - 1);
            var atlasDir = spritesDir.Replace("/Sprites", "/Atlas");

            if (!Directory.Exists(atlasDir))
            {
                return true;
            }

            var folderName = relativePath.Substring(0, slashIndex);

            var atlasFileNames = Directory.GetFiles(atlasDir).Where(file => !file.EndsWith(".meta")).ToArray();

            foreach (var atlasPath in atlasFileNames)
            {
                var atlasFileName = Path.GetFileNameWithoutExtension(atlasPath);
                if (atlasFileName.EndsWith("_" + folderName, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
