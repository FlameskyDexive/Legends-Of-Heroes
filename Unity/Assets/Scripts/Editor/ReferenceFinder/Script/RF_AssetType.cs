using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace ReferenceFinder
{
    internal class AssetType
    {
        // ------------------------------- STATIC -----------------------------

        internal static readonly AssetType[] FILTERS =
        {
            new AssetType("Scene", ".unity"),
            new AssetType("Prefab", ".prefab"),
            new AssetType("Model", ".3df", ".3dm", ".3dmf", ".3dv", ".3dx", ".c5d", ".lwo", ".lws", ".ma", ".mb",
                ".mesh", ".vrl", ".wrl", ".wrz", ".fbx", ".dae", ".3ds", ".dxf", ".obj", ".skp", ".max", ".blend"),
            new AssetType("Material", ".mat", ".cubemap", ".physicsmaterial"),
            new AssetType("Texture", ".ai", ".apng", ".png", ".bmp", ".cdr", ".dib", ".eps", ".exif", ".ico", ".icon",
                ".j", ".j2c", ".j2k", ".jas", ".jiff", ".jng", ".jp2", ".jpc", ".jpe", ".jpeg", ".jpf", ".jpg", "jpw",
                "jpx", "jtf", ".mac", ".omf", ".qif", ".qti", "qtif", ".tex", ".tfw", ".tga", ".tif", ".tiff", ".wmf",
                ".psd", ".exr", ".rendertexture"),
            new AssetType("Video", ".asf", ".asx", ".avi", ".dat", ".divx", ".dvx", ".mlv", ".m2l", ".m2t", ".m2ts",
                ".m2v", ".m4e", ".m4v", "mjp", ".mov", ".movie", ".mp21", ".mp4", ".mpe", ".mpeg", ".mpg", ".mpv2",
                ".ogm", ".qt", ".rm", ".rmvb", ".wmv", ".xvid", ".flv"),
            new AssetType("Audio", ".mp3", ".wav", ".ogg", ".aif", ".aiff", ".mod", ".it", ".s3m", ".xm"),
            new AssetType("Script", ".cs", ".js", ".boo", ".h"),
            new AssetType("Text", ".txt", ".json", ".xml", ".bytes", ".sql"),
            new AssetType("Shader", ".shader", ".cginc"),
            new AssetType("Animation", ".anim", ".controller", ".overridecontroller", ".mask"),
            new AssetType("Unity Asset", ".asset", ".guiskin", ".flare", ".fontsettings", ".prefs", ".playable", ".signal"),
            new AssetType("Others") //
        };

        private static RF_Ignore _ignore;
        public HashSet<string> extension;
        public string name;

        public AssetType(string name, params string[] exts)
        {
            this.name = name;
            extension = new HashSet<string>();
            for (var i = 0; i < exts.Length; i++)
            {
                extension.Add(exts[i]);
            }
        }

        private static RF_Ignore ignore
        {
            get
            {
                if (_ignore == null) _ignore = new RF_Ignore();

                return _ignore;
            }
        }

        public static int GetIndex(string ext)
        {
            for (var i = 0; i < FILTERS.Length - 1; i++)
            {
                if (FILTERS[i].extension.Contains(ext)) return i;
            }

            return FILTERS.Length - 1; //Others
        }

        public static bool DrawSearchFilter()
        {
            int n = FILTERS.Length;
            var nCols = 4;
            int nRows = Mathf.CeilToInt(n / (float)nCols);
            var result = false;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("All", EditorStyles.toolbarButton) && !RF_Setting.IsIncludeAllType())
                {
                    RF_Setting.IncludeAllType();
                    result = true;
                }

                if (GUILayout.Button("None", EditorStyles.toolbarButton) && (RF_Setting.GetExcludeType() != -1))
                {
                    RF_Setting.ExcludeAllType();
                    result = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            for (var i = 0; i < nCols; i++)
            {
                GUILayout.BeginVertical();
                for (var j = 0; j < nRows; j++)
                {
                    int idx = i * nCols + j;
                    if (idx >= n) break;

                    bool s = !RF_Setting.IsTypeExcluded(idx);
                    bool s1 = GUILayout.Toggle(s, FILTERS[idx].name);
                    if (s1 != s)
                    {
                        result = true;
                        RF_Setting.ToggleTypeExclude(idx);
                    }
                }

                GUILayout.EndVertical();
                if ((i + 1) * nCols >= n) break;
            }

            GUILayout.EndHorizontal();

            return result;
        }

        public static void SetDirtyIgnore()
        {
            ignore.SetDirty();
        }

        public static bool DrawIgnoreFolder()
        {
            var change = false;
            ignore.Draw();




            // RF_Helper.GuiLine();
            // List<string> lst = RF_Setting.IgnoreFolder.ToList();
            // bool change = false;
            // pos = EditorGUILayout.BeginScrollView(pos);
            // for(int i =0; i < lst.Count; i++)
            // {
            // 	GUILayout.BeginHorizontal();
            // 	{
            // 		if(GUILayout.Button("X", GUILayout.Width(30)))
            // 		 {
            // 			 change = true;
            // 			 RF_Setting.RemoveIgnore(lst[i]);
            // 		 }
            // 		 GUILayout.Label(lst[i]);
            // 	}GUILayout.EndHorizontal();
            // }
            // EditorGUILayout.EndScrollView();
            return change;
        }

        private class RF_Ignore
        {
            private readonly RF_TreeUI2.GroupDrawer groupIgnore;
            private bool dirty;
            private Dictionary<string, RF_Ref> refs;

            public RF_Ignore()
            {
                groupIgnore = new RF_TreeUI2.GroupDrawer(DrawGroup, DrawItem)
                {
                    hideGroupIfPossible = true
                };

                ApplyFiter();
            }

            private void DrawItem(Rect r, string guid)
            {
                RF_Ref rf;
                if (!refs.TryGetValue(guid, out rf)) return;

                if (rf.depth == 1) //mode != Mode.Dependency && 
                {
                    Color c = GUI.color;
                    GUI.color = Color.blue;
                    GUI.DrawTexture(new Rect(r.x - 4f, r.y + 2f, 2f, 2f), EditorGUIUtility.whiteTexture);
                    GUI.color = c;
                }

                rf.asset.Draw(r, false,
                    true,
                    false, false, false, false);

                Rect drawR = r;
                drawR.x = drawR.x + drawR.width - 50f; // (groupDrawer.TreeNoScroll() ? 60f : 70f) ;
                drawR.width = 30;
                drawR.y += 1;
                drawR.height -= 2;

                if (GUI.Button(drawR, "X", EditorStyles.miniButton)) RF_Setting.RemoveIgnore(rf.asset.assetPath);
            }

            private void DrawGroup(Rect r, string id, int childCound)
            {
                GUI.Label(r, RF_GUIContent.FromString(id), EditorStyles.boldLabel);
                if (childCound <= 1) return;

                Rect drawR = r;
                drawR.x = drawR.x + drawR.width - 50f; // (groupDrawer.TreeNoScroll() ? 60f : 70f) ;
                drawR.width = 30;
                drawR.y += 1;
                drawR.height -= 2;
            }

            public void SetDirty()
            {
                dirty = true;
            }

            //private float sizeRatio {
            //    get{
            //        if(RF_Window.window != null)
            //            return RF_Window.window.sizeRatio;
            //        return .3f;
            //    }
            //}

            public void Draw()
            {
                if (dirty) ApplyFiter();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(4f);
                    Object[] drops = GUI2.DropZone("Drag & Drop folders here to exclude", 100, 95);
                    if ((drops != null) && (drops.Length > 0))
                    {
                        for (var i = 0; i < drops.Length; i++)
                        {
                            string path = AssetDatabase.GetAssetPath(drops[i]);
                            if (path.Equals(RF_Cache.DEFAULT_CACHE_PATH)) continue;
                            RF_Setting.AddIgnore(path);
                        }
                    }

                    groupIgnore.DrawLayout();
                }
                GUILayout.EndHorizontal();
            }


            private void ApplyFiter()
            {
                dirty = false;
                refs = new Dictionary<string, RF_Ref>();

                //foreach (KeyValuePair<string, List<string>> item in RF_Setting.IgnoreFiltered)
                foreach (string item2 in RF_Setting.s.listIgnore)
                {
                    string guid = AssetDatabase.AssetPathToGUID(item2);
                    if (string.IsNullOrEmpty(guid)) continue;

                    RF_Asset asset = RF_Cache.Api.Get(guid, true);
                    var r = new RF_Ref(0, 0, asset, null, "Ignore");
                    refs.Add(guid, r);
                }

                groupIgnore.Reset
                (
                    refs.Values.ToList(),
                    rf => rf.asset != null ? rf.asset.guid : "",
                    GetGroup,
                    SortGroup
                );
            }

            private string GetGroup(RF_Ref rf)
            {
                return "Ignore";
            }

            private void SortGroup(List<string> groups)
            { }
        }
    }
}
