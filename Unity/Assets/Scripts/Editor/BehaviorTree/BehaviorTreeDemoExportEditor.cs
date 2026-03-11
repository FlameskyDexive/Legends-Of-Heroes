using System.IO;
using UnityEditor;

namespace ET
{
    public static class BehaviorTreeDemoExportEditor
    {
        [MenuItem("ET/AI/Export Demo AITest.bytes", false, 1008)]
        public static void ExportDemoAITest()
        {
            byte[] bytes = ET.Client.BehaviorTreeClientDemoFactory.CreateRobotPatrolBytes();
            string filePath = Path.Combine("Assets/Bundles/AI/Bytes", "AITest.bytes");
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(filePath, bytes);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("BehaviorTree", $"Exported demo file:\n{filePath}", "OK");
        }
    }
}
