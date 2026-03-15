using System.IO;
using UnityEditor;

namespace ET
{
    public static class BTDemoExportEditor
    {
        [MenuItem("ET/AI/Export Demo AITest.bytes", false, 1008)]
        public static void ExportDemoAITest()
        {
            byte[] bytes = CreateAITestBytes();
            string clientFilePath = Path.Combine(BTBytesLoader.ClientBehaviorTreeBytesDir, "AITest.bytes");
            string clientDirectory = Path.GetDirectoryName(clientFilePath);
            if (!Directory.Exists(clientDirectory))
            {
                Directory.CreateDirectory(clientDirectory);
            }

            string serverFilePath = Path.Combine(BTBytesLoader.ServerBehaviorTreeBytesDir, "AITest.bytes");
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

        private static byte[] CreateAITestBytes()
        {
            BTDefinition tree = new()
            {
                TreeId = "demo.shared.ai_test",
                TreeName = "AITest",
                Description = "Shared client/server demo behavior tree.",
                RootNodeId = "root",
            };

            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateRootNode("root", "Root", childIds: new[] { "repeat" }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateRepeaterNode("repeat", "Repeat Tick", string.Empty, 0, new[] { "sequence" }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateSequenceNode("sequence", "Tick Sequence", childIds: new[] { "log", "wait" }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateActionNode("log", "Log Tick", string.Empty, BTBuiltinNodeTypes.Log, "Log",
                new[]
                {
                    new BTArgumentData
                    {
                        Name = "message",
                        Value = new BTSerializedValue
                        {
                            ValueType = BTValueType.String,
                            StringValue = "AITest tick",
                        },
                    },
                }));
            tree.Nodes.Add(BTEditorRuntimeNodeFactory.CreateWaitNode("wait", "Wait", string.Empty, 1000));

            BTPackage package = new()
            {
                PackageId = tree.TreeId,
                PackageName = tree.TreeName,
                EntryTreeId = tree.TreeId,
                EntryTreeName = tree.TreeName,
                Trees = { tree },
            };

            return BTSerializer.Serialize(package);
        }
    }
}
