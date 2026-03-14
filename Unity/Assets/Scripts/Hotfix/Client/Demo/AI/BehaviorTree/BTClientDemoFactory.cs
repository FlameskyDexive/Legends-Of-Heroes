namespace ET.Client
{
    public static class BTClientDemoFactory
    {
        public static byte[] CreateAITestBytes()
        {
            BTDefinition tree = new()
            {
                TreeId = "demo.shared.ai_test",
                TreeName = "AITest",
                Description = "Shared client/server demo behavior tree.",
                RootNodeId = "root",
            };

            tree.Nodes.Add(new BTRootNodeData()
            {
                NodeId = "root",
                Title = "Root",
                ChildIds = { "repeat" },
            });

            tree.Nodes.Add(new BTRepeaterNodeData()
            {
                NodeId = "repeat",
                Title = "Repeat Tick",
                ChildIds = { "sequence" },
            });

            tree.Nodes.Add(new BTSequenceNodeData()
            {
                NodeId = "sequence",
                Title = "Tick Sequence",
                ChildIds = { "log", "wait" },
            });

            tree.Nodes.Add(new BTActionNodeData()
            {
                NodeId = "log",
                Title = "Log Tick",
                TypeId = BTBuiltinNodeTypes.Log,
                ActionHandlerName = "Log",
                Arguments =
                {
                    new BTArgumentData()
                    {
                        Name = "message",
                        Value = new BTSerializedValue()
                        {
                            ValueType = BTValueType.String,
                            StringValue = "AITest tick",
                        },
                    },
                },
            });

            tree.Nodes.Add(new BTWaitNodeData()
            {
                NodeId = "wait",
                Title = "Wait",
                WaitMilliseconds = 1000,
            });

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
