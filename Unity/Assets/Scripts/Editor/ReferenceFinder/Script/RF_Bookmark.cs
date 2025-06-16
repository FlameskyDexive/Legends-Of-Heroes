using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace ReferenceFinder
{
    internal class RF_Bookmark : IRefDraw
    {
        internal static HashSet<string> guidSet = new HashSet<string>();
        internal static HashSet<string> instSet = new HashSet<string>(); // Do not reference directly to SceneObject (which might be destroyed anytime)

        // ------------ instance

        //private readonly RF_TreeUI2.GroupDrawer groupDrawer;
        private static bool dirty;
        private readonly RF_RefDrawer drawer;
        internal Dictionary<string, RF_Ref> refs = new Dictionary<string, RF_Ref>();

        public RF_Bookmark(IWindow window, Func<RF_RefDrawer.Sort> getSortMode, Func<RF_RefDrawer.Mode> getGroupMode)
        {
            this.window = window;
            drawer = new RF_RefDrawer(window, getSortMode, getGroupMode)
            {
                messageNoRefs = "Do bookmark something!",
                groupDrawer =
                {
                    hideGroupIfPossible = true
                },
                forceHideDetails = true,
                level0Group = string.Empty
            };

            dirty = true;
            drawer.SetDirty();
        }

        public static int Count => guidSet.Count + instSet.Count;

        public IWindow window { get; set; }

        public int ElementCount()
        {
            return refs == null ? 0 : refs.Count;
        }

        public bool DrawLayout()
        {
            if (dirty) RefreshView();
            return drawer.DrawLayout();
        }

        public bool Draw(Rect rect)
        {
            if (dirty) RefreshView();
            if (refs == null)
            {
                Debug.Log("Refs is null!");
                return false;
            }

            var bottomRect = new Rect(rect.x + 1f, rect.yMax - 16f, rect.width - 2f, 16f);
            DrawButtons(bottomRect);

            rect.yMax -= 16f;
            return drawer.Draw(rect);
        }

        public static bool Contains(string guidOrInstID)
        {
            return guidSet.Contains(guidOrInstID) || instSet.Contains(guidOrInstID);
        }

        public static bool Contains(UnityObject sceneObject)
        {
            var id = sceneObject.GetInstanceID().ToString();
            return instSet.Contains(id);
        }
        public static bool Contains(RF_Ref rf)
        {
            if (rf.isSceneRef)
            {
                if (instSet == null) return false;
                return instSet.Contains(rf.component.GetInstanceID().ToString());
            }
            if (guidSet == null) return false;
            return guidSet.Contains(rf.asset.guid);
        }
        public static void Add(UnityObject sceneObject)
        {
            if (sceneObject == null) return;
            var id = sceneObject.GetInstanceID().ToString();
            instSet.Add(id); // hashset does not need to check exist before add
            dirty = true;
        }

        public static void Add(string guid)
        {
            if (guidSet.Contains(guid)) return;
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning("Invalid GUID: " + guid);
                return;
            }

            guidSet.Add(guid);
            dirty = true;

            //Debug.Log(instSet.Count + " : " + guidSet.Count);
        }

        public static void Remove(UnityObject sceneObject)
        {
            if (sceneObject == null) return;
            var id = sceneObject.GetInstanceID().ToString();
            instSet.Remove(id);
            dirty = true;
        }

        public static void Remove(string guidOrInstID)
        {
            guidSet.Remove(guidOrInstID);
            instSet.Remove(guidOrInstID);
            dirty = true;
        }

        public static void Clear()
        {
            guidSet.Clear();
            instSet.Clear();
            dirty = true;
        }

        public static void Add(RF_Ref rf)
        {

            if (rf.isSceneRef)

                //Debug.Log("add " + rf.component);
                Add(rf.component);
            else
                Add(rf.asset.guid);
        }

        public static void Remove(RF_Ref rf)
        {

            if (rf.isSceneRef)

                //Debug.Log("remove: " + rf.component);
                Remove(rf.component);
            else
                Remove(rf.asset.guid);
        }

        public static void Commit()
        {
            var list = new HashSet<UnityObject>();

            foreach (string guid in guidSet)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UnityObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityObject));
                if (obj != null) list.Add(obj);
            }

            foreach (string instID in instSet)
            {
                int id = int.Parse(instID);
                UnityObject obj = EditorUtility.InstanceIDToObject(id);
                if (obj == null) continue;
                list.Add(obj is Component c ? c.gameObject : obj);
            }

            Selection.objects = list.ToArray();
        }

        public void SetDirty()
        {
            drawer.SetDirty();
        }

        private void DrawButtons(Rect rect)
        {
            if (Count == 0) return;

            GUILayout.BeginArea(rect);
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Select"))
                    {
                        Commit();
                        window.WillRepaint = true;
                    }

                    if (GUILayout.Button("Clear"))
                    {
                        Clear();
                        window.WillRepaint = true;
                    }

                    if (GUILayout.Button("CSV")) RF_Export.ExportCSV(refs.Values.ToArray());

                    if (GUILayout.Button("Delete"))
                    {
                        RF_Unity.BackupAndDeleteAssets(refs.Values.ToArray());
                        Clear();
                        GUIUtility.ExitGUI();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        public void RefreshView()
        {
            if (refs == null) refs = new Dictionary<string, RF_Ref>();
            refs.Clear();

            foreach (string guid in guidSet)
            {
                RF_Asset asset = RF_Cache.Api.Get(guid);
                refs.Add(guid, new RF_Ref(0, 0, asset, null));
            }

            foreach (string instId in instSet)
            {
                refs.Add(instId, new RF_SceneRef(0, EditorUtility.InstanceIDToObject(int.Parse(instId))));
            }


            drawer.SetRefs(refs);

            //Debug.Log("RefreshView: " + refs.Count);
            dirty = false;
        }

        internal void RefreshSort()
        {
            drawer.RefreshSort();
        }
    }
}
