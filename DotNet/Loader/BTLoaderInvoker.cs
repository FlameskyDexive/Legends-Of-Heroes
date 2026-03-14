using System.IO;

namespace ET
{
    [Invoke]
    public class GetOneBehaviorTreeBytesInvoker : AInvokeHandler<BTBytesLoader.GetOneBehaviorTreeBytes, byte[]>
    {
        public override byte[] Handle(BTBytesLoader.GetOneBehaviorTreeBytes args)
        {
            if (string.IsNullOrWhiteSpace(args.TreeName))
            {
                Log.Error("behavior tree name is empty");
                return null;
            }

            string filePath = Path.Combine("..", BTBytesLoader.ServerBehaviorTreeBytesDir, $"{args.TreeName}.bytes");
            if (!File.Exists(filePath))
            {
                Log.Error($"behavior tree bytes not found: {filePath}");
                return null;
            }

            return File.ReadAllBytes(filePath);
        }
    }
}
