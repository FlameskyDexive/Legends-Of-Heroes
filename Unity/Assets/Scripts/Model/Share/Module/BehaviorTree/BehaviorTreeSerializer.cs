namespace ET
{
    public static class BehaviorTreeSerializer
    {
        public static byte[] Serialize(BehaviorTreePackage package)
        {
            return MemoryPackHelper.Serialize(package);
        }

        public static BehaviorTreePackage Deserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            return MemoryPackHelper.Deserialize(typeof(BehaviorTreePackage), bytes, 0, bytes.Length) as BehaviorTreePackage;
        }
    }
}
