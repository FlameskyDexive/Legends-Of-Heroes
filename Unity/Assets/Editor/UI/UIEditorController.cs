using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using System.IO;
using ET;
using UnityEngine;

namespace ClientEditor
{
    class UIEditorController
    {
        [MenuItem("GameObject/SpawnEUICode", false, -2)]
        static public void CreateNewCode()
        {
            GameObject go = Selection.activeObject as GameObject;
            UICodeSpawner.SpawnEUICode(go);
        }
    }
}
