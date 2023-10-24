using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace ET.Client
{
    public static class IconHelper
    {
        /// <summary>
        /// 同步加载图集图片资源
        /// </summary>
        /// <OtherParam name="spriteName"></OtherParam>
        /// <returns></returns>
        public static Sprite LoadIconSprite(Scene scene, string atlasName,  string spriteName)
        {
            try
            {
                SpriteAtlas spriteAtlas = scene.GetComponent<ResourcesLoaderComponent>().LoadAssetSync<SpriteAtlas>(atlasName.StringToAB()); 
                Sprite sprite = spriteAtlas.GetSprite(spriteName);
                if ( null == sprite )
                {
                    Log.Error($"sprite is null: {spriteName}");
                }
                return sprite;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        /// <summary>
        /// 异步加载图集图片资源
        /// </summary>
        /// <OtherParam name="spriteName"></OtherParam>
        /// <returns></returns>
        public static async ETTask<Sprite> LoadIconSpriteAsync(Scene scene ,string atlasName,  string spriteName)
        {
            try
            {
                SpriteAtlas spriteAtlas = await scene.GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<SpriteAtlas>(atlasName.StringToAB()); 
                Sprite sprite = spriteAtlas.GetSprite(spriteName);
                if (null == sprite)
                {
                    Log.Error($"sprite is null: {spriteName}");
                }
                return sprite;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
        }
    }
}

