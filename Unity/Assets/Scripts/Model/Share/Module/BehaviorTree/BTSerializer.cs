using Nino.Core;

namespace ET
{
    public static class BTSerializer
    {
        public static byte[] Serialize(BTPackage package)
        {
            return NinoSerializer.Serialize(package);
        }

        public static BTPackage Deserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            return NinoDeserializer.Deserialize<BTPackage>(bytes);
        }
    }
}
