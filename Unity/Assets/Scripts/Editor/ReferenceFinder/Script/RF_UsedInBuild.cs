using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace ReferenceFinder
{
    internal class RF_UsedInBuild : IRefDraw
    {
        private readonly RF_RefDrawer drawer;
        private readonly RF_TreeUI2.GroupDrawer groupDrawer;

        private bool dirty;
        internal Dictionary<string, RF_Ref> refs;

        public RF_UsedInBuild(IWindow window, Func<RF_RefDrawer.Sort> getSortMode, Func<RF_RefDrawer.Mode> getGroupMode)
        {
            this.window = window;
            drawer = new RF_RefDrawer(window, getSortMode, getGroupMode)
            {
                messageNoRefs = "No scene enabled in Build Settings!"
            };

            dirty = true;
            drawer.SetDirty();
        }

        public IWindow window { get; set; }


        public int ElementCount()
        {
            return refs?.Count ?? 0;
        }

        public bool Draw(Rect rect)
        {
            if (dirty) RefreshView();
            return drawer.Draw(rect);
        }

        public bool DrawLayout()
        {
            if (dirty) RefreshView();
            return drawer.DrawLayout();
        }

        public void SetDirty()
        {
            dirty = true;
            drawer.SetDirty();
        }

        public void RefreshView()
        {
            var scenes = new HashSet<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene == null) continue;
                if (scene.enabled == false) continue;
                string sce = AssetDatabase.AssetPathToGUID(scene.path);
                if (scenes.Contains(sce)) continue;
                scenes.Add(sce);
            }

            refs = new Dictionary<string, RF_Ref>();
            var directRefs = RF_Ref.FindUsage(scenes.ToArray());
            foreach (string scene in scenes)
            {
                if (!directRefs.TryGetValue(scene, out RF_Ref asset)) continue;
                asset.depth = 1;
            }

            List<RF_Asset> list = RF_Cache.Api.AssetList;
            int count = list.Count;

            // Collect assets in Resources / Streaming Assets
            for (var i = 0; i < count; i++)
            {
                RF_Asset item = list[i];
                if (item.inEditor) continue;
                if (item.IsExcluded) continue;
                if (item.IsFolder) continue;
                if (!item.assetPath.StartsWith("Assets/", StringComparison.Ordinal)) continue;

                if (item.inResources || item.inStreamingAsset || item.inPlugins || item.forcedIncludedInBuild
                    || !string.IsNullOrEmpty(item.AssetBundleName)
                    || !string.IsNullOrEmpty(item.AtlasName))
                {
                    if (refs.ContainsKey(item.guid)) continue;
                    refs.Add(item.guid, new RF_Ref(0, 1, item, null));
                }
            }

            // Collect direct references
            foreach (var kvp in directRefs)
            {
                var item = kvp.Value.asset;
                if (item.inEditor) continue;
                if (item.IsExcluded) continue;
                if (!item.assetPath.StartsWith("Assets/", StringComparison.Ordinal)) continue;
                if (refs.ContainsKey(item.guid)) continue;
                refs.Add(item.guid, new RF_Ref(0, 1, item, null));
            }

            drawer.SetRefs(refs);
            dirty = false;
        }

        internal void RefreshSort()
        {
            drawer.RefreshSort();
        }
    }
}
