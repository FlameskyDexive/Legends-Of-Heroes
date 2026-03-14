using System.IO;
using UnityEditor;

namespace ET
{
    public static class BTDemoExportEditor
    {
        [MenuItem("ET/AI/Export Demo AITest.bytes", false, 1008)]
        public static void ExportDemoAITest()
        {
            byte[] bytes = ET.Client.BTClientDemoFactory.CreateAITestBytes();
            string clientFilePath = Path.Combine(BTLoader.ClientBehaviorTreeBytesDir, "AITest.bytes");
            string clientDirectory = Path.GetDirectoryName(clientFilePath);
            if (!Directory.Exists(clientDirectory))
            {
                Directory.CreateDirectory(clientDirectory);
            }

            string serverFilePath = Path.Combine(BTLoader.ServerBehaviorTreeBytesDir, "AITest.bytes");
            string serverDirectory = Path.GetDirectoryName(serverFilePath);
            if (!Directory.Exists(serverDirectory))
            {
                Directory.CreateDirectory(serverDirectory);
            }

            File.WriteAllBytes(clientFilePath, bytes);
            File.WriteAllBytes(serverFilePath, bytes);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("BehaviorTree", $"Exported demo files:\n{clientFilePath}\n{serverFilePath}", "OK");
        }
    }
}
