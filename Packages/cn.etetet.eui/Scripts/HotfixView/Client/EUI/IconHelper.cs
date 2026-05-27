using System;
using UnityEngine;
using UnityEngine.U2D;

namespace ET.Client
{
    public static class IconHelper
    {
        public static Sprite LoadIconSprite(Scene scene, string atlasName, string spriteName)
        {
            try
            {
                SpriteAtlas spriteAtlas = scene.GetComponent<ResourcesLoaderComponent>().LoadAssetSync<SpriteAtlas>(atlasName);
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

        public static async ETTask<Sprite> LoadIconSpriteAsync(Scene scene, string atlasName, string spriteName)
        {
            try
            {
                SpriteAtlas spriteAtlas = await scene.GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<SpriteAtlas>(atlasName);
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
