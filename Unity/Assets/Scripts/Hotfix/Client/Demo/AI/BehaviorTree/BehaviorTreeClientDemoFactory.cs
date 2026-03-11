namespace ET.Client
{
    public static class BehaviorTreeClientDemoFactory
    {
        public static byte[] CreateRobotPatrolBytes()
        {
            BehaviorTreeDefinition tree = new()
            {
                TreeId = "demo.client.robot.patrol",
                TreeName = "RobotPatrol",
                Description = "Client demo patrol behavior tree for robot scene.",
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
                Title = "Repeat Patrol",
                NodeKind = BehaviorTreeNodeKind.Repeater,
                ChildIds = { "sequence" },
            });

            tree.Nodes.Add(new BehaviorTreeNodeDefinition()
            {
                NodeId = "sequence",
                Title = "Patrol Sequence",
                NodeKind = BehaviorTreeNodeKind.Sequence,
                ChildIds = { "hasPath", "patrol", "wait" },
            });

            tree.Nodes.Add(new BehaviorTreeNodeDefinition()
            {
                NodeId = "hasPath",
                Title = "Has Patrol Path",
                NodeKind = BehaviorTreeNodeKind.Condition,
                HandlerName = "DemoClientHasXunLuoPath",
            });

            tree.Nodes.Add(new BehaviorTreeNodeDefinition()
            {
                NodeId = "patrol",
                Title = "Patrol One Point",
                NodeKind = BehaviorTreeNodeKind.Action,
                HandlerName = "DemoClientPatrol",
            });

            tree.Nodes.Add(new BehaviorTreeNodeDefinition()
            {
                NodeId = "wait",
                Title = "Wait",
                NodeKind = BehaviorTreeNodeKind.Wait,
                WaitMilliseconds = 300,
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
