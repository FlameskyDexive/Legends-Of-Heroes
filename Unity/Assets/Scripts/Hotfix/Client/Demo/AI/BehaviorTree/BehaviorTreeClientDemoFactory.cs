namespace ET.Client
{
    public static class BehaviorTreeClientDemoFactory
    {
        public static byte[] CreateAITestBytes()
        {
            BehaviorTreeDefinition tree = new()
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
                TypeId = BehaviorTreeBuiltinNodeTypes.Log,
                ActionHandlerName = "Log",
                Arguments =
                {
                    new BehaviorTreeArgumentDefinition()
                    {
                        Name = "message",
                        Value = new BehaviorTreeSerializedValue()
                        {
                            ValueType = BehaviorTreeValueType.String,
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

            BehaviorTreePackage package = new()
            {
                PackageId = tree.TreeId,
                PackageName = tree.TreeName,
                EntryTreeId = tree.TreeId,
                EntryTreeName = tree.TreeName,
                Trees = { tree },
            };

            return BehaviorTreeSerializer.Serialize(package);
        }
    }
}
