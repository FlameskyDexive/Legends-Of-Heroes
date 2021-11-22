using System;

namespace ET
{
    /// <summary>
    /// AB实用函数集，主要是路径拼接
    /// </summary>
    public static class AssetPathHelper
    {
        public static string GetTexturePath(string fileName)
        {
            return $"Assets/Bundles/Altas/{fileName}.prefab";
        }
        
        public static string ToUIPath(this string fileName)
        {
            return String.Format("Assets/Bundles/UI/Dlg/{0}.prefab" , fileName);
        }
        
        public static string ToUISpriteAtlasPath( this string fileName)
        {
            return $"Assets/Res/UIAtlas/{fileName}.spriteatlas";
        }
        
        public static string GetNormalConfigPath(string fileName)
        {
            return $"Assets/Bundles/Independent/{fileName}.prefab";
        }
 
        public static string ToSoundPath(this string fileName)
        {
            return String.Format("Assets/Bundles/Sound/{0}.mp3" , fileName);
        }
        
        public static string ToConfigPath(this string fileName)
        {
            return String.Format("Assets/Bundles/Config/{0}.bytes" , fileName);
        }
        
        public static string ToSQLiteDBPath(this string fileName)
        {
            return String.Format("Assets/Bundles/SQLiteDB/{0}.db" , fileName);
        }
 
        public static string GetSkillConfigPath(string fileName)
        {
            return $"Assets/Bundles/SkillConfigs/{fileName}.prefab";
        }
 
        public static string ToPrefabPath(this string fileName)
        {
            return String.Format("Assets/Bundles/Prefab/{0}.prefab" , fileName);
        }
 
        public static string GetScenePath(this string fileName)
        {
            //return $"Assets/Scenes/{fileName}.unity";
            return $"{fileName}.unity";
        }

        public static string ToUICommonPath(this string fileName)
        {
            return $"Assets/Bundles/UI/Common/{fileName}.prefab";
        }
        
        public static string ToUIItemPath(this string fileName)
        {
            return $"Assets/Bundles/UI/Item/{fileName}.prefab";
        }
        
        public static string ToUnitModelPath(this string fileName)
        {
            return $"Assets/Bundles/Unit/{fileName}.prefab";
        }

        public static string ToUnitHUDPath(this string fileName)
        {
            return $"Assets/Bundles/UnitHUD/{fileName}HUD.prefab";
        }
        
    }
}