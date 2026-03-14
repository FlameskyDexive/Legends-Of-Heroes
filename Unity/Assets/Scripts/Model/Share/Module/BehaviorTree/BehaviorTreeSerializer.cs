using Nino.Core;

namespace ET
{
    public static class BehaviorTreeSerializer
    {
        public static byte[] Serialize(BehaviorTreePackage package)
        {
            return NinoSerializer.Serialize(package);
        }

        public static BehaviorTreePackage Deserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            return NinoDeserializer.Deserialize<BehaviorTreePackage>(bytes);
        }
    }
}
