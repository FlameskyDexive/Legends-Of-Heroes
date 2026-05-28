using Nino.Core;

namespace ET
{
    public static class BTreeSerializer
    {
        public static byte[] Serialize(BTreePackage package)
        {
            return NinoSerializer.Serialize(package);
        }

        public static BTreePackage Deserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            return NinoDeserializer.Deserialize<BTreePackage>(bytes);
        }
    }
}
