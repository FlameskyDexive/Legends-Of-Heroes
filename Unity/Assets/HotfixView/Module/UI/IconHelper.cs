// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.U2D;
// using UnityEngine.UIElements;
// using Image = UnityEngine.UI.Image;
//
// namespace ET
// {
//     public static class IconHelper
//     {
//         private static string CommonAtlasName = "Common/Common"; // 文件夹/图集名
//         private static string IconAtlasName   = "Icon/CommonIcon/CommonIcon"; // 文件夹/图集名
//         
//         /// <summary>
//         /// 同步加载icon图片资源
//         /// </summary>
//         /// <OtherParam name="spriteName"></OtherParam>
//         /// <returns></returns>
//         public static Sprite LoadIconSprite(string spriteName)
//         {
//             try
//             {
//                 SpriteAtlas spriteAtlas = ET.Game.Scene.GetComponent<ResourcesComponent>().LoadAsset<SpriteAtlas>(IconAtlasName.ToUISpriteAtlasPath());
//                 Sprite sprite = spriteAtlas.GetSprite(spriteName);
//                 if ( null == sprite )
//                 {
//                     Log.Error($"sprite is null: {spriteName}");
//                 }
//                 return sprite;
//             }
//             catch (Exception e)
//             {
//                 Log.Error(e);
//                 return null;
//             }
//         }
//         
//         /// <summary>
//         /// 异步加载icon图片资源
//         /// </summary>
//         /// <OtherParam name="spriteName"></OtherParam>
//         /// <returns></returns>
//         public static async ETTask<Sprite> LoadIconSpriteAsync(string spriteName)
//         {
//             try
//             {
//                  SpriteAtlas spriteAtlas = await ET.Game.Scene.GetComponent<ResourcesComponent>()
//                      .LoadAssetAsync<SpriteAtlas>(IconAtlasName.ToUISpriteAtlasPath());
//                  Sprite sprite = spriteAtlas.GetSprite(spriteName);
//                  if ( null == sprite )
//                  {
//                      Log.Error($"sprite is null: {spriteName}");
//                  }
//                  return sprite;
//             }
//             catch (Exception e)
//             {
//                 Log.Error(e);
//                 return null;
//             }
//         }
//         
//         public static  void SetCommonSprite(this Image image, string SpriteName)
//         {
//             image.overrideSprite = LoadCommonSprite(SpriteName);
//         }
//         
//
//         /// <summary>
//         /// 同步加载common图片资源
//         /// </summary>
//         /// <OtherParam name="spriteName"></OtherParam>
//         /// <returns></returns>
//         public static Sprite LoadCommonSprite(string spriteName)
//         {
//             try
//             {
//                 SpriteAtlas spriteAtlas = ET.Game.Scene.GetComponent<ResourcesComponent>()
//                     .LoadAsset<SpriteAtlas>(CommonAtlasName.ToUISpriteAtlasPath());
//                 Sprite sprite = spriteAtlas.GetSprite(spriteName);
//                 if ( null == sprite )
//                 {
//                     Log.Error($"sprite is null: {spriteName}");
//                 }
//                 return sprite;
//             }
//             catch (Exception e)
//             {
//                 Log.Error(e);
//                 return null;
//             }
//         }
//
//
//         public static async ETVoid SetCommonSpriteAsync(this Image image, string SpriteName)
//         {
//             image.overrideSprite = await LoadCommonSpriteAsync(SpriteName);
//         }
//         
//         /// <summary>
//         /// 异步加载common图片资源
//         /// </summary>
//         /// <OtherParam name="spriteName"></OtherParam>
//         /// <returns></returns>
//         public static async ETTask<Sprite> LoadCommonSpriteAsync(string spriteName)
//         {
//             try
//             {
//                 SpriteAtlas spriteAtlas = await ET.Game.Scene.GetComponent<ResourcesComponent>()
//                     .LoadAssetAsync<SpriteAtlas>(CommonAtlasName.ToUISpriteAtlasPath());
//                 Sprite sprite = spriteAtlas.GetSprite(spriteName);
//                 if ( null == sprite )
//                 {
//                     Log.Error($"sprite is null: {spriteName}");
//                 }
//                 return sprite;
//             }
//             catch (Exception e)
//             {
//                 Log.Error(e);
//                 return null;
//             }
//         }
//
//         /// <summary>
//         /// 同步加载图片资源
//         /// </summary>
//         /// <OtherParam name="spriteName"></OtherParam>
//         /// <OtherParam name="atlasFolderAndName"></OtherParam>
//         /// <returns></returns>
//         public static Sprite LoadSprite(string spriteName , string atlasFolderAndName)
//         {
//             try
//             {
//                 SpriteAtlas spriteAtlas = ET.Game.Scene.GetComponent<ResourcesComponent>()
//                     .LoadAsset<SpriteAtlas>(atlasFolderAndName.ToUISpriteAtlasPath());
//                 Sprite sprite = spriteAtlas.GetSprite(spriteName);
//                 if ( null == sprite )
//                 {
//                     Log.Error($"sprite is null: {spriteName}");
//                 }
//                 return sprite;
//             }
//             catch (Exception e)
//             {
//                 Log.Error(e);
//                 return null;
//             }
//         }
//
//         /// <summary>
//         /// 异步加载图片资源
//         /// </summary>
//         /// <OtherParam name="spriteName"></OtherParam>
//         /// <OtherParam name="atlasFolderAndName"></OtherParam>
//         /// <returns></returns>
//         public static async ETTask<Sprite> LoadSpriteAsync(string spriteName , string atlasFolderAndName)
//         {
//             try
//             {
//                 SpriteAtlas spriteAtlas = await ET.Game.Scene.GetComponent<ResourcesComponent>()
//                     .LoadAssetAsync<SpriteAtlas>(atlasFolderAndName.ToUISpriteAtlasPath());
//                 Sprite sprite = spriteAtlas.GetSprite(spriteName);
//                 if ( null == sprite )
//                 {
//                     Log.Error($"sprite is null: {spriteName}");
//                 }
//                 return sprite;
//             }
//             catch (Exception e)
//             {
//                 Log.Error(e);
//                 return null;
//             }
//         }
//     }
// }
//
