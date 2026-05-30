#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Type = System.Type;
using static VHierarchy.VHierarchyCache;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;
// using static VTools.VDebug;


namespace VHierarchy
{
    public class VHierarchyData : ScriptableObject, ISerializationCallbackReceiver
    {

        public SerializableDictionary<string, SceneData> sceneDatas_byGuid = new();


        [System.Serializable]
        public class SceneData
        {
            public SerializableDictionary<GlobalID, GameObjectData> goDatas_byGlobalId = new();
        }


        [System.Serializable]
        public class GameObjectData
        {
            public int colorIndex;
            public string iconNameOrGuid = "";

            public bool isIconRecursive;
            public bool isColorRecursive;


            [System.NonSerialized]
            public SceneData sceneData; // set in GetGameObjectData

        }

        public void OnBeforeSerialize() => VHierarchy.OnDataSerialization();
        public void OnAfterDeserialize() { }





        public List<Bookmark> bookmarks = new();

        [System.Serializable]
        public class Bookmark
        {

            public GameObject go
            {
                get
                {
                    if (_go == null)
                        if (!failedToLoadSceneObject) // to prevent continuous GlobalID.GetObjects() calls if object is deleted
                            VHierarchy.unloadedSceneBookmarks_sceneGuids.Add(globalId.guid);

                    return _go;

                }
            }
            public GameObject _go;

            [System.NonSerialized]
            public bool failedToLoadSceneObject;



            public bool isLoadable => go != null;

            public bool isDeleted
            {
                get
                {
                    if (isLoadable)
                        return false;

                    if (!AssetDatabase.LoadAssetAtPath<SceneAsset>(globalId.guid.ToPath()))
                        return true;

                    for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                        if (EditorSceneManager.GetSceneAt(i).path == globalId.guid.ToPath())
                            return true;

                    return false;

                }
            }

            public string assetPath => globalId.guid.ToPath();

            public string name => go ? go.name : "Can't load object";



            public Bookmark(GameObject o)
            {
                globalId = o.GetGlobalID();

                _go = o;

            }

            public GlobalID globalId;

        }




        public List<string> bookmarkedScenePaths = new();







        [CustomEditor(typeof(VHierarchyData))]
        class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                var style = new GUIStyle(EditorStyles.label) { wordWrap = true };

                void normal()
                {
                    if (teamModeEnabled) return;

                    SetGUIEnabled(false);
                    BeginIndent(0);

                    Space(10);
                    EditorGUILayout.LabelField("This file stores data about which icons and colors are assigned to objects, along with bookmarks from navigation bar and scene selector.", style);

                    Space(6);
                    GUILayout.Label("If there are multiple people working on the project, you might want to store icon/color data in scenes to avoid merge conflicts. To do that, click the  ⋮  button at the top right corner and enable Team Mode.", style);

                    EndIndent(10);
                    ResetGUIEnabled();
                }
                void teamMode()
                {
                    if (!teamModeEnabled) return;

                    SetGUIEnabled(false);
                    BeginIndent(0);

                    Space(10);
                    EditorGUILayout.LabelField("Now that Team Mode is enabled, create an empty script that inherits from VHierarchy.VHierarchyDataComponent and add it to any object in a scene.", style);

                    Space(6);
                    GUILayout.Label("If such a script is present in a scene - all icon/color data for that scene will be serialized in that script, otherwise icon/color data for the scene will end up in this file.", style);

                    EndIndent(10);
                    ResetGUIEnabled();
                }

                normal();
                teamMode();

            }
        }

        public static bool teamModeEnabled { get => EditorPrefsCached.GetBool("vHierarchy-teamModeEnabled", false); set => EditorPrefsCached.SetBool("vHierarchy-teamModeEnabled", value); }



        [ContextMenu("Enable Team Mode", isValidateFunction: false, priority: 1)]
        public void EnableTeamMode()
        {
            var option = EditorUtility.DisplayDialogComplex("Licensing notice",
                                                            "To use vHierarchy 2 within a team, licenses must be purchased for each individual user as per the Asset Store EULA.\n\n Sharing one license across the team is illegal and considered piracy.",
                                                            "Acknowledge",
                                                            "Cancel",
                                                            "Purchase more seats");
            if (option == 2)
                Application.OpenURL("https://prf.hn/click/camref:1100lGLBn/pubref:teammode/destination:https://assetstore.unity.com/packages/tools/utilities/vhierarchy-2-253397");
            // Application.OpenURL("https://assetstore.unity.com/packages/slug/253397");



            if (option != 0) return;

            teamModeEnabled = true;

            VHierarchy.goInfoCache.Clear();
            VHierarchy.goDataCache.Clear();

            EditorApplication.RepaintHierarchyWindow();

        }

        [ContextMenu("Disable Team Mode", isValidateFunction: false, priority: 2)]
        public void DisableTeamMode()
        {
            teamModeEnabled = false;

            VHierarchy.goInfoCache.Clear();
            VHierarchy.goDataCache.Clear();

            EditorApplication.RepaintHierarchyWindow();

        }

        [ContextMenu("Enable Team Mode", isValidateFunction: true, priority: 1)] bool asd() => !teamModeEnabled;
        [ContextMenu("Disable Team Mode", isValidateFunction: true, priority: 2)] bool ads() => teamModeEnabled;





    }
}
#endif