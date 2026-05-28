namespace ET.Client
{
    public static class BTreeClientDemoFactory
    {
        public static byte[] CreateAITestBytes()
        {
            BTreeDefinition tree = new()
            {
                TreeId = "demo.shared.ai_test",
                TreeName = "AITest",
                Description = "Shared client/server demo behavior tree.",
                RootNodeId = "root",
            };

            tree.Nodes.Add(new BTreeRootNodeData()
            {
                NodeId = "root",
                Title = "Root",
                ChildIds = { "repeat" },
            });

            tree.Nodes.Add(new BTreeRepeaterNodeData()
            {
                NodeId = "repeat",
                Title = "Repeat Tick",
                ChildIds = { "sequence" },
            });

            tree.Nodes.Add(new BTreeSequenceNodeData()
            {
                NodeId = "sequence",
                Title = "Tick Sequence",
                ChildIds = { "log", "wait" },
            });

            tree.Nodes.Add(new BTreeLogNodeData()
            {
                NodeId = "log",
                Title = "Log Tick",
                Arguments =
                {
                    new BTreeArgumentData()
                    {
                        Name = "message",
                        Value = new BTreeSerializedValue()
                        {
                            ValueType = BTreeValueType.String,
                            StringValue = "AITest tick",
                        },
                    },
                },
            });

            tree.Nodes.Add(new BTreeWaitNodeData()
            {
                NodeId = "wait",
                Title = "Wait",
                WaitMilliseconds = 1000,
            });

            BTreePackage package = new()
            {
                PackageId = tree.TreeId,
                PackageName = tree.TreeName,
                EntryTreeId = tree.TreeId,
                EntryTreeName = tree.TreeName,
                Trees = { tree },
            };

            return BTreeSerializer.Serialize(package);
        }
    }
}
