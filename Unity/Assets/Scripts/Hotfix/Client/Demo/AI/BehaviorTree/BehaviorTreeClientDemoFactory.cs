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

            tree.Nodes.Add(new BehaviorTreeNodeDefinition()
            {
                NodeId = "root",
                Title = "Root",
                NodeKind = BehaviorTreeNodeKind.Root,
                ChildIds = { "repeat" },
            });

            tree.Nodes.Add(new BehaviorTreeNodeDefinition()
            {
                NodeId = "repeat",
                Title = "Repeat Tick",
                NodeKind = BehaviorTreeNodeKind.Repeater,
                ChildIds = { "sequence" },
            });

            tree.Nodes.Add(new BehaviorTreeNodeDefinition()
            {
                NodeId = "sequence",
                Title = "Tick Sequence",
                NodeKind = BehaviorTreeNodeKind.Sequence,
                ChildIds = { "log", "wait" },
            });

            tree.Nodes.Add(new BehaviorTreeNodeDefinition()
            {
                NodeId = "log",
                Title = "Log Tick",
                NodeKind = BehaviorTreeNodeKind.Action,
                HandlerName = "Log",
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

            tree.Nodes.Add(new BehaviorTreeNodeDefinition()
            {
                NodeId = "wait",
                Title = "Wait",
                NodeKind = BehaviorTreeNodeKind.Wait,
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
