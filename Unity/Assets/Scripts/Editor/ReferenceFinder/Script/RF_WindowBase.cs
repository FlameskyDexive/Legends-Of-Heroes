using UnityEditor;
using UnityEngine;
namespace ReferenceFinder
{
    public interface IWindow
    {
        bool WillRepaint { get; set; }
        void Repaint();
        void OnSelectionChange();
    }

    internal interface IRefDraw
    {
        IWindow window { get; }
        int ElementCount();
        bool DrawLayout();
        bool Draw(Rect rect);
    }

    public abstract class RF_WindowBase : EditorWindow, IWindow
    {
        public bool WillRepaint { get; set; }
        protected bool showFilter, showIgnore;

        //[NonSerialized] protected bool lockSelection;
        //[NonSerialized] internal List<RF_Asset> Selected;

        public static bool isNoticeIgnore;

        public void AddItemsToMenu(GenericMenu menu)
        {
            RF_Cache api = RF_Cache.Api;
            if (api == null) return;

            menu.AddDisabledItem(RF_GUIContent.FromString("RF - v2.5.11"));
            menu.AddSeparator(string.Empty);

            menu.AddItem(RF_GUIContent.FromString("Enable"), !RF_SettingExt.disable, () => { RF_SettingExt.disable = !RF_SettingExt.disable; });
            menu.AddItem(RF_GUIContent.FromString("Refresh"), false, () =>
            {
                //RF_Asset.lastRefreshTS = Time.realtimeSinceStartup;
                Resources.UnloadUnusedAssets();
                EditorUtility.UnloadUnusedAssetsImmediate();
                RF_Cache.Api.Check4Changes(true);
                RF_SceneCache.Api.SetDirty();
            });

#if RF_DEV
            menu.AddItem(RF_GUIContent.FromString("Refresh Usage"), false, () => RF_Cache.Api.Check4Usage());
            menu.AddItem(RF_GUIContent.FromString("Refresh Selected"), false, ()=> RF_Cache.Api.RefreshSelection());
            menu.AddItem(RF_GUIContent.FromString("Clear Cache"), false, () => RF_Cache.Api.Clear());
#endif
        }

        public abstract void OnSelectionChange();
        protected abstract void OnGUI();

#if UNITY_2018_OR_NEWER
        protected void OnSceneChanged(Scene arg0, Scene arg1)
        {
            if (IsFocusingFindInScene || IsFocusingSceneToAsset || IsFocusingSceneInScene)
            {
                OnSelectionChange();
            }
        }
#endif  
        
        protected bool DrawEnable()
        {
            RF_Cache api = RF_Cache.Api;
            if (api == null) return false;
            if (!RF_SettingExt.disable) return true;

            bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
            string message = isPlayMode
                ? "Find References 2 is disabled in play mode!"
                : "Find References 2 is disabled!";
            
            EditorGUILayout.HelpBox(RF_GUIContent.From(message, RF_Icon.Warning.image));
            if (GUILayout.Button(RF_GUIContent.FromString("Enable")))
            {
                RF_SettingExt.disable = !RF_SettingExt.disable;
                Repaint();
            }
            
            return false;
        }

    }
}
