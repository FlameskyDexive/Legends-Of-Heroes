using UnityEditor;
using UnityEngine;
namespace ReferenceFinder
{
    internal class RF_SearchView
    {

        public static GUIStyle toolbarSearchField;
        public static GUIStyle toolbarSearchFieldCancelButton;
        public static GUIStyle toolbarSearchFieldCancelButtonEmpty;
        private bool caseSensitive;
        private string searchTerm = string.Empty;

        public static void InitSearchStyle()
        {
            toolbarSearchField = "ToolbarSeachTextFieldPopup";
            toolbarSearchFieldCancelButton = "ToolbarSeachCancelButton";
            toolbarSearchFieldCancelButtonEmpty = "ToolbarSeachCancelButtonEmpty";
        }

        public bool DrawLayout()
        {
            var dirty = false;

            if (toolbarSearchField == null) InitSearchStyle();

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                bool v = GUILayout.Toggle(caseSensitive, "Aa", EditorStyles.toolbarButton, GUI2.GLW_24);
                if (v != caseSensitive)
                {
                    caseSensitive = v;
                    dirty = true;
                }

                GUILayout.Space(2f);
                string value = GUILayout.TextField(searchTerm, toolbarSearchField, GUI2.GLW_140);
                if (searchTerm != value)
                {
                    searchTerm = value;
                    dirty = true;
                }

                GUIStyle style = string.IsNullOrEmpty(searchTerm)
                    ? toolbarSearchFieldCancelButtonEmpty
                    : toolbarSearchFieldCancelButton;
                if (GUILayout.Button("Cancel", style))
                {
                    searchTerm = string.Empty;
                    dirty = true;
                }
            }
            GUILayout.EndHorizontal();

            return dirty;
        }
    }
}
